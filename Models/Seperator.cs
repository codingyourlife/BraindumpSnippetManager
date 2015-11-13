namespace SnippetManager.Models
{
    using GalaSoft.MvvmLight;
    using Interfaces;

    public class Seperator : ViewModelBase, ISnippetListItem
    {
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
    }
}
