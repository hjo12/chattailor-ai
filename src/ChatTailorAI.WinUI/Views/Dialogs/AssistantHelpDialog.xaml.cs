using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace ChatTailorAI.WinUI.Views.Dialogs
{
    public sealed partial class AssistantHelpDialog : ContentDialog
    {
        public AssistantHelpDialog()
        {
            this.InitializeComponent();
        }

        private async void Hyperlink_Click(Microsoft.UI.Xaml.Documents.Hyperlink sender, Microsoft.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            var assistantsUri = new Uri("https://platform.openai.com/assistants");
            await Windows.System.Launcher.LaunchUriAsync(assistantsUri);
        }
    }
}
