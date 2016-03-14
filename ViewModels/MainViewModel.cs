namespace SnippetManager.ViewModels
{
    using Models;
    using Interfaces;
    using System.Collections.ObjectModel;
    using GalaSoft.MvvmLight;
    using System.Linq;
    using Newtonsoft.Json;
    using System.ComponentModel;

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.SnippetList = new ObservableCollection<ISnippetListItem>();
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
                this.RaisePropertyChanged();
            }
        }

        public bool IsInPresentMode { get; set; }

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
            //remember facts
            var tmpA = this.SnippetList[indexA];
            var tmpB = this.SnippetList[indexB];

            //swap elements
            this.SnippetList[indexA] = tmpB;
            this.SnippetList[indexB] = tmpA;

            this.FixIds();

            this.SelectedSnippet = this.SnippetList[indexB];
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

        internal void RemoveSelected()
        {
            this.SnippetList.Remove(this.SelectedSnippet);
            this.FixIds();
            this.RaisePropertyChanged(nameof(this.SnippetList));
        }

        internal void Add(string label, string data)
        {
            this.SnippetList.Add(new Snippet(this.SnippetCounter, label, data));
            this.SnippetCounter++;
            this.RaisePropertyChanged(nameof(this.SnippetList));
        }

        internal void AddSeperator()
        {
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
    }
}
