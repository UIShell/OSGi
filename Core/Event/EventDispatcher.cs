namespace UIShell.OSGi.Core.Event
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using OSGi;

    internal class EventDispatcher<T> : IDisposable where T: EventArgs
    {
        private EventThread<T> _eventThread;
        private EventManager _owner;

        public EventDispatcher(EventManager em)
        {
            _owner = em;
        }

        public void DispatchToAsyncListeners(object sender, T e, IEnumerable<EventHandler<T>> listeners)
        {
            this.DispatchToListeners(sender, e, listeners, delegate (EventThreadItem<T> item) {
                item.DispatcherType = "Async";
                base.DispatcherEventThread.Push(item);
            });
        }

        private void DispatchToListeners(object sender, T e, IEnumerable<EventHandler<T>> listeners, DispatchEventItemDelegate<T> dispatchEventItemDelegate)
        {
            foreach (EventHandler<T> handler in listeners)
            {
                EventThreadItem<T> item = new EventThreadItem<T> {
                    Sender = sender,
                    EventArgs = e,
                    Listener = handler
                };
                try
                {
                    dispatchEventItemDelegate(item);
                }
                catch (Exception exception)
                {
                    FrameworkEventArgs args = new FrameworkEventArgs(FrameworkEventType.Error, this, exception);
                    this._owner.DispatchFrameworkEvent(this, args);
                }
            }
        }

        public void DispatchToSyncListeners(object sender, T e, IEnumerable<EventHandler<T>> listeners)
        {
            this.DispatchToListeners(sender, e, listeners, delegate (EventThreadItem<T> item) {
                item.DispatcherType = "Sync";
                EventDispatcher<T>.SyncDispatchEventItem(item);
            });
        }

        public void Dispose()
        {
            if (this._eventThread != null)
            {
                this._eventThread.Dispose();
                this._eventThread = null;
            }
            GC.SuppressFinalize(this);
        }

        private static void SyncDispatchEventItem(EventThreadItem<T> item)
        {
            if (item.Listener != null)
            {
                object data = Thread.GetData(Thread.GetNamedDataSlot("EventSlotName"));
                Thread.SetData(Thread.GetNamedDataSlot("EventSlotName"), item);
                item.Listener(item.Sender, item.EventArgs);
                Thread.SetData(Thread.GetNamedDataSlot("EventSlotName"), data);
            }
        }

        private EventThread<T> DispatcherEventThread
        {
            get
            {
                if (this._eventThread == null)
                {
                    this._eventThread = new EventThread<T>();
                }
                return this._eventThread;
            }
        }

        private delegate void DispatchEventItemDelegate(EventDispatcher<T>.EventThreadItem item);

        private class EventThread
        {
            private Thread _dispatcherThread;
            private AutoResetEvent _exitEvent;
            private bool _isExited;
            private object _lockObject;
            private Queue<EventDispatcher<T>.EventThreadItem> _queue;
            private AutoResetEvent _waitEvent;

            public EventThread()
            {
                this._lockObject = new object();
                this._queue = new Queue<EventDispatcher<T>.EventThreadItem>();
                this._waitEvent = new AutoResetEvent(false);
                this._exitEvent = new AutoResetEvent(false);
                this._dispatcherThread = new Thread(new ThreadStart(this.Dispatch));
                this._dispatcherThread.Name = "EventDispatcherThread";
                this._dispatcherThread.Start();
            }

            public void Dispatch()
            {
                while (!this._isExited)
                {
                    if (this.Peek() == null)
                    {
                        try
                        {
                            this._waitEvent.WaitOne();
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        EventDispatcher<T>.SyncDispatchEventItem(this.Pop());
                    }
                }
                this._exitEvent.Set();
            }

            public void Dispose()
            {
                TimerCallback callback = null;
                this._isExited = true;
                this._lockObject = null;
                this._queue.Clear();
                this._queue = null;
                this._waitEvent.Set();
                Thread.Sleep(100);
                if (this._dispatcherThread.IsAlive)
                {
                    Thread.Sleep(100);
                    if (this._dispatcherThread.IsAlive)
                    {
                        int num = 2;
                        if (callback == null)
                        {
                            callback = (TimerCallback) (state => base._exitEvent.Set());
                        }
                        TimerCallback callback2 = callback;
                        Timer timer = new Timer(callback2, null, 0, num * 0x3e8);
                        this._exitEvent.WaitOne();
                        timer.Dispose();
                        timer = null;
                        if (this._dispatcherThread.IsAlive)
                        {
                            this._dispatcherThread.Abort();
                        }
                    }
                }
                this._dispatcherThread = null;
                this._waitEvent = null;
            }

            public EventDispatcher<T>.EventThreadItem Peek()
            {
                lock (this._lockObject)
                {
                    if (this._queue.Count == 0)
                    {
                        return null;
                    }
                    return this._queue.Peek();
                }
            }

            public EventDispatcher<T>.EventThreadItem Pop()
            {
                lock (this._lockObject)
                {
                    return this._queue.Dequeue();
                }
            }

            public void Push(EventDispatcher<T>.EventThreadItem item)
            {
                lock (this._lockObject)
                {
                    this._queue.Enqueue(item);
                }
                this._waitEvent.Set();
            }
        }

        private class EventThreadItem
        {
            public string DispatcherType;
            public T EventArgs;
            public EventHandler<T> Listener;
            public object Sender;

            public EventThreadItem()
            {
                DispatcherType = "Sync";
            }

            public override string ToString() => 
                ("EventThreadItem[" + this.DispatcherType + "," + this.Sender.ToString() + ", " + this.EventArgs.ToString() + "]");
        }
    }
}

