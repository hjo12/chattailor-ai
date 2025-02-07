using ChatTailorAI.WinUI.Views.Pages;
using Microsoft.UI.Xaml;

namespace ChatTailorAI.WinUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            Content = new ShellPage();
        }
    }
}