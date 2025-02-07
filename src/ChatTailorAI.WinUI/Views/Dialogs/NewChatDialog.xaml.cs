using ChatTailorAI.Shared.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;
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
    public sealed partial class NewChatDialog : ContentDialog
    {
        public NewChatDialogViewModel ViewModel => (NewChatDialogViewModel)DataContext;

        public NewChatDialog()
        {
            this.InitializeComponent();
            ChatTypeRadioButtons.SelectedIndex = 0;

            this.DataContext =
                 App.Current.ServiceHost.Services.GetService<NewChatDialogViewModel>();
        }

        private async void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.RefreshModelsAsync();
        }
    }
}