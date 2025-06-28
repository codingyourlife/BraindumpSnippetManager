namespace SnippetManager.Models
{
    using System;
    using GalaSoft.MvvmLight;
    using ICSharpCode.AvalonEdit.Document;
    using Interfaces;

    public class Seperator : ViewModelBase, ISnippetListItemReadOnly
    {
        public Seperator()
        {
            this.uniqueGuid = Guid.NewGuid();
        }

        private readonly string theSeparatorSnippet = " ---------------------------- ";

        public bool IsSeperator
        {
            get
            {
                return true;
            }
        }

        public string Label
        {
            get
            {
                return this.theSeparatorSnippet;
            }
        }

        private Guid uniqueGuid;

        public Guid UniqueGuid
        {
            get
            {
                return uniqueGuid;
            }
        }

        public TextDocument Document
        {
            get
            {
                return null;
            }
        }
    }
}
