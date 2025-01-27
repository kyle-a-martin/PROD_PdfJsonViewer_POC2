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
        public required DataTemplate JsonObservableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Debug.WriteLine($"SelectTemplate called with: {item?.GetType()?.Name}");

            if (item is ObservableJsonNode observableNode)
            {
                Debug.WriteLine($"SelectTemplate called with: {observableNode.Node?.GetType()?.Name}");
                return JsonObservableTemplate;
            }
            else if (item is JsonNode jsonNode)
            {
                Debug.WriteLine($"SelectTemplate called with: {jsonNode?.GetType()?.Name}");
                return SelectJsonNodeTemplate(jsonNode);
            }
            return base.SelectTemplate(item, container);
        }

        public DataTemplate SelectJsonNodeTemplate(JsonNode jsonNode)
        {
            if (jsonNode is JsonObject)
            {
                Debug.WriteLine($"SelectJsonNodeTemplate called with: {jsonNode?.GetType()?.Name}");
                Debug.WriteLine(jsonNode?.ToString());
                return JsonObjectTemplate;
            }
            else if (jsonNode is JsonArray)
            {
                Debug.WriteLine($"SelectJsonNodeTemplate called with: {jsonNode?.GetType()?.Name}");
                Debug.WriteLine(jsonNode?.ToString());
                return JsonArrayTemplate;
            }
            else if (jsonNode is JsonValue)
            {
                Debug.WriteLine($"SelectJsonNodeTemplate called with: {jsonNode?.GetType()?.Name}");
                Debug.WriteLine(jsonNode?.ToString());
                return JsonValueTemplate;
            }
            return null;
        }
    }
}
