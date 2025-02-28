﻿using System;
using System.Collections;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace ChatTailorAI.WinUI.Common.Converters
{
    public class CollectionEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var collection = value as ICollection;
            return collection == null || collection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Converting from Visibility to Collection is not supported.");
        }
    }
}
