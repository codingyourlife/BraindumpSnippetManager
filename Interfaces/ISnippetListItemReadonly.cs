namespace SnippetManager.Interfaces
{
    using System;
    using System.ComponentModel;

    public interface ISnippetListItemReadOnly : INotifyPropertyChanged
    {
        string Label { get; }
        bool IsSeperator { get; }
        string ToString();

        Guid UniqueGuid { get; }

    }
}
