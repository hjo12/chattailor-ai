using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTailorAI.Shared.Services.Audio
{
    public interface IAudioSessionService
    {
        event Action<byte[]> AudioChunkCaptured;

        Task StartRecordingAsync();
        void StopRecording();
        void ClearPlayer();
        Task PlayAudioFromBase64(string base64Data);
    }
}
