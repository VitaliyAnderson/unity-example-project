public static class StorageTypes
{
    public struct StorageTypeInfo
    {
        public ItemGroup[] AllowItemGroups;
        public int Count;
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum StorageType
    {
        Inventory,
        Equipment,
        Growbed,
        Count, // Last element for the count. Don't remove!
    }

    public static StorageTypeInfo[] List = new StorageTypeInfo[(int)StorageType.Count]
    {
        /// <see cref="StorageType.Inventory"/>
        new()
        {
            AllowItemGroups = null,
            Count = 30,
        },
        /// <see cref="StorageType.Equipment"/>
        new()
        {
            AllowItemGroups = new[] { ItemGroup.Tool },
            Count = 10,
        },
        /// <see cref="StorageType.Growbed"/>
        new()
        {
            AllowItemGroups = new[] { ItemGroup.Seed },
            Count = 1,
        },
    };
}
