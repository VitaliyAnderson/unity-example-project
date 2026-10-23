using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ConfigItemLanguage : ConfigItem<Locale>
{
    public ConfigItemLanguage(string title)
        : base(title, LocalizationSettings.ProjectLocale)
    {
    }

    public override void ResetToDefault()
    {
        RealValue = LocalizationSettings.ProjectLocale;
    }

    public override void Apply()
    {
        LocalizationSettings.SelectedLocale = RealValue;
    }

    protected override Locale ToRealValue(string value)
    {
        foreach (Locale localizationKey in LocalizationSettings.AvailableLocales.Locales)
        {
            if (localizationKey.LocaleName == value)
            {
                return localizationKey;
            }
        }

        return LocalizationSettings.SelectedLocale;
    }

    protected override string ToValue(Locale value)
    {
        return value.LocaleName;
    }
}
