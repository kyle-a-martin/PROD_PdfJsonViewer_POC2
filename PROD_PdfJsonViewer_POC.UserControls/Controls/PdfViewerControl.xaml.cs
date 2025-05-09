using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
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

        public PdfViewerControl(PdfViewerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += PdfViewer_Loaded;
        }

        public void SetViewModel(PdfViewerViewModel viewModel)
        {
            DataContext = viewModel;
            Loaded += PdfViewer_Loaded;
        }

        private async void PdfViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is PdfViewerViewModel viewModel)
            {
                await viewModel.InitializeWebView2Async(webView);
            }
        }

        static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PdfViewerControl pdfViewer &&
                pdfViewer.DataContext is PdfViewerViewModel viewModel)
            {
                viewModel.FilePath = (string)e.NewValue;
            }

        }
    }
}
