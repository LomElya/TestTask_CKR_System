namespace TestTask_CKR_System.Common
{
    public interface ITimer : System.IDisposable
    {
        event System.Action<float> TimeUpdated;
        event System.Action TimeFinished;
        event System.Action TimerStopped;

        float MaxSeconds { get; }
        float RemainigSeconds { get; }

        bool IsActive { get; }
        bool IsPaused { get; }

        float PassedSeconds { get; }

        void Tick(float delta);

        void Start();
        void Start(float seconds);
        void Stop();

        void SetTime(float seconds);

        void SetPause(bool isPause);
        void Pause();
        void Unpause();
        void AddTime(float seconds);
    }
}