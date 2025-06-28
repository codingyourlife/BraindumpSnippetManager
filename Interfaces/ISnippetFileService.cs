namespace SnippetManager.Interfaces
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    public interface ISnippetFileService
    {
        string SerializeSnippets(ObservableCollection<ISnippetListItemReadOnly> snippets);

        ObservableCollection<ISnippetListItemReadOnly> DeserializeSnippets(string jsonContent);

        bool SaveSnippetsToFile(ObservableCollection<ISnippetListItemReadOnly> snippets, string defaultFileName = "New Snippet");

        ObservableCollection<ISnippetListItemReadOnly> LoadSnippetsFromFile(string defaultFileName = "New Snippet");

        bool SaveSnippetsToPath(ObservableCollection<ISnippetListItemReadOnly> snippets, string filePath);

        ObservableCollection<ISnippetListItemReadOnly> LoadSnippetsFromPath(string filePath);
    }
} 