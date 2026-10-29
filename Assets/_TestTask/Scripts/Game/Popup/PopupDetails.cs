namespace TestTask_CKR_System.Game.Popup
{
    [System.Serializable]
    public struct PopupDetails
    {
        public PopupDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}
