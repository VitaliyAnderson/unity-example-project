using Unity.Properties;
using UnityEngine.Localization;

public class UILocalizationItem
{
    public Locale Locale { get; }

    [CreateProperty] public string Name => Locale.LocaleName;

    public UILocalizationItem(Locale locale)
    {
        Locale = locale;
    }
}
