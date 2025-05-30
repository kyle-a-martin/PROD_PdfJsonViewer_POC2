﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PROD_PdfJsonViewer_POC.UserControls.Models
{
    public class JsonTreeItem : ObservableObject
    {
        public string Key { get; set; } = string.Empty;
        private object? _value;
        public object? Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        public ObservableCollection<JsonTreeItem> Children { get; } = new();
        public bool HasChildren => Children.Any();
    }
}
