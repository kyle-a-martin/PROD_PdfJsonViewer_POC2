using PROD_PdfJsonViewer_POC.UserControls.Models;
using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PROD_PdfJsonViewer_POC.UserControls.Controls
{
    /// <summary>
    /// Interaction logic for ValidationPanelControl.xaml
    /// </summary>
    public partial class ValidationPanelControl : UserControl
    {
        public ValidationPanelControl()
        {
            InitializeComponent();

            if (!(DataContext is ValidationPanelViewModel))
            {
                DataContext = new ValidationPanelViewModel();
            }
        }

        #region Dependency Properties

        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.Register("IsPinned", typeof(bool), typeof(ValidationPanelControl), new PropertyMetadata(null));

        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ValidationPanelControl), new PropertyMetadata(null));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register(
                nameof(Files),
                typeof(ObservableCollection<ContextFile>), 
                typeof(ValidationPanelControl),
                new FrameworkPropertyMetadata(
                    new ObservableCollection<ContextFile>(),
                    OnFilesChanged));

        public ObservableCollection<ContextFile> Files
        {
            get { return (ObservableCollection<ContextFile>)GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }

        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register(
                nameof(SelectedFile),
                typeof(ContextFile), 
                typeof(ValidationPanelControl), 
                new PropertyMetadata(
                    new ContextFile(),
                    OnSelectedFileChanged));

        public ContextFile SelectedFile
        {
            get { return (ContextFile)GetValue(SelectedFileProperty); }
            set { SetValue(SelectedFileProperty, value); }
        }

        #endregion

        #region Commands

        private static void OnSelectedFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("OnSelectedFileChanged");
            Debug.WriteLine(e.NewValue);
            
            // TODO: Update the view model Selected File property when the SelectedFile property changes.
            if (d is ValidationPanelControl control && control.DataContext is ValidationPanelViewModel vm)
            {
                vm.SelectedFile = e.NewValue as ContextFile ?? new ContextFile();
            }
        }

        private static void OnFilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("OnFilesChanged");
            Debug.WriteLine(e.NewValue);
            
            if (d is ValidationPanelControl control && control.DataContext is ValidationPanelViewModel vm)
            {
                vm.Files = e.NewValue as ObservableCollection<ContextFile> ?? new ObservableCollection<ContextFile>();
            }
        }

        #endregion
    }
}
