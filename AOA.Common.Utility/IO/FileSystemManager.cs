using System;
using System.Collections.Generic;
using System.IO;

namespace AOA.Common.Utility.IO
{
    /// <summary>
    /// FileSystemManager
    /// </summary>
    public class FileSystemManager
    {

        /// <summary>
        /// �����ϴ�����չ�������Ըĳɴ������ļ��ж���
        /// </summary>
        public static string[] safeExtensions = { "htm", "html", "txt", "js", "css", "xml", "sitemap", "jpg", "gif", "png", "rar", "zip" };
        /// <summary>
        /// �������ϴ�����չ�������Ըĳɴ������ļ��ж��� 
        /// </summary>
        public static string[] unSafeExtensions = { "", "asp", "aspx", "cs", "net", "dll", "config", "ascx", "master", "asmx", "asax", "cd", "browser", "rpt", "ashx", "xsd", "mdf", "resx", "xsd" };
        /// <summary>
        /// ����༭����չ�������Ըĳɴ������ļ��ж��� 
        /// </summary>
        public static string[] canEditExtension = { "htm", "html", "txt", "js", "css", "xml", "sitemap" };

        #region RootFolder �ļ�����ĸ��ļ���
        private static string rootFolder = ConfigReader.GetString("FileSystemManagerRoot", AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>
        /// �ļ�����ĸ��ļ���
        /// </summary>
        public static string RootFolder
        {
            get { return FileSystemManager.rootFolder; }
            set { FileSystemManager.rootFolder = value; }
        }
        #endregion

        #region FileSystemManager
        /// <summary>
        /// ��̬��������
        /// </summary>
        static FileSystemManager()
        {
            if (string.IsNullOrEmpty(rootFolder))
                rootFolder = AppDomain.CurrentDomain.BaseDirectory;
        }
        #endregion

        #region GetItems ��ȡ�ļ�����ĸ��ļ����µ��ļ��м��ļ��б�
        /// <summary>
        /// ��ȡ�ļ�����ĸ��ļ����µ��ļ��м��ļ��б�
        /// </summary>
        /// <returns>�ļ�����ĸ��ļ��м��ļ��б�</returns>
        public static List<FileSystemItem> GetItems()
        {
            return GetItems(rootFolder);
        }
        #endregion

        #region GetItems ��ȡָ���ļ����µ��ļ��м��ļ��б�
        /// <summary>
        /// ��ȡָ���ļ����µ��ļ��м��ļ��б�������[���ļ���]����[��һ��]������Ŀ¼
        /// </summary>
        /// <param name="path">ָ���ļ���</param>
        /// <returns>�ļ��м��ļ��б�</returns>
        public static List<FileSystemItem> GetItems(string path)
        {
            return GetItems(path, true);
        }
        #endregion

        #region GetItems ��ȡָ���ļ����µ��ļ��м��ļ��б�
        /// <summary>
        /// ��ȡָ���ļ����µ��ļ��м��ļ��б�
        /// </summary>
        /// <param name="path">ָ���ļ���</param>
        /// <param name="withParent">�Ƿ������[���ļ���]����[��һ��]������Ŀ¼</param>
        /// <returns>�ļ��м��ļ��б�</returns>
        public static List<FileSystemItem> GetItems(string path, bool withParent)
        {
            string[] folders = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            List<string> foldersList = new List<string>(folders);
            foldersList.Sort();
            List<string> filesList = new List<string>(files);
            filesList.Sort();

            List<FileSystemItem> list = new List<FileSystemItem>();
            foreach (string s in foldersList)
            {
                FileSystemItem item = new FileSystemItem();
                DirectoryInfo di = new DirectoryInfo(s);
                item.Name = di.Name;
                item.FullName = di.FullName;
                item.CreationDate = di.CreationTime;
                item.LastAccessDate = di.LastAccessTime;
                item.LastWriteDate = di.LastWriteTime;
                item.IsFolder = true;
                list.Add(item);
            }
            foreach (string s in filesList)
            {
                FileSystemItem item = new FileSystemItem();
                FileInfo fi = new FileInfo(s);
                item.Name = fi.Name;
                item.FullName = fi.FullName;
                item.CreationDate = fi.CreationTime;
                item.LastAccessDate = fi.LastAccessTime;
                item.LastWriteDate = fi.LastWriteTime;
                item.IsFolder = false;
                item.Size = fi.Length;
                list.Add(item);
            }

            if (withParent && path.ToLower() != rootFolder.ToLower())
            {
                DirectoryInfo topdi = new DirectoryInfo(path).Parent;
                if (topdi != null)
                {
                    FileSystemItem topitem = new FileSystemItem();
                    topitem.Name = "[��һ��]";
                    topitem.FullName = topdi.FullName;
                    topitem.CreationDate = topdi.CreationTime;
                    topitem.LastAccessDate = topdi.LastAccessTime;
                    topitem.LastWriteDate = topdi.LastWriteTime;
                    topitem.IsFolder = true;
                    list.Insert(0, topitem);
                }

                DirectoryInfo rootdi = new DirectoryInfo(rootFolder);
                if (rootdi != null)
                {
                    FileSystemItem rootitem = new FileSystemItem();
                    rootitem.Name = "[���ļ���]";
                    rootitem.FullName = rootdi.FullName;
                    rootitem.CreationDate = rootdi.CreationTime;
                    rootitem.LastAccessDate = rootdi.LastAccessTime;
                    rootitem.LastWriteDate = rootdi.LastWriteTime;
                    list.Insert(0, rootitem);
                }
            }
            return list;
        }
        #endregion

        #region CreateFolder �����ļ���
        /// <summary>
        /// �����ļ���
        /// </summary>
        /// <param name="name">�ļ�������</param>
        /// <param name="parentName">���ļ�������</param>
        public static void CreateFolder(string name, string parentName)
        {
            DirectoryInfo di = new DirectoryInfo(parentName);
            di.CreateSubdirectory(name);
        }
        #endregion

        #region DeleteFolder ɾ���ļ���
        /// <summary>
        /// ��ָ��·��ɾ����Ŀ¼��
        /// </summary>
        /// <param name="path">Ҫ�Ƴ���Ŀ¼�����ơ�</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool DeleteFolder(string path)
        {
            try
            {
                Directory.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region DeleteFolder ɾ���ļ���
        /// <summary>
        /// ɾ��ָ����Ŀ¼�������ָʾ��ɾ����Ŀ¼�е��κ���Ŀ¼��
        /// </summary>
        /// <param name="path">Ҫ�Ƴ���Ŀ¼�����ơ�</param>
        /// <param name="recursive">��Ҫ�Ƴ� path �е�Ŀ¼����Ŀ¼���ļ�����Ϊ true������Ϊ false��</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool DeleteFolder(string path, bool recursive)
        {
            try
            {
                Directory.Delete(path, recursive);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region MoveFolder �ƶ��ļ���
        /// <summary>
        /// �ƶ��ļ���
        /// </summary>
        /// <param name="oldPath">���ļ���·��</param>
        /// <param name="newPath">���ļ���·��</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool MoveFolder(string oldPath, string newPath)
        {
            try
            {
                Directory.Move(oldPath, newPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region CreateFile �����ļ�
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="filename">�ļ�����</param>
        /// <param name="path">�ļ���·��</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool CreateFile(string filename, string path)
        {
            try
            {
                FileStream fs = File.Create(String.Format("{0}\\{1}", path, filename));
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region CreateFile �����ļ���ͬʱд������
        /// <summary>
        /// �����ļ���ͬʱд������
        /// </summary>
        /// <param name="filename">�ļ�����</param>
        /// <param name="path">�ļ���·��</param>
        /// <param name="contents">�ļ�����</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool CreateFile(string filename, string path, byte[] contents)
        {
            try
            {
                FileStream fs = File.Create(String.Format("{0}\\{1}", path, filename));
                fs.Write(contents, 0, contents.Length);
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region DeleteFile ɾ���ļ�
        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        /// <param name="path">�ļ�·��</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region MoveFile �ƶ��ļ�
        /// <summary>
        /// �ƶ��ļ�
        /// </summary>
        /// <param name="oldPath">���ļ�·��</param>
        /// <param name="newPath">���ļ�·��</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool MoveFile(string oldPath, string newPath)
        {
            try
            {
                File.Move(oldPath, newPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region GetItemInfo ��ȡ�ļ����ļ�����Ϣ
        /// <summary>
        /// ��ȡ�ļ����ļ�����Ϣ
        /// </summary>
        /// <param name="path">�ļ����ļ���·��</param>
        /// <returns>�ļ����ļ�����Ϣ</returns>
        public static FileSystemItem GetItemInfo(string path)
        {
            FileSystemItem item = new FileSystemItem();
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                item.Name = di.Name;
                item.FullName = di.FullName;
                item.CreationDate = di.CreationTime;
                item.IsFolder = true;
                item.LastAccessDate = di.LastAccessTime;
                item.LastWriteDate = di.LastWriteTime;
                item.FileCount = di.GetFiles().Length;
                item.SubFolderCount = di.GetDirectories().Length;
            }
            else
            {
                FileInfo fi = new FileInfo(path);
                item.Name = fi.Name;
                item.FullName = fi.FullName;
                item.CreationDate = fi.CreationTime;
                item.LastAccessDate = fi.LastAccessTime;
                item.LastWriteDate = fi.LastWriteTime;
                item.IsFolder = false;
                item.Size = fi.Length;
            }
            return item;
        }
        #endregion

        #region CopyFolder �����ļ���
        /// <summary>
        /// �����ļ���
        /// </summary>
        /// <param name="source">Դ�ļ�·��</param>
        /// <param name="destination">Ŀ���ļ�·��</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public static bool CopyFolder(string source, string destination)
        {
            try
            {
                String[] files;
                if (destination[destination.Length - 1] != Path.DirectorySeparatorChar)
                    destination += Path.DirectorySeparatorChar;
                if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
                files = Directory.GetFileSystemEntries(source);
                foreach (string element in files)
                {
                    if (Directory.Exists(element))
                        CopyFolder(element, destination + Path.GetFileName(element));
                    else
                        File.Copy(element, destination + Path.GetFileName(element), true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region GetExtension ��ȡ�ļ�����չ��
        /// <summary>
        /// ��ȡ�ļ�����չ��
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <returns>��չ��</returns>
        public static string GetExtension(string fileName)
        {
            //�õ�string��.XXX���ļ�����׺ LastIndexOf���õ����λ�ã� Substring�����д�X��λ�ã�
            return fileName.LastIndexOf(".") >= 0 ? fileName.ToLower().Substring(fileName.LastIndexOf(".") + 1) : "";
        }
        #endregion

        #region IsSafeName �ж��Ƿ�Ϊ��ȫ�ļ���
        /// <summary>
        /// �ж��Ƿ�Ϊ��ȫ�ļ���
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <returns>�Ƿ�</returns>
        public static bool IsSafeName(string fileName)
        {
            string strExtension = GetExtension(fileName);
            for (int i = 0; i < safeExtensions.Length; i++)
                if (strExtension.Equals(safeExtensions[i]))
                    return true;
            return false;
        }
        #endregion

        #region IsUnsafeName �ж��Ƿ�Ϊ����ȫ�ļ���
        /// <summary>
        /// �ж��Ƿ�Ϊ����ȫ�ļ���
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <returns>�Ƿ�</returns>
        public static bool IsUnsafeName(string fileName)
        {
            string strExtension = GetExtension(fileName);
            for (int i = 0; i < unSafeExtensions.Length; i++)
                if (strExtension.Equals(unSafeExtensions[i]))
                    return true;
            return false;
        }
        #endregion

        #region IsCanEdit �ж��Ƿ�Ϊ�ɱ༭�ļ�
        /// <summary>
        /// �ж��Ƿ�Ϊ�ɱ༭�ļ�
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <returns>�Ƿ�</returns>
        public static bool IsCanEdit(string fileName)
        {
            string strExtension = GetExtension(fileName);
            for (int i = 0; i < canEditExtension.Length; i++)
                if (strExtension.Equals(canEditExtension[i]))
                    return true;
            return false;
        }
        #endregion

    }

}