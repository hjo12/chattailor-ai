﻿using ChatTailorAI.Shared.ViewModels.Dialogs;
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
    public sealed partial class EditPromptDialog : ContentDialog
    {
        public EditPromptDialogViewModel ViewModel => (EditPromptDialogViewModel)DataContext;
        public EditPromptDialog()
        {
            this.InitializeComponent();

            this.DataContext =
                 App.Current.ServiceHost.Services.GetService<EditPromptDialogViewModel>();
        }
    }
}
