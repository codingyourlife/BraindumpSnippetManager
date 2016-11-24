namespace SnippetManager
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using ViewModels;
    using Models;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public MainViewModel MainViewModel => DataContext as MainViewModel;

        public ClipboardNotification SnippetLogic { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //listen to clipboard
            ClipboardNotification.ClipboardUpdate += ClipboardUpdate;

            //Tooltip improvements like do not hide Tooltips fix (credit: http://stackoverflow.com/a/8308254/828184)
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(0));
            ToolTipService.BetweenShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(0));
        }

        private void ClipboardUpdate(object sender, EventArgs e)
        {
            if (this.MainViewModel.IsClipboardManager)
            {
                MainViewModel.InsertNewSnippetMethod(true);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Application.Current.Exit += ApplicationExit;
        }

        void ApplicationExit(object sender, ExitEventArgs e)
        {
            MainViewModel.ExitMethod();
        }

        private void PresentButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.IsInPresentMode = true;
            MainViewModel.IsClipboardManager = false;

            var presentWindow = new PresentWindow();
            presentWindow.Show();
            presentWindow.Closed += PresentWindowClosedAction;
            this.Hide();
        }

        private void PresentWindowClosedAction(object sender, EventArgs e)
        {
            this.Show();
        }

        private void lstSnippets_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var ewl = new EditWindowLogic(this.MainViewModel, this.LstSnippets);
            ewl.OpeningRequest(this.MainViewModel.SelectedSnippet);
        }

        private void LstSnippets_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!MainViewModel.IsInPresentMode)
            {
                if (e.Key == Key.Up)
                {
                    MainViewModel.MoveSnippetUpMethod();
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    MainViewModel.MoveSnippetDownMethod();
                    e.Handled = true;
                }
            }
        }
    }
}
