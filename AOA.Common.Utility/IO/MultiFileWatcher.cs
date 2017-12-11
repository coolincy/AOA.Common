using System;
using System.Collections;
using System.IO;

namespace AOA.Common.Utility.IO
{

    /// <summary>
    /// 文件变更监控类
    /// </summary>
    public class MultiFileWatcher : IDisposable
    {

        private ArrayList _watchers = new ArrayList();

        /// <summary>
        /// 文件变更事件
        /// </summary>
        public event FileSystemEventHandler OnChange;

        /// <summary>
        /// 构建函数
        /// </summary>
        public MultiFileWatcher()
        {
        }

        /// <summary>
        /// 构建函数
        /// </summary>
        public MultiFileWatcher(FileSystemEventHandler onChange)
        {
            OnChange += onChange;
        }

        /// <summary>
        /// 构建函数
        /// </summary>
        public MultiFileWatcher(ICollection fileNames)
        {
            Watch(fileNames);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        public void Dispose()
        {
            StopWatching();
        }

        /// <summary>
        /// 停止监视
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
        /// 监视一系列文件
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
        /// 监视一个文件
        /// </summary>
        /// <param name="fileName"></param>
        public void Watch(string fileName)
        {
            FileSystemWatcher watcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(fileName),
                Filter = Path.GetFileName(fileName),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes, /* 开始监视*/
                EnableRaisingEvents = true
            };
            watcher.Created += OnWatcherChanged;
            watcher.Changed += OnWatcherChanged;
            watcher.Deleted += OnWatcherChanged;
            NLogUtility.InfoLog(string.Format("监视文件: {0}\\{1}", watcher.Path, watcher.Filter), "Watch", "MultiFileWatcher");

            lock (this)
            {
                _watchers.Add(watcher);
            }
        }

        /// <summary>
        /// 监视的文件发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                FileSystemWatcher watcher = (FileSystemWatcher)sender;
                NLogUtility.InfoLog(string.Format("文件已变更({0}): {1}\\{2}", e.ChangeType, watcher.Path, watcher.Filter), "OnWatcherChanged", "MultiFileWatcher");
                if (OnChange != null)
                    OnChange(sender, e);
            }
        }

    }

}

