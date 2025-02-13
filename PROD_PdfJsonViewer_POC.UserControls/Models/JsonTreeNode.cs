using System.Collections.ObjectModel;

namespace PROD_PdfJsonViewer_POC.UserControls.Models
{
    public class JsonTreeNode
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ObservableCollection<JsonTreeNode> Children { get; set; }
        public bool IsEditable { get; set; }

        public JsonTreeNode()
        {
            Children = new ObservableCollection<JsonTreeNode>();
        }

        public JsonTreeNode(string name, string value)
        {
            Name = name;
            Value = value;
            Children = new ObservableCollection<JsonTreeNode>();
        }
    }
}
