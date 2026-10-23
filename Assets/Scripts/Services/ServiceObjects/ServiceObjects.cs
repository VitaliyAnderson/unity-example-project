using UnityEngine;

public static class ServiceObjects
{
    public struct ServiceObjectInfo
    {
        public GameObject Prefab;
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum ServiceObject
    {
        None,
        Empty,
        UnityEvents,
        Camera,
        Ui,
        Count, // Last element for the count. Don't remove!
    }

    public static ServiceObjectInfo[] List = new ServiceObjectInfo[(int)ServiceObject.Count]
    {
        /// <see cref="ServiceObject.None"/>
        new()
        {
        },
        /// <see cref="ServiceObject.Empty"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("Empty"),
        },
        /// <see cref="ServiceObject.UnityEvents"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("UnityEventsService"),
        },
        /// <see cref="ServiceObject.Camera"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("Camera"),
        },
        /// <see cref="ServiceObject.Ui"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("Ui"),
        },
    };
}
