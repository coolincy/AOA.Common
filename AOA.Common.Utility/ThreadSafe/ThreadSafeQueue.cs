using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AOA.Common.Utility.ThreadSafe
{

    #region ILockedQueue
    /// <summary>
    /// 锁队列接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILockedQueue<T> : IDisposable
    {
        /// <summary>
        /// 队列长度
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 是否包含某个值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Contains(T value);

        /// <summary>
        /// 出队列
        /// </summary>
        /// <returns></returns>
        T Dequeue();
    }
    #endregion

    #region ThreadSafeQueue 线程安全的队列
    /// <summary>
    /// 线程安全的队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ThreadSafeQueue<T>
    {

        #region private sealed class LockedQueue
        private sealed class LockedQueue : ILockedQueue<T>
        {
            private ThreadSafeQueue<T> m_outer;

            #region LockedQueue
            internal LockedQueue(ThreadSafeQueue<T> outer)
            {
                m_outer = outer;
                Monitor.Enter(m_outer.m_lock);
            }
            #endregion

            #region ILockedQueue<T> Members

            public int Count
            {
                get { return m_outer.m_queue.Count; }
            }

            public bool Contains(T value)
            {
                return m_outer.m_queue.Contains(value);
            }

            public T Dequeue()
            {
                return m_outer.m_queue.Dequeue();
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                Debug.Assert(disposing, "ILockedQueue implementations must be explicitly disposed");
                if (disposing)
                {
                    Monitor.Exit(m_outer.m_lock);
                }
            }

            ~LockedQueue()
            {
                Dispose(false);
            }

            #endregion

        }
        #endregion

        private Queue<T> m_queue = new Queue<T>();
        private object m_lock = new object();

        #region 构造函数 ThreadSafeQueue
        /// <summary>
        /// 构造函数
        /// </summary>
        public ThreadSafeQueue()
        {
        }
        #endregion

        #region Enqueue
        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="value"></param>
        public void Enqueue(T value)
        {
            lock (m_lock)
            {
                m_queue.Enqueue(value);
            }
        }
        #endregion

        #region Dequeue
        /// <summary>
        /// 出队列
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            //using (var locked = this.Lock())
            //{
            //    if (locked.Count > 0)
            //    {
            //        return locked.Dequeue();
            //    }
            //}
            lock (m_lock)
            {
                if (m_queue.Count > 0)
                    return m_queue.Dequeue();
                else
                    return default(T);
            }
        }
        #endregion

        #region Clear
        /// <summary>
        /// 清除队列
        /// </summary>
        public void Clear()
        {
            lock (m_lock)
            {
                m_queue.Clear();
            }
        }
        #endregion

        #region GetCount
        /// <summary>
        /// 获取队列长度
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            lock (m_lock)
            {
                return m_queue.Count;
            }
        }
        #endregion

        #region Lock
        /// <summary>
        /// 锁定队列，返回对象释放后，解锁
        /// </summary>
        /// <returns></returns>
        public ILockedQueue<T> Lock()
        {
            return new LockedQueue(this);
        }
        #endregion

        #region Example1
        static void Example1(ThreadSafeQueue<int> queue)
        {
            using (var locked = queue.Lock())
            {
                if (locked.Count > 0)
                {
                    var first = locked.Dequeue();
                }
            }
        }
        #endregion

    }
    #endregion

}
