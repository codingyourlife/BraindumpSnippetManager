﻿namespace SnippetManager
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using MahApps.Metro.Controls;
    using ViewModels;
    using System.Windows.Controls;
    using CommonServiceLocator;
    using SnippetManager.Interfaces;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public MainViewModel MainViewModel => DataContext as MainViewModel;
        public IEditWindowService ews => ServiceLocator.Current.GetInstance<IEditWindowService>();

        public MainWindow()
        {
            InitializeComponent();

            this.Closed += MainWindow_Closed;

            //Tooltip improvements like do not hide Tooltips fix (credit: http://stackoverflow.com/a/8308254/828184)
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(0));
            ToolTipService.BetweenShowDelayProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(0));
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (MainViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Application.Current.Exit += ApplicationExit;
        }

        void ApplicationExit(object sender, ExitEventArgs e)
        {
            if (MainViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }
            
            MainViewModel.ExitMethod();
        }

        private void PresentButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.IsInPresentMode = true;
            MainViewModel.ClipboardActions.StopListening();
            MainViewModel.RaisePropertyChanged(nameof(MainViewModel.IsClipboardManager));

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
            ews.OpeningRequest(this.MainViewModel.SelectedSnippet);
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
