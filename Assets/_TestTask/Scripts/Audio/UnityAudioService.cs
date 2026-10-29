using UnityEngine;

namespace TestTask_CKR_System.Audio
{
    public class UnityAudioService : IAudioService
    {
        private readonly AudioSource _audioSource;
        private readonly SoundConfig _config;

        public UnityAudioService(AudioSource audioSource, SoundConfig config)
        {
            _audioSource = audioSource ??
                throw new System.ArgumentNullException(nameof(audioSource));

            _config = config;
        }

        public void Play(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null)
                return;

            _audioSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
        }

        public void Play(SoundType soundType)
        {
            if (!_config.TryGet(soundType, out var sound) || sound.Clip == null)
            {
                Debug.LogWarning($"Sound '{soundType}' is not configured.");
                return;
            }

            _audioSource.PlayOneShot(sound.Clip, sound.Volume);
        }

        public void Stop()
        {
            _audioSource.Stop();
        }
    }
}