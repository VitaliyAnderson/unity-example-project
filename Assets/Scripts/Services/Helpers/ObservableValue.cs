using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

public class ObservableValue<T> : IDisposable
{
    private readonly BehaviorSubject<T> _subject = new(default);

    public T Value
    {
        get => _subject.Value;
        set => _subject.OnNext(value);
    }

    public IDisposable Subscribe(Action<T> onInit, Action<T, T> onChange, Action<T> onDeinit)
    {
        onInit?.Invoke(_subject.Value);

        var changesSubscription = _subject
            .Scan((Old: default(T), New: default(T)), (state, next) => (state.New, next))
            .Skip(1)
            .Subscribe(pair => onChange?.Invoke(pair.Old, pair.New));

        return Disposable.Create(() =>
        {
            changesSubscription.Dispose();
            onDeinit?.Invoke(_subject.Value);
        });
    }

    public void Dispose()
    {
        _subject.Dispose();
    }
}
