namespace TestTask_CKR_System.Game.Breeds
{
    public struct BreedDetails
    {
        public BreedDetails(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
    }
}