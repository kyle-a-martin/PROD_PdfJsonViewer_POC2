using FontAwesome.WPF;
using PROD_PdfJsonViewer_POC.UserControls.Enums;
using PROD_PdfJsonViewer_POC.UserControls.Models;
using System.Globalization;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UserControls.Converters
{
    internal class ValidationStatusConverter : IValueConverter
    {
        private static readonly ValidationStatusDisplay PendingStatus = new(FontAwesomeIcon.ClockOutline, "#757575", "Pending");
        private static readonly ValidationStatusDisplay ValidatedStatus = new(FontAwesomeIcon.CheckCircle, "#43a047", "Validated");
        private static readonly ValidationStatusDisplay RejectedStatus = new(FontAwesomeIcon.TimesCircle, "#e53935", "Rejected");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ValidationStatus status)
                return null;

            var statusDisplay = status switch
            {
                ValidationStatus.Pending => PendingStatus,
                ValidationStatus.Validated => ValidatedStatus,
                ValidationStatus.Rejected => RejectedStatus,
                _ => PendingStatus
            };

            // Return the specific property based on the parameter
            return parameter?.ToString()?.ToLower() switch
            {
                "icon" => statusDisplay.Icon,
                "color" => statusDisplay.Color,
                "statustext" => statusDisplay.StatusText,
                "textcolor" => statusDisplay.TextColor,
                _ => null
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
