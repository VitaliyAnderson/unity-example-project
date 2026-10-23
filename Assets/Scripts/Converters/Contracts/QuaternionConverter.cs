using Newtonsoft.Json;
using System;
using UnityEngine;

// https://gist.github.com/zcyemi/e0c71c4f8ba8a92944a17f888253db0d
public class QuaternionConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Quaternion);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Quaternion v = (Quaternion)value;
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WritePropertyName("z");
        writer.WriteValue(v.z);
        writer.WritePropertyName("w");
        writer.WriteValue(v.w);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return JsonConvert.DeserializeObject<Quaternion>(serializer.Deserialize(reader).ToString());
    }
}
