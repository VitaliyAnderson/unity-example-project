using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

[UxmlObject]
public partial class LocalizedProperty : CustomBinding
{
    [UxmlAttribute("table")]
    public TableReference Table { get; set; }

    [UxmlAttribute("data-source-path-for-translate")]
    public string DataSourcePathForTranslate { get; set; }

    private VisualElement _element;
    private BindingId _bindingId;
    private LocalizedString _ls;

    protected override void OnActivated(in BindingActivationContext context)
    {
    }

    protected override void OnDataSourceChanged(in DataSourceContextChanged context)
    {
        if (_ls is not null)
        {
            _ls.StringChanged -= SetLocalizedText;
            _element = null;
            _bindingId = null;
            _ls = null;
        }

        if (context.newContext.dataSourcePath.IsEmpty)
        {
            var valuePath = new PropertyPath(DataSourcePathForTranslate);

            if (PropertyContainer.TryGetValue(context.newContext.dataSource, in valuePath, out string value) && value is not null)
            {
                _element = context.targetElement;
                _bindingId = context.bindingId;
                _ls = new LocalizedString(Table, value);
                _ls.StringChanged += SetLocalizedText;
            }
        }
        else
        {
            if (PropertyContainer.TryGetValue(context.newContext.dataSource, context.newContext.dataSourcePath, out object item) && item is not null)
            {
                var valuePath = new PropertyPath(DataSourcePathForTranslate);

                if (PropertyContainer.TryGetValue(item, in valuePath, out string value) && value is not null)
                {
                    _element = context.targetElement;
                    _bindingId = context.bindingId;
                    _ls = new LocalizedString(Table, value);
                    _ls.StringChanged += SetLocalizedText;
                }
            }
        }
    }

    protected override void OnDeactivated(in BindingActivationContext context)
    {
        if (_ls is not null)
        {
            _ls.StringChanged -= SetLocalizedText;
            _element = null;
            _bindingId = null;
            _ls = null;
        }
    }

    private void SetLocalizedText(string value)
    {
        ConverterGroups.TrySetValueGlobal(ref _element, _bindingId, value, out var _);
    }
}