using System;
using System.Collections;
using System.Threading;

namespace AOA.Common.Utility
{

    /// <summary>
    /// 特殊队列对象，用于同步处理多消费者和多生产者之间关系定制．利用线程间通讯方式实现
    /// </summary>
    public sealed class BlockQueue<T> : ICollection where T : new()
    {

        private T[] buffer;
        private int count;
        private int size;
        private int head;
        private int tail;
        private readonly object syncRoot;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="size"></param>
        public BlockQueue(int size)
        {
            if (size < 1)
                throw new ArgumentOutOfRangeException("size 必须大于0");
            syncRoot = new object();
            this.size = size;
            buffer = new T[size];
            count = 0;
            head = 0;
            tail = 0;
        }

        /// <summary>
        /// 队列值列表
        /// </summary>
        public T[] Values
        {
            get
            {
                T[] values;
                lock (syncRoot)
                {
                    values = new T[count];
                    int pos = head;
                    for (int i = 0; i < count; i++)
                    {
                        values[i] = buffer[pos];
                        pos = (pos + 1) % size;
                    }
                }
                return values;
            }
        }

        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="value"></param>
        public void Enqueue(T value)
        {
            Enqueue(value, Timeout.Infinite);
        }

        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="value"></param>
        /// <param name="millisecondsTimeout"></param>
        public void Enqueue(T value, int millisecondsTimeout)
        {
            lock (syncRoot)
            {
                while (count == size)
                {
                    try
                    {
                        if (!Monitor.Wait(syncRoot, millisecondsTimeout))
                            throw new QueueTimeoutException();
                    }
                    catch
                    {
                        Monitor.PulseAll(syncRoot);
                        throw;
                    }
                }
                buffer[tail] = value;
                tail = (tail + 1) % size;
                count++;
                if (count == 1)
                    Monitor.PulseAll(syncRoot);
            }
        }

        /// <summary>
        /// 入队列
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryEnqueue(T value)
        {
            lock (syncRoot)
            {
                if (count == size)
                    return false;
                buffer[tail] = value;
                tail = (tail + 1) % size;
                count++;
                if (count == 1)
                    Monitor.PulseAll(syncRoot);
            }
            return true;
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            return Dequeue(Timeout.Infinite);
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public T Dequeue(int millisecondsTimeout)
        {
            T value;
            lock (syncRoot)
            {
                while (count == 0)
                {
                    try
                    {
                        if (!Monitor.Wait(syncRoot, millisecondsTimeout))
                            throw new QueueTimeoutException();
                    }
                    catch
                    {
                        Monitor.PulseAll(syncRoot);
                        throw;
                    }
                }
                value = buffer[head];
                buffer[head] = default(T);
                head = (head + 1) % size;
                count--;
                if (count == (size - 1))
                    Monitor.PulseAll(syncRoot);
            }
            return value;
        }

        /// <summary>
        /// 出队列
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryDequeue(out T value)
        {
            lock (syncRoot)
            {
                if (count == 0)
                {
                    value = default(T);
                    return false;
                }
                value = buffer[head];
                buffer[head] = default(T);
                head = (head + 1) % size;
                count--;
                if (count == (size - 1))
                    Monitor.PulseAll(syncRoot);
            }
            return true;
        }

        /// <summary>
        /// 取队列头
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            lock (syncRoot)
            {
                if (count == 0)
                    throw new InvalidOperationException("The Queue is empty.");

                T value = buffer[head];
                return value;
            }
        }

        /// <summary>
        /// 取队列头
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryPeek(out T value)
        {
            lock (syncRoot)
            {
                if (count == 0)
                {
                    value = default(T);
                    return false;
                }
                value = buffer[head];
            }
            return true;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            lock (syncRoot)
            {
                count = 0;
                head = 0;
                tail = 0;
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = default(T);
                }
            }
        }

        /// <summary>
        /// 是否同步
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// 队列总长度
        /// </summary>
        public int Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// 队列数据长度
        /// </summary>
        public int Count
        {
            get { lock (syncRoot) { return count; } }
        }

        /// <summary>
        /// 复制到数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            T[] tmpArray = Values;
            tmpArray.CopyTo(array, index);
        }

        /// <summary>
        /// 取锁对象
        /// </summary>
        public object SyncRoot
        {
            get { return this.syncRoot; }
        }

        /// <summary>
        /// 取IEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException("未实现.");
        }

    }

    /// <summary>
    /// 队列超时异常
    /// </summary>
    public class QueueTimeoutException : Exception
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public QueueTimeoutException()
            : base("队列方法超时.")
        {
        }

    }

}