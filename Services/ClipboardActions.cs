namespace SnippetManager.Services;

using System;
using System.Windows;
using Interfaces;

/// <summary>
/// Service for handling clipboard operations
/// </summary>
public class ClipboardActions : IClipboardActions
{
    public bool IsClipboardManagerListening { get; private set; }

    /// <summary>
    /// Event that fires when clipboard content is updated
    /// </summary>
    public event EventHandler ClipboardUpdate;

    public ClipboardActions()
    {
    }

    /// <summary>
    /// Gets text from the clipboard
    /// </summary>
    /// <returns>The text content from clipboard, or null if clipboard is empty or contains non-text data</returns>
    public string GetText()
    {
        try
        {
            return Clipboard.GetText();
        }
        catch (Exception)
        {
            // Clipboard access can fail in certain scenarios
            return null;
        }
    }

    /// <summary>
    /// Sets text to the clipboard
    /// </summary>
    /// <param name="text">The text to set to clipboard</param>
    public void SetText(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            Clipboard.SetText(text);
        }
    }

    /// <summary>
    /// Starts listening for clipboard updates
    /// </summary>
    public void StartListening()
    {
        ClipboardNotification.ClipboardUpdate += OnClipboardNotificationUpdate;
        IsClipboardManagerListening = true;
    }

    /// <summary>
    /// Stops listening for clipboard updates
    /// </summary>
    public void StopListening()
    {
        ClipboardNotification.ClipboardUpdate -= OnClipboardNotificationUpdate;
        IsClipboardManagerListening = false;
    }

    /// <summary>
    /// Handles clipboard update events from the underlying ClipboardNotification
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    private void OnClipboardNotificationUpdate(object sender, EventArgs e)
    {
        ClipboardUpdate?.Invoke(this, e);
    }

    /// <summary>
    /// Disposes the service and stops listening for clipboard updates
    /// </summary>
    public void Dispose()
    {
        StopListening();
    }
}
