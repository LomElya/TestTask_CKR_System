using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TestTask_CKR_System.Game.Breeds
{
    public class BreedItemView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _indexText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private GameObject _loader;

        private BreedSummary _breed;
        private System.Action<BreedSummary> _selected;
        public string BreedId => _breed.Id;

        private void OnEnable() => _button.onClick.AddListener(Select);
        private void OnDisable() => _button.onClick.RemoveListener(Select);

        private void Initialize(
            BreedSummary breed,
            int displayIndex,
            System.Action<BreedSummary> selected)
        {
            _breed = breed;
            _selected = selected;
            _indexText.SetText("{0}", displayIndex);
            _nameText.text = breed.Name;
            SetLoading(false);
            transform.SetAsLastSibling();
        }

        private void ResetState()
        {
            _breed = default;
            _selected = null;
            SetLoading(false);
        }

        public void SetLoading(bool isLoading)
        {
            if (_loader != null)
                _loader.SetActive(isLoading);
            _button.interactable = !isLoading;
        }

        private void Select()
        {
            _selected?.Invoke(_breed);
        }

        public class Pool :
            MonoMemoryPool<BreedSummary, int, System.Action<BreedSummary>, BreedItemView>
        {
            protected override void Reinitialize(
                BreedSummary breed,
                int displayIndex,
                System.Action<BreedSummary> selected,
                BreedItemView item)
            {
                item.Initialize(breed, displayIndex, selected);
            }

            protected override void OnDespawned(BreedItemView item)
            {
                item.ResetState();
                base.OnDespawned(item);
            }
        }
    }
}