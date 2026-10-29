using TestTask_CKR_System.Audio;
using TestTask_CKR_System.Core.Utils.Coroutin;
using TestTask_CKR_System.Core.Utils.Serializer;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Core
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner _coroutineRunner;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SoundConfig _soundConfig;

        public override void InstallBindings()
        {
            Container.Bind<IJsonSerializer>()
                .To<NewtonsoftJsonSerializer>()
                .AsSingle();

            Container.Bind<ICoroutineRunner>()
                .FromInstance(_coroutineRunner)
                .AsSingle();

            Container.BindInstance(_soundConfig).AsSingle();

            IAudioService audioService = new UnityAudioService(_audioSource, _soundConfig);
            Container.Bind<IAudioService>()
                .FromInstance(audioService)
                .AsSingle();
            //   Container.BindInterfacesAndSelfTo<RequestQueue>().AsSingle();
        }
    }
}