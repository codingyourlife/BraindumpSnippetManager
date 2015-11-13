namespace SnippetManager.Models
{
    using System;
    using GalaSoft.MvvmLight;
    using Interfaces;

    public class Seperator : ViewModelBase, ISnippetListItem
    {
        public Seperator()
        {
            this.uniqueGuid = Guid.NewGuid();
        }

        private readonly string theSeparatorSnippet = " ---------------------------- ";

        public string Data
        {
            get
            {
                return string.Empty;
            }
        }

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
    }
}
