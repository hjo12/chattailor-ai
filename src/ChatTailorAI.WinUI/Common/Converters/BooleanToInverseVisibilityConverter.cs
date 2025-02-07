using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace ChatTailorAI.WinUI.Common.Converters
{
    public class BooleanToInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool boolValue = (bool)value;
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility visibility = (Visibility)value;
            return visibility != Visibility.Visible;
        }
    }


}
