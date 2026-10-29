using UnityEngine;

namespace TestTask_CKR_System.Game.Breeds.Config
{
    [CreateAssetMenu(fileName = "BreedConfig", menuName = "TestTask CKR-System/BreedConfig")]
    public class BreedConfig : ScriptableObject, IBreedConfig
    {
        [SerializeField] private int _maxListSize = 10;

        [Header("Pool")]
        [SerializeField] private int _initialPoolSize = 10;
        [SerializeField] private BreedItemView _itemPrefab;

        public int MaxListSize => _maxListSize;
        public int InitialPoolSize => _initialPoolSize;
        public BreedItemView ItemPrefab => _itemPrefab;
    }
}
