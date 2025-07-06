namespace SnippetManager.ViewModels;

using GalaSoft.MvvmLight;
using SnippetManager.Models;

public class EditViewModel :ViewModelBase
{
    private Snippet snippetToEdit;

    public Snippet SnippetToEdit {
        get
        {
            return snippetToEdit;
        }
        set
        {
            snippetToEdit = value;
            this.RaisePropertyChanged();
        }
    }
}
