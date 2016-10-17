namespace SnippetManager.Interfaces
{
    using ICSharpCode.AvalonEdit.Document;
    using System;
    using System.ComponentModel;

    public interface ISnippetListItem : INotifyPropertyChanged
    {
        string Label { get; }
        string Data { get; }
        TextDocument Document { get; }
        bool IsSeperator { get; }
        string ToString();

        Guid UniqueGuid { get; }
    }
}
