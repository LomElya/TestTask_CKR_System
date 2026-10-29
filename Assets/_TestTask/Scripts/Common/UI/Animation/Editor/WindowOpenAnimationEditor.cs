#if UNITY_EDITOR
using UnityEditor;

namespace TestTask_CKR_System.Common.UI.Animation
{
    [CustomEditor(typeof(WindowOpenAnimation))]
    [CanEditMultipleObjects]
    public sealed class WindowOpenAnimationEditor : Editor
    {
        private SerializedProperty _animationType;
        private SerializedProperty _duration;
        private SerializedProperty _delay;
        private SerializedProperty _ease;
        private SerializedProperty _popEase;
        private SerializedProperty _useUnscaledTime;
        private SerializedProperty _disableInteractionDuringAnimation;
        private SerializedProperty _startScale;
        private SerializedProperty _horizontalOffset;
        private SerializedProperty _verticalOffset;
        private SerializedProperty _rotationAngle;
        private SerializedProperty _flipAngle;

        private void OnEnable()
        {
            _animationType = serializedObject.FindProperty("_animationType");
            _duration = serializedObject.FindProperty("_duration");
            _delay = serializedObject.FindProperty("_delay");
            _ease = serializedObject.FindProperty("_ease");
            _popEase = serializedObject.FindProperty("_popEase");
            _useUnscaledTime = serializedObject.FindProperty("_useUnscaledTime");
            _disableInteractionDuringAnimation =
                serializedObject.FindProperty("_disableInteractionDuringAnimation");
            _startScale = serializedObject.FindProperty("_startScale");
            _horizontalOffset = serializedObject.FindProperty("_horizontalOffset");
            _verticalOffset = serializedObject.FindProperty("_verticalOffset");
            _rotationAngle = serializedObject.FindProperty("_rotationAngle");
            _flipAngle = serializedObject.FindProperty("_flipAngle");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawScriptReference();
            EditorGUILayout.PropertyField(_animationType);

            if (_animationType.hasMultipleDifferentValues)
            {
                EditorGUILayout.HelpBox(
                    "У выбранных объектов разные типы анимации. " +
                    "Выберите один тип, чтобы увидеть его параметры.",
                    MessageType.Info);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            var animationType =
                (WindowOpenAnimationType)_animationType.enumValueIndex;
            if (animationType != WindowOpenAnimationType.None)
            {
                DrawTiming(animationType);
                DrawBehaviour();
                DrawTypeSpecificSettings(animationType);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptReference()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField(
                    "Script",
                    MonoScript.FromMonoBehaviour((WindowOpenAnimation)target),
                    typeof(WindowOpenAnimation),
                    false);
            }
        }

        private void DrawTiming(WindowOpenAnimationType animationType)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_duration);
            EditorGUILayout.PropertyField(_delay);

            if (animationType == WindowOpenAnimationType.Pop)
                EditorGUILayout.PropertyField(_popEase);
            else
                EditorGUILayout.PropertyField(_ease);
        }

        private void DrawBehaviour()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Behaviour", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_useUnscaledTime);
            EditorGUILayout.PropertyField(_disableInteractionDuringAnimation);
        }

        private void DrawTypeSpecificSettings(
            WindowOpenAnimationType animationType)
        {
            if (UsesScale(animationType))
            {
                DrawSettingsHeader();
                EditorGUILayout.PropertyField(_startScale);
                return;
            }

            if (UsesHorizontalOffset(animationType))
            {
                DrawSettingsHeader();
                EditorGUILayout.PropertyField(_horizontalOffset);
                return;
            }

            if (UsesVerticalOffset(animationType))
            {
                DrawSettingsHeader();
                EditorGUILayout.PropertyField(_verticalOffset);
                return;
            }

            if (UsesRotationAngle(animationType))
            {
                DrawSettingsHeader();
                EditorGUILayout.PropertyField(_rotationAngle);
                return;
            }

            if (UsesFlipAngle(animationType))
            {
                DrawSettingsHeader();
                EditorGUILayout.PropertyField(_flipAngle);
            }
        }

        private static void DrawSettingsHeader()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Start values", EditorStyles.boldLabel);
        }

        private static bool UsesScale(WindowOpenAnimationType animationType)
        {
            return animationType == WindowOpenAnimationType.Scale ||
                   animationType == WindowOpenAnimationType.ScaleAndFade ||
                   animationType == WindowOpenAnimationType.Pop;
        }

        private static bool UsesHorizontalOffset(
            WindowOpenAnimationType animationType)
        {
            return animationType == WindowOpenAnimationType.SlideFromLeft ||
                   animationType == WindowOpenAnimationType.SlideFromRight ||
                   animationType == WindowOpenAnimationType.SlideFromLeftAndFade ||
                   animationType == WindowOpenAnimationType.SlideFromRightAndFade;
        }

        private static bool UsesVerticalOffset(
            WindowOpenAnimationType animationType)
        {
            return animationType == WindowOpenAnimationType.SlideFromTop ||
                   animationType == WindowOpenAnimationType.SlideFromBottom ||
                   animationType == WindowOpenAnimationType.SlideFromTopAndFade ||
                   animationType == WindowOpenAnimationType.SlideFromBottomAndFade;
        }

        private static bool UsesRotationAngle(
            WindowOpenAnimationType animationType)
        {
            return animationType == WindowOpenAnimationType.RotateClockwise ||
                   animationType == WindowOpenAnimationType.RotateCounterClockwise;
        }

        private static bool UsesFlipAngle(WindowOpenAnimationType animationType)
        {
            return animationType == WindowOpenAnimationType.FlipHorizontal ||
                   animationType == WindowOpenAnimationType.FlipVertical;
        }
    }
}
#endif