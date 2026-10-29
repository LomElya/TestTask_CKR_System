using UnityEngine;

namespace TestTask_CKR_System.Audio
{
    public interface IAudioService
    {
        void Play(SoundType soundType);
        void Play(AudioClip clip, float volumeScale = 1f);
        void Stop();
    }
}