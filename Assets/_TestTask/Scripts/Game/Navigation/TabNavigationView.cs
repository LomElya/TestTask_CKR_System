using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TestTask_CKR_System.Game.Navigation
{
    public class TabNavigationView : MonoBehaviour
    {
        [SerializeField] private TabBinding[] _tabs;

        private UnityAction[] _buttonActions;

        public event System.Action<AppTab> TabSelected;

        public AppTab[] ConfiguredTabs
        {
            get
            {
                if (_tabs == null)
                    return Array.Empty<AppTab>();

                var tabs = new AppTab[_tabs.Length];
                for (var index = 0; index < _tabs.Length; index++)
                    tabs[index] = _tabs[index].Tab;

                return tabs;
            }
        }

        private void Awake()
        {
            _buttonActions = new UnityAction[_tabs.Length];
            for (var index = 0; index < _tabs.Length; index++)
            {
                var capturedTab = _tabs[index].Tab;
                _buttonActions[index] = () => TabSelected?.Invoke(capturedTab);
                _tabs[index].Button.onClick.AddListener(_buttonActions[index]);
            }
        }

        private void OnDestroy()
        {
            if (_buttonActions == null)
                return;

            for (var index = 0; index < _tabs.Length; index++)
                _tabs[index].Button.onClick.RemoveListener(_buttonActions[index]);
        }

        public void Show(AppTab selectedTab)
        {
            foreach (var binding in _tabs)
            {
                var selected = binding.Tab == selectedTab;
                binding.Content.SetActive(selected);
                binding.Button.interactable = !selected;
            }
        }

        public void ChangeActive(AppTab tab, bool isActive)
        {
            for (int i = 0; i < _tabs.Length; i++)
            {
                if (_tabs[i].Tab == tab)
                    _tabs[i].Button?.gameObject.SetActive(isActive);
            }
        }

        [System.Serializable]
        private class TabBinding
        {
            [field: SerializeField] public AppTab Tab { get; private set; }
            [field: SerializeField] public Button Button { get; private set; }
            [field: SerializeField] public GameObject Content { get; private set; }
        }
    }
}