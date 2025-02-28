﻿using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using ChatTailorAI.Shared.Models.Chat.OpenAI;
using ChatTailorAI.Shared.ViewModels;

namespace ChatTailorAI.WinUI.Common.Converters
{
    public class ChatImageMessageToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ChatMessageViewModel)
            {
                var chatMessage = value as ChatMessageViewModel;
                return chatMessage.IsImageMessage ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}