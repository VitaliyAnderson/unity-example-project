using System;
using System.IO;

public class SettingsService : IServiceSingleton
{
    public string ProfileFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public string ProfileFolderName = "UnityExample";
    public string ConfigName = "config.json";
    public string SaveFolderName = "saves";

    public SettingsService()
    {
        if (!Directory.Exists(GetProfileFolderPath()))
        {
            Directory.CreateDirectory(GetProfileFolderPath());
        }

        if (!Directory.Exists(GetSaveFolderPath()))
        {
            Directory.CreateDirectory(GetSaveFolderPath());
        }
    }

    public string GetProfileFolderPath()
    {
        return Environment.ExpandEnvironmentVariables(Path.Combine(ProfileFolderPath, ProfileFolderName));
    }

    public string GetConfigFolderPath()
    {
        return Environment.ExpandEnvironmentVariables(Path.Combine(ProfileFolderPath, ProfileFolderName));
    }

    public string GetConfigFilePath()
    {
        return Environment.ExpandEnvironmentVariables(Path.Combine(ProfileFolderPath, ProfileFolderName, ConfigName));
    }

    public string GetSaveFolderPath()
    {
        return Environment.ExpandEnvironmentVariables(Path.Combine(ProfileFolderPath, ProfileFolderName, SaveFolderName));
    }
}
