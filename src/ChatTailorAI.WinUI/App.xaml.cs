using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ChatTailorAI.DataAccess.Database;
using System.Threading.Tasks;
using ChatTailorAI.Shared.Services.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using ChatTailorAI.WinUI.Helpers;
using ChatTailorAI.Services.WinUI.System;
using ChatTailorAI.WinUI.Configuration;

namespace ChatTailorAI.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public IHost ServiceHost { get; set; }
        public new static App Current => (App)Application.Current;
        private Window? m_window;
        public static Window? MainWindow { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            ServiceHost = ServiceConfiguration.BuildServiceHost();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException("Unhandled Exception", ex);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException("Unobserved Task Exception", e.Exception);
            e.SetObserved();
        }

        private void LogException(string context, Exception ex)
        {
            Console.WriteLine($"[{context}] {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();

            await InitializeApplication();
        }

        private async Task InitializeApplication()
        {
            var windowHandleService = ServiceHost.Services.GetRequiredService<IWindowHandleService>();
            windowHandleService.SetWindowHandle(WindowNative.GetWindowHandle(MainWindow));

            var appNotificationService = ServiceHost.Services.GetRequiredService<IAppNotificationService>();
            appNotificationService.Initialize();

            ConfigureLogger();
            await InitializeDatabaseAsync();
        }

        private void ConfigureLogger()
        {
            var logger = ServiceHost.Services.GetService<ILoggerService>();
            logger.Info("Starting ChatTailorAI.WinUI...");
        }

        private async Task InitializeDatabaseAsync()
        {
            var dbInitializer = ServiceHost.Services.GetService<DbInitializer>();
            await dbInitializer.InitializeAsync();
        }
    }
}
