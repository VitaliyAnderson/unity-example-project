using System.Collections.Generic;

public static class SpawnableExtensions
{
    public static IEnumerable<T> ByType<T>(this List<Spawnable> spawnables)
        where T : Spawnable
    {
        foreach (var spawnable in spawnables)
        {
            if (spawnable is T castedValue)
            {
                yield return castedValue;
            }
        }
    }
}
