using PROD_PdfJsonViewer_POC.UI.Helper;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace PROD_PdfJsonViewer_POC.UI.Controls
{
    public class JsonEditorControl : Control
    {
        static JsonEditorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(JsonEditorControl),
                new FrameworkPropertyMetadata(typeof(JsonEditorControl)));
        }

        // 1. Define a read-only dependency property "JsonContent"
        //    We provide a "key" for internal use, plus the public DP.
        private static readonly DependencyPropertyKey JsonContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(JsonContent),
                typeof(JsonNode),
                typeof(JsonEditorControl),
                new PropertyMetadata(default(JsonNode), OnJsonContentChanged));

        public static readonly DependencyProperty JsonContentProperty =
            JsonContentPropertyKey.DependencyProperty;

        /// <summary>
        /// A read-only DP for the JSON content of the control.
        /// </summary>
        public JsonNode JsonContent
        {
            get => (JsonNode)GetValue(JsonContentProperty);
            // No public setter, because it's read-only.
        }

        private static void OnJsonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // This method is called whenever the control's JsonContent changes.
            // You could place debugging or additional logic here if needed.
            // For example:
            //
            var control = (JsonEditorControl)d;
            var oldValue = (JsonNode)e.OldValue;
            var newValue = (JsonNode)e.NewValue;

             Debug.WriteLine($"JsonContent changed from {oldValue} to {newValue}");
             Debug.WriteLine($"IsEditing: {control.IsEditing}");
             Debug.WriteLine($"FilePath: {control.FilePath}");
             
        }

        // 2. Public dependency properties for FilePath, IsEditing, etc.
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

        // 3. Public getters/setters for FilePath, IsEditing
        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        private static void FilePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is JsonEditorControl control)
            {
                var newFilePath = e.NewValue as string;
                var oldFilePath = e.OldValue as string;
                if (!string.Equals(newFilePath, oldFilePath, StringComparison.OrdinalIgnoreCase))
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

        // 4. Event to notify external code if needed
        public event EventHandler ContentChanged;

        // 5. Commands (if you want to keep them)
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
                // If there's no content or FilePath is invalid, do nothing
                if (JsonContent is null || string.IsNullOrWhiteSpace(FilePath))
                    return;

                // Convert JsonNode back to a string
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonContent.ToJsonString(options);

                // Write to file
                File.WriteAllText(FilePath, jsonString);
                MessageBox.Show("JSON saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving JSON: {ex.Message}");
            }
        }

        private bool CanSaveJson() => JsonContent != null;

        private void ToggleEdit() => IsEditing = !IsEditing;

        
        private void UpdateContent(JsonNode newContent)
        {
            // SetValue(...) is how we update a read-only DP from inside the control.
            SetValue(JsonContentPropertyKey, newContent);

            // If you still want to raise your custom event, do it here
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
