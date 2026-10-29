using System.Collections.Generic;
using UnityEngine;

namespace TestTask_CKR_System.Audio
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "TestTask CKR-System/SoundConfig")]
    public class SoundConfig : ScriptableObject
    {
        [SerializeField]
        private List<SoundDefinition> _sounds = new();

        private Dictionary<SoundType, SoundDefinition> _lookup;

        public bool TryGet(SoundType id, out SoundDefinition sound)
        {
            if (_lookup == null)
                RebuildLookup();

            return _lookup.TryGetValue(id, out sound);
        }

        private void RebuildLookup()
        {
            _lookup = new();
            foreach (var sound in _sounds)
            {
                if (sound.Equals(default(SoundDefinition)))
                    continue;

                if (_lookup.ContainsKey(sound.Id))
                {
                    Debug.LogError(
                        $"SoundConfig contains duplicate id '{sound.Id}'.",
                        this);
                    continue;
                }

                _lookup.Add(sound.Id, sound);
            }
        }
    }

    [System.Serializable]
    public struct SoundDefinition
    {
        [SerializeField] private SoundType _id;
        [SerializeField] private AudioClip _clip;
        [SerializeField, Range(0f, 1f)] private float _volume;

        public SoundType Id => _id;
        public AudioClip Clip => _clip;
        public float Volume => Mathf.Clamp01(_volume);
    }
}