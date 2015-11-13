namespace SnippetManager.Interfaces
{
    using System;
    using System.ComponentModel;

    public interface ISnippetListItem : INotifyPropertyChanged
    {
        string Label { get; }
        string Data { get; }
        bool IsSeperator { get; }
        string ToString();

        Guid UniqueGuid { get; }
    }
}
