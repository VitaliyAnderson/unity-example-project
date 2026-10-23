using Unity.Properties;

public class UISavefileItem
{
    public Savefile Savefile { get; }

    [CreateProperty] public string Name => Savefile.Name;
    [CreateProperty] public string Path => Savefile.Path;

    public UISavefileItem(Savefile savefile)
    {
        Savefile = savefile;
    }
}
