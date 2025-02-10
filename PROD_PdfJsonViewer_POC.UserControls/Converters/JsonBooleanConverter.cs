using System.Globalization;
using System.Text.Json;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UserControls.Converters
{
    class JsonBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JsonValueKind.True)
            {
                return true;
            }
            else if (value is JsonValueKind.False) 
            { 
                return false; 
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool == true)
            {
                return JsonValueKind.True;
            }
            else
            {
                return JsonValueKind.False;
            }
        }
    }
}
