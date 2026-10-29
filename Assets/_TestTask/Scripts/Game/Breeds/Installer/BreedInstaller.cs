using TestTask_CKR_System.Game.Breeds.Config;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Breeds.Installer
{
    public class BreedInstaller : MonoInstaller
    {
        [SerializeField] private BreedConfig _config;
        [SerializeField] private BreedListView _breedListView;

        public override void InstallBindings()
        {
            Container.Bind<IBreedConfig>()
                .FromInstance(_config)
                .AsSingle();

            Container.BindMemoryPool<BreedItemView, BreedItemView.Pool>()
                .WithInitialSize(_config.InitialPoolSize)
                .FromComponentInNewPrefab(_config.ItemPrefab)
                .UnderTransform(_breedListView.Content);

            Container.Bind<BreedListView>()
                .FromInstance(_breedListView)
                .AsSingle();

            Container.Bind<IBreedService>()
                .To<BreedService>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<BreedsPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}