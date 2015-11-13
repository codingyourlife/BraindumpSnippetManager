namespace SnippetManager
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
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

        private HwndSource source = null;
        private IntPtr nextClipboardViewer;
       
        private IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }

        private bool IsClipboardManager { get; set; }

        private ContextMenu mainMenu;
        private ContextMenu snippetsMenu;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMainMenu();
            InitializeContextMenu();
            IsClipboardManager = false;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);

            Application.Current.Exit += ApplicationExit;
        }

        /// <summary>
        /// Initializes the context menu.
        /// </summary>
        private void InitializeContextMenu()
        {
            snippetsMenu = new ContextMenu();
            snippetsMenu.IsOpen = false;
            lstSnippets.ContextMenu = snippetsMenu;

            var itemInsertSnippet = new MenuItem { Header = "insert new snippet from clipboard" };
            itemInsertSnippet.Click += new RoutedEventHandler(InsertSnippetClick);
            snippetsMenu.Items.Add(itemInsertSnippet);

            var itemDeleteSnippet = new MenuItem { Header = "delete selected snippet" };
            itemDeleteSnippet.Click += new RoutedEventHandler(DeleteSnippetClick);
            snippetsMenu.Items.Add(itemDeleteSnippet);

            var itemMoveSnippetUp = new MenuItem { Header = "move selected snippet up" };
            itemMoveSnippetUp.Click += new RoutedEventHandler(MoveSnippetUpClick);
            snippetsMenu.Items.Add(itemMoveSnippetUp);

            var itemMoveSnippetDown = new MenuItem { Header = "move selected snippet down" };
            itemMoveSnippetDown.Click += new RoutedEventHandler(MoveSnippetDownClick);
            snippetsMenu.Items.Add(itemMoveSnippetDown);
            
            var itemInsertSeparator = new MenuItem { Header = "insert separator" };
            itemInsertSeparator.Click += new RoutedEventHandler(InsertSeparatorClick);
            snippetsMenu.Items.Add(itemInsertSeparator);
        }

        /// <summary>
        /// Initializes the menu.
        /// </summary>
        private void InitializeMainMenu()
        {
            mainMenu = new ContextMenu();
            mainMenu.Width = 200; 

            var mainMenuItemAlwaysOntop = new MenuItem { Header = "Keep Always On Top" };
            mainMenuItemAlwaysOntop.IsCheckable = true;
            mainMenuItemAlwaysOntop.IsChecked = true;
            mainMenuItemAlwaysOntop.Click += ItemAlwaysOnTopClick;
            mainMenu.Items.Add(mainMenuItemAlwaysOntop);

            var mainMenuItemActAsClipboardManager = new MenuItem { Header = "Act as clipboard manager" };
            mainMenuItemActAsClipboardManager.IsCheckable = true;
            mainMenuItemActAsClipboardManager.IsChecked = IsClipboardManager;
            mainMenuItemActAsClipboardManager.Click += ItemActAsClipboardManagerClick;
            mainMenu.Items.Add(mainMenuItemActAsClipboardManager);

            var itemSave = new MenuItem { Header = "Save" };
            itemSave.Click += new RoutedEventHandler(ItemSaveClick);
            mainMenu.Items.Add(itemSave);

            var itemLoad = new MenuItem { Header = "Load" };
            itemLoad.Click += new RoutedEventHandler(ItemLoadClick);
            mainMenu.Items.Add(itemLoad);

            var itemNew = new MenuItem { Header = "New" };
            itemNew.Click += new RoutedEventHandler(ItemNewClick);
            mainMenu.Items.Add(itemNew);

            var itemExit = new MenuItem { Header = "Exit" };
            itemExit.Click += new RoutedEventHandler(ItemExitClick);
            mainMenu.Items.Add(itemExit);

            mainMenu.PlacementTarget = this;
            mainMenu.IsOpen = false;
        }

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (IsClipboardManager)
                    {
                        InsertCurrentClipboardDataToSnippets();
                    }
                    SendMessage(nextClipboardViewer, msg, wParam, lParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (wParam == nextClipboardViewer)
                        nextClipboardViewer = lParam;
                    else
                        SendMessage(nextClipboardViewer, msg, wParam, lParam);
                    break;
            }
            return IntPtr.Zero;
        }

        void ApplicationExit(object sender, ExitEventArgs e)
        {
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
        }

        private void ClearSnippetsList()
        {
            MainViewModel.Clear();
        }

        private void ListSnippetsMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            snippetsMenu.IsOpen = true;
        }

        void InsertSeparatorClick(object sender, RoutedEventArgs e)
        {
            MainViewModel.AddSeperator();
        }

        void DeleteSnippetClick(object sender, RoutedEventArgs e)
        {
            if (lstSnippets.SelectedIndex != -1)
            {
                MainViewModel.RemoveSelected();
            }
        }

        void MoveSnippetUpClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = lstSnippets.SelectedIndex;
            if (selectedIndex != -1 && selectedIndex != 0)
            {
                MainViewModel.SwapSnippets(selectedIndex, selectedIndex - 1);
            }
        }

        void MoveSnippetDownClick(object sender, RoutedEventArgs e)
        {
            int selectedIndex = lstSnippets.SelectedIndex;
            if (selectedIndex != -1 && selectedIndex != lstSnippets.Items.Count - 1)
            {
                MainViewModel.SwapSnippets(selectedIndex, selectedIndex + 1);
            }
        }

        void InsertSnippetClick(object sender, RoutedEventArgs e)
        {
            InsertCurrentClipboardDataToSnippets();
        }
  
        private void InsertCurrentClipboardDataToSnippets()
        {
            string clipbaordData = null;
            try
            {
                clipbaordData = Clipboard.GetText();
            }
            catch (Exception)
            {
                //this failed at least once...
            }

            if (clipbaordData != null)
            {
                var clipboardString = clipbaordData as String;
                if(clipboardString != null)
                {
                    if (!MainViewModel.ItemWithDataExists(clipboardString))
                    {
                        this.AddSnippet(clipboardString);
                    }
                }
            }
        }

        private void AddSnippet(string clipboardString)
        {
            MainViewModel.Add(clipboardString, clipboardString);
        }

        private void ListSnippetsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            popupHint.IsOpen = false;
            if (lstSnippets.SelectedIndex != -1 && !(lstSnippets.SelectedItem is string))
            {
                MainViewModel.SelectSnippet(MainViewModel.GetItemByListId(lstSnippets.SelectedIndex));
                Clipboard.SetText(MainViewModel.SelectedSnippet.Data);
            }
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            mainMenu.IsOpen = true;
        }

        void ItemExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void ItemNewClick(object sender, RoutedEventArgs e)
        {
            ClearSnippetsList();
        }

        void ItemActAsClipboardManagerClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item != null)
            {
                if(IsClipboardManager)
                {
                    item.IsChecked = false;
                    IsClipboardManager = false;
                }
                else 
                {
                    item.IsChecked= true;
                    IsClipboardManager = true;
                }
            }
        }

        void ItemAlwaysOnTopClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (this.Topmost)
            {
                this.Topmost = false;
                if (item != null)
                {
                    item.IsChecked = false;
                }
            }
            else
            {
                this.Topmost = true;
                if (item != null)
                {
                    item.IsChecked = true; 
                }
            }
        }

        void ItemLoadClick(object sender, RoutedEventArgs e)
        { 
             // Configure save file dialog box
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.FileName = "New Snippet"; // Default file name
            openDialog.DefaultExt = ".json"; // Default file extension
            openDialog.Filter = "JSON documents (.json)|*.json"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = openDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string filename = openDialog.FileName;
                TextReader reader = new StreamReader(filename);

                //render json
                var fileContent = reader.ReadToEnd();
                MainViewModel.DeserializeList(fileContent);
                MainViewModel.SelectedSnippet = null;
            }  
        }

        void ItemSaveClick(object sender, RoutedEventArgs e)
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = "New Snippet"; // Default file name
            saveDialog.DefaultExt = ".json"; // Default file extension
            saveDialog.Filter = "JSON documents (.json)|*.json"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = saveDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = saveDialog.FileName;

                //serialize list as JSON
                string jsonToSave = MainViewModel.SerializeList();

                TextWriter writer = new StreamWriter(filename);
                writer.Write(jsonToSave);
                writer.Close();
            }
         
        }

        private void PresentButton_Click(object sender, RoutedEventArgs e)
        {
            ButtonBar.Visibility = Visibility.Hidden;
            mainWindow.ShowMinButton = false;
            mainWindow.ResizeMode = ResizeMode.NoResize;
            mainWindow.UseNoneWindowStyle = true;
            mainWindow.WindowStyle = WindowStyle.ToolWindow;
            mainWindow.ShowInTaskbar = false;
            mainWindow.lstSnippets.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);

            int marginTop = 25;
            int marginBottom = 25;
            mainWindow.Width = 13;

            mainWindow.MaxWidth = MinWidth;
            mainWindow.Height = SystemParameters.PrimaryScreenHeight - marginTop - marginBottom;
            mainWindow.Top = marginTop;
            mainWindow.Left = SystemParameters.PrimaryScreenWidth - mainWindow.Width;

            this.IsClipboardManager = false;
        }

        private void lstSnippets_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstSnippets.SelectedIndex != -1)
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
    }
}
