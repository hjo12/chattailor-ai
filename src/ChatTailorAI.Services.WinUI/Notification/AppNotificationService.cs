using ChatTailorAI.Shared.Services.Common;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Windowing;
using Microsoft.UI;

namespace ChatTailorAI.Services.WinUI.Notification
{
    public class AppNotificationService : IAppNotificationService
    {
        private readonly IWindowHandleService _windowHandleService;

        private bool _isInitialized;

        public AppNotificationService(
            IWindowHandleService windowHandleService)
        {
            _windowHandleService = windowHandleService;
            _isInitialized = false;

            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;
        }

        private void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            // TODO: Handle notification clicked on by user.
        }

        /// <summary>
        /// Initializes the notification system. Call this once when the app starts,
        /// after the Window is created and before showing notifications.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            try
            {
                // Registers the application for notifications so it can show and receive them
                AppNotificationManager.Default.Register();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Shows a simple notification with a title and message.
        /// </summary>
        public void ShowNotification(string title, string message)
        {
            if (!_isInitialized)
            {
                // Either throw or silently ignore.
                throw new InvalidOperationException("AppNotificationService not initialized. Call Initialize() first.");
            }

            // For now, only display notifications if the application is in a minimized state
            // If we change in the future, verify it doesn't conflict with the custom PiP mode
            var windowId = Win32Interop.GetWindowIdFromWindow(_windowHandleService.GetWindowHandle());
            var aw = AppWindow.GetFromWindowId(windowId);
            
            if (aw.Presenter is OverlappedPresenter presenter)
            {
                if (presenter.State == OverlappedPresenterState.Minimized)
                {
                    var builder = new AppNotificationBuilder()
                                    .AddText(title)
                                    .AddText(message);

                    var notification = builder.BuildNotification();
                    AppNotificationManager.Default.Show(notification);
                }
            }
        }

        public void Unregister()
        {
            if (_isInitialized)
            {
                AppNotificationManager.Default.Unregister();
                _isInitialized = false;
            }
        }
    }
}
