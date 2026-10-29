using TestTask_CKR_System.Audio;
using UnityEngine;

namespace TestTask_CKR_System.Game.Clicker.Config
{

    [CreateAssetMenu(fileName = "ClickerBalanceConfig", menuName = "TestTask CKR-System/Clicker Balance")]
    public class ClickerBalanceConfig : ScriptableObject, IClickerBalanceConfig
    {
        [Header("Currency")]
        [SerializeField, Min(1)] private int _currencyPerClick = 1;

        [Header("Energy")]
        [SerializeField, Min(1)] private int _maximumEnergy = 1000;
        [SerializeField, Min(0)] private int _initialEnergy = 1000;
        [SerializeField, Min(1)] private int _manualClickEnergyCost = 1;
        [SerializeField, Min(1)] private int _autoClickEnergyCost = 1;
        [SerializeField, Min(0.1f)] private float _energyRestoreIntervalSeconds = 10f;
        [SerializeField, Min(1)] private int _energyRestoreAmount = 10;

        [Header("Auto collection")]
        [SerializeField, Min(0.1f)] private float _autoClickIntervalSeconds = 3f;

        [Header("Feedback")]
        [SerializeField, Range(0.5f, 1f)] private float _pressedScale = 0.88f;
        [SerializeField, Min(0.01f)] private float _buttonAnimationDuration = 0.15f;
        [SerializeField, Min(0.01f)] private float _currencyFlightDuration = 0.65f;
        [SerializeField, Min(1f)] private float _currencyFlightDistance = 140f;
        [SerializeField, Min(0.01f)] private float _particleBurstDuration = 0.55f;
        [SerializeField, Min(1f)] private float _particleBurstDistance = 190f;

        [Header("Pools")]
        [SerializeField] private CurrencyFlyView _currencyFlyViewPrefab;
        [SerializeField, Min(1)] private int _currencyFlyViewPoolSize = 8;

        [Header("Audio")]
        [SerializeField] private SoundType _collectSound;

        public int CurrencyPerClick => _currencyPerClick;
        public int MaximumEnergy => _maximumEnergy;
        public int InitialEnergy => Mathf.Clamp(_initialEnergy, 0, _maximumEnergy);
        public int ManualClickEnergyCost => _manualClickEnergyCost;
        public int AutoClickEnergyCost => _autoClickEnergyCost;
        public float EnergyRestoreIntervalSeconds => _energyRestoreIntervalSeconds;
        public int EnergyRestoreAmount => _energyRestoreAmount;
        public float AutoClickIntervalSeconds => _autoClickIntervalSeconds;
        public float PressedScale => _pressedScale;
        public float ButtonAnimationDuration => _buttonAnimationDuration;
        public float CurrencyFlightDuration => _currencyFlightDuration;
        public float CurrencyFlightDistance => _currencyFlightDistance;
        public float ParticleBurstDuration => _particleBurstDuration;
        public float ParticleBurstDistance => _particleBurstDistance;
        public SoundType CollectSound => _collectSound;

        public CurrencyFlyView CurrencyFlyViewPrefab => _currencyFlyViewPrefab;
        public int CurrencyFlyViewPoolSize => _currencyFlyViewPoolSize;


        private void OnValidate()
        {
            _initialEnergy = Mathf.Clamp(_initialEnergy, 0, _maximumEnergy);
        }
    }
}