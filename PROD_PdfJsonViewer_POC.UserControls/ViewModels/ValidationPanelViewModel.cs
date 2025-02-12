using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PROD_PdfJsonViewer_POC.UserControls.Enums;
using PROD_PdfJsonViewer_POC.UserControls.Models;
using PROD_PdfJsonViewer_POC.UserControls.Services.Implementations;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PROD_PdfJsonViewer_POC.UserControls.ViewModels
{
    public partial class ValidationPanelViewModel : ObservableObject
    {
        private readonly ICollectionView _filteredView;
        private readonly IJsonFileService _jsonFileService;
        private ILogger<ValidationPanelViewModel> _logger;
        private readonly string _jsonFilePath = "validation_report.json";
        private string _currentDirectory;
        
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
            _logger = new Logger<ValidationPanelViewModel>(new LoggerFactory());
            _jsonFileService = new JsonFileService(new Logger<JsonFileService>(new LoggerFactory()));
            Files = new ObservableCollection<ContextFile>();
            _filteredView = CollectionViewSource.GetDefaultView(Files);
            _filteredView.Filter = FilterPredicate;
            _currentDirectory = string.Empty;

            isExpanded = true;

            this.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(SearchTerm))
                {
                    _filteredView.Refresh();
                }
            };
        }

        partial void OnSelectedFileChanged(ContextFile? oldValue, ContextFile newValue)
        {
            Debug.WriteLine("OnFilesChanged");
            
            if (newValue is not null)
            {
                Debug.WriteLine($"OnFilesChanged: {newValue.FilePath}");
                
                if (newValue != oldValue)
                {
                    Debug.WriteLine("OnFilesChanged: newValue != oldValue");
                    _currentDirectory = Path.GetDirectoryName(newValue.FilePath);
                    Task.Run(async () => await SyncronizationWithJsonReport());
                }
            }
        }

        partial void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
            Debug.WriteLine($"OnIsExpandedChanged: {newValue}");
        }

        public async Task InitializeWithDirectoryAsync(string directoryPath)
        {
            string reportPath = $"{directoryPath}";
            
            if (!Directory.Exists(reportPath))
            {
                throw new DirectoryNotFoundException($"Directory Not Found: {directoryPath}");
            }

            _currentDirectory = reportPath;

            await SyncronizationWithJsonReport();
        }

        private async Task SyncronizationWithJsonReport()
        {
            Debug.WriteLine("SyncronizationWithJsonReport");
            
            ValidationReport? report;

            try
            {
                // Load the validation report from the JSON file.
                var json = await _jsonFileService.LoadJsonAsync($"{_currentDirectory}{_jsonFilePath}");
                report = JsonSerializer.Deserialize<ValidationReport>(json);
            }
            catch (FileNotFoundException ex)
            {   
                report = new ValidationReport()
                {
                    DirectoryPath = _currentDirectory,
                    Files = new List<ContextFile>(),
                    LastUpdated = DateTime.Now
                };
            }

            // convert ObservableCollection<ContextFile> to List<ContextFile>
            var currentFiles = Files.AsEnumerable().ToList();

            var updatedEntries = new List<ContextFile>();

            // Update report based on current file group supplied from main window.
            foreach (var file in currentFiles)
            {
                var existingEntry = report.Files.FirstOrDefault(f => f.FileName == file.FileName);
                if (existingEntry != null)
                {
                    updatedEntries.Add(existingEntry);
                }
                else
                {
                    updatedEntries.Add(file);
                }
            }

            report.Files = updatedEntries;
            report.LastUpdated = DateTime.Now;

            var jsonReport = JsonSerializer.Serialize(report);
            await _jsonFileService.SaveJsonAsync($"{_currentDirectory}/reports/{_jsonFilePath}", jsonReport);
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
            Debug.WriteLine("Collapse");
            IsExpanded = IsExpanded ? false : true;
            Debug.WriteLine($"IsExpanded: {IsExpanded}");
        }

        [RelayCommand]
        private static void ValidateFile(ContextFile file) 
        {
            file.ValidationDate = DateTime.Now;
            file.ValidationStatus = ValidationStatus.Validated;
            file.IsValidated = true;
        }

        [RelayCommand]
        private static void RejectFile(ContextFile file)
        {
            file.ValidationDate = DateTime.Now;
            file.ValidationStatus = ValidationStatus.Rejected;
            file.IsValidated = false;
        }
    }
}
