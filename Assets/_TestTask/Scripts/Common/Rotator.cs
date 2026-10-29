using DG.Tweening;
using UnityEngine;

namespace TestTask_CKR_System.Common
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private GameObject _root;

        [Header("Settings")]
        [SerializeField] private float _duration = 1.0f;
        [SerializeField] private float _angle = -360f;
        [SerializeField] private RotateMode _rotateMode = RotateMode.LocalAxisAdd;
        [SerializeField] private Ease _ease = Ease.Linear;

        private void OnEnable() => StartAnimation();
        private void OnDisable() => StopAnimation();

        public void StartAnimation()
        {
            if (_root == null) return;
            _root.transform.DOKill();

            Tweener rotationTween = _root.transform.DOLocalRotate(
                new Vector3(0, 0, _angle),
                _duration,
                _rotateMode
            );

            rotationTween.SetLoops(-1, LoopType.Incremental);

            rotationTween.SetEase(_ease);
        }

        public void StopAnimation()
        {
            if (_root == null) return;
            _root.transform.DOKill();
        }
    }
}