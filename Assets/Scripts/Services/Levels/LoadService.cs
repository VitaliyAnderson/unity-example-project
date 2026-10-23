using Autofac;
using Newtonsoft.Json;
using System.IO;

public class LoadService : IServiceScoped
{
    private readonly ILifetimeScope _lifetimeScope;

    public LoadService(
        ILifetimeScope lifetimeScope,
        LevelService levelService,
        NewGameService newGameService)
    {
        _lifetimeScope = lifetimeScope;

        if (levelService.IsMenu is false)
        {
            if (levelService.LoadingLevelPath == null)
            {
                newGameService.NewGame();
            }
            else
            {
                Restore(new StreamReader(levelService.LoadingLevelPath));
                levelService.LoadingLevelPath = null;
            }
        }
    }

    private void Restore(StreamReader sr)
    {
        using var reader = new JsonTextReader(sr);

        var serializer = new JsonSerializer
        {
            ContractResolver = new AutofacContractResolver(_lifetimeScope),
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        };

        serializer.Populate(reader, _lifetimeScope.Resolve<SaveContent>());
    }
}
