namespace SnippetManager.ViewModels;

using System;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using Models;
using Interfaces;
using GalaSoft.MvvmLight;
using System.ComponentModel;

public class MainViewModel : ViewModelBase, IDisposable
{
    public IClipboardActions ClipboardActions { get; }
    public ISnippetFileService FileService { get; }
    private bool _disposed = false;

    public MainViewModel(IClipboardActions clipboardActions, ISnippetFileService fileService)
    {
        ClipboardActions = clipboardActions ?? throw new ArgumentNullException(nameof(clipboardActions));
        FileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        
        this.SnippetList = new ObservableCollection<ISnippetListItemReadOnly>();

        InitializeMainMenu();

        this.OpenMenu = new RelayCommand(this.OpenMenuMethod);
        this.InsertNewSnippet = new RelayCommand<bool>(this.InsertNewSnippetMethod);
        this.DeleteSnippet = new RelayCommand(this.DeleteSnippetMethod);
        this.MoveSnippetUp = new RelayCommand(this.MoveSnippetUpMethod);
        this.MoveSnippetDown = new RelayCommand(this.MoveSnippetDownMethod);
        this.InsertSeperator = new RelayCommand(this.InsertSeperatorMethod);
        this.Exit = new RelayCommand(ExitMethod);

        ClipboardActions.ClipboardUpdate += OnClipboardUpdate;
    }

    private void OnClipboardUpdate(object sender, EventArgs e)
    {
        if (this.IsClipboardManager)
        {
            this.InsertNewSnippetMethod(true);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (ClipboardActions != null)
            {
                ClipboardActions.ClipboardUpdate -= OnClipboardUpdate;
                ClipboardActions.Dispose();
            }
            _disposed = true;
        }
    }

    public RelayCommand Exit { get; set; }

    public RelayCommand InsertSeperator { get; set; }
    public RelayCommand MoveSnippetDown { get; set; }
    public RelayCommand MoveSnippetUp { get; set; }
    public RelayCommand DeleteSnippet { get; set; }

    public RelayCommand<bool> InsertNewSnippet { get; set; }

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

    private ObservableCollection<ISnippetListItemReadOnly>  snippetList = new ObservableCollection<ISnippetListItemReadOnly>();
    private int SnippetCounter = 1;

    public ObservableCollection<ISnippetListItemReadOnly> SnippetList { get { return snippetList; } set { snippetList = value; } }

    public bool IsDirty { get; set; }

    private ISnippetListItemReadOnly selectedSnippet;
    public ISnippetListItemReadOnly SelectedSnippet
    {
        get
        {
            return selectedSnippet;
        }
        set
        {
            selectedSnippet = value;

            if (selectedSnippet != null && selectedSnippet is ISnippetListItemEditable)
            {
                ClipboardActions.SetText((selectedSnippet as ISnippetListItemEditable).Data);
            }

            this.RaisePropertyChanged();
        }
    }

    public bool IsInPresentMode { get; set; }
    public bool IsClipboardManager { get { return this.ClipboardActions.IsClipboardManagerListening; } }

    internal void SelectSnippet(ISnippetListItemReadOnly snippetListItem)
    {
        this.SelectedSnippet = snippetListItem;
        this.SelectedSnippet.PropertyChanged += SelectedSnippetChangedEvent;
    }

    internal bool ItemWithDataExists(string data)
    {
        return this.SnippetList
            .Where(x => x is ISnippetListItemEditable)
            .Cast<ISnippetListItemEditable>()
            .Any(x => x.Data == data);
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

    internal ISnippetListItemReadOnly GetItemByListId(int selectedIndex)
    {
        return this.SnippetList[selectedIndex];
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
        if (FileService.SaveSnippetsToFile(this.SnippetList))
        {
            this.IsDirty = false;
        }
    }

    void ItemLoadClick(object sender, RoutedEventArgs e)
    {
        var loadedSnippets = FileService.LoadSnippetsFromFile();
        if (loadedSnippets != null)
        {
            this.SnippetList = loadedSnippets;
            this.RaisePropertyChanged(nameof(this.SnippetList));
            this.SelectedSnippet = null;
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
                this.ClipboardActions.StopListening();
                this.RaisePropertyChanged(nameof(IsClipboardManager));
            }
            else
            {
                item.IsChecked = true;
                this.ClipboardActions.StartListening();
                this.RaisePropertyChanged(nameof(IsClipboardManager));
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

    public void InsertNewSnippetMethod(bool ignoreIfDuplicate = false)
    {
        this.IsDirty = true;
        string clipbaordData = ClipboardActions.GetText();

        if (clipbaordData != null)
        {
            var clipboardString = clipbaordData as String;
            if (clipboardString != null)
            {
                if (!ignoreIfDuplicate || !this.ItemWithDataExists(clipboardString))
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
}
