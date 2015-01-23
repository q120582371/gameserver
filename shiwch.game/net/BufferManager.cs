using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace shiwch.game.net
{
    class BufferManager
    {
        int capacity;
        byte[] bufferBlock;
        ThreadSafeStack<int> freeIndexPool;
        int currentIndex;
        int saeaSize;

        public BufferManager(int capacity, int saeaSize)
        {
            this.capacity = capacity;
            this.saeaSize = saeaSize;
            this.freeIndexPool = new ThreadSafeStack<int>();
        }

        public void InitBuffer()
        {
            this.bufferBlock = new byte[capacity];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            int freeIdx;
            if (freeIndexPool.TryPop(out freeIdx))
            {
                args.SetBuffer(bufferBlock, freeIdx, saeaSize);
            }
            else
            {
                if ((capacity - saeaSize) < currentIndex)
                {
                    return false;
                }
                args.SetBuffer(bufferBlock, currentIndex, saeaSize);
                currentIndex += saeaSize;
            }
            return true;
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            this.freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
