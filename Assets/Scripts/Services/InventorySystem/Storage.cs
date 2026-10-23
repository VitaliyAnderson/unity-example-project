using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class Storage
{
    public StorageTypes.StorageType StorageTypeKey { get; set; }
    public ObservableCollection<Items.Item> ObservableItems { get; } = new();

    public Storage(StorageTypes.StorageType storageTypeKey)
    {
        StorageTypeKey = storageTypeKey;

        var storageTypeInfo = StorageTypes.List[(int)StorageTypeKey];

        foreach (var item in Enumerable.Repeat(Items.Item.None, storageTypeInfo.Count))
        {
            ObservableItems.Add(item);
        }
    }

    public bool CanTakeItem(Items.Item item)
    {
        if (item == Items.Item.None)
        {
            return true;
        }

        var storageTypeInfo = StorageTypes.List[(int)StorageTypeKey];

        if (storageTypeInfo.AllowItemGroups == null)
        {
            return true;
        }

        var itemInfo = Items.List[(int)item];
        if (storageTypeInfo.AllowItemGroups.Any(x => x == itemInfo.Group))
        {
            return true;
        }

        return false;
    }

    public bool TakeItem(Items.Item item)
    {
        var idxOfNoneItem = ObservableItems.IndexOf(Items.Item.None);

        if (idxOfNoneItem == -1)
        {
            return false;
        }

        ObservableItems[idxOfNoneItem] = item;

        return true;
    }

    public bool CanTakeAllItems(Storage from)
    {
        var itemsCount = from.CountItems();
        return itemsCount == 0 || itemsCount <= CountEmpty();
    }

    public bool TakeAllItems(Storage from)
    {
        if (CanTakeAllItems(from))
        {
            foreach (var item in from.ObservableItems)
            {
                TakeItem(item);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanTakeItems(Dictionary<int, int> itemsToAdd)
    {
        return ObservableItems.Count(x => x == Items.Item.None) >= itemsToAdd.Sum(x => x.Value);
    }

    public bool TakeItems(Dictionary<Items.Item, int> requiredItems)
    {
        foreach (var requiredItem in requiredItems)
        {
            var item = requiredItem.Key;
            var requiredCount = requiredItem.Value;

            for (int i = 0; i < requiredCount; i++)
            {
                ObservableItems[ObservableItems.IndexOf(Items.Item.None)] = item;
            }
        }

        return true;
    }

    public bool CanGiveItems(Dictionary<Items.Item, int> requiredItems)
    {
        var itemCounts = ObservableItems
            .GroupBy(item => item)
            .ToDictionary(group => group.Key, group => group.Count());

        foreach (var requiredItem in requiredItems)
        {
            var item = requiredItem.Key;
            var requiredCount = requiredItem.Value;

            if (!itemCounts.TryGetValue(item, out int availableCount) || availableCount < requiredCount)
            {
                return false;
            }
        }

        return true;
    }

    public bool GiveItems(Dictionary<Items.Item, int> requiredItems)
    {
        foreach (var requiredItem in requiredItems)
        {
            var item = requiredItem.Key;
            var requiredCount = requiredItem.Value;

            for (int i = 0; i < requiredCount; i++)
            {
                ObservableItems[ObservableItems.IndexOf(item)] = Items.Item.None;
            }
        }

        return true;
    }

    public bool GiveItem(int idx, out Items.Item item)
    {
        item = ObservableItems[idx];

        if (item == Items.Item.None)
        {
            return false;
        }

        ObservableItems[idx] = Items.Item.None;

        return true;
    }

    public bool IsEmpty()
    {
        return ObservableItems.All(x => x == Items.Item.None);
    }

    public void Clear()
    {
        for (int i = 0; i < ObservableItems.Count; ++i)
        {
            if (ObservableItems[i] != Items.Item.None)
            {
                ObservableItems[i] = Items.Item.None;
            }
        }
    }

    public int CountItems()
    {
        return ObservableItems.Count(x => x != Items.Item.None);
    }

    public int CountEmpty()
    {
        return ObservableItems.Count(x => x == Items.Item.None);
    }

    public int CountStorage()
    {
        return ObservableItems.Count;
    }
}
