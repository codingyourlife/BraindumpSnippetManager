namespace SnippetManager;

using System;

public static class ClipboardWatcher
{
    /// <summary>
    /// Raised when the clipboard content changes.
    /// </summary>
    public static event EventHandler ClipboardUpdate;

    private static bool _isListening;

    /// <summary>
    /// Starts listening for clipboard updates.
    /// </summary>
    public static void StartListening()
    {
        if (_isListening) return;

#if WINDOWS
        ClipboardWatcherWindows.ClipboardUpdate += OnPlatformClipboardUpdate;
#elif IOS || MACCATALYST || MACOS
            ClipboardWatcherMac.ClipboardUpdate += OnPlatformClipboardUpdate;
#else
            throw new PlatformNotSupportedException(
                "ClipboardWatcher is not supported on this platform.");
#endif

        _isListening = true;
    }

    /// <summary>
    /// Stops listening for clipboard updates.
    /// </summary>
    public static void StopListening()
    {
        if (!_isListening) return;

#if WINDOWS
        ClipboardWatcherWindows.ClipboardUpdate -= OnPlatformClipboardUpdate;
#elif IOS || MACCATALYST || MACOS
            ClipboardWatcherMac.ClipboardUpdate -= OnPlatformClipboardUpdate;
#else
            throw new PlatformNotSupportedException(
                "ClipboardWatcher is not supported on this platform.");
#endif

        _isListening = false;
    }

    /// <summary>
    /// True if StartListening() has been called without a matching StopListening().
    /// </summary>
    public static bool IsListening => _isListening;

    private static void OnPlatformClipboardUpdate(object sender, EventArgs e)
    {
        // bubble up the event
        ClipboardUpdate?.Invoke(null, e);
    }
}
