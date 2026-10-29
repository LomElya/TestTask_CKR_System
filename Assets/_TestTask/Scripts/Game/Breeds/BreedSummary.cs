namespace TestTask_CKR_System.Game.Breeds
{
    public struct BreedSummary
    {
        public BreedSummary(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}