using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PROD_PdfJsonViewer_POC.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for PdfViewerControl.xaml
    /// </summary>
    public partial class PdfViewerControl : UserControl
    {
        // Dependency Property for FilePath to PDF to be displayed
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(PdfViewerControl), new PropertyMetadata(string.Empty, OnFilePathChanged));
        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        public PdfViewerControl()
        {
            InitializeComponent();
        }

        static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PdfViewerControl;

            if (control != null)
            {
                control.Dispatcher.InvokeAsync(() => control.PdfViewer.Navigate(e.NewValue as string ?? string.Empty));
                //Task.Run( () =>  control.PdfViewer.Navigate(e.NewValue as string ?? string.Empty));
            }
            
        }
    }
}
