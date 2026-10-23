public abstract class ConfigItem<TRealValue> : IConfigItem
{
    public string Name { get; }
    public string Value { get => ToValue(RealValue); set => RealValue = ToRealValue(value); }

    protected TRealValue RealValue;

    public ConfigItem(string name, TRealValue realValue)
    {
        Name = name;
        RealValue = realValue;
    }

    public abstract void ResetToDefault();
    public abstract void Apply();

    protected abstract TRealValue ToRealValue(string value);
    protected abstract string ToValue(TRealValue value);
}
