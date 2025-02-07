using ChatTailorAI.Shared.Events;
using ChatTailorAI.Shared.Models.Conversations;
using ChatTailorAI.Shared.Services.Common.Navigation;
using ChatTailorAI.Shared.Services.Events;
using ChatTailorAI.Shared.ViewModels;
using ChatTailorAI.Shared.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Windowing;
using ChatTailorAI.WinUI.Helpers;

namespace ChatTailorAI.WinUI.Views.Pages
{
    public sealed partial class ChatsPage : Page
    {
        public ChatsPageViewModel ViewModel => (ChatsPageViewModel)DataContext;
        private TabViewItem? _selectedTab;

        public ChatsPage()
        {
            this.InitializeComponent();

            this.DataContext = App.Current.ServiceHost.Services.GetService<ChatsPageViewModel>();

            var navigationService = App.Current.ServiceHost.Services.GetService<IChatPageNavigationService>();

            // We won't set ChatFrame anymore since we're using TabView now.
            // navigationService.ChatFrame = ContentFrame; // Remove this line if it existed.

            var eventAggregator = App.Current.ServiceHost.Services.GetService<IEventAggregator>();
            //eventAggregator.ChatUpdated += OnChatUpdated;
        }

        //private void OnChatUpdated(object? sender, ChatUpdatedEvent e)
        //{
        //    // TODO: Check issue with convo esst in editchatdialog after update
        //    // breaking tab change from changing currentconversation
        //    var updatedChatId = e.Conversation.Id;
        //    var tabToUpdate = ChatTabView.TabItems
        //        .OfType<TabViewItem>()
        //        .FirstOrDefault(t => t.Tag is ConversationViewModel vm && vm.Id == updatedChatId);

        //    if (tabToUpdate != null)
        //    {
        //        tabToUpdate.Header = e.Conversation.Title;
        //    }
        //}

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            // Cleanup all tabs if needed
            //foreach (var tab in ChatTabView.TabItems.OfType<TabViewItem>())
            //{
            //    if (tab.Content is Frame frame && frame.Content is ChatPage chatPage)
            //    {
            //        chatPage.NavigationCleanup();
            //    }
            //}

            ViewModel.Dispose();
        }

        private void ChatsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedChat = e.ClickedItem as ConversationViewModel;
            if (clickedChat == null) return;

            ContentFrame.Navigate(typeof(ChatPage), clickedChat.ToDto());

            //// Check if a tab for this chat already exists
            //var existingTab = ChatTabView.TabItems
            //    .OfType<TabViewItem>()
            //    // TODO: Remove Id check once VM stored in tab is updated when convo is updated
            //    .FirstOrDefault(t => t.Tag is ConversationViewModel vm && (vm == clickedChat || vm.Id == clickedChat.Id));

            //if (existingTab != null)
            //{
            //    // Tab already exists, just select it
            //    ChatTabView.SelectedItem = existingTab;
            //}
            //else
            //{
            //    var newTab = new TabViewItem
            //    {
            //        Header = clickedChat.Title,
            //        Tag = clickedChat,
            //        IconSource = new SymbolIconSource() { Symbol = Symbol.Message }
            //    };

            //    var frame = new Frame();
            //    frame.Navigate(typeof(ChatPage), clickedChat.ToDto());

            //    newTab.Content = frame;
            //    ChatTabView.TabItems.Add(newTab);
            //    ChatTabView.SelectedItem = newTab;
            //}

            //// Now we have at least one tab open
            //ViewModel.HasOpenTabs = ChatTabView.TabItems.Count > 0;
        }

        //private void ChatTabView_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        //{
        //    var tab = args.Tab as TabViewItem;
        //    if (tab?.Content is Frame frame && frame.Content is ChatPage chatPage)
        //    {
        //        chatPage.NavigationCleanup();
        //    }

        //    sender.TabItems.Remove(args.Tab);

        //    // Update HasOpenTabs after removing a tab
        //    ViewModel.HasOpenTabs = ChatTabView.TabItems.Count > 0;
        //}

        private void HidePaneButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsChatsPaneOpen = !ViewModel.IsChatsPaneOpen;
        }

        private void ChatTabView_RightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            DependencyObject sourceElement = e.OriginalSource as DependencyObject;

            while (sourceElement != null && sourceElement is not TabViewItem)
            {
                sourceElement = VisualTreeHelper.GetParent(sourceElement);
            }

            if (sourceElement is TabViewItem tabViewItem)
            {
                _selectedTab = tabViewItem;

                // Dynamically assign the predefined flyout
                // Doing this to avoid the flyout displaying in every part of the
                // tabs content which conflicts with other context menus in ChatPage
                var flyout = Resources["TabHeaderFlyout"] as MenuFlyout;
                if (flyout != null)
                {
                    flyout.ShowAt(tabViewItem, new FlyoutShowOptions
                    {
                        Position = e.GetPosition(tabViewItem)
                    });
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        //private void CloseTab_Click(object sender, RoutedEventArgs e)
        //{
        //    // TODO: Verify "CurrentConversation" changes in ChatsPage when 
        //    // a single tab is closed
        //    ChatTabView.TabItems.Remove(_selectedTab);
        //}

        //private void CloseAllTabs_Click(object sender, RoutedEventArgs e)
        //{
        //    ChatTabView.TabItems.Clear();
        //}
    }
}
