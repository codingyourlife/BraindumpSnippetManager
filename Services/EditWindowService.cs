namespace SnippetManager.Services
{
    using SnippetManager.Interfaces;
    using SnippetManager.Models;
    using System.ComponentModel;
    using ViewModels;

    public class EditWindowService : IEditWindowService
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public MainViewModel MainViewModel;


        public EditWindowService(MainViewModel mainViewModel)
        {
            this.MainViewModel = mainViewModel;
        }

        public void OpeningRequest(ISnippetListItemReadOnly selectedSnippet)
        {
            if(selectedSnippet == null)
            {
                return;
            }

            if (!selectedSnippet.IsSeperator)
            {
                var editWindow = new EditWindow((Snippet)MainViewModel.SelectedSnippet);
                editWindow.EditViewModel.SnippetToEdit.PropertyChanged += EditWindowChange;
                editWindow.Show();
            }
        }

        private void EditWindowChange(object sender, PropertyChangedEventArgs e)
        {
            MainViewModel.SelectedSnippet = (Snippet)sender;
            MainViewModel.IsDirty = true;
        }
    }
}
