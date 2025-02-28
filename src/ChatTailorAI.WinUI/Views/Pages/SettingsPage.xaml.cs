﻿using ChatTailorAI.Shared.Models.Shared;
using ChatTailorAI.Shared.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ChatTailorAI.WinUI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            this.DataContext =
                App.Current.ServiceHost.Services.GetService<SettingsPageViewModel>();
        }

        public SettingsPageViewModel ViewModel => (SettingsPageViewModel)this.DataContext;

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateBack();
        }

        private void SetPasswordRevealMode(CheckBox checkBox, PasswordBox passwordBox)
        {
            passwordBox.PasswordRevealMode = checkBox.IsChecked == true ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden;
        }

        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            SetPasswordRevealMode(revealModeCheckBox, apiKeyBox);
            SetPasswordRevealMode(revealModeCheckBox1, azureApiKeyBox);
            SetPasswordRevealMode(revealModeCheckBox2, elevenLabsApiKeyBox);
            SetPasswordRevealMode(revealModeCheckBox3, AnthropicApiKeyBox);
            SetPasswordRevealMode(revealModeCheckBox4, GoogleAIApiKeyBox);
        }

        private void apiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenAIApiKey = (sender as PasswordBox).Password;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadModelsAsync();
            await ViewModel.LoadVoicesAsync();
            await ViewModel.LoadPromptsAsync();
            await ViewModel.LoadSpeechModelsAsync();
        }

        private void functionsListView_Loaded(object sender, RoutedEventArgs e)
        {
            // HACK
            // Trying with ItemContainerStyle it kept overriding default and 
            // no way to set it back to WinUI2 default
            var listView = sender as ListView;

            if (listView != null && ViewModel != null)
            {
                foreach (var group in ViewModel.GroupInfoCollection)
                {
                    foreach (FunctionListItem item in group.Items)
                    {
                        if (item.IsSelected)
                        {
                            listView.SelectedItems.Add(item);
                        }
                    }
                }
            }
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            if (this.DeletePromptButton.Flyout is Flyout f)
                f.Hide();
        }

        private void OnNo2Click(object sender, RoutedEventArgs e)
        {
            if (this.DeletePromptsButton.Flyout is Flyout f)
                f.Hide();
        }

        private async void DeletePromptButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeletePromptButton.Flyout is Flyout f)
                f.Hide();

            await ViewModel.DeletePrompt();
        }

        private async void DeletePromptsButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeletePromptsButton.Flyout is Flyout f)
                f.Hide();

            await ViewModel.DeletePrompts();
        }
    }
}
