using System.Collections.Generic;
using Unity.Properties;

public class UIBuildingItemsForBuild
{
    public KeyValuePair<Items.Item, int> ItemForBuild { get; }

    [CreateProperty] public string Name => Items.List[(int)ItemForBuild.Key].Name;
    [CreateProperty] public int Count => ItemForBuild.Value;

    public UIBuildingItemsForBuild(KeyValuePair<Items.Item, int> itemForBuild)
    {
        ItemForBuild = itemForBuild;
    }
}
