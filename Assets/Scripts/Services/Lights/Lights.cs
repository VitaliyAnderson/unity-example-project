using UnityEngine;

public static class Lights
{
    public struct LightInfo
    {
        public GameObject Prefab { get; set; }
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum Light
    {
        Sun,
        Count, // Last element for the count. Don't remove!
    }

    public static LightInfo[] List = new LightInfo[(int)Light.Count]
    {
        /// <see cref="Light.Sun"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("Lights/Sun"),
        },
    };
}
