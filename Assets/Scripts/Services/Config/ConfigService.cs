using Newtonsoft.Json;
using System.IO;
using System.Linq;
using UnityEditor;

public class ConfigService : IServiceScoped
{
    private readonly SettingsService _settingsService;

    public ConfigItemLanguage Language { get; }
    public ConfigItemDisplayResolution DisplayResolution { get; }
    public ConfigItemDisplayFullscreen DisplayFullscreen { get; }
    public ConfigItemInputAction Interact { get; }
    public ConfigItemInputAction Jump { get; }
    public ConfigItemInputAction Sprint { get; }
    public ConfigItemInputAction Inventory { get; }

    public ConfigService(
        SettingsService settingsService,
        DisplayService displayService,
        InputController inputController)
    {
        _settingsService = settingsService;

        Language = new("Language");
        DisplayResolution = new("DisplayResolution", displayService);
        DisplayFullscreen = new("DisplayFullscreen", displayService);
        Interact = new(inputController.Interact);
        Jump = new(inputController.Jump);
        Sprint = new(inputController.Sprint);
        Inventory = new(inputController.Inventory);

        Init();
    }

    private void Init()
    {
        if (!File.Exists(_settingsService.GetConfigFilePath()))
        {
            using var fs = new FileStream(_settingsService.GetConfigFilePath(), FileMode.Create);
            fs.Close();

            Save();
        }
        else
        {
            Load();
        }

        Apply();
    }

    public void ResetToDefault()
    {
        GetType()
            .GetProperties()
            .Where(x => typeof(IConfigItem).IsAssignableFrom(x.PropertyType))
            .Select(x => x.GetValue(this) as IConfigItem)
            .ToList()
            .ForEach(x => x.ResetToDefault());
    }

    public void Apply()
    {
        GetType()
            .GetProperties()
            .Where(x => typeof(IConfigItem).IsAssignableFrom(x.PropertyType))
            .Select(x => x.GetValue(this) as IConfigItem)
            .ToList()
            .ForEach(x => x.Apply());
    }

    private void Load()
    {
        using var sr = new StreamReader(_settingsService.GetConfigFilePath());
        using var reader = new JsonTextReader(sr);
        var serializer = new JsonSerializer();

        serializer.Populate(reader, this);
    }

    public void Save()
    {
        using var sw = new StreamWriter(_settingsService.GetConfigFilePath());
        using var writer = new JsonTextWriter(sw);
        var serializer = new JsonSerializer();

        writer.Formatting = Formatting.Indented;

        serializer.Serialize(writer, this);
    }
}
