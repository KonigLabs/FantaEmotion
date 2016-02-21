using System.Collections.Generic;

namespace KonigLabs.FantaEmotion.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static void CopyTo<TItem>(this IEnumerable<TItem> source, IList<TItem> destination)
        {
            destination.Clear();
            foreach (var item in source)
            {
                destination.Add(item);
            }
        }
    }
}
