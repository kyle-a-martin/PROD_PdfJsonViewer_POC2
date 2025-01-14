using PROD_PdfJsonViewer_POC.UI.Helper;
using System.IO;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace PROD_PdfJsonViewer_POC.UI.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PROD_PdfJsonViewer_POC.UI.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PROD_PdfJsonViewer_POC.UI.Controls;assembly=PROD_PdfJsonViewer_POC.UI.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:JsonEditor/>
    ///
    /// </summary>
    public class JsonEditorControl : Control
    {
        static JsonEditorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(JsonEditorControl),
                new FrameworkPropertyMetadata(typeof(JsonEditorControl)));
        }

        // Dependency Properties for external configuration
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register(
                nameof(FilePath),
                typeof(string),
                typeof(JsonEditorControl),
                new PropertyMetadata(string.Empty, FilePathPropertyChanged));

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                nameof(IsEditing),
                typeof(bool),
                typeof(JsonEditorControl),
                new PropertyMetadata(false));

        // Internal state for JSON content
        private JsonNode _jsonContent;

        // Public properties
        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        private static void FilePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as JsonEditorControl;
            if (control != null)
            {
                string? newFilePath = e.NewValue as string;
                string? oldFilePath = e.OldValue as string;
                if (newFilePath != oldFilePath)
                {
                    control.LoadJson();
                }
            }
        }

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        // Read-only access to JSON content if needed
        public JsonNode JsonContent => _jsonContent;

        // Event to notify of content changes
        public event EventHandler ContentChanged;

        // Commands for button actions
        public RelayCommand LoadCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ToggleEditCommand { get; }

        public JsonEditorControl()
        {
            LoadCommand = new RelayCommand(LoadJson);
            SaveCommand = new RelayCommand(SaveJson, CanSaveJson);
            ToggleEditCommand = new RelayCommand(ToggleEdit);
        }

        private void LoadJson()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
                {
                    MessageBox.Show("Please specify a valid file path.");
                    return;
                }

                string jsonString = File.ReadAllText(FilePath);
                var newContent = JsonNode.Parse(jsonString);

                // Update content and notify
                UpdateContent(newContent);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading JSON: {ex.Message}");
            }
        }

        private void SaveJson()
        {
            try
            {
                if (_jsonContent == null || string.IsNullOrWhiteSpace(FilePath))
                    return;

                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                string jsonString = _jsonContent.ToJsonString(options);
                File.WriteAllText(FilePath, jsonString);
                MessageBox.Show("JSON saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving JSON: {ex.Message}");
            }
        }

        private bool CanSaveJson() => _jsonContent != null;

        private void ToggleEdit() => IsEditing = !IsEditing;

        // Helper method to update content and raise change notification
        private void UpdateContent(JsonNode newContent)
        {
            _jsonContent = newContent;
            ContentChanged?.Invoke(this, EventArgs.Empty);

            // Force a refresh of the visual tree
            var template = Template;
            if (template != null)
            {
                var contentPresenter = template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null)
                {
                    contentPresenter.Content = _jsonContent;
                }
            }
        }
    }
}
