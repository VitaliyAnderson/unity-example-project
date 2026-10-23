using System;
using System.Collections.Generic;
using System.IO;

public class LevelService : IServiceSingleton
{
    private readonly SettingsService _settingsService;

    public bool IsMenu { get; set; } = false;
    public string LoadingLevelPath { get; set; }

    public LevelService(
        SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public void LoadMenu()
    {
        Program.DependencyInjection.DeleteScope();

        IsMenu = true;
        LoadingLevelPath = null;

        Program.DependencyInjection.CreateScope();
    }

    public void NewGame()
    {
        Program.DependencyInjection.DeleteScope();

        IsMenu = false;
        LoadingLevelPath = null;

        Program.DependencyInjection.CreateScope();
    }

    public void LoadGame(Savefile savefile)
    {
        Program.DependencyInjection.DeleteScope();

        IsMenu = false;
        LoadingLevelPath = savefile.Path;

        Program.DependencyInjection.CreateScope();
    }

    public IEnumerable<Savefile> RetrieveSaves()
    {
        var saveFolderPath = _settingsService.GetSaveFolderPath();

        IList<Savefile> savefiles = new List<Savefile>();

        try
        {
            if (Directory.Exists(saveFolderPath))
            {
                foreach (var file in new DirectoryInfo(saveFolderPath).GetFiles("*.save"))
                {
                    savefiles.Add(new Savefile
                    {
                        Name = file.Name,
                        Path = Path.Combine(saveFolderPath, file.FullName),
                    });
                }
            }
        }
        catch (Exception)
        {
        }

        foreach (var savefile in savefiles)
        {
            yield return savefile;
        }
    }
}
