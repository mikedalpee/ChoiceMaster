using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoiceMaster
{
    public static class LinkedListExtensionMethod
    {
        public static LinkedListNode<T> GetNodeAt<T>(this LinkedList<T> _list, int position)
        {
            LinkedListNode<T> mark = _list.First;
            int i = 0;
            while (i < position)
            {
                mark = mark.Next;
                i++;
            }
            return mark;
        }
        public static void Remove<T>(this LinkedListNode<T> _node)
        {
            _node.List.Remove(_node);
        }
    }
}
