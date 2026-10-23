using System.Linq;
using UnityEngine.InputSystem;

public class ConfigItemInputAction : ConfigItem<InputAction>
{
    private readonly string _defaultValue;

    public ConfigItemInputAction(InputAction action)
        : base(action.name, action)
    {
        _defaultValue = action.bindings.FirstOrDefault() is InputBinding firstBinding
            ? firstBinding.effectivePath
            : string.Empty;

        ResetToDefault();
    }

    public override void ResetToDefault()
    {
        RealValue.ApplyBindingOverride(_defaultValue);
        RealValue.Enable();
    }

    public override void Apply()
    {
    }

    protected override InputAction ToRealValue(string value)
    {
        RealValue.ApplyBindingOverride(value);
        RealValue.Enable();

        return RealValue;
    }

    protected override string ToValue(InputAction value)
    {
        if (value is not null && value.bindings.FirstOrDefault() is InputBinding firstBinding)
        {
            return firstBinding.effectivePath;
        }

        return string.Empty;
    }
}
