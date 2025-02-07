using NAudio.Wave;
using ChatTailorAI.Shared.Services.Audio;

namespace ChatTailorAI.Services.Audio
{
    public class AudioSessionService : IAudioSessionService
    {
        public event Action<byte[]> AudioChunkCaptured;

        private WaveInEvent waveIn;
        private WaveOutEvent waveOut;

        // Input: PCM16(24kHz, mono)
        private readonly WaveFormat inputFormat = new WaveFormat(24000, 16, 1);
        // Intermediate after resample: PCM16(48kHz, mono)
        private readonly WaveFormat intermediateFormat = new WaveFormat(48000, 16, 1);
        // Final playback format: float32(48kHz, mono)
        private readonly WaveFormat floatOutputFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1);

        private StreamingWaveProvider streamingWaveProvider;
        private bool startedPlayback = false;
        private double accumulatedBytes = 0; // track how many bytes accumulated before starting playback
        private readonly object lockObj = new object();

        // How many seconds to buffer before starting playback
        private double initialBufferSeconds = 1.0;
        private double initialBufferThresholdBytes => floatOutputFormat.AverageBytesPerSecond * initialBufferSeconds;

        public async Task StartRecordingAsync()
        {
            if (waveIn != null) return;

            waveIn = new WaveInEvent
            {
                WaveFormat = inputFormat,
                BufferMilliseconds = 100
            };

            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;

            waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] pcmData = new byte[e.BytesRecorded];
            Array.Copy(e.Buffer, pcmData, e.BytesRecorded);
            AudioChunkCaptured?.Invoke(pcmData);
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            // Dispose WaveIn here safely after it has fully stopped
            if (waveIn != null)
            {
                waveIn.DataAvailable -= OnDataAvailable;
                waveIn.RecordingStopped -= OnRecordingStopped;
                waveIn.Dispose();
                waveIn = null;
            }
        }

        public void StopRecording()
        {
            if (waveIn == null) return;
            // Stop capturing audio
            waveIn.StopRecording();

            // Also stop playback and clear pending data so that audio stops playing
            ClearPlayer();
        }

        public void ClearPlayer()
        {
            lock (lockObj)
            {
                waveOut?.Stop();
                waveOut?.Dispose();
                waveOut = null;

                streamingWaveProvider?.Clear();
                streamingWaveProvider = null;
                startedPlayback = false;
                accumulatedBytes = 0;
            }
        }

        public async Task PlayAudioFromBase64(string base64Data)
        {
            // Step 1: Decode base64 to PCM16(24kHz)
            byte[] pcm24kHz = Convert.FromBase64String(base64Data);

            // Step 2: Resample PCM16(24kHz) to PCM16(48kHz)
            byte[] pcm48kHz = ResamplePCM16(pcm24kHz, inputFormat, intermediateFormat);

            // Step 3: Convert PCM16(48kHz) → float32(48kHz)
            float[] float48kHz = PCM16ToFloat32(pcm48kHz);

            // Step 4: Convert float32 array to byte[] in IEEE float format
            byte[] floatBytes = Float32ToBytes(float48kHz);

            lock (lockObj)
            {
                if (streamingWaveProvider == null)
                {
                    streamingWaveProvider = new StreamingWaveProvider(floatOutputFormat);
                }

                // Enqueue the converted audio data
                streamingWaveProvider.Enqueue(floatBytes);

                // If we haven't started playback yet, accumulate until threshold
                if (!startedPlayback)
                {
                    accumulatedBytes += floatBytes.Length;

                    if (accumulatedBytes >= initialBufferThresholdBytes)
                    {
                        StartPlayback();
                        startedPlayback = true;
                    }
                }
            }
        }

        private void StartPlayback()
        {
            waveOut = new WaveOutEvent();
            waveOut.Init(streamingWaveProvider);
            waveOut.Play();
        }

        private byte[] ResamplePCM16(byte[] inputPCM, WaveFormat inFormat, WaveFormat outFormat)
        {
            using var msIn = new MemoryStream(inputPCM);
            using var rawIn = new RawSourceWaveStream(msIn, inFormat);
            using var resampler = new MediaFoundationResampler(rawIn, outFormat)
            {
                ResamplerQuality = 60
            };
            return ReadFully(resampler);
        }

        private float[] PCM16ToFloat32(byte[] pcmBytes)
        {
            int sampleCount = pcmBytes.Length / 2;
            float[] floatData = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                short sample = (short)(pcmBytes[i * 2] | (pcmBytes[i * 2 + 1] << 8));
                floatData[i] = sample / 32768f;
            }
            return floatData;
        }

        private byte[] Float32ToBytes(float[] floatData)
        {
            byte[] bytes = new byte[floatData.Length * 4];
            Buffer.BlockCopy(floatData, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private byte[] ReadFully(IWaveProvider waveProvider)
        {
            using var ms = new MemoryStream();
            var buffer = new byte[4096];
            int read;
            while ((read = waveProvider.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}
