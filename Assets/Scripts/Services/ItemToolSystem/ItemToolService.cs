using UnityEngine;

public abstract class ItemToolService : IServiceTransient
{
    public abstract bool Lock { get; }

    public abstract void OnSpawn(SceneObjectPawn from);
    public abstract void OnDespawn(SceneObjectPawn from);
    public abstract void OnUpdate(SceneObjectPawn from, float deltaTime, RaycastHit[] rays);
    public abstract void NextAction(SceneObjectPawn from);
    public abstract void PrevAction(SceneObjectPawn from);
    public abstract void FirstActionStart(SceneObjectPawn from);
    public abstract void FirstActionStop(SceneObjectPawn from);
    public abstract void SecondActionStart(SceneObjectPawn from);
    public abstract void SecondActionStop(SceneObjectPawn from);
    public abstract void SpecialActionExecute(SceneObjectPawn from);
}
