using NAudio.Wave;
using System.Collections.Concurrent;

namespace ChatTailorAI.Services.Audio
{
    /// <summary>
    /// A wave provider that streams audio from a queue of byte arrays.
    /// If not enough data is available, it fills with silence.
    /// This ensures continuous playback without stutter.
    /// </summary>
    public class StreamingWaveProvider : IWaveProvider
    {
        private readonly ConcurrentQueue<byte[]> audioQueue = new ConcurrentQueue<byte[]>();
        private readonly WaveFormat waveFormat;
        private byte[] leftoverBuffer = null;
        private int leftoverOffset = 0;

        public StreamingWaveProvider(WaveFormat format)
        {
            waveFormat = format ?? throw new ArgumentNullException(nameof(format));
        }

        public WaveFormat WaveFormat => waveFormat;

        /// <summary>
        /// Enqueue a chunk of audio data (already in the desired WaveFormat).
        /// </summary>
        public void Enqueue(byte[] data)
        {
            // You can optionally split large buffers into smaller chunks,
            // but it's generally fine to enqueue as-is.
            audioQueue.Enqueue(data);
        }

        /// <summary>
        /// Clear all queued audio.
        /// </summary>
        public void Clear()
        {
            while (audioQueue.TryDequeue(out _)) { }
            leftoverBuffer = null;
            leftoverOffset = 0;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int bytesReadTotal = 0;

            // First, use any leftover data from previous chunk
            if (leftoverBuffer != null)
            {
                int available = leftoverBuffer.Length - leftoverOffset;
                int needed = Math.Min(available, count);
                Array.Copy(leftoverBuffer, leftoverOffset, buffer, offset, needed);
                leftoverOffset += needed;
                bytesReadTotal += needed;
                offset += needed;

                if (leftoverOffset >= leftoverBuffer.Length)
                {
                    leftoverBuffer = null;
                    leftoverOffset = 0;
                }
            }

            // If still need more data, dequeue from queue
            while (bytesReadTotal < count && audioQueue.TryDequeue(out var chunk))
            {
                int needed = count - bytesReadTotal;
                if (chunk.Length > needed)
                {
                    // Partially consume this chunk, leftover will be saved
                    Array.Copy(chunk, 0, buffer, offset, needed);
                    bytesReadTotal += needed;
                    // Save remainder for next read
                    leftoverBuffer = chunk;
                    leftoverOffset = needed;
                }
                else
                {
                    // Consume entire chunk
                    Array.Copy(chunk, 0, buffer, offset, chunk.Length);
                    bytesReadTotal += chunk.Length;
                }

                offset += (bytesReadTotal < count) ? chunk.Length : needed;

                // If we filled the request exactly, break
                if (bytesReadTotal >= count)
                    break;
            }

            // If we didn't get enough data, fill the rest with silence
            if (bytesReadTotal < count)
            {
                int silenceBytes = count - bytesReadTotal;
                Array.Clear(buffer, offset, silenceBytes);
                bytesReadTotal += silenceBytes;
            }

            return bytesReadTotal;
        }
    }
}
