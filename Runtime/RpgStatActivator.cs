using System;
using System.Runtime.InteropServices;
using AV.CancelFoldout.Runtime;
using AV.Lifetime.Realtime;
using UnityEngine;
using Variable.RPG;

namespace AV.RpgStats.Runtime
{
    /// <summary>
    ///     Applies temporary stat modifiers to targets when enabled and removes them when disabled.
    ///     Requires an InitializeMono component to provide target context.
    /// </summary>
    [RequireComponent(typeof(InitializeMono))]
    public sealed class RpgStatActivator : MonoBehaviour
    {
        [SerializeField] private RpgStatsModEntry[] rpgStatsModEntry;
        private InitializeMono _initializeMono;

        private void OnEnable()
        {
            _initializeMono = GetComponent<InitializeMono>();
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
                        $"[RpgStatActivator] Failed to resolve IRpgStatsMap for target '{entry.Target}'",
                        this
                    );
                    continue;
                }

                statsMap.Apply(entry.RpgStatScript.ID, entry.Modifier);
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
                    statsMap.Apply(entry.RpgStatScript.ID, inverseModifier);
                else
                    Debug.LogWarning(
                        $"[RpgStatActivator] Cannot auto-remove modifier '{modifier.Operation}'",
                        this
                    );
            }
        }

        private bool TryGetStatsMap(in ETarget target, out IRpgStatsMap statsMap)
        {
            if (
                TargetLogic.TryGetTransform(
                    transform, _initializeMono.targetContext, in target, out var targetTransform
                )
            ) return targetTransform.TryGetComponent(out statsMap);
            statsMap = null;
            return false;
        }

        /// <summary>
        ///     Defines a stat modifier entry with target specification.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RpgStatsModEntry
        {
            public RpgStatScript RpgStatScript;
            [CancelFoldout] public RpgStatModifier Modifier;
            public ETarget Target;
        }
    }
}