using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PROD_PdfJsonViewer_POC.UserControls.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return null;

            // Default colors if no parameter is provided
            var trueColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976d2"));  // Blue
            var falseColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#757575")); // Gray

            // Check if custom colors are provided through parameter
            if (parameter is string customColors)
            {
                var colors = customColors.Split(':');
                if (colors.Length == 2)
                {
                    try
                    {
                        trueColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[0]));
                        falseColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[1]));
                    }
                    catch (Exception)
                    {
                        // If color parsing fails, fall back to defaults
                    }
                }
            }

            return boolValue ? trueColor : falseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
