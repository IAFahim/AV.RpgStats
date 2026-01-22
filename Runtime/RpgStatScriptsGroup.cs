using BovineLabs.Core.ObjectManagement;
using UnityEngine;

namespace AV.RpgStats.Runtime
{
    [CreateAssetMenu(fileName = nameof(RpgStatScriptsGroup), menuName = "AV/RpgStatScript/Group", order = 0)]
    public class RpgStatScriptsGroup : ScriptableObject, IUID
    {
        [SerializeField] private int id;
        public RpgStatScript[] attributeScripts;

        public int ID
        {
            get => id;
            set => id = value;
        }
    }
}