using System.Collections.Generic;
using UnityEngine;

public static class Buildings
{
    public struct BuildingInfo
    {
        public string Name;
        public Sprite Icon;
        public GameObject Prefab;
        public GameObject PrefabExample;
        public Vector3 Size;
        public Dictionary<Items.Item, int> ItemsForBuild;
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum Building
    {
        None,
        Storage,
        Base2,
        Base3,
        Growbed,
        Count, // Last element for the count. Don't remove!
    }

    public static BuildingInfo[] List = new BuildingInfo[(int)Building.Count]
    {
        /// <see cref="Building.None"/>
        new()
        {
            Name = "None",
            Icon = Resources.Load<Sprite>("Construction/Buildings/None Icon"),
            Prefab = Resources.Load<GameObject>("Construction/Buildings/None Model"),
            PrefabExample = Resources.Load<GameObject>("Construction/Buildings/None Example"),
            Size = new Vector3(1, 1, 1),
            ItemsForBuild = null,
        },
        /// <see cref="Building.Storage"/>
        new()
        {
            Name = "Storage",
            Icon = Resources.Load<Sprite>("Construction/Buildings/Base 1 Icon"),
            Prefab = Resources.Load<GameObject>("Construction/Buildings/Base 1 Model"),
            PrefabExample = Resources.Load<GameObject>("Construction/Buildings/Base 1 Example"),
            Size = new Vector3(1, 1, 1),
            ItemsForBuild = null,
        },
        /// <see cref="Building.Base2"/>
        new()
        {
            Name = "Base 2",
            Icon = Resources.Load<Sprite>("Construction/Buildings/Base 2 Icon"),
            Prefab = Resources.Load<GameObject>("Construction/Buildings/Base 2 Model"),
            PrefabExample = Resources.Load<GameObject>("Construction/Buildings/Base 2 Example"),
            Size = new Vector3(1, 1, 1),
            ItemsForBuild = null,
        },
        /// <see cref="Building.Base3"/>
        new()
        {
            Name = "Base 3",
            Icon = Resources.Load<Sprite>("Construction/Buildings/Base 3 Icon"),
            Prefab = Resources.Load<GameObject>("Construction/Buildings/Base 3 Model"),
            PrefabExample = Resources.Load<GameObject>("Construction/Buildings/Base 3 Example"),
            Size = new Vector3(1, 1, 1),
            ItemsForBuild = null,
        },
        /// <see cref="Building.Growbed"/>
        new()
        {
            Name = "Growbed",
            Icon = Resources.Load<Sprite>("Construction/Buildings/Base 1 Icon"),
            Prefab = Resources.Load<GameObject>("Construction/Buildings/Base 1 Model"),
            PrefabExample = Resources.Load<GameObject>("Construction/Buildings/Base 1 Example"),
            Size = new Vector3(1, 1, 1),
            ItemsForBuild = new()
            {
                { Items.Item.Wood, 2 },
                { Items.Item.Copper, 1 },
            },
        },
    };
}
