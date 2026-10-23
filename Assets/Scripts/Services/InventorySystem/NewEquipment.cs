using System.Collections.ObjectModel;
using System.Linq;

public class NewEquipment : IServiceTransient
{
    public ObservableCollection<Items.Item> ObservableItems { get; } = new();

    public NewEquipment(int count)
    {
        foreach (var item in Enumerable.Repeat(Items.Item.None, count))
        {
            ObservableItems.Add(item);
        }
    }
}
