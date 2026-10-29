using System;
using System.Collections.Generic;
using TestTask_CKR_System.Core.Networking;
using TestTask_CKR_System.Game.Popup;
using UnityEngine;
using Zenject;

namespace TestTask_CKR_System.Game.Breeds
{
    public class BreedsPresenter : ITabController, IDisposable, IInitializable
    {
        public AppTab Tab => AppTab.Breeds;

        private readonly IRequestQueue _requestQueue;
        private readonly IBreedService _breedService;
        private readonly BreedListView _view;
        private readonly PopupView _popup;
        private readonly BreedItemView.Pool _itemPool;
        private readonly List<BreedItemView> _items = new();
        private readonly string _listOwnerId = $"breed-list-{Guid.NewGuid():N}";
        private readonly string _detailsOwnerId = $"breed-details-{Guid.NewGuid():N}";

        private RequestHandle<BreedDetails> _detailsHandle;
        private bool _isActive;
        private int _activationVersion;
        private int _selectionVersion;


        public BreedsPresenter(
            IRequestQueue requestQueue,
            IBreedService breedService,
            BreedListView view,
            PopupView popup,
            BreedItemView.Pool itemPool)
        {
            _requestQueue = requestQueue;
            _breedService = breedService;
            _view = view;
            _popup = popup;
            _itemPool = itemPool;
        }

        public void Initialize()
        {
            _popup.Hide();
        }

        public void Activate()
        {
            _isActive = true;
            _activationVersion++;
            LoadListAsync(_activationVersion);
        }

        public void Deactivate()
        {
            _isActive = false;
            _activationVersion++;
            _selectionVersion++;
            _requestQueue.CancelOwner(_listOwnerId);
            _requestQueue.CancelOwner(_detailsOwnerId);
            _detailsHandle = null;
            _view.SetListLoading(false);
            SetLoadingItem(null);
            _popup.Hide();
        }

        public void Dispose()
        {
            _isActive = false;
            _activationVersion++;
            _selectionVersion++;
            _requestQueue.CancelOwner(_listOwnerId);
            _requestQueue.CancelOwner(_detailsOwnerId);
            _detailsHandle = null;
            _items.Clear();
        }

        private async void LoadListAsync(int activationVersion)
        {
            ClearItems();
            _view.ClearError();
            _view.SetListLoading(true);

            var handle = _requestQueue.Enqueue(
                _listOwnerId,
                token => _breedService.GetBreedsAsync(token));

            try
            {
                var breeds = await handle.Completion;
                if (!_isActive || activationVersion != _activationVersion)
                    return;

                for (var index = 0; index < breeds.Count; index++)
                    _items.Add(_itemPool.Spawn(breeds[index], index + 1, OnBreedSelected));
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                if (_isActive && activationVersion == _activationVersion)
                    _view.ShowError($"Не удалось загрузить породы: {exception.Message}");
            }
            finally
            {
                if (_isActive && activationVersion == _activationVersion)
                    _view.SetListLoading(false);
            }
        }

        private void OnBreedSelected(BreedSummary breed)
        {
            if (!_isActive || breed.Equals(default(BreedSummary)))
                return;

            _detailsHandle?.Cancel();
            _selectionVersion++;
            SetLoadingItem(breed.Id);
            LoadDetailsAsync(breed, _selectionVersion, _activationVersion);
        }

        private async void LoadDetailsAsync(
            BreedSummary breed,
            int selectionVersion,
            int activationVersion)
        {
            var handle = _requestQueue.Enqueue(
                _detailsOwnerId,
                token => _breedService.GetBreedAsync(breed.Id, token));
            _detailsHandle = handle;

            try
            {
                var details = await handle.Completion;
                if (IsCurrent(selectionVersion, activationVersion))
                    _popup.Show(new(details.Name, details.Description));
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Cancel Details");
            }
            catch (Exception exception)
            {
                if (IsCurrent(selectionVersion, activationVersion))
                    _view.ShowError($"Не удалось загрузить породу: {exception.Message}");
            }
            finally
            {
                if (IsCurrent(selectionVersion, activationVersion))
                {
                    SetLoadingItem(null);
                    _detailsHandle = null;
                }
            }
        }

        private bool IsCurrent(int selectionVersion, int activationVersion)
        {
            return _isActive &&
                   selectionVersion == _selectionVersion &&
                   activationVersion == _activationVersion;
        }

        private void SetLoadingItem(string breedId)
        {
            foreach (var item in _items)
                item.SetLoading(item.BreedId == breedId);
        }

        private void ClearItems()
        {
            foreach (var item in _items)
                _itemPool.Despawn(item);

            _items.Clear();
        }
    }
}