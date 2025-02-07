using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTailorAI.Shared.Services.Common
{
    public interface IAppNotificationService
    {
        /// <summary>
        /// Initializes the notification system. Call this once at application startup.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shows a basic notification with the provided title and message.
        /// </summary>
        /// <param name="title">Notification title text.</param>
        /// <param name="message">Notification body text.</param>
        void ShowNotification(string title, string message);
    }
}
