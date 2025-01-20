using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace PROD_PdfJsonViewer_POC.UI.Helper
{
    class JsonNodeTypeSelector : DataTemplateSelector
    {
        public required DataTemplate JsonObjectTemplate { get; set; }
        public required DataTemplate JsonArrayTemplate { get; set; }
        public required DataTemplate JsonValueTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Debug.WriteLine($"SelectTemplate called with: {item?.GetType()?.Name}");

            if (item is JsonNode jsonNode)
            {
                Debug.WriteLine($"JsonNode kind: {jsonNode.GetValueKind().ToString()}");
                return jsonNode switch
                {
                    JsonObject _ => JsonObjectTemplate,
                    JsonArray _ => JsonArrayTemplate,
                    JsonValue _ => JsonValueTemplate,
                    _ => base.SelectTemplate(item, container)
                };
            }
            return base.SelectTemplate(item, container);
        }
    }
}
