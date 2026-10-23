using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class NewStorage : IServiceTransient
{
    public Dictionary<Items.Item, uint> Items { get; } = new();

    public event EventHandler OnItemsChanged;

    public NewStorage()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanTakeItems(Items.Item type, uint count)
    {
        if (count == 0)
        {
            return true;
        }

        if (!Items.TryGetValue(type, out var currentCount) || currentCount < count)
        {
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanTakeItems(Dictionary<Items.Item, uint> items)
    {
        foreach (var (type, count) in items)
        {
            if (!CanTakeItems(type, count))
            {
                return false;
            }
        }

        return true;
    }

    public bool TakeItems(Items.Item type, uint count)
    {
        if (count == 0)
        {
            return true;
        }

        if (!Items.TryGetValue(type, out var currentCount) || currentCount < count)
        {
            return false;
        }

        var newCount = currentCount - count;

        if (newCount > 0)
        {
            Items[type] = newCount;
        }
        else
        {
            Items.Remove(type);
        }

        OnItemsChanged?.Invoke(this, EventArgs.Empty);

        return true;
    }

    public bool TakeItems(Dictionary<Items.Item, uint> items)
    {
        if (!CanTakeItems(items))
        {
            return false;
        }

        var onChanged = false;

        foreach (var (type, count) in items)
        {
            if (count == 0)
            {
                continue;
            }

            if (!Items.TryGetValue(type, out var currentCount) || currentCount < count)
            {
                return false;
            }

            var newCount = currentCount - count;

            if (newCount > 0)
            {
                Items[type] = newCount;
            }
            else
            {
                Items.Remove(type);
            }

            onChanged = true;
        }

        if (onChanged)
        {
            OnItemsChanged?.Invoke(this, EventArgs.Empty);
        }

        return true;
    }

    public void GiveItems(Items.Item type, uint count)
    {
        if (count == 0)
        {
            return;
        }

        if (Items.TryGetValue(type, out var currentCount))
        {
            if (count > uint.MaxValue - currentCount)
            {
                Items[type] = uint.MaxValue;
            }
            else
            {
                Items[type] = currentCount + count;
            }
        }
        else
        {
            Items[type] = count;
        }

        OnItemsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void GiveItems(Dictionary<Items.Item, uint> items)
    {
        var onChanged = false;

        foreach (var (type, count) in items)
        {
            if (count == 0)
            {
                continue;
            }

            if (Items.TryGetValue(type, out var currentCount))
            {
                if (count > uint.MaxValue - currentCount)
                {
                    Items[type] = uint.MaxValue;
                }
                else
                {
                    Items[type] = currentCount + count;
                }
            }
            else
            {
                Items[type] = count;
            }

            onChanged = true;
        }

        if (onChanged)
        {
            OnItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEmpty()
    {
        return Items.Count == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Count(Items.Item type)
    {
        return Items.GetValueOrDefault(type);
    }
}
