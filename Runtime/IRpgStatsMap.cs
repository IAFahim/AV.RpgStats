using System.Collections.Generic;
using AV.StoreCaches.Runtime;
using Variable.RPG;

namespace AV.RpgStats.Runtime
{
    public interface IRpgStatsMap : IEnumerable<KeyValuePair<int, RpgStat>> , IChangeAble
    {
        void Apply(int id, RpgStatModifier modifier);
        public bool TryGet(int id, out RpgStat stat);
    }
}