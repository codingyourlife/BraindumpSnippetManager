using ICSharpCode.AvalonEdit.Document;
using Newtonsoft.Json;

namespace SnippetManager.Interfaces
{
    public interface ISnippetData
    {
        string Data { get; }

        [JsonIgnore]
        TextDocument Document { get; }
    }
}
