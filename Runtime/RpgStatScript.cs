using AV.CancelFoldout.Runtime;
using BovineLabs.Core.ObjectManagement;
using UnityEngine;
using Variable.RPG;

[HelpURL("https://github.com/IAFahim/AV.RpgStats")]

namespace AV.RpgStats.Runtime
{
    [CreateAssetMenu(fileName = nameof(RpgStatScript), menuName = "AV/" + nameof(RpgStatScript) + "/New")]
    public class RpgStatScript : ScriptableObject, IUID
    {
        [SerializeField] private int id;

        [CancelFoldout] public RpgStat rpgStat = new(1);

        private void OnValidate()
        {
            rpgStat.Recalculate();
        }

        public int ID
        {
            get => id;
            set => id = value;
        }
    }
}