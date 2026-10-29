using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Navigation.Installer
{
    public class NavigationInstaller : MonoInstaller
    {
        [SerializeField] private TabNavigationView _navigationView;

        public override void InstallBindings()
        {
            Container.BindInstance(_navigationView).AsSingle();

            Container.BindInterfacesAndSelfTo<TabNavigationPresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}