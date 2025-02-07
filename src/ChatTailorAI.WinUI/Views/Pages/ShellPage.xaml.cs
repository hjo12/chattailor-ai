using ChatTailorAI.Shared.Services.Common;
using ChatTailorAI.Shared.Services.Common.Navigation;
using ChatTailorAI.Shared.Services.Events;
using ChatTailorAI.Shared.ViewModels.Pages;
using ChatTailorAI.WinUI.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace ChatTailorAI.WinUI.Views.Pages
{
    public sealed partial class ShellPage : Page
    {
        public ShellPageViewModel ViewModel => (ShellPageViewModel)DataContext;

        public ShellPage()
        {
            this.InitializeComponent();

            Loaded += Page_Loaded;

            this.DataContext =
                App.Current.ServiceHost.Services.GetService<ShellPageViewModel>();

            var navService = App.Current.ServiceHost.Services.GetRequiredService<INavigationService>();
            navService.MainFrame = ContentFrame;

            var eventAggregator = App.Current.ServiceHost.Services.GetRequiredService<IEventAggregator>();
            eventAggregator.NotificationRaised += EventAggregator_NotificationRaised;

            ContentFrame.Navigate(typeof(ChatPage));
        }

        ~ShellPage()
        {
            var eventAggregator = App.Current.ServiceHost.Services.GetRequiredService<IEventAggregator>();
            eventAggregator.NotificationRaised -= EventAggregator_NotificationRaised;
        }

        private void EventAggregator_NotificationRaised(object sender, Shared.Events.NotificationRaisedEvent e)
        {
            //this.InAppNotification.Show(e.Message, e.Duration);
        }

        private void OnNavigationViewItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (DataContext is ShellPageViewModel viewModel)
            {
                viewModel.NavigationViewItemInvokedCommand.Execute(args.InvokedItemContainer.Tag);
            }
        }

        private void mainNav_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            OnNavigationViewItemInvoked(sender, args);
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }

        private async void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.Controls.NavigationViewItem navViewItem)
            {
                if (navViewItem.Tag?.ToString() == "PictureInPicture")
                {
                    var windowManagerService = App.Current.ServiceHost.Services.GetService<IWindowManagerService>();
                    windowManagerService?.ToggleCompactOverlayMode();
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBarHelper.ExpandViewIntoTitleBar(App.MainWindow, CustomTitleBar);
            TitleBarHelper.SetupTitleBar(App.MainWindow);
        }
    }
}
