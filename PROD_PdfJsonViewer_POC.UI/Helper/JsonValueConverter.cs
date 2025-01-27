using System.Globalization;
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JsonValue jsonValue)
            {
                // Try bool
                if (jsonValue.TryGetValue<bool>(out bool boolVal))
                    return boolVal.ToString();

                // Try int
                if (jsonValue.TryGetValue<int>(out int intVal))
                    return intVal.ToString();

                // Try double
                if (jsonValue.TryGetValue<double>(out double doubleVal))
                    return doubleVal.ToString(culture);

                // Try string
                if (jsonValue.TryGetValue<string>(out string strVal))
                    return strVal;

                // If we reach here, it's a primitive of some other type 
                // (or the library sees it as something else).
                // Fallback: show the raw JSON.
                return jsonValue.ToJsonString();
            }

            // If it's not a JsonValue, return empty (or handle differently).
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
                    return JsonNode.Parse(strValue);
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
