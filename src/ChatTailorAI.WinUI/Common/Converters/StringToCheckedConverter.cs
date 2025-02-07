using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTailorAI.WinUI.Common.Converters
{
    public class StringToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string currentPlatform && parameter is string radioPlatform)
            {
                return currentPlatform == radioPlatform;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // If radio button is checked, parameter is the platform name.
            if (value is bool isChecked && isChecked && parameter is string radioPlatform)
            {
                return radioPlatform;
            }
            return null;
        }
    }
}
