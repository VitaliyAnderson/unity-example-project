using System;
using UnityEngine;

public static class Items
{
    public struct ItemInfo
    {
        public ItemGroup Group;
        public string Name;
        public Sprite Icon;
        public GameObject Prefab;
        public object AdditionalInfo;
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum Item
    {
        None,
        Wood,
        Iron,
        Copper,
        Silver,
        Gold,
        Axe,
        Pick,
        Hammer,
        Constructor,
        Seed,
        Count, // Last element for the count. Don't remove!
    }

    public static ItemInfo[] List = new ItemInfo[(int)Item.Count]
    {
        /// <see cref="Item.None"/>
        new()
        {
            Group = ItemGroup.None,
            Name = "None",
            Icon = Resources.Load<Sprite>("Items/None Icon"),
            Prefab = Resources.Load<GameObject>("Items/None Model"),
            AdditionalInfo = new ItemInfoTool
            {
                ItemToolKey = ItemTools.ItemTool.None,
            },
        },
        /// <see cref="Item.Wood"/>
        new()
        {
            Group = ItemGroup.Resource,
            Name = "Wood",
            Icon = Resources.Load<Sprite>("Items/Wood Icon"),
            Prefab = Resources.Load<GameObject>("Items/Wood Model"),
            AdditionalInfo = null,
        },
        /// <see cref="Item.Iron"/>
        new()
        {
            Group = ItemGroup.Resource,
            Name = "Iron",
            Icon = Resources.Load<Sprite>("Items/Iron Icon"),
            Prefab = Resources.Load<GameObject>("Items/Iron Model"),
            AdditionalInfo = null,
        },
        /// <see cref="Item.Copper"/>
        new()
        {
            Group = ItemGroup.Resource,
            Name = "Copper",
            Icon = Resources.Load<Sprite>("Items/Copper Icon"),
            Prefab = Resources.Load<GameObject>("Items/Copper Model"),
            AdditionalInfo = null,
        },
        /// <see cref="Item.Silver"/>
        new()
        {
            Group = ItemGroup.Resource,
            Name = "Silver",
            Icon = Resources.Load<Sprite>("Items/Silver Icon"),
            Prefab = Resources.Load<GameObject>("Items/Silver Model"),
            AdditionalInfo = null,
        },
        /// <see cref="Item.Gold"/>
        new()
        {
            Group = ItemGroup.Resource,
            Name = "Gold",
            Icon = Resources.Load<Sprite>("Items/Gold Icon"),
            Prefab = Resources.Load<GameObject>("Items/Gold Model"),
            AdditionalInfo = null,
        },
        /// <see cref="Item.Axe"/>
        new()
        {
            Group = ItemGroup.Tool,
            Name = "Axe",
            Icon = Resources.Load<Sprite>("Items/Builder Icon"),
            Prefab = Resources.Load<GameObject>("Items/Builder Model"),
            AdditionalInfo = new ItemInfoTool
            {
                ItemToolKey = ItemTools.ItemTool.Axe,
            },
        },
        /// <see cref="Item.Pick"/>
        new()
        {
            Group = ItemGroup.Tool,
            Name = "Pick",
            Icon = Resources.Load<Sprite>("Items/Builder Icon"),
            Prefab = Resources.Load<GameObject>("Items/Builder Model"),
            AdditionalInfo = new ItemInfoTool
            {
                ItemToolKey = ItemTools.ItemTool.Pick,
            },
        },
        /// <see cref="Item.Hammer"/>
        new()
        {
            Group = ItemGroup.Tool,
            Name = "Hammer",
            Icon = Resources.Load<Sprite>("Items/Builder Icon"),
            Prefab = Resources.Load<GameObject>("Items/Builder Model"),
            AdditionalInfo = new ItemInfoTool
            {
                ItemToolKey = ItemTools.ItemTool.Hammer,
            },
        },
        /// <see cref="Item.Constructor"/>
        new()
        {
            Group = ItemGroup.Tool,
            Name = "Constructor",
            Icon = Resources.Load<Sprite>("Items/Builder Icon"),
            Prefab = Resources.Load<GameObject>("Items/Builder Model"),
            AdditionalInfo = new ItemInfoTool
            {
                ItemToolKey = ItemTools.ItemTool.Constructor,
            },
        },
        /// <see cref="Item.Seed"/>
        new()
        {
            Group = ItemGroup.Seed,
            Name = "Seed",
            Icon = Resources.Load<Sprite>("Items/Gold Icon"),
            Prefab = Resources.Load<GameObject>("Items/Gold Model"),
            AdditionalInfo = new ItemInfoSeed()
            {
                ItemToSpawn = Item.Gold,
                RipeningPeriod = new TimeSpan(0, 0, 10),
                Prefab = Resources.Load<GameObject>("Seeds/Gold Model"),
            },
        },
    };
}
