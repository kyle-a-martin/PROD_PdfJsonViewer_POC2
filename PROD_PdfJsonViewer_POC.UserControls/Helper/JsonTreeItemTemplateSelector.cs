using PROD_PdfJsonViewer_POC.UserControls.Models;
using System.Windows;
using System.Windows.Controls;

namespace PROD_PdfJsonViewer_POC.UserControls.Helper
{
    public class JsonTreeItemTemplateSelector : DataTemplateSelector
    {
        // Standard template for a leaf (value node)
        public DataTemplate LeafTemplate { get; set; }
        // Template for a group (object/array) with multiple children
        public DataTemplate GroupTemplate { get; set; }
        // Template for a group that contains exactly one leaf child (to flatten the header)
        public DataTemplate FlattenedLeafTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is JsonTreeItem jsonItem)
            {
                // If there are children...
                if (jsonItem.Children != null && jsonItem.Children.Count > 0)
                {
                    // If there's exactly one child and that child is a leaf (has no children),
                    // then return the flattened template so that the parent's header is merged with the child's data.
                    if (jsonItem.Children.Count == 1 &&
                        (jsonItem.Children[0].Children == null || jsonItem.Children[0].Children.Count == 0))
                    {
                        return FlattenedLeafTemplate;
                    }
                    // Otherwise, use the standard group template.
                    return GroupTemplate;
                }
                else
                {
                    // No children: it's a leaf.
                    return LeafTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
