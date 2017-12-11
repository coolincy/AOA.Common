using System;
using System.Collections;
using System.IO;

namespace AOA.Common.Utility.IO
{

    /// <summary>
    /// �ļ���������
    /// </summary>
    public class MultiFileWatcher : IDisposable
    {

        private ArrayList _watchers = new ArrayList();

        /// <summary>
        /// �ļ�����¼�
        /// </summary>
        public event FileSystemEventHandler OnChange;

        /// <summary>
        /// ��������
        /// </summary>
        public MultiFileWatcher()
        {
        }

        /// <summary>
        /// ��������
        /// </summary>
        public MultiFileWatcher(FileSystemEventHandler onChange)
        {
            OnChange += onChange;
        }

        /// <summary>
        /// ��������
        /// </summary>
        public MultiFileWatcher(ICollection fileNames)
        {
            Watch(fileNames);
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void Dispose()
        {
            StopWatching();
        }

        /// <summary>
        /// ֹͣ����
        /// </summary>
        public void StopWatching()
        {
            lock (this)
            {
                foreach (FileSystemWatcher watcher in _watchers)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
                _watchers.Clear();
            }
        }

        /// <summary>
        /// ����һϵ���ļ�
        /// </summary>
        /// <param name="fileNames"></param>
        public void Watch(ICollection fileNames)
        {
            if (fileNames == null)
                return;

            foreach (string s in fileNames)
                Watch(s);
        }

        /// <summary>
        /// ����һ���ļ�
        /// </summary>
        /// <param name="fileName"></param>
        public void Watch(string fileName)
        {
            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(fileName),
                Filter = Path.GetFileName(fileName),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes, /* ��ʼ����*/
                EnableRaisingEvents = true
            };
            watcher.Created += OnWatcherChanged;
            watcher.Changed += OnWatcherChanged;
            watcher.Deleted += OnWatcherChanged;
            NLogUtility.InfoLog(string.Format("�����ļ�: {0}\\{1}", watcher.Path, watcher.Filter), "Watch", "MultiFileWatcher");

            lock (this)
            {
                _watchers.Add(watcher);
            }
        }

        /// <summary>
        /// ���ӵ��ļ������ı�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                FileSystemWatcher watcher = (FileSystemWatcher)sender;
                NLogUtility.InfoLog(string.Format("�ļ��ѱ��({0}): {1}\\{2}", e.ChangeType, watcher.Path, watcher.Filter), "OnWatcherChanged", "MultiFileWatcher");
                if (OnChange != null)
                    OnChange(sender, e);
            }
        }

    }

}

