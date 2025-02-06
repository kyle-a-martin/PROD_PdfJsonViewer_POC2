using PROD_PdfJsonViewer_POC.UI.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace PROD_PdfJsonViewer_POC.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            //var jsonEditorVm = ((App)Application.Current).ServiceProvider?.GetRequiredService<JsonEditorViewModel>();
            //JsonEditor.DataContext = jsonEditorVm;

            // Subscribe to property changes in the view model.
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // When the PdfSource property changes, navigate the WebBrowser.
            if (e.PropertyName == nameof(MainWindowViewModel.PdfSource)
                && DataContext is MainWindowViewModel vm
                && vm.PdfSource != null)
            {
                PdfViewer.Navigate(vm.PdfSource);
            }
        }
    }
}
