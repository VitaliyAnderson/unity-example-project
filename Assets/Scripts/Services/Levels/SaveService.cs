using Autofac;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class SaveService : IServiceScoped
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly SettingsService _settingsService;

    public SaveService(
        ILifetimeScope lifetimeScope,
        SettingsService settingsService)
    {
        _lifetimeScope = lifetimeScope;
        _settingsService = settingsService;
    }

    public void SaveGame(Savefile savefile)
    {
        try
        {
            Debug.Log("[Level] Save game...");

            if (savefile != null)
            {
                Save(new StreamWriter(
                    savefile.Path
                    ?? Path.Combine(
                        _settingsService.GetSaveFolderPath(),
                        DateTimeOffset.UtcNow.ToString("dd/MM/yyyy_HH_mm_ss_fff") + ".save")));
            }

            Debug.Log("[Level] Save game completed");
        }
        catch (Exception ex)
        {
            Debug.Log($"[Level] Save game failed for the reason: {ex}");
        }
    }

    private void Save(StreamWriter sw)
    {
        using var writer = new JsonTextWriter(sw)
        {
            Formatting = Formatting.Indented,
            Indentation = 4,
        };

        var serializer = new JsonSerializer
        {
            ContractResolver = new UnityContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };

        serializer.Serialize(writer, _lifetimeScope.Resolve<SaveContent>());
    }
}
