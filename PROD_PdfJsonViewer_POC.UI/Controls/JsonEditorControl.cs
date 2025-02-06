using PROD_PdfJsonViewer_POC.UI.Helper;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PROD_PdfJsonViewer_POC.UI.Controls
{
    public class JsonEditorControl : Control, INotifyPropertyChanged
    {
        #region Fields and Properties

        private StackPanel _mainStackPanel;

        public static readonly DependencyProperty JsonContentProperty =
            DependencyProperty.Register(
                nameof(JsonContent),
                typeof(ObservableJsonNode),
                typeof(JsonEditorControl),
                new FrameworkPropertyMetadata(default(ObservableJsonNode), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnJsonContentChanged));

        public ObservableJsonNode JsonContent
        {
            get => (ObservableJsonNode)GetValue(JsonContentProperty);
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
            LoadCommand = new RelayCommand_old(LoadJson);
            SaveCommand = new RelayCommand_old(SaveJsonFromUI, CanSaveJson);
            ToggleEditCommand = new RelayCommand_old(ToggleEdit);
            TextChangedCommand = new RelayCommand_old(OnValueChanged);
        }

        #endregion

        #region Commands

        public RelayCommand_old LoadCommand { get; }
        public RelayCommand_old SaveCommand { get; }
        public RelayCommand_old ToggleEditCommand { get; }

        #endregion

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler ContentChanged;
        private static void OnJsonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (JsonEditorControl)d;
            var oldValue = (ObservableJsonNode)e.OldValue;
            var newValue = (ObservableJsonNode)e.NewValue;

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
        private void OnValueChanged()
        {
            RaisePropertyChanged(nameof(JsonContent));
        }

        public ICommand TextChangedCommand { get; }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(JsonContent));
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
                    JsonContent = new ObservableJsonNode
                    {
                        Node = newContent
                    };
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
                if (JsonContent?.Node is null || string.IsNullOrWhiteSpace(FilePath))
                    return;
                Debug.WriteLine($"Save JSON File...");
                Debug.WriteLine($"FilePath: {FilePath}");

                // Convert JsonNode back to a string
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonContent.Node.ToJsonString(options);
                Debug.WriteLine(jsonString);

                // Write to file
                File.WriteAllText(FilePath, jsonString);
                MessageBox.Show("JSON saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving JSON: {ex.Message}");
            }
        }

        private bool CanSaveJson() => JsonContent?.Node != null;

        private void ToggleEdit() => IsEditing = !IsEditing;

        #endregion

        #region SaveFromUI

        private JsonNode ExtractJsonFromUI(DependencyObject parent)
        {
            if (parent is TextBox textBox)
            {
                // Assuming the TextBox contains a JsonValue
                return JsonValue.Create(textBox.Text);
            }
            else if (parent is ItemsControl itemsControl)
            {
                if (itemsControl.ItemsSource is JsonArray)
                {
                    var jsonArray = new JsonArray();
                    foreach (var item in itemsControl.Items)
                    {
                        var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as DependencyObject;
                        jsonArray.Add(ExtractJsonFromUI(container));
                    }
                    return jsonArray;
                }
                else if (itemsControl.ItemsSource is JsonObject)
                {
                    var jsonObject = new JsonObject();
                    foreach (var item in itemsControl.Items)
                    {
                        var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as DependencyObject;
                        var keyTextBlock = FindChild<TextBlock>(container);
                        var valueContainer = FindChild<ContentPresenter>(container);
                        var key = keyTextBlock?.Text;
                        var value = ExtractJsonFromUI(valueContainer);
                        if (key != null && value != null)
                        {
                            jsonObject[key] = value;
                        }
                    }
                    return jsonObject;
                }
            }
            return null;
        }

        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T castChild)
                {
                    return castChild;
                }
                var foundChild = FindChild<T>(child);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
            return null;
        }

        public void SaveJsonFromUI()
        {
            // Ensure you have a named root element in your XAML, e.g., MainGrid
            var rootElement = this.FindName("RootElement") as DependencyObject;
            if (rootElement == null)
            {
                MessageBox.Show("Root element not found.");
                return;
            }

            var jsonNode = ExtractJsonFromUI(rootElement);

            // Serialize and save the JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = jsonNode.ToJsonString(options);
            File.WriteAllText(FilePath, jsonString);
            MessageBox.Show("JSON saved successfully!");
        }

        #endregion
    }

}
