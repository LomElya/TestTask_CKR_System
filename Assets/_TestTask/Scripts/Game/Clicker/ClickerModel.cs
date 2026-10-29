using System;
using TestTask_CKR_System.Game.Clicker.Config;
using UnityEngine;

namespace TestTask_CKR_System.Game.Clicker
{
    public class ClickerModel : IClickerModel
    {
        public event Action<ClickerState> Changed;
        private readonly int _maximumEnergy;
        private long _currency;
        private int _energy;

        public ClickerState State => new ClickerState(_currency, _energy, _maximumEnergy);

        public ClickerModel(IClickerBalanceConfig config)
        {
            _maximumEnergy = config.MaximumEnergy;
            _energy = config.InitialEnergy;
        }

        public bool TryCollect(int energyCost, int currencyReward)
        {
            if (energyCost < 0)
                throw new ArgumentOutOfRangeException(nameof(energyCost));
            if (currencyReward < 0)
                throw new ArgumentOutOfRangeException(nameof(currencyReward));
            if (_energy < energyCost)
                return false;

            _energy -= energyCost;
            _currency += currencyReward;
            Changed?.Invoke(State);
            return true;
        }

        public void RestoreEnergy(int amount)
        {
            if (amount <= 0 || _energy >= _maximumEnergy)
                return;

            _energy = Mathf.Min(_energy + amount, _maximumEnergy);
            Changed?.Invoke(State);
        }
    }
}