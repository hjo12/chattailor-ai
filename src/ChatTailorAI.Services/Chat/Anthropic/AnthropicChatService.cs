﻿
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using ChatTailorAI.Shared.Events;
using ChatTailorAI.Shared.Models.Chat.Anthropic.Responses;
using ChatTailorAI.Shared.Models.Chat.Anthropic.Requests;
using ChatTailorAI.Shared.Models.Chat.Anthropic.Content;
using ChatTailorAI.Shared.Models.Settings;
using ChatTailorAI.Shared.Services.Common;
using ChatTailorAI.Shared.Services.Events;
using ChatTailorAI.Shared.Services.Chat.Anthropic;
using ChatTailorAI.Shared.Models.Chat.Anthropic.Events;
using ChatTailorAI.Shared.Dto.Chat.Anthropic;
using ChatTailorAI.Shared.Models.Chat.Anthropic;
using Newtonsoft.Json.Serialization;
using ChatTailorAI.WinUI.Services.System;

namespace ChatTailorAI.Services.Chat.Anthropic
{
    public class AnthropicChatService : IAnthropicChatService
    {
        private readonly IUserSettingsService _userSettingsService;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IEventAggregator _eventAggregator;

        private readonly HttpClient _httpClient;
        private CancellationTokenSource _cancellationTokenSource;

        public AnthropicChatService(
            IUserSettingsService userSettingsService,
            IAppSettingsService appSettingsService,
            IEventAggregator eventAggregator,
            HttpClient client
            )
        {
            _userSettingsService = userSettingsService;
            _appSettingsService = appSettingsService;
            _eventAggregator = eventAggregator;

            _cancellationTokenSource = new CancellationTokenSource();

            var anthropicApiKey = _appSettingsService.AnthropicApiKey != null && _appSettingsService.AnthropicApiKey != "" ?
                _appSettingsService.AnthropicApiKey :
                _userSettingsService.Get<string>(UserSettings.AnthropicApiKey);

            client = new HttpClient { Timeout = TimeSpan.FromMinutes(4) };
            client.BaseAddress = new Uri("https://api.anthropic.com/");
            client.Timeout = TimeSpan.FromMinutes(4);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", $"{anthropicApiKey}");
            client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            client.DefaultRequestHeaders.Add("anthropic-beta", "messages-2023-12-15");
            _httpClient = client;
        }

        public bool ValidateApiKey()
        {
            var anthropicApiKey = _appSettingsService.AnthropicApiKey != null && _appSettingsService.AnthropicApiKey != "" ?
                _appSettingsService.AnthropicApiKey :
                _userSettingsService.Get<string>(UserSettings.AnthropicApiKey);

            return !string.IsNullOrEmpty(anthropicApiKey);
        }

        public async Task<AnthropicChatResponseDto> GenerateChatResponseAsync(AnthropicChatRequest chatRequest)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var body = new AnthropicMessagesRequest
            {
                Model = chatRequest.Model,
                Messages = chatRequest.Messages,
                MaxTokens = chatRequest.Settings.MaxTokens,
                Stream = chatRequest.Settings.Stream,
                Temperature = chatRequest.Settings.Temperature,
                System = chatRequest.Instructions ?? ""
            };

            using (var response = await SendMessagesRequest(body, "v1/messages", _cancellationTokenSource))
            {
                var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync());
                var assistantMessage = new StringBuilder();
                var assistantRole = "assistant";
                AnthropicMessagesResponse messagesResponse = null;

                while (!streamReader.EndOfStream)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    var line = await streamReader.ReadLineAsync();
                    var test = line;

                    if (string.IsNullOrEmpty(line))
                    {
                        continue; // Skip empty lines, common in SSE
                    }

                    if (line.Contains("\"type\":\"error\""))
                    {
                        var error = JsonService.SafeDeserialize<AnthropicMessagesErrorResponse>(line);
                        throw new Exception($"Error in response from Anthropic API: {error.Error.Message}");
                    }

                    if (chatRequest.Settings.Stream)
                    {
                        if (line.StartsWith("event:"))
                        {
                            var eventType = line.Substring(7);
                            continue; 
                        } 
                        else if (line.StartsWith("data: "))
                        {
                            var data = line.Substring(6);
                            
                            messagesResponse = JsonService.SafeDeserialize<AnthropicMessagesResponse>(data);
                            var tt = messagesResponse;

                            if (data.Contains("\"type\":\"content_block_delta\""))
                            {
                                var deltaEvent = JsonService.SafeDeserialize<ContentBlockDeltaEvent>(data);
                                if (deltaEvent.Delta?.Type != "text_delta")
                                {
                                    continue;
                                }

                                assistantMessage.Append(deltaEvent.Delta.Text);

                                _eventAggregator.PublishMessageReceived(new MessageReceivedEvent
                                {
                                    ChatId = chatRequest.ChatId,
                                    Message = new AnthropicChatResponseDto
                                    {
                                        Content = assistantMessage.ToString(),
                                        Role = assistantRole
                                    }
                                });
                            }
                        }
                    }
                    else
                    {
                        messagesResponse = JsonService.SafeDeserialize<AnthropicMessagesResponse>(line);
                        var test2 = messagesResponse;

                        if (messagesResponse.Type == "error")
                        {
                            throw new Exception("Error in response from Anthropic API");
                        }

                        if (messagesResponse.Type == "message")
                        {
                            foreach (var contentMessage in messagesResponse.Content)
                            {
                                if (contentMessage.Type == "text")
                                {
                                    assistantMessage.Append(((AnthropicTextContentPart)contentMessage).Text);
                                }
                            }
                        }
                    }

                }

                return new AnthropicChatResponseDto { Content = assistantMessage.ToString(), Role = "assistant" };
            }
        }

        private async Task<HttpResponseMessage> SendMessagesRequest(AnthropicMessagesRequest body, string endpoint, CancellationTokenSource cancellationTokenSource)
        {
            var jsonString = JsonService.SafeSerialize(body);
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request, cancellationTokenSource.Token);
            return response;
        }

        public void CancelStream()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}