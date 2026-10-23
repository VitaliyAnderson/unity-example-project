using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIPanelService : IServiceScoped, IDisposable
{
    protected VisualElement Panel { get; }

    protected abstract bool ShowCursor { get; }

    protected UIPanelService(UIService uiService, string panelName)
    {
        Panel = uiService.RootVisualElement.Q<VisualElement>(panelName).SetVisible(false);
        Panel.dataSource = this;
    }

    public void Dispose()
    {
        if (IsVisible())
        {
            Hide();
        }
    }

    public void Show()
    {
        Panel.SetVisible(true);
        Panel.Focus();
        OnShow();
        if (ShowCursor)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Hide()
    {
        Panel.SetVisible(false);
        OnHide();
    }

    public bool IsVisible()
    {
        return Panel.IsVisible();
    }

    public virtual void OnShow()
    {
    }

    public virtual void OnHide()
    {
    }
}
