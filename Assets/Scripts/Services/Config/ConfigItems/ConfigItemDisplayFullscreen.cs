public class ConfigItemDisplayFullscreen : ConfigItem<bool>
{
    private readonly DisplayService _displayService;

    public ConfigItemDisplayFullscreen(string title, DisplayService displayService)
        : base(title, displayService.GetFullscreenDefault())
    {
        _displayService = displayService;
    }

    public override void ResetToDefault()
    {
        RealValue = _displayService.GetFullscreenDefault();
    }

    public override void Apply()
    {
        _displayService.SetFullscreen(RealValue);
    }

    protected override bool ToRealValue(string value)
    {
        return bool.Parse(value);
    }

    protected override string ToValue(bool value)
    {
        return value.ToString();
    }
}
