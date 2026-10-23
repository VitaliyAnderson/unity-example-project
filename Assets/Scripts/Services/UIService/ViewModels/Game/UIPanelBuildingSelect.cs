using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Unity.Properties;
using UnityEngine.UIElements;

public class UIPanelBuildingSelect : UIPanelService
{
    private readonly PlayerController _playerController;

    protected override bool ShowCursor => true;

    [CreateProperty] public List<UIBuildingItem> Items { get; set; } = new();

    [CreateProperty] public ICommand PointerDownEvent { get; }

    [CreateProperty] public ICommand PointerUpEvent { get; }

    public UIPanelBuildingSelect(
        UIService uiService,
        PlayerController playerController)
        : base(uiService, nameof(UIPanelBuildingSelect))
    {
        _playerController = playerController;

        PointerDownEvent = new DelegateCommand(evt =>
        {
            if (playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent
                && evt is PointerDownEvent pointerDownEvent)
            {
                switch (pointerDownEvent.button)
                {
                    case 0:
                        itemToolComponent.ItemTool.ItemToolService?.FirstActionStart(playerController.CurrentPlayer.Value);
                        break;

                    case 1:
                        itemToolComponent.ItemTool.ItemToolService?.SecondActionStart(playerController.CurrentPlayer.Value);
                        break;
                }
            }
        });

        PointerUpEvent = new DelegateCommand(evt =>
        {
            if (playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent
                && evt is PointerUpEvent pointerUpEvent)
            {
                switch (pointerUpEvent.button)
                {
                    case 0:
                        itemToolComponent.ItemTool.ItemToolService?.FirstActionStop(playerController.CurrentPlayer.Value);
                        break;

                    case 1:
                        itemToolComponent.ItemTool.ItemToolService?.SecondActionStop(playerController.CurrentPlayer.Value);
                        break;
                }
            }
        });
    }

    public override void OnShow()
    {
        Items.AddRange(Enum
            .GetValues(typeof(Buildings.Building))
            .Cast<Buildings.Building>()
            .Where(buildingKey => buildingKey is not (Buildings.Building.None or Buildings.Building.Count))
            .Select(buildingKey => new UIBuildingItem(
                buildingKey,
                new DelegateCommand(evt =>
                {
                    if (_playerController.CurrentPlayer.Value is IItemToolComponent itemToolComponent
                        && itemToolComponent?.ItemTool?.ItemToolService is ConstructorTool constructorTool
                        && evt is PointerDownEvent pointerDownEvent
                        && pointerDownEvent.button is 0)
                    {
                        constructorTool.BuildingKey = buildingKey;
                    }
                }))));
    }

    public override void OnHide()
    {
        Items.Clear();
    }
}
