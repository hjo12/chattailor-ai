using System;
using System.Web;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using ChatTailorAI.Shared.Dto;
using ChatTailorAI.Shared.Dto.Conversations;
using ChatTailorAI.Shared.Services.Events;
using ChatTailorAI.Shared.Events;
using ChatTailorAI.Shared.ViewModels.Pages;
using Microsoft.UI.Windowing;
using Microsoft.UI.Input;
using Windows.UI.Core;
using Microsoft.UI;
using Windows.Graphics;
using ChatTailorAI.Shared.Services.Common;

namespace ChatTailorAI.WinUI.Views.Pages
{
    public sealed partial class ChatPage : Page
    {
        public ChatPageViewModel ChatViewModel => (ChatPageViewModel)DataContext;
        private readonly IEventAggregator _eventAggregator;

        public ChatPage()
        {
            this.InitializeComponent();
            
            this.DataContext = 
                App.Current.ServiceHost.Services.GetService<ChatPageViewModel>();

            _eventAggregator = App.Current.ServiceHost.Services.GetService<IEventAggregator>();
            _eventAggregator.AuthenticationRequested += OnAuthenticationRequested;
        }

        private async void OnAuthenticationRequested(object sender, AuthenticationRequestedEvent e)
        {

            if (Dispatcher.HasThreadAccess)
            {
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.CookieManager.DeleteAllCookies();
                webView.Source = new Uri(e.AuthenticationUrl);
                webView.Visibility = Visibility.Visible;
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await webView.EnsureCoreWebView2Async();
                    webView.CoreWebView2.CookieManager.DeleteAllCookies();
                    webView.Source = new Uri(e.AuthenticationUrl);
                    webView.Visibility = Visibility.Visible;
                });
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ChatViewModel.Dispose();

            _eventAggregator.AuthenticationRequested -= OnAuthenticationRequested;
        }

        public void NavigationCleanup()
        {
            ChatViewModel.Dispose();
            _eventAggregator.AuthenticationRequested -= OnAuthenticationRequested;
        } 

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           base.OnNavigatedTo(e);

            ChatViewModel.IsActiveViewModel = true;

            switch (e.Parameter)
            {
                case AssistantDto assistantDto:
                    // Assistant was selected to chat with from assistants page
                    ChatViewModel.InitializeChat(assistantDto);
                    break;
                case ConversationDto conversation:
                    // Conversation was selected from the chat list or new convo was created
                    // Note: Can be either a normal chat or a chat with an assistant
                    ChatViewModel.InitializeChat(conversation);
                    await ChatViewModel.LoadMessages();
                    break;
                case PromptDto promptDto:
                    await ChatViewModel.InitializeInstantChat(promptDto);
                    break;
                default:
                    // Instant chat selected, creates with default settings
                    await ChatViewModel.InitializeInstantChat();
                    break;
            }
        }

        private async void PiPButton_Click(object sender, RoutedEventArgs e)
        {
            var windowManagerService = App.Current.ServiceHost.Services.GetService<IWindowManagerService>();
            windowManagerService?.ToggleCompactOverlayMode();
        }

        public void ToggleCompactOverlayMode(Window window, bool enableCompactMode)
        {
            // Get the AppWindow for the current window
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hWnd));

            if (enableCompactMode)
            {
                // Use OverlappedPresenter to enable resizing
                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsResizable = true;  // Allow resizing
                    presenter.IsAlwaysOnTop = true;  // Keep window on top for PIP effect
                }

                // Resize the window to CompactOverlay-like dimensions (e.g., 320x240)
                var size = new SizeInt32(320, 240);
                appWindow.Resize(size);

                // Move the window to the top-right corner
                var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);
                var x = displayArea.WorkArea.Width - size.Width - 10; // Adjust for margin
                var y = 10; // Margin from top
                appWindow.Move(new PointInt32(x, y));
            }
            else
            {
                // Restore the presenter to default mode with normal behavior
                if (appWindow.Presenter is OverlappedPresenter presenter)
                {
                    presenter.IsResizable = true;
                    presenter.IsAlwaysOnTop = false;
                }

                appWindow.SetPresenter(AppWindowPresenterKind.Default);

                // Optionally resize back to a default size
                appWindow.Resize(new SizeInt32(800, 600)); // Adjust as needed
            }
        }



        private async void InputTextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
                if ((shiftState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
                {
                    // Add newline only if Shift and Enter are pressed
                    var textBox = (TextBox)sender;
                    var selectionStart = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(selectionStart, Environment.NewLine);
                    textBox.SelectionStart = selectionStart + Environment.NewLine.Length;
                }
                else if (ChatViewModel.IsTyping)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = true;
                    await ChatViewModel.SendMessage();
                }
            }
        }

        private void ChatListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            //if (args.ItemIndex == ChatViewModel.Messages.Count - 1)
            //{
            //    sender.ScrollIntoView(args.Item);
            //}
        }

        private async void exportButton_Click(object sender, RoutedEventArgs e)
        {
            await ChatViewModel.ExportChatToFile();
        }

        private async void importButton_Click(object sender, RoutedEventArgs e)
        {
            await ChatViewModel.ImportChatFromFile();
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
                return null;

            if (parent is T parentAsT)
                return parentAsT;
            else
                return FindParent<T>(parent);
        }

        private async void WebView_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Uri uri = new Uri(args.Uri);
            if (uri.Host.Equals("localhost") && uri.Port == 5000)
            {
                sender.Visibility = Visibility.Collapsed;
                var queryItems = HttpUtility.ParseQueryString(uri.Query);
                var authCode = queryItems.Get("code");

                args.Cancel = true;

                // Navigate to a blank page
                await webView.EnsureCoreWebView2Async();

                // Pass the auth code back to the ViewModel to get the access token
                var tokenObtained = await ChatViewModel.RequestAccessToken(authCode);

                ChatViewModel.AuthenticationComplete(tokenObtained);
            }
        }

        private async void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            await ChatViewModel.InitializeMediaCapture();
        }
    }
}