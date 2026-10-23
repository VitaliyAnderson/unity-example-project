#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif

public class QuitService : IServiceSingleton
{
    public QuitService()
    {
    }

    public void Execute()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
