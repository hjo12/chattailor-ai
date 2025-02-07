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

namespace ChatTailorAI.WinUI.Views
{
    public sealed partial class DeleteMessagesDialog : ContentDialog
    {
        public bool IsDeleteConfirmed { get; private set; }

        public DeleteMessagesDialog()
        {
            this.InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsDeleteConfirmed = false;
            this.Hide();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            IsDeleteConfirmed = true;
            this.Hide();
        }
    }
}