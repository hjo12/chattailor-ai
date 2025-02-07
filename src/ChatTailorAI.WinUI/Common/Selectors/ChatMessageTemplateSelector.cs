using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using ChatTailorAI.Shared.ViewModels;

namespace ChatTailorAI.WinUI.Common.Selectors
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserMessageTemplate { get; set; }
        public DataTemplate BotMessageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var message = (ChatMessageViewModel)item;

            if (message.IsUser)
            {
                return UserMessageTemplate;
            }
            else
            {
                return BotMessageTemplate;
            }
        }
    }
}
