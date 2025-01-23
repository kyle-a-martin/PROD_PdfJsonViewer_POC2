using PROD_PdfJsonViewer_POC.UI.Helper;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PROD_PdfJsonViewer_POC.UI.Controls
{
    public class JsonEditorControl : Control
    {
        #region Fields and Properties

        private StackPanel _mainStackPanel;

        public static readonly DependencyProperty JsonContentProperty =
            DependencyProperty.Register(
                nameof(JsonContent),
                typeof(JsonNode),
                typeof(JsonEditorControl),
                new FrameworkPropertyMetadata(default(JsonNode), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJsonContentChanged));

        public JsonNode JsonContent
        {
            get => (JsonNode)GetValue(JsonContentProperty);
            set => SetValue(JsonContentProperty, value);
        }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register(
                nameof(FilePath),
                typeof(string),
                typeof(JsonEditorControl),
                new PropertyMetadata(string.Empty, OnFilePathPropertyChanged));

        public string FilePath
        {
            get => (string)GetValue(FilePathProperty);
            set => SetValue(FilePathProperty, value);
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                nameof(IsEditing),
                typeof(bool),
                typeof(JsonEditorControl),
                new PropertyMetadata(false));

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        #endregion

        #region Constructors and Initialization

        static JsonEditorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(JsonEditorControl),
                new FrameworkPropertyMetadata(typeof(JsonEditorControl)));
        }

        public JsonEditorControl()
        {
            LoadCommand = new RelayCommand(LoadJson);
            SaveCommand = new RelayCommand(SaveJson, CanSaveJson);
            ToggleEditCommand = new RelayCommand(ToggleEdit);
        }

        #endregion

        #region Commands

        public RelayCommand LoadCommand { get; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand ToggleEditCommand { get; }

        #endregion

        #region Event Handlers

        public event EventHandler ContentChanged;
        private static void OnJsonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (JsonEditorControl)d;
            var oldValue = (JsonNode)e.OldValue;
            var newValue = (JsonNode)e.NewValue;

            Debug.WriteLine($"JsonContent changed from {oldValue} to {newValue}");
            Debug.WriteLine($"IsEditing: {control.IsEditing}");
            Debug.WriteLine($"FilePath: {control.FilePath}");

            control.ContentChanged?.Invoke(control, EventArgs.Empty);
        }

        private static void OnFilePathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //_mainStackPanel = (StackPanel)GetTemplateChild("MainStackPanel");

            //if (_mainStackPanel is null)
            //    throw new InvalidOperationException("Could not find MainStackPanel in template.");
        }

        private void LoadJson()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(FilePath) && !File.Exists(FilePath))
                {
                    MessageBox.Show("Please specify a valid file path.");
                    return;
                }

                if (File.Exists(FilePath))
                {
                    string jsonString = File.ReadAllText(FilePath);
                    var newContent = JsonNode.Parse(jsonString);

                    // Update content and notify
                    JsonContent = newContent;
                }
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
                Debug.WriteLine($"Save JSON File...");
                Debug.WriteLine($"FilePath: {FilePath}");

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

        #endregion
    }
}
