using UnityEngine;

namespace TestTask_CKR_System.Common
{
    public class CutomTimer : ITimer
    {
        public event System.Action<float> TimeUpdated;
        public event System.Action TimeFinished;
        public event System.Action TimerStopped;

        public float MaxSeconds { get; private set; }
        public float RemainigSeconds { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsPaused { get; private set; }

        public float PassedSeconds => Mathf.Max(0f, MaxSeconds - RemainigSeconds);

        private float _updateInterval = 1f;
        private float _secondsAccumulator;

        public void SetTime(float seconds)
        {
            MaxSeconds = Mathf.Max(0f, seconds);
            RemainigSeconds = MaxSeconds;

            InvokeSeconds();
        }

        public void Start()
        {
            if (IsActive)
                return;

            if (RemainigSeconds == 0)
            {
                Debug.LogError("Таймер начался со значением 0");
                FinishedTimer();
                return;
            }

            IsActive = true;
            IsPaused = false;
            _secondsAccumulator = 0f;

            InvokeSeconds();
        }

        public void Start(float seconds)
        {
            if (IsActive)
                return;

            SetTime(seconds);
            Start();
        }

        public void SetPause(bool isPaused)
        {
            if (isPaused)
                Pause();

            else
                Unpause();
        }

        public void Pause()
        {
            if (IsPaused || !IsActive)
                return;

            IsPaused = true;

            InvokeSeconds();
        }

        public void Unpause()
        {
            if (!IsPaused || !IsActive)
                return;

            IsPaused = false;

            InvokeSeconds();
        }

        public void AddTime(float seconds)
        {
            if (!IsActive)
                return;

            RemainigSeconds = Mathf.Max(0f, RemainigSeconds + seconds);
            InvokeSeconds();
        }

        public void ReduceTime(float seconds)
        {
            if (!IsActive)
                return;

            RemainigSeconds = Mathf.Max(0f, RemainigSeconds - seconds);

            InvokeSeconds();
            CheckFinished();
        }

        public void Stop()
        {
            if (!IsActive)
                return;

            RemainigSeconds = 0f;

            IsPaused = false;
            IsActive = false;

            InvokeSeconds();
            //FinishedTimer();
            StopTimer();
        }

        public void Tick(float delta)
        {
            if (!IsActive || IsPaused)
                return;

            if (delta <= 0f)
                return;

            RemainigSeconds = Mathf.Max(0f, RemainigSeconds - delta);
            _secondsAccumulator += delta;

            while (_secondsAccumulator >= _updateInterval)
            {
                _secondsAccumulator -= _updateInterval;
                InvokeSeconds();
            }

            CheckFinished();
        }

        private void CheckFinished()
        {
            if (RemainigSeconds > 0f)
                return;

            // Stop();
            RefreshTime();
            FinishedTimer();
        }

        private void RefreshTime()
        {
            if (!IsActive)
                return;

            _secondsAccumulator = 0f;
            RemainigSeconds = MaxSeconds;
        }

        private void InvokeSeconds() => TimeUpdated?.Invoke(RemainigSeconds);
        private void FinishedTimer() => TimeFinished?.Invoke();
        private void StopTimer() => TimerStopped?.Invoke();
        public void Dispose()
        {
        }
    }
}