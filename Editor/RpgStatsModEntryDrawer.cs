using AV.Lifetime.Realtime;
using AV.RpgStats.Runtime;
using UnityEditor;
using UnityEngine;

namespace AV.RpgStats.Editor
{
    [CustomPropertyDrawer(typeof(RpgStatModifierToggle.RpgStatsModEntry))]
    public sealed class RpgStatsModEntryDrawer : PropertyDrawer
    {
        private ITargetContextSystem _contextSystem;
        private SerializedProperty _modifierProp;
        private SerializedProperty _rpgStatScriptProp;
        private SerializedProperty _targetProp;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeProperties(property);

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            // Draw RpgStatScript
            EditorGUI.PropertyField(rect, _rpgStatScriptProp);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Draw Modifier
            EditorGUI.PropertyField(rect, _modifierProp);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Draw Target with ping functionality
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, _targetProp);
            if (EditorGUI.EndChangeCheck()) PingTarget(_targetProp.enumValueIndex);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitializeProperties(property);
            var lineCount = 3;
            var spacing = EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
            return EditorGUIUtility.singleLineHeight * lineCount + spacing;
        }

        private void InitializeProperties(SerializedProperty property)
        {
            _rpgStatScriptProp = property.FindPropertyRelative("RpgStatScript");
            _modifierProp = property.FindPropertyRelative("Modifier");
            _targetProp = property.FindPropertyRelative("Target");

            // Cache ITargetContextSystem reference
            if (_contextSystem == null)
            {
                var targetObject = property.serializedObject.targetObject;
                if (targetObject is MonoBehaviour behaviour) 
                    _contextSystem = behaviour.GetComponent<ITargetContextSystem>();
            }
        }

        private void PingTarget(int targetIndex)
        {
            if (_contextSystem == null) return;

            var target = (ETarget)targetIndex;
            if (!TryGetTargetTransform(target, out var targetTransform)) return;

            if (targetTransform != null) EditorGUIUtility.PingObject(targetTransform);
        }

        private bool TryGetTargetTransform(ETarget target, out Transform transform)
        {
            transform = null;
            if (_contextSystem == null) return false;

            if (_contextSystem.TryGetContext(out var context) != TargetContextResult.Success)
                return false;

            var self = (_contextSystem as MonoBehaviour).transform;

            return TargetContextLogic.TryGetTransform(self, context, in target, out transform);
        }
    }
}
