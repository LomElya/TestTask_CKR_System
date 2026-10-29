namespace TestTask_CKR_System.Game.Clicker
{
    public interface IClickerModel
    {
        event System.Action<ClickerState> Changed;
        ClickerState State { get; }
        bool TryCollect(int energyCost, int currencyReward);
        void RestoreEnergy(int amount);
    }
}