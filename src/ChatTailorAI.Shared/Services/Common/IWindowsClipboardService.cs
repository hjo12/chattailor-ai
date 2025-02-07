using System.Threading.Tasks;
using ChatTailorAI.Shared.Dto.Chat;

namespace ChatTailorAI.Shared.Services.Common
{
    public interface IWindowsClipboardService
    {
        void CopyToClipboard(ChatMessageDto message);
        void CopyToClipboard(string text);
        Task<byte[]> GetImageFromClipboardAsync();
        Task<byte[]> GetImageFromClipboardAsPngAsync();
    }
}