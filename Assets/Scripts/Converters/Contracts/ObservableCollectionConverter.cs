using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class ObservableCollectionConverter<T> : JsonConverter<ObservableCollection<T>>
{
    public override void WriteJson(JsonWriter writer, ObservableCollection<T> value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.ToList());
    }

    public override ObservableCollection<T> ReadJson(JsonReader reader, Type objectType, ObservableCollection<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var items = serializer.Deserialize<List<T>>(reader);

        if (hasExistingValue && existingValue != null)
        {
            existingValue.Clear();

            foreach (var item in items)
            {
                existingValue.Add(item);
            }

            return existingValue;
        }

        return new ObservableCollection<T>(items);
    }
}
