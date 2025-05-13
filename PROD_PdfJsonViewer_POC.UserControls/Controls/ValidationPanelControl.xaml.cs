using Microsoft.Extensions.Logging;
using PROD_PdfJsonViewer_POC.UserControls.Models;
using PROD_PdfJsonViewer_POC.UserControls.Services.Implementations;
using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        }

        public void SetViewModel(ValidationPanelViewModel viewModel)
        {
            DataContext = viewModel;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Initialize the ViewModel with current values from dependency properties
            viewModel.Files = Files;
            viewModel.SelectedFile = SelectedFile;
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
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
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
                new FrameworkPropertyMetadata(
                    new ContextFile(),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedFileChanged));

        public ContextFile SelectedFile
        {
            get => (ContextFile)GetValue(SelectedFileProperty); 
            set => SetValue(SelectedFileProperty, value); 
        }

        #endregion

        #region Commands

        private static void OnSelectedFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ValidationPanelControl)d;

            // When the dependency property changes from outside, update the ViewModel
            if (control.DataContext is ValidationPanelViewModel vm)
            {
                // Prevent circular updates by checking if the values are already in sync
                if (!ReferenceEquals(vm.SelectedFile, e.NewValue))
                {
                    vm.SelectedFile = e.NewValue as ContextFile;
                }
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

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = (ValidationPanelViewModel)sender;

            // When ViewModel's SelectedFile changes, update the dependency property
            if (e.PropertyName == nameof(ValidationPanelViewModel.SelectedFile))
            {
                // Prevent circular updates by checking if the values are already in sync
                if (!ReferenceEquals(SelectedFile, vm.SelectedFile))
                {
                    SelectedFile = vm.SelectedFile;
                }
            }
        }
        #endregion

    }
}
