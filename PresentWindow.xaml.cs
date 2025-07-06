namespace SnippetManager
{
    using System.Windows;
    using ViewModels;
    using System.Windows.Controls;
    using MahApps.Metro.Controls;
    using Models;
    using CommonServiceLocator;


    /// <summary>
    /// Interaction logic for PresentWindow.xaml
    /// </summary>
    public partial class PresentWindow : MetroWindow
    {
        /// <summary>
        /// Access to the ViewModel.
        /// </summary>
        public MainViewModel MainViewModel => DataContext as MainViewModel;
        public IEditWindowLogic ewl => ServiceLocator.Current.GetInstance<IEditWindowLogic>();

        public PresentWindow()
        {
            InitializeComponent();

            //position and margin
            int marginTop = 25;
            int marginBottom = 25;
            PresentWindowWindow.Height = SystemParameters.PrimaryScreenHeight - marginTop - marginBottom;
            PresentWindowWindow.Top = marginTop;
            PresentWindowWindow.Left = SystemParameters.PrimaryScreenWidth - PresentWindowWindow.Width;

            //Disable ScrollBars
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

        private void CloseButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lstSnippets_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ewl.OpeningRequest(this.MainViewModel.SelectedSnippet);
        }
    }
}
