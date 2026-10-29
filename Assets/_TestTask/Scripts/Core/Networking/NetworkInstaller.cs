using TestTask_CKR_System.Core.Networking.Config;
using TestTask_CKR_System.Core.Networking.Interfaces;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Core.Networking
{
    public class NetworkInstaller : MonoInstaller
    {
        [SerializeField] private NetworkConfig _networkConfig;

        public override void InstallBindings()
        {
            BindConfiguration();
            Container.BindInterfacesAndSelfTo<RequestQueue>().AsSingle();
            Container.Bind<IHttpClient>().To<UnityWebRequestHttpClient>().AsSingle();
            Container.Bind<IRemoteImageService>().To<RemoteImageService>().AsSingle();
        }

        private void BindConfiguration()
        {
            Container.BindInstance(_networkConfig).AsSingle();
        }
    }
}