using System;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UiStorage : IDisposable
{
    private readonly VisualElement _parent;
    private readonly VisualElement _panel;

    private Storage _storage;

    private string _selectedStyle;
    private int _selectedIndex = -1;

    public UiStorage(
        VisualElement parent,
        string className,
        Storage storage,
        string selectedStyle)
    {
        _parent = parent;

        _panel = _parent
            .Create<VisualElement>()
            .WithClassName(className);

        _storage = storage;
        _storage.ObservableItems.CollectionChanged += OnCollectionChanged;
        _selectedStyle = selectedStyle;
        _selectedIndex = -1;

        OnCollectionChanged(this, new(NotifyCollectionChangedAction.Reset));
    }

    public void Dispose()
    {
        _storage.ObservableItems.CollectionChanged -= OnCollectionChanged;
        _storage = null;
        _selectedStyle = string.Empty;
        _selectedIndex = -1;

        _panel.Clear();

        _parent.Remove(_panel);
    }

    public VisualElement Get()
    {
        return _panel;
    }

    public Storage GetStorage()
    {
        return _storage;
    }

    public int GetSelectedIndex()
    {
        return _selectedIndex;
    }

    public void SetSelected(int index)
    {
        var oldIndex = _selectedIndex;
        _selectedIndex = index;
        RefreshStyle(oldIndex);
        RefreshStyle(index);
    }

    public (VisualElement val, int idx) GetItemUnderThePointer(Vector2 position)
    {
        return _panel
            .Children()
            .Select((val, idx) => (val, idx))
            .FirstOrDefault(x => x.val.worldBound.Contains(position));
    }

    public (VisualElement val, int idx) GetExistingItemUnderThePointer(Vector2 position)
    {
        return _panel
            .Children()
            .Select((val, idx) => (val, idx))
            .FirstOrDefault(x => x.val.worldBound.Contains(position)
                && _storage.ObservableItems.ElementAt(x.idx) != (int)Items.Item.None);
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    int offsetAdd = 0;

                    foreach (int newItem in e.NewItems)
                    {
                        var index = e.NewStartingIndex + offsetAdd;

                        _panel
                            .Create<Label>()
                            .WithClassName(StyleClassNames.InventoryItem)
                            .WithTexture(Items.List[newItem].Icon.texture);

                        offsetAdd++;
                    }
                }

                break;

            case NotifyCollectionChangedAction.Remove:
                {
                    int offsetRemove = 0;

                    foreach (int oldItem in e.OldItems)
                    {
                        var index = e.OldStartingIndex + offsetRemove;

                        if (index >= 0 && index < _panel.childCount)
                        {
                            _panel.RemoveAt(index);
                        }

                        offsetRemove++;
                    }
                }

                break;

            case NotifyCollectionChangedAction.Replace:
                {
                    int offsetReplace = 0;

                    foreach (int newItem in e.NewItems)
                    {
                        var index = e.OldStartingIndex + offsetReplace;

                        if (index >= 0 && index < _panel.childCount)
                        {
                            _panel.RemoveAt(index);

                            _panel
                                .CreateAt<Label>(index)
                                .WithClassName(StyleClassNames.InventoryItem)
                                .WithTexture(Items.List[newItem].Icon.texture);

                            RefreshStyle(index);
                        }

                        offsetReplace++;
                    }
                }

                break;

            case NotifyCollectionChangedAction.Reset:
                {
                    _panel.Clear();

                    foreach (var item in _storage.ObservableItems.Select((value, index) => (value, index)))
                    {
                        _panel
                            .Create<Label>()
                            .WithClassName(StyleClassNames.InventoryItem)
                            .WithTexture(Items.List[(int)item.value].Icon.texture);

                        RefreshStyle(item.index);
                    }
                }
                break;
        }
    }

    private void RefreshStyle(int index)
    {
        if (index < 0 || index >= _panel.childCount)
        {
            return;
        }

        if (index == _selectedIndex)
        {
            _panel.ElementAt(index).AddToClassList(_selectedStyle);
        }
        else
        {
            _panel.ElementAt(index).RemoveFromClassList(_selectedStyle);
        }
    }
}
