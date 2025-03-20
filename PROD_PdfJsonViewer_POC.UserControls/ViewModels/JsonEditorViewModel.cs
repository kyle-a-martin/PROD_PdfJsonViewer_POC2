using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PROD_PdfJsonViewer_POC.UserControls.Models;
using PROD_PdfJsonViewer_POC.UserControls.Services.Implementations;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Nodes;

namespace PROD_PdfJsonViewer_POC.UserControls.ViewModels
{
    public partial class JsonEditorViewModel : ObservableObject
    {
        private readonly IJsonFileService _jsonFileService;
        private readonly ILogger<JsonEditorViewModel> _logger;

        public JsonEditorViewModel()
        {

            _logger = new Logger<JsonEditorViewModel>(new LoggerFactory());
            _jsonFileService = new JsonFileService(new Logger<JsonFileService>(new LoggerFactory()));
        }

        public JsonEditorViewModel(IJsonFileService jsonFileService, ILogger<JsonEditorViewModel> logger)
        {
            _jsonFileService = jsonFileService;
            _logger = logger;
        }

        // Bound to the FilePath dependency property in the view.
        [ObservableProperty]
        private string filePath = string.Empty;

        // Holds the loaded JSON as a JsonNode (if needed for saving)
        [ObservableProperty]
        private JsonNode? jsonContent;

        // Represents the JSON as a hierarchical tree (for display/editing).
        [ObservableProperty]
        private ObservableCollection<JsonTreeItem> jsonTreeItems = new();

        // Edit mode toggle: when true, fields become editable.
        [ObservableProperty]
        private bool isEditable = false;

        // Busy indicator for long-running operations.
        [ObservableProperty]
        private bool isBusy = false;

        // For displaying any error messages.
        [ObservableProperty]
        private string errorMessage = string.Empty;
        partial void OnFilePathChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && File.Exists(value))
            {
                LoadAsync().ConfigureAwait(false);
            }
        }

        // Command to load JSON from the file.
        [RelayCommand]
        private async Task LoadAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ErrorMessage = "File path cannot be empty.";
                return;
            }
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var json = await _jsonFileService.LoadJsonAsync(FilePath);
                if (json is not null)
                {
                    JsonContent = json;
                    var filename = Path.GetFileName(FilePath);
                    // Generate the tree items from the JSON node.
                    JsonTreeItems = GenerateTreeItems(json, rootKey: filename);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Failed to load JSON from {FilePath}", FilePath);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Command to save the JSON to the file.
        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ErrorMessage = "File path cannot be empty.";
                return;
            }
            if (JsonContent is null)
            {
                ErrorMessage = "No JSON content to save.";
                return;
            }
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                // (Optionally, update JsonContent from JsonTreeItems before saving.)
                await _jsonFileService.SaveJsonAsync(FilePath, JsonContent);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Failed to save JSON to {FilePath}", FilePath);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Recursively converts a JsonNode into a tree of JsonTreeItem objects.
        /// </summary>
        private static ObservableCollection<JsonTreeItem> GenerateTreeItems(JsonNode? node, string key = "", string rootKey = "")
        {
            var items = new ObservableCollection<JsonTreeItem>();
            if (node is JsonObject obj)
            {
                var item = new JsonTreeItem { Key = string.IsNullOrEmpty(key) ? rootKey : key, Value = obj.Count == 0 ? "{}" : null };
                if (obj.Count > 0)
                {
                    foreach (var property in obj)
                    {
                        var childItems = GenerateTreeItems(property.Value, property.Key);
                        foreach (var child in childItems)
                        {
                            item.Children.Add(child);
                        }
                    }
                }
                items.Add(item);
            }
            else if (node is JsonArray arr)
            {
                var item = new JsonTreeItem { Key = key, Value = arr.Count == 0 ? "[]" : null };
                if (arr.Count > 0)
                {
                    for (int i = 0; i < arr.Count; i++)
                    {
                        var childItems = GenerateTreeItems(arr[i], $"[{i}]");
                        foreach (var child in childItems)
                        {
                            item.Children.Add(child);
                        }
                    }
                }
                items.Add(item);
            }
            else if (node is JsonValue value)
            {
                items.Add(new JsonTreeItem { Key = key, Value = value.ToString() });
            }
            else if (node == null)
            {
                items.Add(new JsonTreeItem { Key = key, Value = "null" });
            }
            return items;
        }
    }
}
