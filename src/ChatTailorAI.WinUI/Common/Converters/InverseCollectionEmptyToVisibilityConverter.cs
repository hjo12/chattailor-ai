using System;
using System.Collections;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace ChatTailorAI.WinUI.Common.Converters
{
    public class InverseCollectionEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var collection = value as ICollection;
            return collection == null || collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Converting from Visibility to Collection is not supported.");
        }
    }
}
