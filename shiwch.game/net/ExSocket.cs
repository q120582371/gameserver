using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shiwch.game.net
{
    class ExSocket
    {
        private List<byte[]> buffer;
        private bool isbusy;
        private object syncObj = new object();

        internal int BufferedCount
        {
            get
            {
                lock (syncObj)
                {
                    return buffer == null ? 0 : buffer.Count;
                }
            }
        }

        internal bool CheckCanDirectSend(byte[] data)
        {
            lock (syncObj)
            {
                if (isbusy)
                {
                    if (buffer == null) buffer = new List<byte[]>();
                    buffer.Add(data);
                    return false;
                }
                else
                {
                    isbusy = true;
                    return true;
                }
            }
        }
        internal List<byte[]> TryGetNeedSend()
        {
            lock (syncObj)
            {
                if (buffer == null)
                {
                    isbusy = false;
                    return null;
                }
                else
                {
                    var result = buffer;
                    buffer = null;
                    return result;
                }
            }
        }
    }
}
