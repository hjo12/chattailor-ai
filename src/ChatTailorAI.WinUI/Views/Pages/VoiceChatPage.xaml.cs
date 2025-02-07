using ChatTailorAI.Shared.Services.Common;
using ChatTailorAI.Shared.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ChatTailorAI.WinUI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VoiceChatPage : Page
    {
        private VoiceChatViewModel ViewModel => DataContext as VoiceChatViewModel;

        private readonly IWindowsClipboardService windowsClipboard;

        public VoiceChatPage()
        {
            this.InitializeComponent();

            this.DataContext =
                App.Current.ServiceHost.Services.GetService<VoiceChatViewModel>();

            windowsClipboard =
                                App.Current.ServiceHost.Services.GetService<IWindowsClipboardService>();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.StartSession();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.EndSession();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
            {
                ViewModel?.SendTextMessage(MessageTextBox.Text.Trim());
                MessageTextBox.Text = string.Empty;
            }
        }

        private void CopyMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Messages != null)
            {
                foreach (var msg in ViewModel.Messages)
                {
                    CombinedTextBlock.Text += msg + "\n";
                }

                windowsClipboard.CopyToClipboard(CombinedTextBlock.Text);
            }
        }

        private void CopyTranscriptButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CopyAudioMessagesButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
