using UnityEngine;

public class MapGenerator : IServiceTransient
{
    public static void Generate(Terrain terrain, int width, int height, int seed)
    {
        var terrainData = terrain.terrainData;
        if (terrainData == null || terrainData.heightmapResolution != width + 1)
        {
            terrainData = new TerrainData
            {
                heightmapResolution = width + 1,
                size = new Vector3(width, 2f, height)
            };
            terrain.terrainData = terrainData;
        }

        float[,] heights = new float[width + 1, height + 1];

        if (seed == 0)
        {
            // Ровная поверхность (например, 0.3 — уровень ~30% от max height)
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    heights[x, y] = 0.3f;
                }
            }
        }
        else
        {
            // Инициализируем псевдослучайность с seed
            System.Random rand = new(seed);

            // Или можешь использовать Perlin Noise:
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    // Простой пример с Perlin Noise + seed
                    float nx = (float)x / width * 2f; // регулируем scale
                    float ny = (float)y / height * 2f;
                    float noise = Mathf.PerlinNoise(nx + seed * 0.10f, ny + seed * 0.10f);
                    heights[x, y] = Mathf.Clamp01(noise * 0.5f + 0.25f); // смещаем в [0.25, 0.75]
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);

        ApplyTextures(terrain);
    }

    private static void ApplyTextures(Terrain terrain)
    {
        var material = Resources.Load<Material>("TerrainDemoScene_HDRP/Terrain/Materials/TerrainLit");

        terrain.materialTemplate = material;

        var terrainData = terrain.terrainData;

        // 1. Настраиваем слои текстур
        var layers = new TerrainLayer[]
        {
            Resources.Load<TerrainLayer>("TerrainDemoScene_HDRP/Terrain/Layers/Grass_A"),
            Resources.Load<TerrainLayer>("TerrainDemoScene_HDRP/Terrain/Layers/Grass_B")
        };

        terrainData.terrainLayers = layers;

        // 2. Генерируем splatmap по высоте
        int w = terrainData.alphamapWidth;
        int h = terrainData.alphamapHeight;
        float[,,] splatmap = new float[w, h, layers.Length];

        float minHeight = terrainData.GetHeight(0, 0); // на самом деле нужно мин/макс всей карты
        float maxHeight = 10f;

        // Находим реальные мин/макс высоты
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution - 1, terrainData.heightmapResolution - 1);
        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                float hVal = heights[x, y] * terrainData.size.y;
                minHeight = Mathf.Min(minHeight, hVal);
                maxHeight = Mathf.Max(maxHeight, hVal);
            }
        }

        float heightRange = maxHeight - minHeight;
        float threshold = minHeight + heightRange * 0.4f; // граница между травой и камнем

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                // Переводим координаты alphamap → heightmap
                float heightmapX = (float)x / (w - 1) * (terrainData.heightmapResolution - 1);
                float heightmapY = (float)y / (h - 1) * (terrainData.heightmapResolution - 1);

                // Берём высоту (в мировых единицах)
                float height = terrainData.GetInterpolatedHeight(x, y);
                int hx = Mathf.Clamp((int)heightmapX, 0, terrainData.heightmapResolution - 2);
                int hy = Mathf.Clamp((int)heightmapY, 0, terrainData.heightmapResolution - 2);
                float hVal = heights[hx, hy] * terrainData.size.y;

                if (hVal < threshold)
                {
                    splatmap[x, y, 0] = 1f; // трава
                    splatmap[x, y, 1] = 0f;
                }
                else
                {
                    splatmap[x, y, 0] = 0f;
                    splatmap[x, y, 1] = 1f; // камень
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmap);
    }
}
