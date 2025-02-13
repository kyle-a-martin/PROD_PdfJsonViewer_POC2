using Microsoft.Extensions.DependencyInjection;
using PROD_PdfJsonViewer_POC.UI.ViewModel;
using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
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

            var validationPanelVm = ((App)Application.Current).AppHost.Services.GetRequiredService<ValidationPanelViewModel>();
            ValidationPanel.SetViewModel(validationPanelVm);
        }
    }
}
