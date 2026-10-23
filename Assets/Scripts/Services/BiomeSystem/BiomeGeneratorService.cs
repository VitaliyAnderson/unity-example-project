using System;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGeneratorService : IServiceTransient
{
    private enum Direction
    {
        North,
        South,
        East,
        West
    }

    // TODO: change the mesh to voxel object
    public Mesh VoxelMapGenerate(int biomeWidth, int biomeHeight, int biomeSeed, int voxelSize)
    {
        byte[,] heightMap = VoxelHeightMapGenerate(biomeWidth, biomeHeight, biomeSeed);

        List<Vector3> vertices = new();
        List<int> triangles = new();

        int width = heightMap.GetLength(0);
        int depth = heightMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                int h = heightMap[x, z];

                if (h == 0) continue;

                AddTopFace(x, z, h, voxelSize, vertices, triangles);

                int hSouth = (z + 1 < depth) ? heightMap[x, z + 1] : 0;
                if (h > hSouth)
                    AddVerticalFace(x, z, hSouth, h, voxelSize, Direction.South, vertices, triangles);

                int hNorth = (z - 1 >= 0) ? heightMap[x, z - 1] : 0;
                if (h > hNorth)
                    AddVerticalFace(x, z, hNorth, h, voxelSize, Direction.North, vertices, triangles);

                int hEast = (x + 1 < width) ? heightMap[x + 1, z] : 0;
                if (h > hEast)
                    AddVerticalFace(x, z, hEast, h, voxelSize, Direction.East, vertices, triangles);

                int hWest = (x - 1 >= 0) ? heightMap[x - 1, z] : 0;
                if (h > hWest)
                    AddVerticalFace(x, z, hWest, h, voxelSize, Direction.West, vertices, triangles);
            }
        }

        Mesh mesh = new();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    private static byte[,] VoxelHeightMapGenerate(int width, int height, int seed)
    {
        if (seed == 0)
        {
            var emptyMap = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    emptyMap[x, y] = 1;
                }
            }
            return emptyMap;
        }

        var rand = new System.Random(seed);
        var heightMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heightMap[x, y] = (float)rand.NextDouble() * 3f;
            }
        }

        var smoothed = new byte[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sum = 0;
                int count = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                        {
                            sum += heightMap[nx, ny];
                            count++;
                        }
                    }
                }
                smoothed[x, y] = (byte)MathF.Round(sum / count);
            }
        }

        return smoothed;
    }

    private void AddTopFace(int x, int z, int h, int voxelSize, List<Vector3> verts, List<int> tris)
    {
        int vi = verts.Count;
        float y = h * voxelSize;

        float x0 = x * voxelSize;
        float x1 = (x + 1) * voxelSize;
        float z0 = z * voxelSize;
        float z1 = (z + 1) * voxelSize;

        verts.Add(new Vector3(x0, y, z0));
        verts.Add(new Vector3(x0, y, z1));
        verts.Add(new Vector3(x1, y, z1));
        verts.Add(new Vector3(x1, y, z0));

        tris.Add(vi);
        tris.Add(vi + 1);
        tris.Add(vi + 2);
        tris.Add(vi);
        tris.Add(vi + 2);
        tris.Add(vi + 3);
    }

    private void AddVerticalFace(int x, int z, int hBot, int hTop, int voxelSize, Direction dir, List<Vector3> verts, List<int> tris)
    {
        int vi = verts.Count;
        float y0 = hBot * voxelSize;
        float y1 = hTop * voxelSize;

        switch (dir)
        {
            case Direction.North:
                {
                    float zz = z * voxelSize;
                    float x0 = x * voxelSize;
                    float x1 = (x + 1) * voxelSize;
                    verts.Add(new Vector3(x0, y0, zz));
                    verts.Add(new Vector3(x0, y1, zz));
                    verts.Add(new Vector3(x1, y1, zz));
                    verts.Add(new Vector3(x1, y0, zz));
                    tris.Add(vi);
                    tris.Add(vi + 1);
                    tris.Add(vi + 2);
                    tris.Add(vi);
                    tris.Add(vi + 2);
                    tris.Add(vi + 3);
                }
                break;

            case Direction.South:
                {
                    float zz = (z + 1) * voxelSize;
                    float x0 = x * voxelSize;
                    float x1 = (x + 1) * voxelSize;
                    verts.Add(new Vector3(x0, y0, zz));
                    verts.Add(new Vector3(x1, y0, zz));
                    verts.Add(new Vector3(x1, y1, zz));
                    verts.Add(new Vector3(x0, y1, zz));
                    tris.Add(vi);
                    tris.Add(vi + 1);
                    tris.Add(vi + 2);
                    tris.Add(vi);
                    tris.Add(vi + 2);
                    tris.Add(vi + 3);
                }
                break;

            case Direction.West:
                {
                    float xx = x * voxelSize;
                    float z0 = z * voxelSize;
                    float z1 = (z + 1) * voxelSize;
                    verts.Add(new Vector3(xx, y0, z0));
                    verts.Add(new Vector3(xx, y0, z1));
                    verts.Add(new Vector3(xx, y1, z1));
                    verts.Add(new Vector3(xx, y1, z0));
                    tris.Add(vi);
                    tris.Add(vi + 1);
                    tris.Add(vi + 2);
                    tris.Add(vi);
                    tris.Add(vi + 2);
                    tris.Add(vi + 3);
                }
                break;

            case Direction.East:
                {
                    float xx = (x + 1) * voxelSize;
                    float z0 = z * voxelSize;
                    float z1 = (z + 1) * voxelSize;
                    verts.Add(new Vector3(xx, y0, z0));
                    verts.Add(new Vector3(xx, y1, z0));
                    verts.Add(new Vector3(xx, y1, z1));
                    verts.Add(new Vector3(xx, y0, z1));
                    tris.Add(vi);
                    tris.Add(vi + 1);
                    tris.Add(vi + 2);
                    tris.Add(vi);
                    tris.Add(vi + 2);
                    tris.Add(vi + 3);
                }
                break;

            default:
                return;
        }
    }
}