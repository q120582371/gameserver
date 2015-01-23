using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shiwch.game.net
{
    sealed class ThreadSynchronizationContext : SynchronizationContext
    {
        private readonly BlockingCollection<object> m_queue;

        public ThreadSynchronizationContext(BlockingCollection<object> queue)
        {
            this.m_queue = queue;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            if (d == null) throw new ArgumentNullException("d");
            m_queue.Add(new CallbackArgs(d, state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            throw new NotSupportedException("Synchronously sending is not supported.");
        }
    }
}
