using UnityEngine.Localization.Tables;

public class LocalizationService : IServiceSingleton
{
    public readonly TableReference CommonStringTableReference = "CommonStringTable";
    public readonly TableEntryReference CommonStringTableEntryReferenceEmpty = "Empty";
}
