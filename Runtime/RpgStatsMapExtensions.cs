using System.Text;

namespace AV.RpgStats.Runtime
{
    public static class RpgStatsMapExtensions
    {
        public static string ToPrettyString(this RpgStatsDictionary dictionary)
        {
            var sb = new StringBuilder();
            var count = 0;
            var total = 0;

            foreach (var _ in dictionary) total++;

            sb.Append($"<b>RpgStatsDictionary</b> ({total}) ");
            sb.AppendLine(dictionary.HasChanged
                ? "<color=yellow>⚠ Dirty</color>"
                : "<color=green>✓ Clean</color>");

            foreach (var kvp in dictionary)
            {
                count++;
                var isLast = count == total;
                var prefix = isLast ? "└─" : "├─";

                sb.Append($"{prefix} [<color=cyan>{kvp.Key:D2}</color>] ");

                sb.AppendLine(kvp.Value.ToString());
            }

            if (total == 0) sb.AppendLine("└─ <color=grey>Empty</color>");

            return sb.ToString();
        }
    }
}