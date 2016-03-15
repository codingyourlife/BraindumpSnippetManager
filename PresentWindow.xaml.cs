namespace SnippetManager
{
    using System.Windows;
    using ViewModels;
    using System.Windows.Controls;
    using MahApps.Metro.Controls;


    /// <summary>
    /// Interaction logic for PresentWindow.xaml
    /// </summary>
    public partial class PresentWindow : MetroWindow
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public MainViewModel MainViewModel => DataContext as MainViewModel;

        public PresentWindow()
        {
            InitializeComponent();

            int marginTop = 25;
            int marginBottom = 25;
            PresentWindowWindow.Height = SystemParameters.PrimaryScreenHeight - marginTop - marginBottom;
            PresentWindowWindow.Top = marginTop;
            PresentWindowWindow.Left = SystemParameters.PrimaryScreenWidth - PresentWindowWindow.Width;

            this.LstSnippets.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
            this.LstSnippets.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
        }

        private void PresentWindowWindow_Closed(object sender, System.EventArgs e)
        {
            this.MainViewModel.IsInPresentMode = false;
        }

        private void MoveButton(object sender, RoutedEventArgs e)
        {
            if (this.Left == 0)
                this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            else
                this.Left = 0;
        }
    }
}
