namespace SnippetManager.Interfaces
{
    using System;

    /// <summary>
    /// Interface for clipboard operations to enable dependency injection and testing
    /// </summary>
    public interface IClipboardActions : IDisposable
    {
        /// <summary>
        /// Gets text from the clipboard
        /// </summary>
        /// <returns>The text content from clipboard, or null if clipboard is empty or contains non-text data</returns>
        string GetText();

        /// <summary>
        /// Sets text to the clipboard
        /// </summary>
        /// <param name="text">The text to set to clipboard</param>
        void SetText(string text);

        /// <summary>
        /// Event that fires when clipboard content is updated
        /// </summary>
        event EventHandler ClipboardUpdate;

        /// <summary>
        /// Starts listening for clipboard updates
        /// </summary>
        void StartListening();

        /// <summary>
        /// Stops listening for clipboard updates
        /// </summary>
        void StopListening();

        bool IsClipboardManagerListening { get; }
    }
} 