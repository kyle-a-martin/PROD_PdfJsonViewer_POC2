using FontAwesome.WPF;
using System.Windows.Media;

namespace PROD_PdfJsonViewer_POC.UserControls.Models
{
    internal class ValidationStatusDisplay
    {
        public FontAwesomeIcon Icon { get; set; }
        public Brush Color { get; set; }
        public string StatusText { get; set; }
        public Brush TextColor { get; set; }

        public ValidationStatusDisplay(FontAwesomeIcon icon, string colorhex, string statusText)
        {
            Icon = icon;
            Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorhex));
            StatusText = statusText;
            TextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorhex));
        }
    }
}
