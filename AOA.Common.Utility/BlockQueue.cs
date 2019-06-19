using System;
using System.Collections;
using System.Threading;

namespace AOA.Common.Utility
{

    /// <summary>
    /// ������ж�������ͬ������������ߺͶ�������֮���ϵ���ƣ������̼߳�ͨѶ��ʽʵ��
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
        /// ���캯��
        /// </summary>
        /// <param name="size"></param>
        public BlockQueue(int size)
        {
            if (size < 1)
                throw new ArgumentOutOfRangeException("size �������0");
            syncRoot = new object();
            this.size = size;
            buffer = new T[size];
            count = 0;
            head = 0;
            tail = 0;
        }

        /// <summary>
        /// ����ֵ�б�
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
        /// �����
        /// </summary>
        /// <param name="value"></param>
        public void Enqueue(T value)
        {
            Enqueue(value, Timeout.Infinite);
        }

        /// <summary>
        /// �����
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
        /// �����
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
        /// ������
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            return Dequeue(Timeout.Infinite);
        }

        /// <summary>
        /// ������
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
        /// ������
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
        /// ȡ����ͷ
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
        /// ȡ����ͷ
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
        /// ���
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
        /// �Ƿ�ͬ��
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// �����ܳ���
        /// </summary>
        public int Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// �������ݳ���
        /// </summary>
        public int Count
        {
            get { lock (syncRoot) { return count; } }
        }

        /// <summary>
        /// ���Ƶ�����
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            T[] tmpArray = Values;
            tmpArray.CopyTo(array, index);
        }

        /// <summary>
        /// ȡ������
        /// </summary>
        public object SyncRoot
        {
            get { return this.syncRoot; }
        }

        /// <summary>
        /// ȡIEnumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException("δʵ��.");
        }

    }

    /// <summary>
    /// ���г�ʱ�쳣
    /// </summary>
    public class QueueTimeoutException : Exception
    {

        /// <summary>
        /// ���캯��
        /// </summary>
        public QueueTimeoutException()
            : base("���з�����ʱ.")
        {
        }

    }

}