using UnityEngine;

public abstract class SceneObjectPawn : Spawnable
{
    public Pawns.Pawn PawnKey { get; set; }

    public SceneObjectPawn()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return Pawns.List[(int)PawnKey].Prefab;
    }
}
