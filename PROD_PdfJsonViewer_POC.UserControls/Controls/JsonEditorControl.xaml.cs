using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
