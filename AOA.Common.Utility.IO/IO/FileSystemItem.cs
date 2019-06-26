using System;

namespace AOA.Common.Utility.IO
{

    /// <summary>
    /// FileSystemItem
    /// </summary>
    public class FileSystemItem
    {

        private string _Version;

        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// �Ƿ����ļ���
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// ��С
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime LastAccessDate { get; set; }

        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        public DateTime LastWriteDate { get; set; }

        /// <summary>
        /// �ļ���
        /// </summary>
        public long FileCount { get; set; }

        /// <summary>
        /// �ļ�����
        /// </summary>
        public long SubFolderCount { get; set; }

        /// <summary>
        /// �汾
        /// </summary>
        /// <returns></returns>
        public string Version()
        {
            if (_Version == null)
                _Version = GetType().Assembly.GetName().Version.ToString();

            return _Version;
        }

    }

}