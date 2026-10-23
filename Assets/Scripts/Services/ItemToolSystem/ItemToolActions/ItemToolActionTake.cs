public class ItemToolActionTake : ItemToolAction
{
    private static readonly string _name = "Take";
    private string _target;

    public override string Name => _name;
    public override string Target => _target;

    public void SetTarget(string target)
    {
        _target = target;
    }
}
