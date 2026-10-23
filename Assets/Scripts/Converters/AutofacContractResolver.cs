using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Newtonsoft.Json.Serialization;
using System;

public class AutofacContractResolver : UnityContractResolver
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacContractResolver(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        // use Autofac to create types that have been registered with it
        if (_lifetimeScope.IsRegistered(objectType))
        {
            JsonObjectContract contract = ResolveContact(objectType);
            contract.DefaultCreator = () => _lifetimeScope.Resolve(objectType);

            return contract;
        }

        return base.CreateObjectContract(objectType);
    }

    private JsonObjectContract ResolveContact(Type objectType)
    {
        // attempt to create the contact from the resolved type
        IComponentRegistration registration;
        if (_lifetimeScope.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out registration))
        {
            Type viewType = (registration.Activator as ReflectionActivator)?.LimitType;
            if (viewType != null)
            {
                return base.CreateObjectContract(viewType);
            }
        }

        // fall back to using the registered type
        return base.CreateObjectContract(objectType);
    }
}
