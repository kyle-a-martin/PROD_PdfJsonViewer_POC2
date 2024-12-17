using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

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
    public class JsonEditor : Control
    {
        public static DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(JsonEditor), new PropertyMetadata("", FilePathChanged));
        public static DependencyProperty IsEditModeProperty = DependencyProperty.Register("IsEditMode", typeof(bool), typeof(JsonEditor), new PropertyMetadata(false));

        private JsonDocument jsonDocument;
        private StackPanel stackPanel;
        private Button saveButton;
        private Button editButton;

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        private bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        private static void FilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var jsonEditor = (JsonEditor)d;
            jsonEditor.LoadJsonFile((string)e.NewValue);
        }

        private void LoadJsonFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                jsonDocument = JsonDocument.Parse(json);
                RenderJsonForm(jsonDocument.RootElement);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RenderJsonForm(JsonElement jsonElement)
        {
            stackPanel = new StackPanel { Orientation = Orientation.Vertical };
            this.AddLogicalChild(stackPanel);

            RenderJsonElement(jsonElement, stackPanel);

            saveButton = new Button { Content = "Save", Margin = new Thickness(5) };
            saveButton.Click += SaveButton_Click;
            stackPanel.Children.Add(saveButton);

            editButton = new Button { Content = "Edit", Margin = new Thickness(5) };
            editButton.Click += EditButton_Click;
            stackPanel.Children.Add(editButton);
        }

        private void RenderJsonElement(JsonElement jsonElement, StackPanel parentPanel)
        {
            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in jsonElement.EnumerateObject())
                {
                    StackPanel propertyPanel = new StackPanel { Orientation = Orientation.Horizontal };
                    parentPanel.Children.Add(propertyPanel);

                    Label label = new Label { Content = property.Name, Width = 150 };
                    propertyPanel.Children.Add(label);

                    if (property.Value.ValueKind == JsonValueKind.String && property.Value.GetString().Length > 150)
                    {
                        TextBox textBox = new TextBox { Text = property.Value.GetString(), Width = 300, Height = 50, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
                        textBox.IsEnabled = IsEditMode;
                        propertyPanel.Children.Add(textBox);
                    }
                    else
                    {
                        TextBox textBox = new TextBox { Text = property.Value.ToString(), Width = 300 };
                        textBox.IsEnabled = IsEditMode;
                        propertyPanel.Children.Add(textBox);
                    }
                }
            }
            else if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var element in jsonElement.EnumerateArray())
                {
                    RenderJsonElement(element, parentPanel);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TO DO: implement save logic
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            IsEditMode = true;
        }

        //static JsonEditor()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(JsonEditor), new FrameworkPropertyMetadata(typeof(JsonEditor)));
        //}
    }
}
