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

        public static readonly DependencyPropertyKey JsonContentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(JsonContent),
                typeof(JsonNode),
                typeof(JsonEditorControl),
                new PropertyMetadata(default(JsonNode), OnJsonContentChanged));

        public static readonly DependencyProperty JsonContentProperty = JsonContentPropertyKey.DependencyProperty;

        public JsonNode JsonContent
        {
            get => (JsonNode)GetValue(JsonContentProperty);
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
                    //UpdateContent(newContent);

                    SetValue(JsonContentPropertyKey, newContent);
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

                var content = ((JsonNode)GetValue(JsonContentProperty)).ToJsonString(options);

                // Write to file
                File.WriteAllText(FilePath, content);
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

            //_mainStackPanel.Children.Clear();
            //PopulateStackPanel(newContent, _mainStackPanel);
        }

        private void PopulateStackPanel(JsonNode jsonNode, Panel parentPanel, int level = 0)
        {
            if (jsonNode is JsonObject jsonObject)
            {
                foreach (var kvp in jsonObject)
                {
                    var border = BuildBorder(level);
                    var grid = BuildGrid();

                    var stackPanel = new StackPanel { Margin = new Thickness(10, 5, 0, 0) };
                    Grid.SetColumn(stackPanel, 1);

                    var label = new Label { Content = kvp.Key };
                    Grid.SetColumn(label, 0);
                    grid.Children.Add(label);
                    grid.Children.Add(stackPanel);

                    //stackPanel.Children.Add(label);

                    if (kvp.Value is JsonObject nestedObject)
                    {
                        PopulateStackPanel(nestedObject, stackPanel, level + 1);
                    }
                    else if (kvp.Value is JsonArray nestedArray)
                    {
                        PopulateStackPanel(nestedArray, stackPanel, level + 1);
                    }
                    else
                    {
                        var textBox = new TextBox
                        {
                            Text = kvp.Value.ToString(),
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(5, 5, 5, 5),
                            Background = System.Windows.Media.Brushes.White,
                            Style = (Style)TryFindResource("ExpandingTextBox")
                        };
                        textBox.SetBinding(TextBox.IsReadOnlyProperty, new Binding { Source = this, Path = new PropertyPath("IsEditing") });
                        stackPanel.Children.Add(textBox);
                    }
                    border.Child = grid;

                    parentPanel.Children.Add(border);
                }
            }
        }

        private void PopulateArrayPanel(JsonArray jsonArray, Panel parentPanel, int level = 0)
        {
            foreach (var item in jsonArray)
            {
                var border = BuildBorder(level);
                var grid = BuildGrid();

                var stackPanel = new StackPanel { Margin = new Thickness(2, 2, 2, 2) };
                Grid.SetColumn(stackPanel, 0);
                Grid.SetColumnSpan(stackPanel, 2);

                grid.Children.Add(stackPanel);

                if (item is JsonObject nestedObject)
                {
                    PopulateStackPanel(nestedObject, stackPanel, level + 1);
                }
                else if (item is JsonArray nestedArray)
                {
                    PopulateArrayPanel(nestedArray, stackPanel, level + 1);
                }
                else
                {
                    var textBlock = new TextBlock
                    {
                        Text = item.ToString(),
                        Background = System.Windows.Media.Brushes.White,
                        Margin = new Thickness(5, 5, 5, 5),
                        VerticalAlignment = VerticalAlignment.Center,
                        Style = (Style)FindResource("ExpandingTextBox")
                    };
                    stackPanel.Children.Add(textBlock);
                }
                border.Child = grid;

                parentPanel.Children.Add(border);
            }
        }

        private Border BuildBorder(int indent)
        {
            var border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(2, 2, 2, 2),
                Padding = new Thickness(5)
            };

            if (indent % 2 == 0)
            {
                border.Background = System.Windows.Media.Brushes.LightGray;
            }
            else
            {
                border.Background = System.Windows.Media.Brushes.White;
            }

            return border;
        }

        private Grid BuildGrid()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            return grid;
        }

        #endregion
    }
}
