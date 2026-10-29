using System;
using System.Collections.Generic;
using Zenject;

namespace TestTask_CKR_System.Game.Navigation
{
    public class TabNavigationPresenter : IInitializable, IDisposable
    {
        private readonly TabNavigationView _view;
        private readonly Dictionary<AppTab, ITabController> _controllers;
        private ITabController _current;

        public TabNavigationPresenter(
            TabNavigationView view,
            ITabController[] controllers)
        {
            _view = view;
            _controllers = new Dictionary<AppTab, ITabController>();

            foreach (var controller in controllers)
            {
                if (_controllers.ContainsKey(controller.Tab))
                    throw new InvalidOperationException($"Duplicate tab controller: {controller.Tab}");

                _controllers.Add(controller.Tab, controller);
            }
        }

        public void Initialize()
        {
            _view.TabSelected += Select;

            AppTab[] tabs = _view.ConfiguredTabs;

            if (tabs == null) return;
            ValidateConfiguredTabs(tabs);

            if (tabs.Length > 0)
                Select(tabs[0]);
        }

        public void Dispose()
        {
            if (_view != null)
                _view.TabSelected -= Select;
            _current = null;
        }

        private void Select(AppTab tab)
        {
            if (_current != null && _current.Tab == tab)
                return;
            if (!_controllers.TryGetValue(tab, out var next))
                throw new InvalidOperationException($"No controller is registered for tab {tab}.");

            _current?.Deactivate();
            _current = next;
            _view.Show(tab);
            _current.Activate();
        }

        private void ValidateConfiguredTabs(IEnumerable<AppTab> tabs)
        {
            foreach (var tab in tabs)
                _view.ChangeActive(tab, _controllers.ContainsKey(tab));
        }
    }
}