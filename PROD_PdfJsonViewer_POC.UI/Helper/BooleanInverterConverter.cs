using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UI.Helper
{
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;
    }
}
