using System.Linq;
using UnityEngine;

public class ConfigItemDisplayResolution : ConfigItem<Resolution>
{
    private readonly DisplayService _displayService;

    public ConfigItemDisplayResolution(string title, DisplayService displayService)
        : base(title, displayService.GetResolutionDefault())
    {
        _displayService = displayService;
    }

    public override void ResetToDefault()
    {
        RealValue = _displayService.GetResolutionDefault();
    }

    public override void Apply()
    {
        _displayService.SetResolution(RealValue);
    }

    protected override Resolution ToRealValue(string value)
    {
        return _displayService.GetResolutions().SingleOrDefault(x => x.ToString() == value);
    }

    protected override string ToValue(Resolution value)
    {
        return value.ToString();
    }
}
