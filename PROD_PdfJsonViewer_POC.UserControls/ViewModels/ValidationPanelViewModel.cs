using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PROD_PdfJsonViewer_POC.UserControls.Enums;
using PROD_PdfJsonViewer_POC.UserControls.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UserControls.ViewModels
{
    public partial class ValidationPanelViewModel : ObservableObject
    {
        private readonly ICollectionView _filteredView;
        
        [ObservableProperty]
        private bool isPinned;

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private ObservableCollection<ContextFile> files = [];

        [ObservableProperty]
        private ContextFile selectedFile = new();

        [ObservableProperty]
        private string searchTerm = string.Empty;

        public ICollectionView FilteredFiles => _filteredView;

        public ValidationPanelViewModel()
        {
            _filteredView = CollectionViewSource.GetDefaultView(Files);
            _filteredView.Filter = FilterPredicate;

            this.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(SearchTerm))
                {
                    _filteredView.Refresh();
                }
            };
        }

        private bool FilterPredicate(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;

            if (item is ContextFile file)
            {
                return file.FileName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                       file.ValidationStatus.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        [RelayCommand]
        private void TogglePin()
        {
            IsPinned = !IsPinned;
        }

        [RelayCommand]
        private void Collapse()
        {
            IsExpanded = !IsExpanded;
        }

        [RelayCommand]
        private void ValidateFile(ContextFile file) 
        {
            file.ValidationDate = DateTime.Now;
            file.ValidationStatus = ValidationStatus.Validated;
            file.IsValidated = true;
        }

        [RelayCommand]
        private void RejectFile(ContextFile file)
        {
            file.ValidationDate = DateTime.Now;
            file.ValidationStatus = ValidationStatus.Rejected;
            file.IsValidated = false;
        }
    }
}
