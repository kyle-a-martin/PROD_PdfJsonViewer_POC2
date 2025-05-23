﻿using PROD_PdfJsonViewer_POC.UserControls.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;

namespace PROD_PdfJsonViewer_POC.UserControls.Helper
{
    public class JsonTreeItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BranchTemplate { get; set; }
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate DateTimeTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate EmptyObjectTemplate { get; set; }
        public DataTemplate EmptyArrayTemplate { get; set; }
        public DataTemplate NullTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is JsonTreeItem jsonTreeItem)
            {
                if (jsonTreeItem.Children.Count > 0)
                {
                    return BranchTemplate;
                }
                else if (jsonTreeItem.Value is string strValue)
                {
                    switch (strValue)
                    {
                        case "{}":
                            return EmptyObjectTemplate;
                        case "[]":
                            return EmptyArrayTemplate;
                        case "null":
                            return NullTemplate;
                        default:
                            return StringTemplate;
                    }
                }
                else
                {
                    // TODO: need to implement a corrected check for data type in the value of the JsonTreeItem
                    if (jsonTreeItem.Value is JsonValue jsonValue)
                    {
                        switch (jsonValue.GetValueKind())
                        {
                            case JsonValueKind.String:
                                return StringTemplate;
                            case JsonValueKind.Number:
                                if (jsonValue.TryGetValue(out DateTime _))
                                {
                                    return DateTimeTemplate;
                                }
                                return StringTemplate;
                            case JsonValueKind.True or JsonValueKind.False:
                                return BooleanTemplate;
                            default:
                                return DefaultTemplate;
                        }

                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
