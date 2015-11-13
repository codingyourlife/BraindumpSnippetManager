namespace SnippetManager
{
    using Models;
    using ViewModels;
    using System.Windows;

    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public EditViewModel EditViewModel => DataContext as EditViewModel;

        public EditWindow(Snippet snippetToEdit)
        {
            InitializeComponent();

            this.EditViewModel.SnippetToEdit = snippetToEdit;
        }
    }
}
