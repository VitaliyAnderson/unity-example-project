public interface IBuilding
{
    Buildings.Building BuildingKey { get; }
    bool CanDestroy(SceneObjectPawn from, ItemToolService tool);
    void Destroy(SceneObjectPawn from, ItemToolService tool);
}
