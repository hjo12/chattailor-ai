using ChatTailorAI.Shared.Dto.Chat;

namespace ChatTailorAI.Shared.Events
{
    public class MessageReceivedEvent
    {
        public string ChatId { get; set; }
        public ChatResponseDto Message { get; set; }
    }
}
