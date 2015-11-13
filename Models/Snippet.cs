namespace SnippetManager.Models
{
    using GalaSoft.MvvmLight;
    using Interfaces;
    using System;

    public class Snippet : ViewModelBase, ISnippetListItem
    {
        private int id = 0;
        public int Id {
            get
            {
                return id;
            }
            set
            {
                id = value;
                this.RaisePropertyChanged();
            }
        }

        private string label;
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                if (value != null && value.Length > 20)
                {
                    value = value.Substring(0, 20);
                    value.Replace("\r\n", "");
                }
                this.label = value;
                this.RaisePropertyChanged();
            }
        }

        private string data;
        public string Data {
            get
            {
                return data;
            }
            set
            {
                data = value;
                this.RaisePropertyChanged();
            }
        }
        public bool IsSeperator { get { return false; } }

        public Snippet(int id, String label, String data)
        {
            this.Label = label;
            this.Data = data;
            Id = id;
        }
    }
}
