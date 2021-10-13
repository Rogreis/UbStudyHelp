﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrantiaBook.Classes
{
    public class TrackStack<T>
    {
        private LinkedList<T> items = new LinkedList<T>();
        public List<T> Items => items.ToList();
        public int Capacity { get; }
        public TrackStack(int capacity)
        {
            Capacity = capacity;
        }

        public void Push(T item)
        {
            // full
            if (items.Count == Capacity)
            {
                // we should remove first, because some times, if we exceeded the size of the internal array
                // the system will allocate new array.
                items.RemoveFirst();
                items.AddLast(item);
            }
            else
            {
                items.AddLast(new LinkedListNode<T>(item));
            }
        }

        public T Pop()
        {
            if (items.Count == 0)
            {
                return default;
            }
            var ls = items.Last;
            items.RemoveLast();
            return ls == null ? default : ls.Value;
        }

        public T Peek()
        {
            if (items.Count == 0)
            {
                return default;
            }
            var ls = items.Last;
            return ls == null ? default : ls.Value;
        }


        public int Count
        {
            get
            {
                return items.Count;
            }
        }

    }


}
