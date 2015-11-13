namespace SnippetManager.Models
{
    using GalaSoft.MvvmLight;
    using Interfaces;
    using System;

    public class Snippet : ViewModelBase, ISnippetListItem
    {
        public Snippet()
        {
            this.uniqueGuid = Guid.NewGuid();
        }

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
                this.RaisePropertyChanged(nameof(this.ToolTipText));
            }
        }

        public string ToolTipText {
            get
            {
                return this.Label + "\r\n" + this.Data;
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
                this.RaisePropertyChanged(nameof(this.ToolTipText));
            }
        }
        public bool IsSeperator { get { return false; } }

        private Guid uniqueGuid;

        public Guid UniqueGuid
        {
            get
            {
                return uniqueGuid;
            }
        }

        public Snippet(int id, String label, String data)
        {
            this.Label = label;
            this.Data = data;
            Id = id;
        }
    }
}
