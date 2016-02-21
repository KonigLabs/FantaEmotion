using System;
using System.Collections.Generic;

namespace KonigLabs.FantaEmotion.Common.Extensions
{
    public static class LinkedListExtensions
    {
        public static LinkedListNode<TItem> TryGet<TItem>(this LinkedList<TItem> list, TItem item)
        {
            var node = list.Find(item);
            if (node == null)
                throw new InvalidOperationException();

            return node;
        } 
    }
}
