using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UI.Helper
{
    class PathToFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.IO.Path.GetFileName((string)value);
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }
}
