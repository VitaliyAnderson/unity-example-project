using System;
using System.Collections.Generic;

public class ObservableValue<T>
{
    private readonly IEqualityComparer<T> _comparer;

    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (_comparer.Equals(_value, value))
            {
                return;
            }

            T oldValue = _value;

            _value = value;

            Changed?.Invoke(oldValue, value);
        }
    }

    private event Action<T, T> Changed;

    public ObservableValue(T initialValue = default!, IEqualityComparer<T> comparer = null)
    {
        _value = initialValue;
        _comparer = comparer ?? EqualityComparer<T>.Default;
    }

    public IDisposable Subscribe(Action<T, T> onChange)
    {
        if (onChange != null) Changed += onChange;

        return new Unsubscriber(() =>
        {
            if (onChange != null) Changed -= onChange;
        });
    }

    public IDisposable Subscribe(Action<T> onInit, Action<T, T> onChange, Action<T> onDeinit)
    {
        onInit?.Invoke(_value);
        if (onChange != null) Changed += onChange;

        return new Unsubscriber(() =>
        {
            if (onChange != null) Changed -= onChange;
            onDeinit?.Invoke(_value);
        });
    }

    private sealed class Unsubscriber : IDisposable
    {
        private Action _action;

        public Unsubscriber(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action.Invoke();
            _action = null;
        }
    }
}
