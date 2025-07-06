namespace SnippetManager.Models
{
    using SnippetManager.Interfaces;
    using System.ComponentModel;
    using ViewModels;

    public interface IEditWindowLogic
    {
        void OpeningRequest(ISnippetListItemReadOnly selectedSnippet);
    }

    public class EditWindowLogic : IEditWindowLogic
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public MainViewModel MainViewModel;


        public EditWindowLogic(MainViewModel mainViewModel)
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
