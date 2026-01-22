using System;
using System.Runtime.InteropServices;
using AV.Lifetime.Realtime;
using UnityEngine;
using Variable.RPG;

namespace AV.RpgStats.Runtime
{
    /// <summary>
    ///     Applies temporary stat modifiers to targets when enabled and removes them when disabled.
    ///     Uses ITargetContextSystem to resolve targets.
    /// </summary>
    public sealed class RpgStatModifierToggle : MonoBehaviour
    {
        [SerializeField] private RpgStatsModEntry[] rpgStatsModEntry;
        private ITargetContextSystem _contextSystem;

        private void OnEnable()
        {
            _contextSystem = GetComponent<ITargetContextSystem>();
            ApplyAllModifiers();
        }

        private void OnDisable()
        {
            RemoveAllModifiers();
        }

        private void ApplyAllModifiers()
        {
            foreach (var entry in rpgStatsModEntry)
            {
                if (!TryGetStatsMap(in entry.Target, out var statsMap))
                {
                    Debug.LogWarning(
                        $"[RpgStatModifierToggle] Failed to resolve IRpgStatsMap for target '{entry.Target}'",
                        this
                    );
                    continue;
                }

                statsMap.TryApply(entry.RpgStatScript.ID, entry.Modifier);
            }
        }

        private void RemoveAllModifiers()
        {
            foreach (var entry in rpgStatsModEntry)
            {
                if (!TryGetStatsMap(in entry.Target, out var statsMap))
                    continue;

                var modifier = entry.Modifier;
                if (modifier.TryGetInverse(out var inverseModifier))
                    statsMap.TryApply(entry.RpgStatScript.ID, inverseModifier);
                else
                    Debug.LogWarning(
                        $"[RpgStatModifierToggle] Cannot auto-remove modifier '{modifier.Operation}'",
                        this
                    );
            }
        }

        private bool TryGetStatsMap(in ETarget target, out IRpgStatsMap statsMap)
        {
            statsMap = null;
            if (_contextSystem == null) return false;
            
            if (_contextSystem.TryGetContext(out var context) != TargetContextResult.Success)
                return false;

            if (TargetContextLogic.TryGetTransform(transform, context, in target, out var targetTransform))
            {
                return targetTransform.TryGetComponent(out statsMap);
            }
            
            return false;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RpgStatsModEntry
        {
            public RpgStatScript RpgStatScript;
            public RpgStatModifier Modifier;
            public ETarget Target;
        }
    }
}
