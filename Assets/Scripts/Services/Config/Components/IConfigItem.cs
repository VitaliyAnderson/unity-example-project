using Newtonsoft.Json;

public interface IConfigItem
{
    [JsonIgnore]
    string Name { get; }
    string Value { get; }

    void ResetToDefault();
    void Apply();
}
