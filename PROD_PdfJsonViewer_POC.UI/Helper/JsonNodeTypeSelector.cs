using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace PROD_PdfJsonViewer_POC.UI.Helper
{
    class JsonNodeTypeSelector : DataTemplateSelector
    {
        public DataTemplate JsonObjectTemplate { get; set; }
        public DataTemplate JsonArrayTemplate { get; set; }
        public DataTemplate JsonValueTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is JsonNode jsonNode)
            {
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
