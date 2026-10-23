using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class DependencyInjection : IDisposable
{
    private readonly List<Type> _singletonServiceTypes;
    private readonly List<Type> _scopedServiceTypes;
    private readonly List<Type> _transientServiceTypes;

    private readonly IContainer _services;
    private ILifetimeScope _sceneScope;

    public DependencyInjection()
    {
        var builder = new ContainerBuilder();

        var types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.IsClass && type.IsAbstract is false && typeof(IService).IsAssignableFrom(type))
            .ToList();

        _singletonServiceTypes = types
            .Where(type => typeof(IServiceSingleton).IsAssignableFrom(type))
            .ToList();

        _scopedServiceTypes = types
            .Where(type => typeof(IServiceScoped).IsAssignableFrom(type))
            .ToList();

        _transientServiceTypes = types
            .Where(x => typeof(IServiceTransient).IsAssignableFrom(x))
            .ToList();

        _singletonServiceTypes
            .ForEach(type => builder.RegisterType(type).SingleInstance().AutoActivate());

        _transientServiceTypes
            .ForEach(type => builder.RegisterType(type).InstancePerDependency());

        _services = builder.Build();
    }

    public void Dispose()
    {
        _services.Dispose();
    }

    public void CreateScope()
    {
        _sceneScope = _services.BeginLifetimeScope(scope =>
            _scopedServiceTypes.ForEach(type => scope.RegisterType(type).InstancePerLifetimeScope().AutoActivate()));
    }

    public void DeleteScope()
    {
        _sceneScope?.Dispose();
        _sceneScope = null;
    }
}
