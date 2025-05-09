using Microsoft.Extensions.DependencyInjection;
using PROD_PdfJsonViewer_POC.UI.ViewModel;
using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            //ValidationPanel.SetViewModel(validationPanelVm);

            var pdfViewerVm = ((App)Application.Current).AppHost.Services.GetRequiredService<PdfViewerViewModel>();
            PdfViewer.SetViewModel(pdfViewerVm);

            // Handle mouse wheel events at the window level
            PreviewMouseWheel += MainWindow_PreviewMouseWheel;
        }

        private void MainWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Mark the event as handled to prevent default behavior
            e.Handled = true;

            // Determine which control is under the mouse
            Point mousePosition = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, mousePosition);

            if (result == null || result.VisualHit == null)
                return;

            // Find the parent scrollviewer based on which pane contains the mouse
            ScrollViewer targetScrollViewer = null;

            // Check if mouse is over the JsonEditor
            if (IsMouseOverElement(JsonEditor, mousePosition))
            {
                // Find ScrollViewer within JsonEditor
                targetScrollViewer = FindVisualChild<ScrollViewer>(JsonEditor);
                if (targetScrollViewer == null)
                {
                    // If we can't find it directly, try to find it in the template
                    var presenter = FindVisualChild<ContentPresenter>(JsonEditor);
                    if (presenter != null)
                    {
                        targetScrollViewer = FindVisualChild<ScrollViewer>(presenter);
                    }
                }
            }
            // Check if mouse is over the PdfViewer
            else if (IsMouseOverElement(PdfViewer, mousePosition))
            {
                // Find ScrollViewer within PdfViewer
                targetScrollViewer = FindVisualChild<ScrollViewer>(PdfViewer);
            }

            // If we found a ScrollViewer, send the wheel event to it
            if (targetScrollViewer != null)
            {
                targetScrollViewer.ScrollToVerticalOffset(
                    targetScrollViewer.VerticalOffset - (e.Delta / 3));
            }
        }

        private bool IsMouseOverElement(FrameworkElement element, Point mousePosition)
        {
            if (element == null) return false;

            Rect bounds = new Rect(element.TranslatePoint(new Point(0, 0), this),
                                  new Size(element.ActualWidth, element.ActualHeight));
            return bounds.Contains(mousePosition);
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    return typedChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }

   
}
