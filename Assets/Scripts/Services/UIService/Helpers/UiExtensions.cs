using System;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiExtensions
{
    public static T Create<T>()
        where T : VisualElement, new()
    {
        var element = new T();
        return element;
    }

    public static T Create<T>(this VisualElement parent)
        where T : VisualElement, new()
    {
        var element = new T();
        parent.Add(element);
        return element;
    }

    public static T CreateAt<T>(this VisualElement parent, int index)
        where T : VisualElement, new()
    {
        var element = new T();
        parent.Insert(index, element);
        return element;
    }

    public static T WithClassName<T>(this T element, string className)
        where T : VisualElement
    {
        element.AddToClassList(className);
        return element;
    }

    public static T WithText<T>(this T element, string text)
        where T : TextElement
    {
        element.text = text;
        return element;
    }

    public static T WithTexture<T>(this T element, Texture2D texture)
        where T : TextElement
    {
        element.style.backgroundImage = texture;
        return element;
    }

    public static T WithTexture<T>(this T element, Background background)
        where T : TextElement
    {
        element.style.backgroundImage = background;
        return element;
    }

    public static T OnClick<T>(this T element, Action action)
        where T : Button
    {
        element.clickable = new Clickable(action);
        return element;
    }

    public static T BindClickHandler<T>(this T element, Action onClick)
        where T : Button
    {
        element.clicked += onClick;
        return element;
    }

    public static T UnbindClickHandler<T>(this T element, Action onClick)
        where T : Button
    {
        element.clicked -= onClick;
        return element;
    }

    public static T SetDataSource<T>(this T element, object dataSource)
        where T : VisualElement
    {
        element.dataSource = dataSource;
        return element;
    }

    public static T SetEnable<T>(this T element, bool value)
        where T : VisualElement
    {
        element.SetEnabled(value);
        return element;
    }

    public static bool IsEnable<T>(this T element)
        where T : VisualElement
    {
        return element.enabledSelf;
    }

    public static T SetVisible<T>(this T element, bool value)
        where T : VisualElement
    {
        element.visible = value;
        return element;
    }

    public static bool IsVisible<T>(this T element)
        where T : VisualElement
    {
        return element.visible;
    }
}
