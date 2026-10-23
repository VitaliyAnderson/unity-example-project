using UnityEngine;

public static class Marks
{
    public struct MarkInfo
    {
        public GameObject Prefab;
        public Material Enabled;
        public Material Disabled;
        public Vector3 Offset;
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum Mark
    {
        Mark1,
        Count, // Last element for the count. Don't remove!
    }

    public static MarkInfo[] List = new MarkInfo[(int)Mark.Count]
    {
        /// <see cref="Mark.Mark1"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("Construction/Marks/Mark"),
            Enabled = Resources.Load<Material>("Construction/Marks/Mark Enabled"),
            Disabled = Resources.Load<Material>("Construction/Marks/Mark Disabled"),
            Offset = new Vector3(0, 1, 0),
        },
    };
}
