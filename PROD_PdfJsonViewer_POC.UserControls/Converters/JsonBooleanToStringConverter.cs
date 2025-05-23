﻿using System.Globalization;
using System.Text.Json;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UserControls.Converters
{
    class JsonBooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JsonValueKind.True)
            {
                return "True";
            }
            else if (value is JsonValueKind.False)
            {
                return "False";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
