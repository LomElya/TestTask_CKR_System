namespace TestTask_CKR_System.Game.Breeds.Config
{
    public interface IBreedConfig
    {
        int MaxListSize { get; }
        int InitialPoolSize { get; }
        BreedItemView ItemPrefab { get; }
    }
}
