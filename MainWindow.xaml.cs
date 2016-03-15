namespace SnippetManager
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using ViewModels;
    using Models;
    using System.ComponentModel;


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
        }

        private void ClipboardUpdate(object sender, EventArgs e)
        {
            if (this.MainViewModel.IsClipboardManager)
            {
                MainViewModel.InsertNewSnippetMethod();
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
            if (LstSnippets.SelectedIndex != -1)
            {
                if(!MainViewModel.SelectedSnippet.IsSeperator)
                {
                    var editWindow = new EditWindow((Snippet)MainViewModel.SelectedSnippet);
                    editWindow.Show();
                    editWindow.EditViewModel.SnippetToEdit.PropertyChanged += EditWindowChange;
                }
            }
        }

        private void EditWindowChange(object sender, PropertyChangedEventArgs e)
        {
            MainViewModel.SelectedSnippet = (Snippet)sender;
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
