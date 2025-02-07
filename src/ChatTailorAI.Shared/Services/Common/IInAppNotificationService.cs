namespace ChatTailorAI.Shared.Services.Common
{
    public interface IInAppNotificationService
    {
        void Display(string message, int duration = 3000);
    }
}
