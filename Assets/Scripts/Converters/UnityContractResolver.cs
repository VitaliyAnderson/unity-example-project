using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class UnityContractResolver : DefaultContractResolver
{
    private readonly Dictionary<Type, JsonConverter> _converters;

    public UnityContractResolver()
    {
        _converters = new Dictionary<Type, JsonConverter>()
        {
            { typeof(ObservableCollection<Items.Item>), new ObservableCollectionConverter<Items.Item>() },
            { typeof(Quaternion), new QuaternionConverter() },
            { typeof(Vector2), new Vector2Converter() },
            { typeof(Vector3), new Vector3Converter() },
        };
    }

    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (objectType != null)
        {
            foreach (var converter in _converters)
            {
                if (converter.Key.IsAssignableFrom(objectType))
                {
                    return converter.Value;
                }
            }
        }

        return base.ResolveContractConverter(objectType);
    }
}
