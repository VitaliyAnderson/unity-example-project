using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : IServiceScoped, IDisposable
{
    private readonly SpawnerService _spawnerService;

    private readonly List<SceneObjectBiome> _biomes = new();
    public IReadOnlyCollection<SceneObjectBiome> Biomes => _biomes;

    public WorldController(
        SpawnerService spawnerService)
    {
        _spawnerService = spawnerService;
    }

    public void Dispose()
    {
        _biomes.ForEach(x => x.Dispose());
        _biomes.Clear();
    }

    public SceneObjectBiome CreateBiome(
        int width,
        int height,
        int seed,
        Vector3 position,
        Quaternion rotation)
    {
        var newBiome = _spawnerService.SpawnBiome(
            width,
            height,
            seed,
            position,
            rotation);

        _biomes.Add(newBiome);

        _spawnerService.SpawnLightSun(
            Vector3.zero,
            Quaternion.Euler(45, 0, 0),
            newBiome);

        return newBiome;
    }

    public void DestroyBiome(SceneObjectBiome biome)
    {
        biome.Dispose();

        _biomes.Remove(biome);
    }
}
