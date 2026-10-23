public static class ItemToolAnimationsProperties
{
    public static ItemToolAnimationsProperty[] All = new ItemToolAnimationsProperty[(int)ItemToolAnimationsEnum.NumberOfTypes]
    {
        // ItemToolAnimationsEnum.None
        new()
        {
            CanInterrupt = true,
        },
        // ItemToolAnimationsEnum.Idle
        new()
        {
            CanInterrupt = true,
        },
        // ItemToolAnimationsEnum.FirstAction
        new()
        {
            CanInterrupt = true,
        },
        // ItemToolAnimationsEnum.SecondAction
        new()
        {
            CanInterrupt = true,
        },
        // ItemToolAnimationsEnum.Reload
        new()
        {
            CanInterrupt = false,
        }
    };
}
