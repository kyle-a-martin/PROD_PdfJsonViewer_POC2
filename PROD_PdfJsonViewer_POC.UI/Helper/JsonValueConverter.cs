using System.Text.Json.Nodes;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UI.Helper
{
    class JsonValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts a JsonValue to a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is JsonValue jsonValue)
            {
                return jsonValue.ToJsonString();
            }
            return string.Empty;
        }

#pragma warning disable CS8603 // Supressed because the value is being handled in the catch block
        /// <summary>
        /// Converts a string to a JsonValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string strValue && !string.IsNullOrEmpty(strValue))
            {
                try
                {
                    return JsonValue.Parse(strValue);
                }
                catch
                {
                    // If parsing fails, return the raw string as a JsonValue
                    return JsonValue.Create(strValue);
                }
            }
            return JsonValue.Create(string.Empty);
        }
    }
}
