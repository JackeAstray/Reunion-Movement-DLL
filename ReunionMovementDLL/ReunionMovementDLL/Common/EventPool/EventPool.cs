using System;
using System.Collections.Generic;

namespace ReunionMovementDLL
{
    /// <summary>
    /// 事件池。
    /// </summary>
    /// <typeparam name="T">事件类型。</typeparam>
    internal sealed partial class EventPool<T> where T : BaseEventArgs
    {
        private readonly ReunionMovementMultiDictionary<int, EventHandler<T>> eventHandlers;
        private readonly Queue<Event> events;
        private readonly Dictionary<object, LinkedListNode<EventHandler<T>>> cachedNodes;
        private readonly Dictionary<object, LinkedListNode<EventHandler<T>>> tempNodes;
        private readonly EventPoolMode eventPoolMode;
        private EventHandler<T> defaultHandler;

        /// <summary>
        /// 初始化事件池的新实例。
        /// </summary>
        /// <param name="mode">事件池模式。</param>
        public EventPool(EventPoolMode mode)
        {
            eventHandlers = new ReunionMovementMultiDictionary<int, EventHandler<T>>();
            events = new Queue<Event>();
            cachedNodes = new Dictionary<object, LinkedListNode<EventHandler<T>>>();
            tempNodes = new Dictionary<object, LinkedListNode<EventHandler<T>>>();
            eventPoolMode = mode;
            defaultHandler = null;
        }

        /// <summary>
        /// 获取事件处理函数的数量。
        /// </summary>
        public int EventHandlerCount
        {
            get
            {
                return eventHandlers.Count;
            }
        }

        /// <summary>
        /// 获取事件数量。
        /// </summary>
        public int EventCount
        {
            get
            {
                return events.Count;
            }
        }

        /// <summary>
        /// 事件池轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            lock (events)
            {
                while (events.Count > 0)
                {
                    Event eventNode = events.Dequeue();
                    HandleEvent(eventNode.Sender, eventNode.EventArgs);
                    ReferencePool.Release(eventNode);
                }
            }
        }

        /// <summary>
        /// 关闭并清理事件池。
        /// </summary>
        public void Shutdown()
        {
            Clear();
            eventHandlers.Clear();
            cachedNodes.Clear();
            tempNodes.Clear();
            defaultHandler = null;
        }

        /// <summary>
        /// 清理事件。
        /// </summary>
        public void Clear()
        {
            lock (events)
            {
                events.Clear();
            }
        }

        /// <summary>
        /// 获取事件处理函数的数量。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <returns>事件处理函数的数量。</returns>
        public int Count(int id)
        {
            ReunionMovementLinkedListRange<EventHandler<T>> range = default(ReunionMovementLinkedListRange<EventHandler<T>>);
            if (eventHandlers.TryGetValue(id, out range))
            {
                return range.Count;
            }

            return 0;
        }

        /// <summary>
        /// 检查是否存在事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要检查的事件处理函数。</param>
        /// <returns>是否存在事件处理函数。</returns>
        public bool Check(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ReunionMovementException("事件处理函数无效。");
            }

            return eventHandlers.Contains(id, handler);
        }

        /// <summary>
        /// 订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要订阅的事件处理函数。</param>
        public void Subscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ReunionMovementException("事件处理函数无效。");
            }

            if (!eventHandlers.Contains(id))
            {
                eventHandlers.Add(id, handler);
            }
            else if ((eventPoolMode & EventPoolMode.AllowMultiHandler) != EventPoolMode.AllowMultiHandler)
            {
                throw new ReunionMovementException(Utility.Text.Format("事件 '{0}' 不允许多个处理函数。", id));
            }
            else if ((eventPoolMode & EventPoolMode.AllowDuplicateHandler) != EventPoolMode.AllowDuplicateHandler && Check(id, handler))
            {
                throw new ReunionMovementException(Utility.Text.Format("事件 '{0}' 不允许重复处理函数。", id));
            }
            else
            {
                eventHandlers.Add(id, handler);
            }
        }

        /// <summary>
        /// 取消订阅事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要取消订阅的事件处理函数。</param>
        public void Unsubscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ReunionMovementException("事件处理函数无效。");
            }

            if (cachedNodes.Count > 0)
            {
                foreach (KeyValuePair<object, LinkedListNode<EventHandler<T>>> cachedNode in cachedNodes)
                {
                    if (cachedNode.Value != null && cachedNode.Value.Value == handler)
                    {
                        tempNodes.Add(cachedNode.Key, cachedNode.Value.Next);
                    }
                }

                if (tempNodes.Count > 0)
                {
                    foreach (KeyValuePair<object, LinkedListNode<EventHandler<T>>> cachedNode in tempNodes)
                    {
                        cachedNodes[cachedNode.Key] = cachedNode.Value;
                    }

                    tempNodes.Clear();
                }
            }

            if (!eventHandlers.Remove(id, handler))
            {
                throw new ReunionMovementException(Utility.Text.Format("事件 '{0}' 不存在指定的处理函数。", id));
            }
        }

        /// <summary>
        /// 设置默认事件处理函数。
        /// </summary>
        /// <param name="handler">要设置的默认事件处理函数。</param>
        public void SetDefaultHandler(EventHandler<T> handler)
        {
            defaultHandler = handler;
        }

        /// <summary>
        /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void Fire(object sender, T e)
        {
            if (e == null)
            {
                throw new ReunionMovementException("事件无效。");
            }

            Event eventNode = Event.Create(sender, e);
            lock (events)
            {
                events.Enqueue(eventNode);
            }
        }

        /// <summary>
        /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        public void FireNow(object sender, T e)
        {
            if (e == null)
            {
                throw new ReunionMovementException("事件无效。");
            }

            HandleEvent(sender, e);
        }

        /// <summary>
        /// 处理事件结点。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件参数。</param>
        private void HandleEvent(object sender, T e)
        {
            bool noHandlerException = false;
            ReunionMovementLinkedListRange<EventHandler<T>> range = default(ReunionMovementLinkedListRange<EventHandler<T>>);
            if (eventHandlers.TryGetValue(e.Id, out range))
            {
                LinkedListNode<EventHandler<T>> current = range.First;
                while (current != null && current != range.Terminal)
                {
                    cachedNodes[e] = current.Next != range.Terminal ? current.Next : null;
                    current.Value(sender, e);
                    current = cachedNodes[e];
                }

                cachedNodes.Remove(e);
            }
            else if (defaultHandler != null)
            {
                defaultHandler(sender, e);
            }
            else if ((eventPoolMode & EventPoolMode.AllowNoHandler) == 0)
            {
                noHandlerException = true;
            }

            ReferencePool.Release(e);

            if (noHandlerException)
            {
                throw new ReunionMovementException(Utility.Text.Format("事件 '{0}' 不允许没有处理函数。", e.Id));
            }
        }
    }
}
