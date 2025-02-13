using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PROD_PdfJsonViewer_POC.UserControls.Models;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UserControls.ViewModels
{
    public partial class ValidationPanelViewModel : ObservableObject
    {
        private readonly ICollectionView _filteredView;
        private readonly IJsonFileService _jsonFileService;
        private readonly ILogger<ValidationPanelViewModel> _logger;
        private const string JsonFilePath = "validation_report.json";
        private string _currentDirectory = string.Empty;

        [ObservableProperty]
        private bool isPinned;

        [ObservableProperty]
        private bool isExpanded = true;

        [ObservableProperty]
        private ObservableCollection<ContextFile> files = [];

        [ObservableProperty]
        private ContextFile? selectedFile;

        [ObservableProperty]
        private string searchTerm = string.Empty;

        public ICollectionView FilteredFiles => _filteredView;

        public ValidationPanelViewModel(IJsonFileService jsonFileService, ILogger<ValidationPanelViewModel> logger)
        {
            _jsonFileService = jsonFileService ?? throw new ArgumentNullException(nameof(jsonFileService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _filteredView = CollectionViewSource.GetDefaultView(files);
            _filteredView.Filter = FilterPredicate;

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SearchTerm))
                {
                    _filteredView.Refresh();
                }
            };
        }

        partial void OnSelectedFileChanged(ContextFile? oldValue, ContextFile? newValue)
        {
            if (newValue is not null && !string.IsNullOrEmpty(newValue.FilePath))
            {
                _currentDirectory = Path.GetDirectoryName(newValue.FilePath) ?? string.Empty;
                Task.Run(async () => await SyncronizationWithJsonReport());
            }
        }

        partial void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
            Debug.WriteLine($"OnIsExpandedChanged: {newValue}");
        }

        public async Task InitializeWithDirectoryAsync(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory Not Found: {directoryPath}");
            }

            _currentDirectory = directoryPath;
            await SyncronizationWithJsonReport();
        }

        private async Task SyncronizationWithJsonReport()
        {
            ValidationReport report;

            try
            {
                report = await _jsonFileService.LoadJsonAsync<ValidationReport>($"{_currentDirectory}/{JsonFilePath}");
                //report = JsonSerializer.Deserialize<ValidationReport>(json);
            }
            catch (FileNotFoundException)
            {
                report = new ValidationReport
                {
                    DirectoryPath = _currentDirectory,
                    Files = new List<ContextFile>(),
                    LastUpdated = DateTime.Now
                };
            }

            var updatedEntries = Files.Select(file =>
            {
                var existingEntry = report.Files.FirstOrDefault(f => f.FileName == file.FileName);
                if (file == existingEntry)
                {
                    return existingEntry;
                }
                return file;
            }).ToList();

            report.Files = updatedEntries;
            report.LastUpdated = DateTime.Now;

            //var jsonReport = JsonSerializer.Serialize(report);
            await _jsonFileService.SaveJsonAsync($"{_currentDirectory}/{JsonFilePath}", report);
        }

        private bool FilterPredicate(object item)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return true;
            }

            return item is ContextFile file &&
                   (file.FileName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    file.ValidationStatus.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
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
            file.Validate();

            _ = SyncronizationWithJsonReport();
        }

        [RelayCommand]
        private void RejectFile(ContextFile file)
        {
            file.Reject();


            _ = SyncronizationWithJsonReport();
        }
    }
}
