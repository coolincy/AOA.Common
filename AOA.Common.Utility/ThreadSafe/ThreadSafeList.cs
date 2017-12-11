using System;
using System.Collections;
using System.Collections.Generic;

namespace AOA.Common.Utility.ThreadSafe
{

    #region ThreadSafeList 线程安全的列表(使用lock)
    /// <summary>
    /// 线程安全的列表(使用lock)
    /// </summary>
    /// <typeparam name="T">泛型类型</typeparam>
    [Serializable]
    public sealed class ThreadSafeList<T> : IEnumerable<T>
    {
        private List<T> m_list = new List<T>();
        private object m_lock = new object();

        #region SyncLock
        /// <summary>
        /// 同步锁
        /// </summary>
        public object SyncLock
        {
            get
            {
                return m_lock;
            }
        }
        #endregion

        #region Count
        /// <summary>
        /// 列表中的项目个数
        /// </summary>
        public int Count
        {
            get
            {
                lock (m_lock)
                {
                    return m_list.Count;
                }
            }
        }
        #endregion

        #region Add
        /// <summary>
        /// 添加一个值
        /// </summary>
        /// <param name="value"></param>
        public void Add(T value)
        {
            lock (m_lock)
            {
                m_list.Add(value);
            }
        }
        #endregion

        #region Remove
        /// <summary>
        /// 移除一个值
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            lock (m_lock)
            {
                m_list.Remove(value);
            }
        }
        #endregion

        #region Contains
        /// <summary>
        /// 检查是否含有某个值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            lock (m_lock)
            {
                return m_list.Contains(value);
            }
        }
        #endregion

        #region T this[int index]
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                lock (m_lock)
                {
                    return m_list[index];
                }
            }
            set
            {
                lock (m_lock)
                {
                    m_list[index] = value;
                }
            }
        }
        #endregion

        #region IEnumerable<T>.GetEnumerator
        /// <summary>
        /// 返回循环访问列表的枚举数
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (m_lock)
            {
                return m_list.GetEnumerator();
            }
        }
        #endregion

        #region IEnumerable.GetEnumerator()
        /// <summary>
        /// 返回循环访问列表的枚举数
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (m_lock)
            {
                return m_list.GetEnumerator();
            }
        }
        #endregion

    }
    #endregion

}
