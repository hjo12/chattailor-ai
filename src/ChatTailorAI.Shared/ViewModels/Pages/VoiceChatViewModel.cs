using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatTailorAI.Shared.Services.Audio;
using ChatTailorAI.Shared.Services.Common;

namespace ChatTailorAI.Shared.ViewModels.Pages
{
    public class VoiceChatViewModel : INotifyPropertyChanged
    {
        private readonly IDispatcherService _dispatcherService;
        private string _connectionStatus = "Disconnected";
        private string _platform = "azure";
        private string _transcript = "";
        private bool _active = false;

        public ObservableCollection<string> Messages { get; } = new();
        public ObservableCollection<string> AudioMessages { get; } = new();

        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cts;
        private readonly IAudioSessionService _audioSession;

        private string _backendUrl = "ws://localhost:3001/realtime";

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        public string Platform
        {
            get => _platform;
            set => SetProperty(ref _platform, value);
        }

        public string Transcript
        {
            get => _transcript;
            set => SetProperty(ref _transcript, value);
        }

        public bool CanConnect => ConnectionStatus == "Disconnected";
        public bool CanDisconnect => ConnectionStatus == "Connected";

        public event PropertyChangedEventHandler PropertyChanged;

        public VoiceChatViewModel(
            IDispatcherService dispatcherService,
            IAudioSessionService audioSessionService)
        {
            _dispatcherService = dispatcherService ?? throw new ArgumentNullException(nameof(dispatcherService));
            _audioSession = audioSessionService;
            _audioSession.AudioChunkCaptured += OnAudioChunkCaptured;
        }

        public async void StartSession()
        {
            if (ConnectionStatus == "Connected") return;

            _active = true;
            await ConnectAsync();
        }

        public async void EndSession()
        {
            if (ConnectionStatus == "Disconnected") return;

            var endSessionMessage = new { type = "end_session" };
            await SendJsonMessageAsync(endSessionMessage);
            await AppendMessageAsync($"Sent session termination message: {JsonSerializer.Serialize(endSessionMessage)}", "sent");
            await CloseAsync("User requested end session");
            await StopRecordingAndCleanupAsync();
        }

        public async void SendTextMessage(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            var jsonData = new
            {
                channel = "realtimespeech",
                text = text.Trim(),
                platform = Platform
            };
            await SendJsonMessageAsync(jsonData);
            await AppendMessageAsync(jsonData.text, "sent");
        }

        private async Task ConnectAsync()
        {
            try
            {
                _webSocket = new ClientWebSocket();
                _cts = new CancellationTokenSource();

                await AppendMessageAsync("Attempting to connect...", "info");
                await _webSocket.ConnectAsync(new Uri($"{_backendUrl}?platform={Platform}"), _cts.Token);

                if (_webSocket.State == WebSocketState.Open)
                {
                    ConnectionStatus = "Connected";
                    await UpdatePropertiesAsync();
                    await AppendMessageAsync("WebSocket connection opened.", "info");
                    _ = Task.Run(ReceiveLoopAsync);
                }
                else
                {
                    ConnectionStatus = "Disconnected";
                    await AppendMessageAsync("Failed to connect.", "error");
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = "Disconnected";
                await AppendMessageAsync($"Connection error: {ex.Message}", "error");
            }
        }

        private async Task CloseAsync(string reason)
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
            }
            ConnectionStatus = "Disconnected";
            await UpdatePropertiesAsync();
            await AppendMessageAsync($"WebSocket connection closed: {reason}", "info");
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[8192];
            var sb = new StringBuilder(); // For accumulating text message fragments

            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(buffer, _cts.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await CloseAsync("Server closed the connection");
                        await StopRecordingAndCleanupAsync();
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        // Append the partial text fragment to sb
                        string fragment = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        sb.Append(fragment);

                        if (result.EndOfMessage)
                        {
                            // We now have the complete text message
                            var completeMessage = sb.ToString();
                            sb.Clear(); // reset for the next message

                            await HandleTextMessageAsync(completeMessage);
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        var data = new byte[result.Count];
                        Array.Copy(buffer, data, result.Count);
                        await HandleBinaryMessageAsync(data);
                    }
                }
            }
            catch (Exception ex)
            {
                await AppendMessageAsync($"WebSocket error: {ex.Message}", "error");
                await CloseAsync("Exception in ReceiveLoop");
                await StopRecordingAndCleanupAsync();
            }
        }

        private async Task HandleTextMessageAsync(string data)
        {
            try
            {
                var jsonData = JsonSerializer.Deserialize<JsonElement>(data);
                var type = jsonData.GetProperty("type").GetString();
                await AppendMessageAsync($"{type}\n{data}", "received");

                switch (type)
                {
                    case "session.created":
                        await AppendMessageAsync("Session created by backend. Starting audio recording.", "info");
                        await _audioSession.StartRecordingAsync();
                        break;

                    case "response.audio_transcript.delta":
                        if (jsonData.TryGetProperty("delta", out var deltaProp) && deltaProp.ValueKind == JsonValueKind.String)
                        {
                            var deltaStr = deltaProp.GetString();
                            Transcript += deltaStr;
                            await UpdatePropertiesAsync(nameof(Transcript));
                        }
                        break;

                    case "response.audio.delta":
                        if (jsonData.TryGetProperty("delta", out var audioDeltaProp) && audioDeltaProp.ValueKind == JsonValueKind.String)
                        {
                            var audioBase64 = audioDeltaProp.GetString();
                            _audioSession.PlayAudioFromBase64(audioBase64);
                        }
                        break;

                    case "input_audio_buffer.speech_started":
                        await AppendAudioMessageAsync("<< Speech Started >>", "info");
                        await ClearAssistantOutputAsync();
                        _audioSession.ClearPlayer();
                        break;

                    case "response.done":
                        await AppendMessageAsync("Response completed.", "info");
                        break;

                    default:
                        // Unhandled type
                        break;
                }
            }
            catch (Exception ex)
            {
                await AppendMessageAsync($"Received invalid JSON: {data}. Error: {ex.Message}", "error");
            }
        }

        private async Task HandleBinaryMessageAsync(byte[] data)
        {
            await AppendMessageAsync($"Received binary data ({data.Length} bytes).", "received");
            // Handle binary data if needed.
        }

        private async Task SendJsonMessageAsync(object obj)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open) return;
            var json = JsonSerializer.Serialize(obj);
            var bytes = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, _cts.Token);
        }

        private async Task SendBinaryMessageAsync(byte[] data)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open) return;
            await _webSocket.SendAsync(data, WebSocketMessageType.Binary, true, _cts.Token);
        }

        private async void OnAudioChunkCaptured(byte[] pcm16Array)
        {
            // Prepend channel ID
            byte channelId = 1; // 'realtimespeech'
            var messageBuffer = new byte[pcm16Array.Length + 1];
            messageBuffer[0] = channelId;
            Array.Copy(pcm16Array, 0, messageBuffer, 1, pcm16Array.Length);

            await SendBinaryMessageAsync(messageBuffer);
            await AppendAudioMessageAsync($"Sent audio chunk ({pcm16Array.Length} bytes)", "sent");
        }

        private async Task AppendMessageAsync(string message, string type = "info")
        {
            var prefixMap = new System.Collections.Generic.Dictionary<string, string> {
                {"sent","Sent: "}, {"received","Received: "}, {"info","Info: "}, {"error","Error: "}
            };

            await _dispatcherService.RunOnUIThreadAsync(() =>
            {
                Messages.Add($"{prefixMap[type]}{message}");
                if (Messages.Count > 20)
                {
                    Messages.RemoveAt(0);
                }
            });
        }

        private async Task AppendAudioMessageAsync(string message, string type = "info")
        {
            var prefixMap = new System.Collections.Generic.Dictionary<string, string> {
                {"sent","Sent: "}, {"received","Received: "}, {"info","Info: "}, {"error","Error: "}
            };

            await _dispatcherService.RunOnUIThreadAsync(() =>
            {
                AudioMessages.Add($"{prefixMap[type]}{message}");
                if (AudioMessages.Count > 20)
                {
                    AudioMessages.RemoveAt(0);
                }
            });
        }

        private async Task ClearAssistantOutputAsync()
        {
            Transcript = "";
            await UpdatePropertiesAsync(nameof(Transcript));
        }

        private async Task StopRecordingAndCleanupAsync()
        {
            _audioSession.StopRecording();
            await ClearAssistantOutputAsync();

            await _dispatcherService.RunOnUIThreadAsync(() =>
            {
                Messages.Clear();
                AudioMessages.Clear();
            });

            Transcript = "";
            ConnectionStatus = "Disconnected";
            await UpdatePropertiesAsync(nameof(CanConnect), nameof(CanDisconnect), nameof(Transcript), nameof(ConnectionStatus));
        }

        private async Task UpdatePropertiesAsync(params string[] propertyNames)
        {
            if (propertyNames.Length == 0)
            {
                // Update all relevant
                propertyNames = new[] { nameof(CanConnect), nameof(CanDisconnect), nameof(ConnectionStatus), nameof(Transcript), nameof(Platform) };
            }

            await _dispatcherService.RunOnUIThreadAsync(() =>
            {
                foreach (var name in propertyNames)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }
            });
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            _ = UpdatePropertiesAsync(propertyName);
            return true;
        }
    }
}
