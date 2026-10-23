using System;
using System.Collections.Generic;

/// <summary>
/// Обёртка над значением, уведомляющая подписчиков об его изменении.
/// </summary>
public class ObservableValue<T>
{
    /// <summary>
    /// Собственный компаратор равенства.
    /// </summary>
    private readonly IEqualityComparer<T> _comparer;

    /// <summary>
    /// Текущее значение.
    /// </summary>
    private T _value;

    /// <summary>
    /// Текущее значение. При установке нового происходит уведомление всех подписчиков.
    /// </summary>
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

    /// <summary>
    /// Оповещение при изменении значения.
    /// </summary>
    private event Action<T, T> Changed;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="initialValue"> Начальное значение. </param>
    /// <param name="comparer"> Собственный компаратор равенства. </param>
    public ObservableValue(T initialValue = default!, IEqualityComparer<T> comparer = null)
    {
        _value = initialValue;
        _comparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <summary>
    /// Подписывается на изменения значения.
    /// </summary>
    /// <param name="onChange"> Обработчик (старое значение, новое значение). Может быть null. </param>
    /// <returns> Объект, при вызове Dispose у которого подписка отменяется. </returns>
    public IDisposable Subscribe(Action<T, T> onChange)
    {
        if (onChange != null) Changed += onChange;

        return new Unsubscriber(() =>
        {
            if (onChange != null) Changed -= onChange;
        });
    }

    /// <summary>
    /// Подписывается на изменения с дополнительными вызовами жизненного цикла подписки.
    /// </summary>
    /// <param name="onInit"> Вызывается сразу при подписке с текущим значением для удобства первичной инициализации. </param>
    /// <param name="onChange"> Обработчик изменений (старое значение, новое значение). </param>
    /// <param name="onDeinit"> Вызывается при отписке с актуальным на тот момент значением. </param>
    /// <returns> Объект, при вызове Dispose у которого подписка отменяется. </returns>
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

    /// <summary>
    /// Одноразовый объект отписки. Выполняет отписку при вызове Dispose.
    /// </summary>
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
