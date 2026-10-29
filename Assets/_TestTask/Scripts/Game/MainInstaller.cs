using TestTask_CKR_System.Game.Popup;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private PopupView _popup;

        public override void InstallBindings()
        {
            Container.BindInstance(_popup).AsSingle();
        }
    }
}
