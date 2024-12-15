using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROD_PdfJsonViewer_POC.UI.Model
{
    internal class JsonTreeNode
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
