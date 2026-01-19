using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AV.DictionaryVisualizer.Runtime;
using UnityEngine;
using Variable.RPG;

namespace AV.RpgStats.Runtime
{
    [HelpURL("https://github.com/IAFahim/AV.RpgStats")]
    [AddComponentMenu("AV/RpgStats/RpgStatsDictionary")]
    [DefaultExecutionOrder(-999)]
    public class RpgStatsDictionary : MonoBehaviour, IRpgStatsMap
    {
        [SerializeField] private bool hasUnsavedChanges;

        [Header("Configuration")] [SerializeField]
        private RpgStatScript[] attributeScripts = Array.Empty<RpgStatScript>();

        [SerializeField] private RpgStatScriptsGroup rpgStatScriptsGroup;

        [ShowDictionary(
            "RpgStats",
            keyFormatter: nameof(GetStatName),
            valueFormatterType: typeof(RpgStatExtensions),
            valueFormatter: nameof(RpgStatExtensions.ToStringCompact)
        )]
        private readonly Dictionary<int, RpgStat> rpgStatsMap = new();

        private void OnEnable()
        {
            ReloadAttributeMap();
        }

        public bool HasChanged
        {
            get => hasUnsavedChanges;
            set => hasUnsavedChanges = value;
        }

        public void Apply(int id, RpgStatModifier modifier)
        {
            if (rpgStatsMap.TryGetValue(id, out var stat))
            {
                var changed = stat.ApplyModifier(modifier);
                if (!changed) return;
                rpgStatsMap[id] = stat;
                hasUnsavedChanges = true;
            }
            else
            {
                Debug.LogWarning($"[RpgStatsDictionary] Stat ID {id} not found.", this);
            }
        }

        public bool TryGet(int id, out RpgStat stat)
        {
            return rpgStatsMap.TryGetValue(id, out stat);
        }

        public IEnumerator<KeyValuePair<int, RpgStat>> GetEnumerator()
        {
            return rpgStatsMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Set(int id, RpgStatModifier modifier)
        {
            Apply(id, modifier);
        }

        [ContextMenu("Log Pretty")]
        public void Log()
        {
            if (rpgStatsMap.Count == 0) return;

            var entries = new List<(string Name, RpgStat Stat)>();
            var maxNameLength = 0;

            foreach (var kvp in rpgStatsMap)
            {
                var name = GetStatName(kvp.Key);
                if (string.IsNullOrEmpty(name)) name = $"ID_{kvp.Key}";

                if (name.Length > maxNameLength) maxNameLength = name.Length;
                entries.Add((name, kvp.Value));
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

            foreach (var script in attributeScripts)
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
            rpgStatsMap.Clear();

            foreach (var script in attributeScripts)
                if (script != null)
                    AddStatSafe(script.ID, script.rpgStat);

            if (rpgStatScriptsGroup != null && rpgStatScriptsGroup.attributeScripts != null)
                foreach (var script in rpgStatScriptsGroup.attributeScripts)
                    if (script != null)
                        AddStatSafe(script.ID, script.rpgStat);

            RecalculateAll();
            hasUnsavedChanges = true;
        }

        private void AddStatSafe(int id, RpgStat stat)
        {
            if (!rpgStatsMap.TryAdd(id, stat))
                Debug.LogWarning($"[RpgStatsDictionary] Duplicate Stat ID detected: {id}. Skipping.", this);
        }

        private void RecalculateAll()
        {
            var keys = new List<int>(rpgStatsMap.Keys);
            foreach (var key in keys)
            {
                var stat = rpgStatsMap[key];
                stat.Recalculate();
                rpgStatsMap[key] = stat;
            }
        }
    }
}