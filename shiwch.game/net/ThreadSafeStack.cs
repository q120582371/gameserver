using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shiwch.game.net
{
    class ThreadSafeStack<T>
    {
        private Stack<T> stack;

        public ThreadSafeStack()
        {
            stack = new Stack<T>();
        }

        public ThreadSafeStack(int capacity)
        {
            stack = new Stack<T>(capacity);
        }
        public int Count
        {
            get
            {
                lock (stack)
                {
                    return stack.Count;
                }
            }
        }

        public bool TryPop(out T value)
        {
            lock (stack)
            {
                if (stack.Count == 0) { value = default(T); return false; }
                else { value = stack.Pop(); return true; }
            }
        }

        public void Push(T item)
        {
            lock (stack)
            {
                stack.Push(item);
            }
        }
    }
}
