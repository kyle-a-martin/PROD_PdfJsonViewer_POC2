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
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace PROD_PdfJsonViewer_POC.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for JsonEditorControl.xaml
    /// </summary>
    public partial class JsonEditorControl : UserControl
    {
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register(
                nameof(FilePath),
                typeof(string),
                typeof(JsonEditorControl),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnFilePathChanged));

        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JsonEditorControl control && control.DataContext is JsonEditorViewModel vm)
            {
                vm.FilePath = e.NewValue as string ?? string.Empty;
            }
        }

        public JsonEditorControl()
        {
            InitializeComponent();

            // If no DataContext is provided externally, set our own view model.
            if (!(DataContext is JsonEditorViewModel))
            {
                // Option 1: Create a new instance.
                DataContext = new JsonEditorViewModel();

                // Option 2 (if using DI): Uncomment and adjust the following line
                //DataContext = ((App)Application.Current).ServiceProvider.GetRequiredService<JsonEditorViewModel>();
            }
        }
    }
}
