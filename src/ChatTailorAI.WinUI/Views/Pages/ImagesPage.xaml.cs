using ChatTailorAI.Shared.ViewModels.Pages;
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
using System.Threading.Tasks;

namespace ChatTailorAI.WinUI.Views.Pages
{
    public sealed partial class ImagesPage : Page
    {
        public ImagesPageViewModel ViewModel => (ImagesPageViewModel)DataContext;

        public ImagesPage()
        {
            this.InitializeComponent();

            this.DataContext =
                App.Current.ServiceHost.Services.GetService<ImagesPageViewModel>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                await ViewModel.LoadImages();
            });
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.Dispose();
        }
    }
}
