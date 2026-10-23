using UnityEngine;

public static class Pawns
{
    public struct PawnInfo
    {
        public GameObject Prefab { get; set; }
    }

    /// <summary>
    /// Sync it with <see cref="List"/>
    /// </summary>
    public enum Pawn
    {
        Player,
        Count, // Last element for the count. Don't remove!
    }

    public static PawnInfo[] List = new PawnInfo[(int)Pawn.Count]
    {
        /// <see cref="Pawn.Player"/>
        new()
        {
            Prefab = Resources.Load<GameObject>("Pawns/Player"),
        },
    };
}
