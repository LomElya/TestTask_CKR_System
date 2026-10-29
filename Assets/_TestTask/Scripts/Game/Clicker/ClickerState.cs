namespace TestTask_CKR_System.Game.Clicker
{
    public readonly struct ClickerState
    {
        public ClickerState(long currency, int energy, int maximumEnergy)
        {
            Currency = currency;
            Energy = energy;
            MaximumEnergy = maximumEnergy;
        }

        public long Currency { get; }
        public int Energy { get; }
        public int MaximumEnergy { get; }
    }
}