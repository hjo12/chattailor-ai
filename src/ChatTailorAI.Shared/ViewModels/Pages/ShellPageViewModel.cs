using ChatTailorAI.Shared.Enums;
using ChatTailorAI.Shared.Services.Common;
using ChatTailorAI.Shared.Services.Common.Navigation;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChatTailorAI.Shared.ViewModels.Pages
{
    public class ShellPageViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;

        public ShellPageViewModel(
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _dialogService = dialogService;
            _navigationService = navigationService;

            NavigationViewItemInvokedCommand =
               new AsyncRelayCommand<string>(OnNavigationViewItemInvoked);

        }

        public ICommand NavigationViewItemInvokedCommand { get; set; }

        private async Task OnNavigationViewItemInvoked(string parameter)
        {
            if (parameter is string tag)
            {
                switch (tag)
                {
                    case "CreateAssistant":
                        await _dialogService.ShowCreateAssistantDialogAsync();
                        break;
                    case "NewChat":
                        await _dialogService.ShowNewChatDialogAsync();
                        break;
                    default:
                        _navigationService.NavigateTo(ConvertTagToPageKey(tag));
                        break;
                }
            }
        }

        private NavigationPageKeys ConvertTagToPageKey(string tag)
        {
            if (tag == "ChatPage") return NavigationPageKeys.ChatPage;
            if (tag == "Assistants") return NavigationPageKeys.AssistantsPage;
            if (tag == "Chats") return NavigationPageKeys.ChatsPage;
            if (tag == "Settings") return NavigationPageKeys.SettingsPage;
            if (tag == "New Image") return NavigationPageKeys.ChatPage;
            if (tag == "Prompts") return NavigationPageKeys.PromptsPage;
            if (tag == "Images") return NavigationPageKeys.ImagesPage;
            if (tag == "VoiceChat") return NavigationPageKeys.VoiceChatPage;
            return NavigationPageKeys.ChatPage;
        }
    }
}