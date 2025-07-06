namespace SnippetManager.Interfaces;

using ICSharpCode.AvalonEdit.Document;
using Newtonsoft.Json;
public interface ISnippetData
{
    string Data { get; }

    [JsonIgnore]
    TextDocument Document { get; }
}
