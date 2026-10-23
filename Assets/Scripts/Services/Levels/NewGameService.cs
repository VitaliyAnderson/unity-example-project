using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewGameService : IServiceTransient
{
    public class ItemGenerate
    {
        public Vector3 Position;
        public int ItemKey;
    }

    public class ItemsGenerateZone
    {
        public Vector3 Center;
        public int Radius;
        public Items.Item[] ItemKeys;
        public int Count;
    }

    private readonly PlayerController _playerController;
    private readonly WorldController _worldController;
    private readonly SpawnerService _spawnerService;

    public NewGameService(
        PlayerController playerController,
        WorldController worldController,
        SpawnerService spawnerService)
    {
        _playerController = playerController;
        _worldController = worldController;
        _spawnerService = spawnerService;
    }

    public void NewGame()
    {
        var biome = _worldController.CreateBiome(
            100,
            100,
            0,
            Vector3.zero,
            Quaternion.identity);

        var player = _spawnerService.SpawnPawn(Pawns.Pawn.Player, new Vector3(10, 2f, 10), Quaternion.identity, biome);

        _playerController.CurrentPlayer.Value = player as SceneObjectPawnPlayer;

        ItemsGenerate(
            new ItemsGenerateZone[]
            {
                new()
                {
                    Center = new Vector3(50, 1.0f, 50),
                    Radius = 25,
                    ItemKeys = new[]
                    {
                        Items.Item.Wood,
                        Items.Item.Iron,
                        Items.Item.Copper,
                        Items.Item.Silver,
                        Items.Item.Gold,
                    },
                    Count = 1000,
                },
            },
            biome);

        _spawnerService.SpawnItem(
            Items.Item.Axe,
            new Vector3(50, 2f, 55),
            Quaternion.identity,
            biome);

        _spawnerService.SpawnItem(
            Items.Item.Pick,
            new Vector3(51, 2f, 55),
            Quaternion.identity,
            biome);

        _spawnerService.SpawnItem(
            Items.Item.Hammer,
            new Vector3(52, 2f, 55),
            Quaternion.identity,
            biome);

        _spawnerService.SpawnItem(
            Items.Item.Constructor,
            new Vector3(53, 2f, 55),
            Quaternion.identity,
            biome);

        _spawnerService.SpawnItem(
            Items.Item.Seed,
            new Vector3(54, 2f, 55),
            Quaternion.identity,
            biome);
    }

    private void ItemsGenerate(
        ItemsGenerateZone[] value,
        SceneObjectBiome biome)
    {
        if (value != null) // Generate new items
        {
            var itemsToSpawn = new List<(Items.Item, Vector3)>();

            foreach (var spawnerZone in value)
            {
                for (var i = 0; i < spawnerZone.Count; i++)
                {
                    var itemKey = spawnerZone.ItemKeys[Random.Range(0, spawnerZone.ItemKeys.Length)];

                    while (true)
                    {
                        var point = Random.insideUnitCircle * spawnerZone.Radius;

                        var gridSize = 1;

                        var roundedVector = new Vector3(
                            spawnerZone.Center.x + Mathf.Round(point.x / gridSize) * gridSize,
                            spawnerZone.Center.y + 0.25f,
                            spawnerZone.Center.z + Mathf.Round(point.y / gridSize) * gridSize);

                        if (!itemsToSpawn.Any(x => x.Item2 == roundedVector))
                        {
                            itemsToSpawn.Add((itemKey, roundedVector));
                            break;
                        }
                    }
                }
            }

            foreach (var item in itemsToSpawn)
            {
                _spawnerService.SpawnItem(
                    item.Item1,
                    item.Item2,
                    Quaternion.identity,
                    biome);
            }
        }
    }
}
