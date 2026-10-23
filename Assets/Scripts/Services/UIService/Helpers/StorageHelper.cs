public static class StorageHelper
{
    public static void SwapItems(
        Storage dragStorageFrom,
        int dragIndexFrom,
        Storage dragStorageTo,
        int dragIndexTo)
    {
        if (dragStorageFrom == dragStorageTo && dragIndexFrom == dragIndexTo)
        {
            return;
        }

        var itemKeyFrom = dragStorageFrom.ObservableItems[dragIndexFrom];
        var itemKeyTo = dragStorageTo.ObservableItems[dragIndexTo];
        if (dragStorageTo.CanTakeItem(itemKeyFrom) && dragStorageFrom.CanTakeItem(itemKeyTo))
        {
            dragStorageTo.ObservableItems[dragIndexTo] = itemKeyFrom;
            dragStorageFrom.ObservableItems[dragIndexFrom] = itemKeyTo;
        }
    }
}
