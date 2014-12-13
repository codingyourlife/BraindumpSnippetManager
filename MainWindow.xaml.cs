using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Serialization;
using MahApps.Metro.Controls;

namespace SnippetManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
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

        private XmlSerializer serializer = new XmlSerializer(typeof(ArrayList), new Type[] { typeof(Snippet), typeof(String) });
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
            listSnippets.ContextMenu = snippetsMenu;

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

            var itemSave = new MenuItem { Header = "Save current snippets" };
            itemSave.Click += new RoutedEventHandler(ItemSaveClick);
            mainMenu.Items.Add(itemSave);

            var itemLoad = new MenuItem { Header = "Load snippets" };
            itemLoad.Click += new RoutedEventHandler(ItemLoadClick);
            mainMenu.Items.Add(itemLoad);

            var itemNew = new MenuItem { Header = "New snippets" };
            itemNew.Click += new RoutedEventHandler(ItemNewClick);
            mainMenu.Items.Add(itemNew);

            var itemExit = new MenuItem { Header = "Exit snippet manager" };
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
            listSnippets.Items.Clear();
            Snippet.counter = 0;
        }

        private void ListSnippetsMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            snippetsMenu.IsOpen = true;
        }

        void InsertSeparatorClick(object sender, RoutedEventArgs e)
        {
            listSnippets.Items.Add(new Snippet(Snippet.TheSeparatorSnippet, Snippet.TheSeparatorSnippet));
            Snippet.counter = 0;
        }

        void DeleteSnippetClick(object sender, RoutedEventArgs e)
        {
            if (listSnippets.SelectedIndex != -1)
            {
                //change the item counters to be -1. example #3 after removing #1 should become the #2
                for (int i = listSnippets.SelectedIndex; i < listSnippets.Items.Count; i++)
                {
                    if (listSnippets.Items[i] is String) // than it is the separator, after this separator the count starts from #1  so stop changes
                    {
                        break;
                    }
                    Snippet item = listSnippets.Items[i] as Snippet;
                    item.Id--;
                    listSnippets.Items.Refresh();
                }
                listSnippets.Items.RemoveAt(listSnippets.SelectedIndex);
                Snippet.counter = Snippet.counter - 1;
            }
        }

        void MoveSnippetUpClick(object sender, RoutedEventArgs e)
        {
            this.MoveSnippetUp();
        }

        void MoveSnippetDownClick(object sender, RoutedEventArgs e)
        {
            this.MoveSnippetDown();
        }

        private void MoveSnippetUp()
        {
            int selectedIndex = listSnippets.SelectedIndex;
            if (selectedIndex != -1 && selectedIndex != 0)
            { //otherwise on top allready
                //get selected item and the one to switch
                var selectedItem = (Snippet)listSnippets.Items[selectedIndex];

                //remove and re-insert those 2
                listSnippets.Items.Remove(selectedItem);
                listSnippets.Items.Insert(selectedIndex - 1, selectedItem);

                this.UpdateItemNumbers();

                listSnippets.Items.Refresh();
            }
        }

        private void MoveSnippetDown()
        {
            int selectedIndex = listSnippets.SelectedIndex;
            if (selectedIndex != -1 && selectedIndex != listSnippets.Items.Count - 1)
            { //otherwise on top allready
                //get selected item and the one to switch
                var selectedItem = (Snippet)listSnippets.Items[selectedIndex];

                //remove and re-insert those 2
                listSnippets.Items.Remove(selectedItem);
                listSnippets.Items.Insert(selectedIndex + 1, selectedItem);

                this.UpdateItemNumbers();

                listSnippets.Items.Refresh();
            }
        }

        private void UpdateItemNumbers()
        {
            //update item numbers
            var newId = 1;
            for (int i = 0; i < listSnippets.Items.Count; i++)
            {
                //get item via loop
                var item = (Snippet)listSnippets.Items[i];

                if (item.Data.Equals(Snippet.TheSeparatorSnippet))
                { //reset newId counter when separator
                    newId = 0;
                }

                if (newId != 0)
                { //update id if not separator
                    item.Id = newId;

                    listSnippets.Items.Remove(item);
                    listSnippets.Items.Insert(i, item);
                }

                newId++;
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
                    if (!listSnippets.Items.Cast<Snippet>().Where(item => item.Data == clipboardString).Any())
                    {
                        var snippet = new Snippet(clipboardString, clipboardString);
                        listSnippets.Items.Add(snippet);    
                    }
                }
            }
        }

        private void ListSnippetsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            popupHint.IsOpen = false;
            if (listSnippets.SelectedIndex != -1 && !(listSnippets.SelectedItem is string))
            {
                var selectedSnipper = listSnippets.Items[listSnippets.SelectedIndex] as Snippet;
                Clipboard.SetText(selectedSnipper.Data);
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
            ArrayList listToSerialize;
             // Configure save file dialog box
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.FileName = "New Snippet"; // Default file name
            openDialog.DefaultExt = ".xml"; // Default file extension
            openDialog.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = openDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string filename = openDialog.FileName;
                TextReader reader = new StreamReader(filename);
                listToSerialize = (ArrayList) serializer.Deserialize(reader);
                reader.Close();
                listSnippets.Items.Clear();
                foreach (Object item in listToSerialize)
                {
                    listSnippets.Items.Add(item);
                }
            }  
        }

        void ItemSaveClick(object sender, RoutedEventArgs e)
        {
            ArrayList listToSerialize=new ArrayList();

            foreach (var item in listSnippets.Items)
            {
                listToSerialize.Add(item);
            }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = "New Snippet"; // Default file name
            saveDialog.DefaultExt = ".xml"; // Default file extension
            saveDialog.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = saveDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = saveDialog.FileName;
                TextWriter writer = new StreamWriter(filename);
                serializer.Serialize(writer, listToSerialize);
                writer.Close();
            }
         
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wrapPanel.Visibility = Visibility.Hidden;
            mainWindow.Width = 13;
            mainWindow.ResizeMode = ResizeMode.NoResize;
            mainWindow.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            mainWindow.ShowMinButton = false;
            //mainWindow.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            mainWindow.Left = 0;
            mainWindow.Top = 0;

            this.IsClipboardManager = false;
        }

    }
}
