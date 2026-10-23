using System.Windows.Input;
using Unity.Properties;
using UnityEngine.UIElements;

[UxmlObject]
public partial class RegisterEventBinding<TEventType> : CustomBinding
    where TEventType : EventBase<TEventType>, new()
{
    [UxmlAttribute("callback")]
    public string Callback { get; set; }

    private VisualElement _element;
    private ICommand _command;

    protected override void OnActivated(in BindingActivationContext context)
    {
    }

    protected override void OnDataSourceChanged(in DataSourceContextChanged context)
    {
        _element?.UnregisterCallback<TEventType>(OnCallback);
        _element = null;
        _command = null;

        if (context.newContext.dataSourcePath.IsEmpty)
        {
            var callbackPath = new PropertyPath(Callback);

            if (PropertyContainer.TryGetValue(context.newContext.dataSource, in callbackPath, out ICommand command) && command is not null)
            {
                _element = context.targetElement;
                _command = command;
                _element.RegisterCallback<TEventType>(OnCallback);
            }
        }
        else
        {
            if (PropertyContainer.TryGetValue(context.newContext.dataSource, context.newContext.dataSourcePath, out object item) && item is not null)
            {
                var callbackPath = new PropertyPath(Callback);

                if (PropertyContainer.TryGetValue(item, in callbackPath, out ICommand command) && command is not null)
                {
                    _element = context.targetElement;
                    _command = command;
                    _element.RegisterCallback<TEventType>(OnCallback);
                }
            }
        }
    }

    protected override void OnDeactivated(in BindingActivationContext context)
    {
        _element?.UnregisterCallback<TEventType>(OnCallback);
        _element = null;
        _command = null;
    }

    private void OnCallback(TEventType evt)
    {
        if (_command?.CanExecute(evt) is true)
        {
            _command.Execute(evt);
        }
    }
}