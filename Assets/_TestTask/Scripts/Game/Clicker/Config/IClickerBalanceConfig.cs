using TestTask_CKR_System.Audio;

namespace TestTask_CKR_System.Game.Clicker.Config
{
    public interface IClickerBalanceConfig
    {
        int CurrencyPerClick { get; }
        int MaximumEnergy { get; }
        int InitialEnergy { get; }
        int ManualClickEnergyCost { get; }
        int AutoClickEnergyCost { get; }
        float EnergyRestoreIntervalSeconds { get; }
        int EnergyRestoreAmount { get; }
        float AutoClickIntervalSeconds { get; }
        float PressedScale { get; }
        float ButtonAnimationDuration { get; }
        float CurrencyFlightDuration { get; }
        float CurrencyFlightDistance { get; }
        float ParticleBurstDuration { get; }
        float ParticleBurstDistance { get; }
        SoundType CollectSound { get; }
    }
}