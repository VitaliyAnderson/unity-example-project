public interface ISceneObjectSpecialAction
{
    bool CanExecute(SceneObjectPawn from, ItemToolService tool);
    void Execute(SceneObjectPawn from, ItemToolService tool);
    ItemToolAction ActionInfo();
}
