using UnityEngine;

public class SceneObjectLightSun : Spawnable
{
    public SceneObjectLightSun()
    {
    }

    protected override GameObject ResolvePrefab()
    {
        return Lights.List[(int)Lights.Light.Sun].Prefab;
    }
}
