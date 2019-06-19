using System;
using System.IO;
using System.Reflection;
using System.Text;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.IO
{
    /// <summary>
    /// 文件内容读写处理工具类
    /// </summary>
    public static class FileUtility
    {

        #region GetFullFileName 获取指定存在的文件，如果不存在返回空
        /// <summary>
        /// 获取指定存在的文件，如果不存在返回空，按照以下顺序获取：
        /// 1. 当前路径\filename;
        /// 2. 当前路径\Configs\fileName;
        /// 3. 当前路径\xxx.exe.fileName;
        /// 4. 当前路径\AOA.Common.Utility.dll.fileName;
        /// </summary>
        /// <param name="fileName">需要获取的文件名，一般是配置文件</param>
        /// <returns></returns>
        public static string GetFullFileName(string fileName)
        {
            string tmpFileName = fileName;

            if (!string.IsNullOrEmpty(tmpFileName))
                if (!File.Exists(tmpFileName))
                    tmpFileName = string.Empty;

            if (string.IsNullOrEmpty(tmpFileName))
            {
                // fileName
                tmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                if (!File.Exists(tmpFileName))
                    tmpFileName = string.Empty;
            }

            if (string.IsNullOrEmpty(tmpFileName))
            {
                // Configs\fileName
                tmpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Configs\\{fileName}");
                if (!File.Exists(tmpFileName))
                    tmpFileName = string.Empty;
            }

            if (string.IsNullOrEmpty(tmpFileName))
            {
                // xxx.exe.fileName
                tmpFileName = Assembly.GetEntryAssembly().Location; // AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                if (tmpFileName != null)
                {
                    tmpFileName = Path.ChangeExtension(tmpFileName, $".{fileName}");
                    if (!File.Exists(tmpFileName))
                        tmpFileName = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(tmpFileName))
            {
                // AOA.Common.Utility.dll.fileName
                Assembly currentAssembly = typeof(FileUtility).Assembly;
                if (!currentAssembly.GlobalAssemblyCache)
                {
                    tmpFileName = currentAssembly.Location + $".{fileName}";
                    if (!File.Exists(tmpFileName))
                        tmpFileName = string.Empty;
                }
            }
            return tmpFileName;
        }
        #endregion

        #region LoadFileToByteArray 读取文件到字节数组
        /// <summary>
        /// 读取文件到字节数组
        /// </summary>
        /// <param name="filePath">含路径的全名</param>
        /// <returns>字节数组</returns>
        public static byte[] LoadFileToByteArray(string filePath)
        {
            byte[] fileData = null;
            try
            {
                if (File.Exists(filePath))
                    using (FileStream myFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        BinaryReader br = new BinaryReader(myFile);
                        try
                        {
                            if (myFile.Length > int.MaxValue)
                                return null;

                            fileData = br.ReadBytes((int)myFile.Length);
                        }
                        catch
                        {
                            return null;
                        }
                        finally
                        {
                            br.Close();
                            myFile.Close();
                        }
                    }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "LoadFileToByteArray", "FileUtility");
                return null;
            }
            return fileData;
        }
        #endregion

        #region LoadFileToDefaultString 读取文件到系统默认编码字符串
        /// <summary>
        /// 读取文件到系统默认编码字符串
        /// </summary>
        /// <param name="filePath">含路径的全名</param>
        /// <returns>字节数组</returns>
        public static string LoadFileToDefaultString(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath, Encoding.Default);
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "LoadFileToDefaultString", "FileUtility");
                return string.Empty;
            }
        }
        #endregion

        #region LoadFileToUTF8String 读取文件到UTF8字符串
        /// <summary>
        /// 读取文件到UTF8字符串
        /// </summary>
        /// <param name="filePath">含路径的全名</param>
        /// <returns>字节数组</returns>
        public static string LoadFileToUTF8String(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "LoadFileToUTF8String", "FileUtility");
                return string.Empty;
            }
        }
        #endregion

        #region LoadFileToBase64 读取文件到Base64字符串
        /// <summary>
        /// 读取文件到Base64字符串
        /// </summary>
        /// <param name="filePath">含路径的全名</param>
        /// <returns>字节数组</returns>
        public static string LoadFileToBase64(string filePath)
        {
            try
            {
                byte[] fileData = LoadFileToByteArray(filePath);
                if (fileData == null || fileData.Length == 0)
                    return string.Empty;
                else
                    return fileData.Base64Encode();
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "LoadFileToBase64", "FileUtility");
                return string.Empty;
            }
        }
        #endregion

        #region WriteByteArrayToNewFile 将字节数组存入新建文件
        /// <summary>
        /// 将字节数组存入新建文件
        /// </summary>
        /// <param name="filePath">新建文件名</param>
        /// <param name="fileData">字节数组数据</param>
        /// <returns></returns>
        public static bool WriteByteArrayToNewFile(string filePath, byte[] fileData)
        {
            bool result = false;
            if (fileData != null)
            {
                try
                {
                    using (FileStream myFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        myFile.Write(fileData, 0, fileData.Length);
                        myFile.Close();
                    }
                    result = true;
                }
                catch (Exception ex)
                {
                    NLogUtility.ExceptionLog(ex, "WriteByteArrayToFile", "FileUtility");
                }
            }
            return result;
        }
        #endregion

        #region WriteBase64DataToNewFile 将Base64编码的字节数组存入新建文件
        /// <summary>
        /// 将Base64编码的字节数组存入新建文件
        /// </summary>
        /// <param name="filePath">新建文件名</param>
        /// <param name="base64Data">Base64编码的字节数组</param>
        /// <returns></returns>
        public static bool WriteBase64DataToNewFile(string filePath, string base64Data)
        {
            byte[] fileData = base64Data.Base64StringDecodeToByteArray();
            return WriteByteArrayToNewFile(filePath, fileData);
        }
        #endregion

        #region ReadText 从文件中读取所有文本
        /// <summary>
        /// 从文件中读取文本
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">文本编码</param>
        /// <returns>读取的文本</returns>
        public static string ReadText(string path, Encoding encoding)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                string output = string.Empty;
                string rl;
                while ((rl = sr.ReadLine()) != null)
                    output += rl + Environment.NewLine;
                sr.Close();
                return output;
            }
        }
        #endregion

        #region ReadText 从文件中读取一段文本
        /// <summary>
        /// 从文件中读取一段文本
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">文本编码</param>
        /// <param name="startLine">起始行</param>
        /// <param name="linesCount">要读取的行数</param>
        /// <param name="readedLinesCount">实际读取到的行数</param>
        /// <returns>读取的文本</returns>
        public static string ReadText(string path, Encoding encoding, int startLine, int linesCount, out int readedLinesCount)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                string output = string.Empty;
                string rl;
                int endLine = startLine + linesCount - 1;
                int currentLine = 0;
                readedLinesCount = 0;
                while ((rl = sr.ReadLine()) != null)
                {
                    if (currentLine >= startLine && currentLine <= endLine)
                    {
                        output += rl + Environment.NewLine;
                        readedLinesCount++;
                    }
                    currentLine++;

                    if (currentLine > endLine)
                        break;
                }
                sr.Close();
                return output;
            }
        }
        #endregion

        #region WriteAllText 创建一个新文件，在其中写入指定的字符串，然后关闭文件。如果目标文件已存在，则覆盖该文件。
        /// <summary>
        /// 创建一个新文件，在其中写入指定的字符串，然后关闭文件。如果目标文件已存在，则覆盖该文件。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="contents">内容</param>
        /// <returns>是否成功</returns>
        public static bool WriteAllText(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents, Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }
}
