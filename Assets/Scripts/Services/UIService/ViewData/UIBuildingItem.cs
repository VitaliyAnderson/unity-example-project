using System.Collections.Generic;
using System.Windows.Input;
using Unity.Properties;
using UnityEngine;

public class UIBuildingItem
{
    public Buildings.Building BuildingKey { get; }

    [CreateProperty] public string Name => Buildings.List[(int)BuildingKey].Name;
    [CreateProperty] public Texture2D Icon => Buildings.List[(int)BuildingKey].Icon.texture;
    [CreateProperty] public List<UIBuildingItemsForBuild> ItemsForBuild { get; set; }

    [CreateProperty] public ICommand PointerDownEvent { get; }

    public UIBuildingItem(Buildings.Building buildingKey, ICommand pointerDownEvent)
    {
        BuildingKey = buildingKey;

        var buildingInfo = Buildings.List[(int)BuildingKey];

        var itemsForBuild = new List<UIBuildingItemsForBuild>();
        foreach (var pair in buildingInfo.ItemsForBuild ?? new())
        {
            itemsForBuild.Add(new UIBuildingItemsForBuild(pair));
        }
        ItemsForBuild = itemsForBuild;

        PointerDownEvent = pointerDownEvent;
    }
}
