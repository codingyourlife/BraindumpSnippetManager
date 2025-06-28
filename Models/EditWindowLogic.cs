namespace SnippetManager.Models
{
    using System.Windows.Controls;
    using SnippetManager.Interfaces;
    using System.ComponentModel;
    using ViewModels;

    public class EditWindowLogic
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public MainViewModel MainViewModel;

        private ListBox LstSnippets;

        public EditWindowLogic(MainViewModel mainViewModel, ListBox lstSnippets)
        {
            this.LstSnippets = lstSnippets;
            this.MainViewModel = mainViewModel;
        }

        internal void OpeningRequest(ISnippetListItemReadOnly selectedSnippet)
        {
            if (LstSnippets.SelectedIndex != -1)
            {
                if (!selectedSnippet.IsSeperator)
                {
                    var editWindow = new EditWindow((Snippet)MainViewModel.SelectedSnippet);
                    editWindow.EditViewModel.SnippetToEdit.PropertyChanged += EditWindowChange;
                    editWindow.Show();
                }
            }
        }

        private void EditWindowChange(object sender, PropertyChangedEventArgs e)
        {
            MainViewModel.SelectedSnippet = (Snippet)sender;
            MainViewModel.IsDirty = true;
        }
    }
}
