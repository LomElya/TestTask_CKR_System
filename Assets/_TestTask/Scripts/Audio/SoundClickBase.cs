using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Audio
{
    public abstract class SoundClickBase : MonoBehaviour
    {
        [SerializeField] protected SoundType _typeClick = SoundType.Click;

        private IAudioService _audioService;

        [Inject]
        private void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected virtual void Play()
        {
            _audioService.Play(_typeClick);
        }
    }
}