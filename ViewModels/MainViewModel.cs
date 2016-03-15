namespace SnippetManager.ViewModels
{
    using System;
    using System.IO;
    using GalaSoft.MvvmLight.Command;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Models;
    using Interfaces;
    using GalaSoft.MvvmLight;
    using Newtonsoft.Json;
    using System.ComponentModel;

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.SnippetList = new ObservableCollection<ISnippetListItem>();

            InitializeMainMenu();

            this.OpenMenu = new RelayCommand(this.OpenMenuMethod);
            this.InsertNewSnippet = new RelayCommand(this.InsertNewSnippetMethod);
            this.DeleteSnippet = new RelayCommand(this.DeleteSnippetMethod);
            this.MoveSnippetUp = new RelayCommand(this.MoveSnippetUpMethod);
            this.MoveSnippetDown = new RelayCommand(this.MoveSnippetDownMethod);
            this.InsertSeperator = new RelayCommand(this.InsertSeperatorMethod);
            this.Exit = new RelayCommand(ExitMethod);
        }

        public RelayCommand Exit { get; set; }

        public RelayCommand InsertSeperator { get; set; }
        public RelayCommand MoveSnippetDown { get; set; }
        public RelayCommand MoveSnippetUp { get; set; }
        public RelayCommand DeleteSnippet { get; set; }

        public RelayCommand InsertNewSnippet { get; set; }

        private void OpenMenuMethod()
        {
            this._mainMenu.IsOpen = true;
        }

        private void SelectedSnippetChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            Snippet modifiedSnippet = (Snippet)sender;
            //this.SnippetList.First(x => x.UniqueGuid == modifiedSnippet.UniqueGuid);
            this.SelectedSnippet = modifiedSnippet;
        }

        private ObservableCollection<ISnippetListItem>  snippetList = new ObservableCollection<ISnippetListItem>();
        private int SnippetCounter = 1;

        public ObservableCollection<ISnippetListItem> SnippetList { get { return snippetList; } set { snippetList = value; } }

        public bool IsDirty { get; set; }

        private ISnippetListItem selectedSnippet;
        public ISnippetListItem SelectedSnippet
        {
            get
            {
                return selectedSnippet;
            }
            set
            {
                selectedSnippet = value;

                this.PopupHintIsOpen = false;
                if (selectedSnippet != null)
                {
                    Clipboard.SetText(selectedSnippet.Data);
                }

                this.RaisePropertyChanged();
            }
        }

        public bool IsInPresentMode { get; set; }
        public bool IsClipboardManager { get; set; }

        internal void SelectSnippet(ISnippetListItem snippetListItem)
        {
            this.SelectedSnippet = snippetListItem;
            this.SelectedSnippet.PropertyChanged += SelectedSnippetChangedEvent;
        }

        internal bool ItemWithDataExists(string data)
        {
            return this.SnippetList.Any(x => x.Data == data);
        }

        internal void SwapSnippets(int indexA, int indexB)
        {
            if (indexB >= 0 && this.SnippetList.Count > indexB)
            {
                //remember facts
                var tmpA = this.SnippetList[indexA];
                var tmpB = this.SnippetList[indexB];

                //swap elements
                this.SnippetList[indexA] = tmpB;
                this.SnippetList[indexB] = tmpA;

                this.FixIds();

                this.SelectedSnippet = this.SnippetList[indexB];
            }
        }

        private void FixIds()
        {
            var counter = 1;
            foreach (var item in this.SnippetList)
            {
                if(!item.IsSeperator)
                {
                    ((Snippet)item).Id = counter;
                    counter++;
                }
                else
                {
                    counter = 1; //start from 1 again after seperator
                }
            }

            this.SnippetCounter = counter;
        }

        internal void DeleteSnippetMethod()
        {
            if (this.SelectedSnippet != null)
            {
                this.IsDirty = true;
                this.SnippetList.Remove(this.SelectedSnippet);
                this.FixIds();
                this.RaisePropertyChanged(nameof(this.SnippetList));
            }
        }

        internal void Add(string label, string data)
        {
            this.SnippetList.Add(new Snippet(this.SnippetCounter, label, data));
            this.SnippetCounter++;
            this.RaisePropertyChanged(nameof(this.SnippetList));
        }

        internal void InsertSeperatorMethod()
        {
            this.IsDirty = true;

            this.SnippetList.Add(new Seperator());
            this.SnippetCounter = 1;
            this.RaisePropertyChanged(nameof(this.SnippetList));
        }

        internal void Clear()
        {
            this.SnippetList.Clear();
            this.RaisePropertyChanged(nameof(this.SnippetList));
        }

        internal ISnippetListItem GetItemByListId(int selectedIndex)
        {
            return this.SnippetList[selectedIndex];
        }

        internal string SerializeList()
        {
            return JsonConvert.SerializeObject(this.SnippetList, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects, TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple });
        }

        internal void DeserializeList(string fileContent)
        {
            this.SnippetList = JsonConvert.DeserializeObject<ObservableCollection<ISnippetListItem>>(fileContent, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            this.RaisePropertyChanged(nameof(this.SnippetList));
        }

        private ContextMenu _mainMenu;
        private bool _isTopmost = true;

        /// <summary>
        /// Initializes the menu.
        /// </summary>
        private void InitializeMainMenu()
        {
            _mainMenu = new ContextMenu();
            _mainMenu.Width = 200;

            var mainMenuItemAlwaysOntop = new MenuItem { Header = "Keep Always On Top" };
            mainMenuItemAlwaysOntop.IsCheckable = true;
            mainMenuItemAlwaysOntop.IsChecked = true;
            mainMenuItemAlwaysOntop.Click += ItemAlwaysOnTopClick;
            _mainMenu.Items.Add(mainMenuItemAlwaysOntop);

            var mainMenuItemActAsClipboardManager = new MenuItem { Header = "Act as clipboard manager" };
            mainMenuItemActAsClipboardManager.IsCheckable = true;
            mainMenuItemActAsClipboardManager.IsChecked = IsClipboardManager;
            mainMenuItemActAsClipboardManager.Click += ItemActAsClipboardManagerClick;
            _mainMenu.Items.Add(mainMenuItemActAsClipboardManager);

            var itemSave = new MenuItem { Header = "Save" };
            itemSave.Click += new RoutedEventHandler(ItemSaveClick);
            _mainMenu.Items.Add(itemSave);

            var itemLoad = new MenuItem { Header = "Load" };
            itemLoad.Click += new RoutedEventHandler(ItemLoadClick);
            _mainMenu.Items.Add(itemLoad);

            var itemNew = new MenuItem { Header = "New" };
            itemNew.Click += new RoutedEventHandler(ItemNewClick);
            _mainMenu.Items.Add(itemNew);

            var itemExit = new MenuItem { Header = "Exit" };
            itemExit.Click += new RoutedEventHandler(ExitItemClick);
            _mainMenu.Items.Add(itemExit);

            _mainMenu.IsOpen = false;
        }

        void ItemSaveClick(object sender, RoutedEventArgs e)
        {
            this.Export();
        }

        public void Export()
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
                string jsonToSave = this.SerializeList();

                TextWriter writer = new StreamWriter(filename);
                writer.Write(jsonToSave);
                writer.Close();

                this.IsDirty = false;
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

            // Process load file dialog box results
            if (result == true)
            {
                string filename = openDialog.FileName;
                TextReader reader = new StreamReader(filename);

                //render json
                var fileContent = reader.ReadToEnd();
                this.DeserializeList(fileContent);
                this.SelectedSnippet = null;

                reader.Close();
            }
        }
        private void ExitItemClick(object sender, RoutedEventArgs e)
        {
            this.ExitMethod();
        }

        public void ExitMethod()
        {
            if (this.IsDirty)
            {
                this.Export();
            }

            Environment.Exit(0);
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
                if (this.IsClipboardManager)
                {
                    item.IsChecked = false;
                    this.IsClipboardManager = false;
                }
                else
                {
                    item.IsChecked = true;
                    this.IsClipboardManager = true;
                }
            }
        }

        void ItemAlwaysOnTopClick(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            this.IsTopmost = !this.IsTopmost;
            if (item != null) item.IsChecked = this.IsTopmost;
        }

        private void ClearSnippetsList()
        {
            this.Clear();
        }

        private void AddSnippet(string clipboardString)
        {
            this.Add(clipboardString, clipboardString);
        }

        public bool IsTopmost
        {
            get { return _isTopmost; }
            set
            {
                _isTopmost = value; 
                this.RaisePropertyChanged(nameof(IsTopmost));
            }
        }

        public RelayCommand OpenMenu { get; private set; }

        void InsertSnippetClick(object sender, RoutedEventArgs e)
        {
            InsertNewSnippetMethod();
        }

        public void InsertNewSnippetMethod()
        {
            this.IsDirty = true;
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
                if (clipboardString != null)
                {
                    if (!this.ItemWithDataExists(clipboardString))
                    {
                        this.AddSnippet(clipboardString);
                    }
                }
            }
        }

        public void MoveSnippetUpMethod()
        {
            this.IsDirty = true;

            int selectedIndex = this.SnippetList.IndexOf(this.SelectedSnippet);
            if (selectedIndex != -1 && selectedIndex != 0)
            {
                this.SwapSnippets(selectedIndex, selectedIndex - 1);
            }
        }

        public void MoveSnippetDownMethod()
        {
            this.IsDirty = true;

            int selectedIndex = this.SnippetList.IndexOf(this.SelectedSnippet);
            if (this.SelectedSnippet != null)
            {
                this.SwapSnippets(selectedIndex, selectedIndex + 1);
            }
        }

        public bool PopupHintIsOpen { get; set; }
    }
}
