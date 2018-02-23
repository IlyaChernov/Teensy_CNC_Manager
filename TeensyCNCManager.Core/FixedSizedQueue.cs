namespace TeensyCNCManager.Core
{
    using System;
    using System.Collections.Concurrent;

    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        public delegate void QueueChangedHandler();

        public event QueueChangedHandler QueueChanged;

        public int Limit { get; set; }

        [Obsolete]
        public new void Enqueue(T obj) { }
        public void EnqueueWithLimit(T obj)
        {
            base.Enqueue(obj);
            lock (this)
            {
                T overflow;
                while (Count > Limit && TryDequeue(out overflow));
            }
            if (QueueChanged != null) QueueChanged();
        }
    }
}
