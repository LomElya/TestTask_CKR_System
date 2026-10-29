using System;
using TestTask_CKR_System.Audio;
using TestTask_CKR_System.Common;
using TestTask_CKR_System.Game.Clicker.Config;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Clicker
{
    public class ClickerPresenter : ITabController, IInitializable, ITickable, IDisposable
    {
        public AppTab Tab => AppTab.Clicker;

        private readonly IClickerModel _model;
        private readonly ClickerView _view;
        private readonly IClickerBalanceConfig _config;
        private readonly IAudioService _audioService;
        private bool _isActive;
        private ITimer _autoClickTimer = new CutomTimer();
        private ITimer _energyRestoreTimer = new CutomTimer();

        public ClickerPresenter(
            IClickerModel model,
            ClickerView view,
            IClickerBalanceConfig config,
            IAudioService audioService)
        {
            _model = model;
            _view = view;
            _config = config;
            _audioService = audioService;
        }


        public void Initialize()
        {
            _view.CollectRequested += OnManualCollectRequested;
            _model.Changed += OnModelChanged;
            OnModelChanged(_model.State);

            InitializeTimer(_autoClickTimer, _config.AutoClickIntervalSeconds, OnAutoClick);
            InitializeTimer(_energyRestoreTimer, _config.EnergyRestoreIntervalSeconds, RestoreEnergy);
        }

        private void OnAutoClick() => Collect(_config.AutoClickEnergyCost);

        private void RestoreEnergy() => _model.RestoreEnergy(_config.EnergyRestoreAmount);

        public void Tick()
        {
            float delta = Time.deltaTime;
            _autoClickTimer.Tick(delta);
            _energyRestoreTimer.Tick(delta);
        }

        public void Activate()
        {
            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
        }

        public void Dispose()
        {
            if (_view != null)
                _view.CollectRequested -= OnManualCollectRequested;
            _model.Changed -= OnModelChanged;

            DisposeTimer(_autoClickTimer, OnAutoClick);
            DisposeTimer(_energyRestoreTimer, RestoreEnergy);
        }

        private void OnManualCollectRequested()
        {
            Collect(_config.ManualClickEnergyCost);
        }

        private void OnModelChanged(ClickerState state)
        {
            _view.Render(state, state.Energy >= _config.ManualClickEnergyCost);
        }

        private void Collect(int energyCost)
        {
            if (!_model.TryCollect(energyCost, _config.CurrencyPerClick) || !_isActive)
                return;

            _view.PlayCollectFeedback(_config.CurrencyPerClick);
            _audioService.Play(_config.CollectSound);
        }

        private void InitializeTimer(ITimer timer, float timeDuration, Action action)
        {
            timer.SetTime(timeDuration);
            timer.TimeFinished += action;
            timer.Start();
        }

        private void DisposeTimer(ITimer timer, Action action)
        {
            if (timer == null) return;
            timer.TimeFinished -= action;
            timer.Stop();
            timer.Dispose();
        }

    }
}