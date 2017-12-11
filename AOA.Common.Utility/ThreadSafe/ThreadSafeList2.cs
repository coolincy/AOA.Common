using System;
using System.Collections.Generic;
using System.Threading;

namespace AOA.Common.Utility.ThreadSafe
{

    #region ThreadSafeList 线程安全的列表(使用ReaderWriterLockSlim)
    /// <summary>
    /// 线程安全的列表(使用ReaderWriterLockSlim)
    /// </summary>
    /// <typeparam name="T">泛型类型</typeparam>
    [Serializable]
    public sealed class ThreadSafeList2<T> : IEnumerable<T>
    {
        private List<T> list_t;
        private int timeout = -1;
        private ReaderWriterLockSlim readWriteLockSlim;

        #region 锁相关

        /// <summary>
        /// 获取读线程锁
        /// </summary>
        public void AcquireReadLock()
        {
            readWriteLockSlim.EnterReadLock();
        }

        /// <summary>
        /// 减少锁计数
        /// </summary>
        public void ReleaseReadLock()
        {
            readWriteLockSlim.ExitReadLock();
        }

        #endregion

        #region 构建函数

        /// <summary>
        /// 构建函数
        /// </summary>
        public ThreadSafeList2()
        {
            list_t = new List<T>();
            readWriteLockSlim = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// 构建函数，带超时时间
        /// </summary>
        /// <param name="tout">锁定超时时间(以毫秒为单位的超时时间)</param>
        public ThreadSafeList2(int tout)
            : this()
        {
            timeout = tout;
        }

        /// <summary>
        /// 构建函数，使用已有列表
        /// </summary>
        /// <param name="collection">已有的枚举列表</param>
        public ThreadSafeList2(IEnumerable<T> collection)
            : this()
        {
            list_t = new List<T>(collection);
        }

        #endregion

        #region Count 属性: 列表中的元素个数
        /// <summary>
        /// 列表中的元素个数
        /// </summary>
        public int Count
        {
            get
            {
                readWriteLockSlim.EnterReadLock();
                try
                {
                    return list_t.Count;
                }
                finally
                {
                    readWriteLockSlim.ExitReadLock();
                }
            }
        }
        #endregion

        #region T this[int index] 索引器
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <returns>泛型对象</returns>
        public T this[int index]
        {
            get
            {
                readWriteLockSlim.EnterReadLock();
                try
                {
                    return list_t[index];
                }
                finally
                {
                    readWriteLockSlim.ExitReadLock();
                }
            }

            set
            {
                readWriteLockSlim.EnterWriteLock();
                try
                {
                    list_t[index] = value;
                }
                finally
                {
                    readWriteLockSlim.ExitWriteLock();
                }
            }
        }
        #endregion

        #region Add 添加一个元素
        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <param name="item">元素</param>
        public void Add(T item)
        {
            readWriteLockSlim.EnterWriteLock();
            try
            {
                list_t.Add(item);
            }
            finally
            {
                readWriteLockSlim.ExitWriteLock();
            }
        }
        #endregion

        #region Insert 插入一个元素
        /// <summary>
        /// 插入一个元素
        /// </summary>
        /// <param name="index">元素索引</param>
        /// <param name="item">元素</param>
        public void Insert(int index, T item)
        {
            readWriteLockSlim.EnterWriteLock();
            try
            {
                list_t.Insert(index, item);
            }
            finally
            {
                readWriteLockSlim.ExitWriteLock();
            }
        }
        #endregion

        #region Remove 移除一个元素
        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <param name="item">元素</param>
        public void Remove(T item)
        {
            readWriteLockSlim.EnterWriteLock();
            try
            {
                list_t.Remove(item);
            }
            finally
            {
                readWriteLockSlim.ExitWriteLock();
            }
        }
        #endregion

        #region RemoveAt 移除一个元素
        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <param name="index">元素索引</param>
        public void RemoveAt(int index)
        {
            readWriteLockSlim.EnterWriteLock();
            try
            {
                list_t.RemoveAt(index);
            }
            finally
            {
                readWriteLockSlim.ExitWriteLock();
            }
        }
        #endregion

        #region Clear 清除列表
        /// <summary>
        /// 清除列表
        /// </summary>
        public void Clear()
        {
            readWriteLockSlim.EnterWriteLock();
            try
            {
                list_t.Clear();
            }
            finally
            {
                readWriteLockSlim.ExitWriteLock();
            }
        }
        #endregion

        #region Contains 确定某元素是否在列表中
        /// <summary>
        /// 确定某元素是否在列表中
        /// </summary>
        /// <param name="item">元素</param>
        /// <returns>是否在列表中</returns>
        public bool Contains(T item)
        {
            readWriteLockSlim.EnterReadLock();
            try
            {
                return list_t.Contains(item);
            }
            finally
            {
                readWriteLockSlim.ExitReadLock();
            }
        }
        #endregion

        #region GetEnumerator 返回循环访问列表的枚举数
        /// <summary>
        /// 返回循环访问列表的枚举数
        /// </summary>
        /// <returns></returns>
        public List<T>.Enumerator GetEnumerator()
        {
            readWriteLockSlim.EnterReadLock();
            try
            {
                return list_t.GetEnumerator();
            }
            finally
            {
                readWriteLockSlim.ExitReadLock();
            }
        }
        #endregion

        #region IEnumerable<TKey> 成员

        /// <summary>
        /// 返回循环访问列表的枚举数
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            readWriteLockSlim.EnterReadLock();
            try
            {
                return list_t.GetEnumerator();
            }
            finally
            {
                readWriteLockSlim.ExitReadLock();
            }
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回循环访问列表的枚举数
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            readWriteLockSlim.EnterReadLock();
            try
            {
                return list_t.GetEnumerator();
            }
            finally
            {
                readWriteLockSlim.ExitReadLock();
            }
        }

        #endregion

    }
    #endregion

}
