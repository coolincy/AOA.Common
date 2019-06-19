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
        /// 允许上传的扩展名，可以改成从配置文件中读出
        /// </summary>
        public static string[] safeExtensions = { "htm", "html", "txt", "js", "css", "xml", "sitemap", "jpg", "gif", "png", "rar", "zip" };
        /// <summary>
        /// 不允许上传的扩展名，可以改成从配置文件中读出 
        /// </summary>
        public static string[] unSafeExtensions = { "", "asp", "aspx", "cs", "net", "dll", "config", "ascx", "master", "asmx", "asax", "cd", "browser", "rpt", "ashx", "xsd", "mdf", "resx", "xsd" };
        /// <summary>
        /// 允许编辑的扩展名，可以改成从配置文件中读出 
        /// </summary>
        public static string[] canEditExtension = { "htm", "html", "txt", "js", "css", "xml", "sitemap" };

        #region RootFolder 文件管理的根文件夹
        private static string rootFolder = ConfigReader.GetString("FileSystemManagerRoot", AppDomain.CurrentDomain.BaseDirectory);
        /// <summary>
        /// 文件管理的根文件夹
        /// </summary>
        public static string RootFolder
        {
            get { return FileSystemManager.rootFolder; }
            set { FileSystemManager.rootFolder = value; }
        }
        #endregion

        #region FileSystemManager
        /// <summary>
        /// 静态构建方法
        /// </summary>
        static FileSystemManager()
        {
            if (string.IsNullOrEmpty(rootFolder))
                rootFolder = AppDomain.CurrentDomain.BaseDirectory;
        }
        #endregion

        #region GetItems 读取文件管理的根文件夹下的文件夹及文件列表
        /// <summary>
        /// 读取文件管理的根文件夹下的文件夹及文件列表
        /// </summary>
        /// <returns>文件管理的根文件夹及文件列表</returns>
        public static List<FileSystemItem> GetItems()
        {
            return GetItems(rootFolder);
        }
        #endregion

        #region GetItems 读取指定文件夹下的文件夹及文件列表
        /// <summary>
        /// 读取指定文件夹下的文件夹及文件列表，包含“[根文件夹]”“[上一级]”两个目录
        /// </summary>
        /// <param name="path">指定文件夹</param>
        /// <returns>文件夹及文件列表</returns>
        public static List<FileSystemItem> GetItems(string path)
        {
            return GetItems(path, true);
        }
        #endregion

        #region GetItems 读取指定文件夹下的文件夹及文件列表
        /// <summary>
        /// 读取指定文件夹下的文件夹及文件列表
        /// </summary>
        /// <param name="path">指定文件夹</param>
        /// <param name="withParent">是否包含“[根文件夹]”“[上一级]”两个目录</param>
        /// <returns>文件夹及文件列表</returns>
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
                    topitem.Name = "[上一级]";
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
                    rootitem.Name = "[根文件夹]";
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

        #region CreateFolder 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="name">文件夹名称</param>
        /// <param name="parentName">父文件夹名称</param>
        public static void CreateFolder(string name, string parentName)
        {
            DirectoryInfo di = new DirectoryInfo(parentName);
            di.CreateSubdirectory(name);
        }
        #endregion

        #region DeleteFolder 删除文件夹
        /// <summary>
        /// 从指定路径删除空目录。
        /// </summary>
        /// <param name="path">要移除的目录的名称。</param>
        /// <returns>是否成功</returns>
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

        #region DeleteFolder 删除文件夹
        /// <summary>
        /// 删除指定的目录并（如果指示）删除该目录中的任何子目录。
        /// </summary>
        /// <param name="path">要移除的目录的名称。</param>
        /// <param name="recursive">若要移除 path 中的目录、子目录和文件，则为 true；否则为 false。</param>
        /// <returns>是否成功</returns>
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

        #region MoveFolder 移动文件夹
        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="oldPath">旧文件夹路径</param>
        /// <param name="newPath">新文件夹路径</param>
        /// <returns>是否成功</returns>
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

        #region CreateFile 创建文件
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="path">文件夹路径</param>
        /// <returns>是否成功</returns>
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

        #region CreateFile 创建文件，同时写入内容
        /// <summary>
        /// 创建文件，同时写入内容
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="path">文件夹路径</param>
        /// <param name="contents">文件内容</param>
        /// <returns>是否成功</returns>
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

        #region DeleteFile 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>是否成功</returns>
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

        #region MoveFile 移动文件
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="oldPath">旧文件路径</param>
        /// <param name="newPath">新文件路径</param>
        /// <returns>是否成功</returns>
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

        #region GetItemInfo 读取文件或文件夹信息
        /// <summary>
        /// 读取文件或文件夹信息
        /// </summary>
        /// <param name="path">文件或文件夹路径</param>
        /// <returns>文件或文件夹信息</returns>
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

        #region CopyFolder 复制文件夹
        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="destination">目标文件路径</param>
        /// <returns>是否成功</returns>
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

        #region GetExtension 获取文件名扩展名
        /// <summary>
        /// 获取文件名扩展名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>扩展名</returns>
        public static string GetExtension(string fileName)
        {
            //得到string的.XXX的文件名后缀 LastIndexOf（得到点的位置） Substring（剪切从X的位置）
            return fileName.LastIndexOf(".") >= 0 ? fileName.ToLower().Substring(fileName.LastIndexOf(".") + 1) : "";
        }
        #endregion

        #region IsSafeName 判断是否为安全文件名
        /// <summary>
        /// 判断是否为安全文件名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否</returns>
        public static bool IsSafeName(string fileName)
        {
            string strExtension = GetExtension(fileName);
            for (int i = 0; i < safeExtensions.Length; i++)
                if (strExtension.Equals(safeExtensions[i]))
                    return true;
            return false;
        }
        #endregion

        #region IsUnsafeName 判断是否为不安全文件名
        /// <summary>
        /// 判断是否为不安全文件名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否</returns>
        public static bool IsUnsafeName(string fileName)
        {
            string strExtension = GetExtension(fileName);
            for (int i = 0; i < unSafeExtensions.Length; i++)
                if (strExtension.Equals(unSafeExtensions[i]))
                    return true;
            return false;
        }
        #endregion

        #region IsCanEdit 判断是否为可编辑文件
        /// <summary>
        /// 判断是否为可编辑文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否</returns>
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