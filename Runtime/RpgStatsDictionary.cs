using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AV.DictionaryVisualizer.Runtime;
using UnityEngine;
using Variable.RPG;

namespace AV.RpgStats.Runtime
{
    [DefaultExecutionOrder(-999)]
    public class RpgStatsDictionary : MonoBehaviour, IRpgStatsMap
    {
        [SerializeField] private bool isDirty;

        [Header("Configuration")] [SerializeField]
        private RpgStatScript[] rpgStatScripts = Array.Empty<RpgStatScript>();

        [SerializeField] private RpgStatScriptsGroup rpgStatScriptsGroup;

        [ShowDictionary(
            "RpgStats",
            keyFormatter: nameof(GetStatName),
            valueFormatterType: typeof(RpgStatExtensions),
            valueFormatter: nameof(RpgStatExtensions.ToStringCompact)
        )]
        private readonly Dictionary<int, RpgStat> _statsDictionary = new();

        private void OnEnable()
        {
            ReloadAttributeMap();
        }

        public bool HasChanged
        {
            get => isDirty;
            set => isDirty = value;
        }

        bool IRpgStatsMap.TryApply(int id, RpgStatModifier modifier)
        {
            if (!_statsDictionary.TryGetValue(id, out var stat))
            {
                Debug.LogWarning($"[RpgStatsDictionary] Stat ID {id} not found.", this);
                return false;
            }

            var changed = stat.ApplyModifier(modifier);
            if (!changed) return false;
            _statsDictionary[id] = stat;
            isDirty = true;
            return true;
        }

        public bool TryGet(int id, out RpgStat stat) => _statsDictionary.TryGetValue(id, out stat);

        public IEnumerator<KeyValuePair<int, RpgStat>> GetEnumerator()
        {
            return _statsDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [ContextMenu("Log Pretty")]
        public void Log()
        {
            if (_statsDictionary.Count == 0) return;

            var entries = new List<(string Name, RpgStat Stat)>();
            var maxNameLength = 0;

            foreach (var kvp in _statsDictionary)
            {
                var statName = GetStatName(kvp.Key);
                if (string.IsNullOrEmpty(statName)) statName = $"ID_{kvp.Key}";

                if (statName.Length > maxNameLength) maxNameLength = statName.Length;
                entries.Add((statName, kvp.Value));
            }

            maxNameLength += 2;

            var summary = new StringBuilder();
            summary.Append("<b>[Stats]</b> ");

            for (var i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                summary.Append($"{e.Name}: <b>{e.Stat.Value:0.##}</b>");

                if (i < entries.Count - 1) summary.Append("  |  ");
            }

            var body = new StringBuilder();
            body.AppendLine(summary.ToString());

            foreach (var e in entries)
            {
                var valueTuple = e;
                var alignedName = valueTuple.Name.PadRight(maxNameLength);

                var alignedValue = $"<b><color=white>{valueTuple.Stat.Value:0.##}</color></b>";

                var rawMath = valueTuple.Stat.ToStringCompact();
                var mathOnly = rawMath.Contains("◀") ? rawMath.Split('◀')[1].Trim() : rawMath;
                var dimmedMath = $"<color=#888888><size=11>{mathOnly}</size></color>";

                body.AppendLine($"{alignedName} :  {alignedValue,-8} {dimmedMath}");
            }

            body.AppendLine("<color=#888888>----------------------------------------------------------------</color>");

            Debug.Log(body.ToString(), this);
        }

        private string GetStatName(object idObj)
        {
            if (idObj is not int id) return "null";

            foreach (var script in rpgStatScripts)
                if (script != null && script.ID == id)
                    return script.name;

            if (rpgStatScriptsGroup != null && rpgStatScriptsGroup.attributeScripts != null)
                foreach (var script in rpgStatScriptsGroup.attributeScripts)
                    if (script != null && script.ID == id)
                        return script.name;

            return $"ID_{id}";
        }

        private void ReloadAttributeMap()
        {
            _statsDictionary.Clear();

            foreach (var s in rpgStatScripts)
                if (s != null)
                    AddStatSafe(s.ID, s.rpgStat);

            if (rpgStatScriptsGroup != null && rpgStatScriptsGroup.attributeScripts != null)
                foreach (var s in rpgStatScriptsGroup.attributeScripts)
                    if (s != null)
                        AddStatSafe(s.ID, s.rpgStat);

            RecalculateAll();
            isDirty = true;
        }

        private void AddStatSafe(int id, RpgStat stat)
        {
            if (!_statsDictionary.TryAdd(id, stat))
                Debug.LogWarning($"[RpgStatsDictionary] Duplicate Stat ID detected: {id}. Skipping.", this);
        }

        private void RecalculateAll()
        {
            var keys = new List<int>(_statsDictionary.Keys);
            foreach (var key in keys)
            {
                var stat = _statsDictionary[key];
                stat.Recalculate();
                _statsDictionary[key] = stat;
            }
        }
    }
}