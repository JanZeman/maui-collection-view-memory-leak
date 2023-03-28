using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace TestApp;

public class MainViewModel : ViewModelBase
{
    private int startIndex;
    private int itemsCount;
    private ObservableCollection<string> itemsSource;
    private string memoryInfo;

    private static readonly Random Random = new();

    public MainViewModel(int startIndex, int itemsCount)
    {
        this.startIndex = startIndex;
        this.itemsCount = itemsCount;

        this.RefreshItemsSourceCommand = new Command(this.RefreshItemsSource);
        this.ModifyItemsCommand = new Command(this.ModifyItems);
        this.RefreshMemoryInfoCommand = new Command(this.RefreshMemoryInfo);

        this.RefreshItemsSource();
        this.RefreshMemoryInfo();
    }

    public ObservableCollection<string> ItemsSource
    {
        get => itemsSource;
        private set => this.SetValue(ref this.itemsSource, value);
    }

    public string MemoryInfo
    {
        get => memoryInfo;
        private set => this.SetValue(ref this.memoryInfo, value);
    }

    public ICommand RefreshItemsSourceCommand { get; }

    public ICommand ModifyItemsCommand { get;  }

    public ICommand RefreshMemoryInfoCommand { get; }

    private void RefreshItemsSource()
    {
        this.ItemsSource = this.CreateNewItemsSource();
        this.startIndex += this.itemsCount;
    }

    private void ModifyItems()
    {
        CallGC();

        var newItems = CreateItemsList();
        this.ItemsSource.Clear();
        foreach (var newItem in newItems)
        {
            this.ItemsSource.Add($"{newItem} {RandomString(3)}" );
        }
    }

    private ObservableCollection<string> CreateNewItemsSource()
    {
        return new ObservableCollection<string>(CreateItemsList());
    }

    private List<string> CreateItemsList()
    {
        return Enumerable.Range(this.startIndex, this.itemsCount)
            .Select(itemIndex => $"Item {itemIndex}")
            .ToList();
    }

    private void RefreshMemoryInfo()
    {
        CallGC();
        this.MemoryInfo = CreateMemoryInfo();
    }

    private static void CallGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private static string CreateMemoryInfo()
    {
        var memoryInfoBuilder = new StringBuilder();

        memoryInfoBuilder.AppendLine($"Total objects: {MemoryTracker.TotalObjectCount}");
        memoryInfoBuilder.AppendLine($"Alive objects: {MemoryTracker.AliveObjectCount}");

        return memoryInfoBuilder.ToString();
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}
