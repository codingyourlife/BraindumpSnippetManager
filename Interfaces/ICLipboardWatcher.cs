namespace SnippetManager.Interfaces;

using System;

public interface IClipboardWatcher
{
    static event EventHandler ClipboardUpdate;
}
