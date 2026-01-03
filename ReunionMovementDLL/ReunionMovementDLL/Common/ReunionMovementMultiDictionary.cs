using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ReunionMovementDLL
{
    /// <summary>
    /// 游戏框架多值字典类。
    /// </summary>
    /// <typeparam name="TKey">指定多值字典的主键类型。</typeparam>
    /// <typeparam name="TValue">指定多值字典的值类型。</typeparam>
    public sealed class ReunionMovementMultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ReunionMovementLinkedListRange<TValue>>>, IEnumerable
    {
        private readonly ReunionMovementLinkedList<TValue> linkedList;
        private readonly Dictionary<TKey, ReunionMovementLinkedListRange<TValue>> dictionary;

        /// <summary>
        /// 初始化游戏框架多值字典类的新实例。
        /// </summary>
        public ReunionMovementMultiDictionary()
        {
            linkedList = new ReunionMovementLinkedList<TValue>();
            dictionary = new Dictionary<TKey, ReunionMovementLinkedListRange<TValue>>();
        }

        /// <summary>
        /// 获取多值字典中实际包含的主键数量。
        /// </summary>
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        /// <summary>
        /// 获取多值字典中指定主键的范围。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>指定主键的范围。</returns>
        public ReunionMovementLinkedListRange<TValue> this[TKey key]
        {
            get
            {
                ReunionMovementLinkedListRange<TValue> range = default(ReunionMovementLinkedListRange<TValue>);
                dictionary.TryGetValue(key, out range);
                return range;
            }
        }

        /// <summary>
        /// 清理多值字典。
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
            linkedList.Clear();
        }

        /// <summary>
        /// 检查多值字典中是否包含指定主键。
        /// </summary>
        /// <param name="key">要检查的主键。</param>
        /// <returns>多值字典中是否包含指定主键。</returns>
        public bool Contains(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 检查多值字典中是否包含指定值。
        /// </summary>
        /// <param name="key">要检查的主键。</param>
        /// <param name="value">要检查的值。</param>
        /// <returns>多值字典中是否包含指定值。</returns>
        public bool Contains(TKey key, TValue value)
        {
            ReunionMovementLinkedListRange<TValue> range = default(ReunionMovementLinkedListRange<TValue>);
            if (dictionary.TryGetValue(key, out range))
            {
                return range.Contains(value);
            }

            return false;
        }

        /// <summary>
        /// 尝试获取多值字典中指定主键的范围。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="range">指定主键的范围。</param>
        /// <returns>是否获取成功。</returns>
        public bool TryGetValue(TKey key, out ReunionMovementLinkedListRange<TValue> range)
        {
            return dictionary.TryGetValue(key, out range);
        }

        /// <summary>
        /// 向指定的主键增加指定的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        public void Add(TKey key, TValue value)
        {
            ReunionMovementLinkedListRange<TValue> range = default(ReunionMovementLinkedListRange<TValue>);
            if (dictionary.TryGetValue(key, out range))
            {
                linkedList.AddBefore(range.Terminal, value);
            }
            else
            {
                LinkedListNode<TValue> first = linkedList.AddLast(value);
                LinkedListNode<TValue> terminal = linkedList.AddLast(default(TValue));
                dictionary.Add(key, new ReunionMovementLinkedListRange<TValue>(first, terminal));
            }
        }

        /// <summary>
        /// 从指定的主键中移除指定的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        /// <returns>是否移除成功。</returns>
        public bool Remove(TKey key, TValue value)
        {
            ReunionMovementLinkedListRange<TValue> range = default(ReunionMovementLinkedListRange<TValue>);
            if (dictionary.TryGetValue(key, out range))
            {
                for (LinkedListNode<TValue> current = range.First; current != null && current != range.Terminal; current = current.Next)
                {
                    if (current.Value.Equals(value))
                    {
                        if (current == range.First)
                        {
                            LinkedListNode<TValue> next = current.Next;
                            if (next == range.Terminal)
                            {
                                linkedList.Remove(next);
                                dictionary.Remove(key);
                            }
                            else
                            {
                                dictionary[key] = new ReunionMovementLinkedListRange<TValue>(next, range.Terminal);
                            }
                        }

                        linkedList.Remove(current);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 从指定的主键中移除所有的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>是否移除成功。</returns>
        public bool RemoveAll(TKey key)
        {
            ReunionMovementLinkedListRange<TValue> range = default(ReunionMovementLinkedListRange<TValue>);
            if (dictionary.TryGetValue(key, out range))
            {
                dictionary.Remove(key);

                LinkedListNode<TValue> current = range.First;
                while (current != null)
                {
                    LinkedListNode<TValue> next = current != range.Terminal ? current.Next : null;
                    linkedList.Remove(current);
                    current = next;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator<KeyValuePair<TKey, ReunionMovementLinkedListRange<TValue>>> IEnumerable<KeyValuePair<TKey, ReunionMovementLinkedListRange<TValue>>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数。
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, ReunionMovementLinkedListRange<TValue>>>, IEnumerator
        {
            private Dictionary<TKey, ReunionMovementLinkedListRange<TValue>>.Enumerator m_Enumerator;

            internal Enumerator(Dictionary<TKey, ReunionMovementLinkedListRange<TValue>> dictionary)
            {
                if (dictionary == null)
                {
                    throw new ReunionMovementException("Dictionary is invalid.");
                }

                m_Enumerator = dictionary.GetEnumerator();
            }

            /// <summary>
            /// 获取当前结点。
            /// </summary>
            public KeyValuePair<TKey, ReunionMovementLinkedListRange<TValue>> Current
            {
                get
                {
                    return m_Enumerator.Current;
                }
            }

            /// <summary>
            /// 获取当前的枚举数。
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return m_Enumerator.Current;
                }
            }

            /// <summary>
            /// 清理枚举数。
            /// </summary>
            public void Dispose()
            {
                m_Enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个结点。
            /// </summary>
            /// <returns>返回下一个结点。</returns>
            public bool MoveNext()
            {
                return m_Enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数。
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<KeyValuePair<TKey, ReunionMovementLinkedListRange<TValue>>>)m_Enumerator).Reset();
            }
        }
    }
}
