using UnityEngine;

public static class ItemTools
{
    public struct ItemToolInfo
    {
        public GameObject Prefab;
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum ItemTool
    {
        None,
        Axe,
        Pick,
        Hammer,
        Constructor,
        Count, // Last element for the count. Don't remove!
    }

    public static ItemToolInfo[] List = new ItemToolInfo[(int)ItemTool.Count]
    {
        /// <see cref="ItemTool.None"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("ItemTools/Cube0"),
        },
        /// <see cref="ItemTool.Axe"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("ItemTools/Cube1"),
        },
        /// <see cref="ItemTool.Pick"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("ItemTools/Cube2"),
        },
        /// <see cref="ItemTool.Hammer"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("ItemTools/Cube3"),
        },
        /// <see cref="ItemTool.Constructor"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("ItemTools/Cube4"),
        },
    };
}
