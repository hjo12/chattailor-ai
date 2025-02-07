using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using ChatTailorAI.Shared.ViewModels;

namespace ChatTailorAI.WinUI.Common.Selectors
{
    public class BotMessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BotMessageWithImageTemplate { get; set; }
        public DataTemplate BotMessageWithoutImageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ChatMessageViewModel)
            {
                var chatMessage = item as ChatMessageViewModel;

                if (chatMessage.IsImageMessage)
                {
                    return BotMessageWithImageTemplate;
                }
                else
                {
                    return BotMessageWithoutImageTemplate;
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}