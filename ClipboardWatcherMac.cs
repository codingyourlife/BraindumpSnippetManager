#if IOS || MACCATALYST || MACOS
using SnippetManager.Interfaces;
using System;
using System.Timers;
#if IOS || MACCATALYST
using UIKit;
#elif MACOS
using AppKit;
#endif

namespace SnippetManager
{
    /// <summary>
    /// Watches the system pasteboard on Apple platforms by polling its ChangeCount.
    /// </summary>
    public sealed class ClipboardWatcherMac : IClipboardWatcher, IDisposable
    {
        /// <summary>
        /// Fires whenever the OS pasteboard’s change count increases.
        /// </summary>
        public static event EventHandler ClipboardUpdate;

        // singleton to start automatically on app launch
        private static readonly ClipboardWatcherMac _instance = new ClipboardWatcherMac();

        private readonly ITimer _timer;
        private readonly IPasteboard _pasteboard;
        private int _lastChangeCount;

        /// <summary>
        /// Default ctor: polls every 500 ms.
        /// </summary>
        /// <param name="pollIntervalMs">Must be &gt; 0.</param>
        /// <param name="pasteboard">Test‐injectable; defaults to native pasteboard.</param>
        public ClipboardWatcherMac(int pollIntervalMs = 500, IPasteboard pasteboard = null)
            : this(pollIntervalMs, pasteboard, new SystemTimerAdapter(pollIntervalMs))
        {
        }

        /// <summary>
        /// For testing: inject both a fake pasteboard and fake timer.
        /// </summary>
        internal ClipboardWatcherMac(int pollIntervalMs, IPasteboard pasteboard, ITimer timer)
        {
            if (pollIntervalMs <= 0)
                throw new ArgumentException("Poll interval must be positive", nameof(pollIntervalMs));

            _pasteboard = pasteboard ?? new NativePasteboardWrapper();
            _lastChangeCount = _pasteboard.ChangeCount;

            _timer = timer ?? throw new ArgumentNullException(nameof(timer));
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            var current = _pasteboard.ChangeCount;
            if (current == _lastChangeCount) 
                return;              // early return if no change

            _lastChangeCount = current;
            ClipboardUpdate?.Invoke(null, EventArgs.Empty);
        }

        public void Dispose()
        {
            _timer.Stop();
            (_timer as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// Abstracts the native pasteboard so you can stub ChangeCount in tests.
    /// </summary>
    public interface IPasteboard
    {
        int ChangeCount { get; }
    }

    /// <summary>
    /// Picks the right native API under the hood.
    /// </summary>
    internal class NativePasteboardWrapper : IPasteboard
    {
        public int ChangeCount
        {
            get
            {
#if IOS || MACCATALYST
                return UIPasteboard.General.ChangeCount;
#elif MACOS
                return (int)NSPasteboard.GeneralPasteboard.ChangeCount;
#else
                throw new PlatformNotSupportedException();
#endif
            }
        }
    }

    /// <summary>
    /// Timer abstraction so you can fire Elapsed manually in tests.
    /// </summary>
    internal interface ITimer
    {
        event EventHandler Elapsed;
        void Start();
        void Stop();
    }

    internal class SystemTimerAdapter : ITimer, IDisposable
    {
        private readonly Timer _timer;
        public event EventHandler Elapsed;

        public SystemTimerAdapter(double intervalMs)
        {
            _timer = new Timer(intervalMs) { AutoReset = true };
            _timer.Elapsed += (_, __) => Elapsed?.Invoke(this, EventArgs.Empty);
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
        public void Dispose() => _timer.Dispose();
    }
}
#endif
