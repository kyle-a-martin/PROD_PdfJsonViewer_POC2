using System.ComponentModel;
using System.Text.Json.Nodes;

namespace PROD_PdfJsonViewer_POC.UI.Helper
{
    public class ObservableJsonNode : INotifyPropertyChanged
    {
        private JsonNode _node;
        public JsonNode Node
        {
            get => _node;
            set
            {
                if (_node != value)
                {
                    _node = value;
                    OnPropertyChanged(nameof(Node));
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public string Value
        {
            get => _node?.ToString();
            set
            {
                if (_node is JsonValue jsonValue)
                {
                    _node = JsonValue.Create(value);
                    OnPropertyChanged(nameof(Value));
                }
                else
                {
                    // Handle other JsonNode types if necessary
                }
                OnPropertyChanged(nameof(Node));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
