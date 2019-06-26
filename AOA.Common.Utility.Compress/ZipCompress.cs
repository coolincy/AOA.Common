using System;
using System.IO;
using System.Text;

using AOA.Common.Utility.ClassExtensions;

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace AOA.Common.Utility.Compress
{

    /// <summary>
    /// Zip压缩算法工具类
    /// </summary>
    public static class ZipCompress
    {

        #region byte[] = byte[] 最基本的压缩解压过程，源和目标都是字节数组

        #region 压缩原字节数组到字节数组 public static byte[] Compress(byte[] source)
        /// <summary>
        /// 压缩原字节数组到字节数组
        /// </summary>
        /// <param name="source">原字节数组</param>
        /// <returns>字节数组</returns>
        public static byte[] Compress(byte[] source)
        {
            using (MemoryStream mMemory = new MemoryStream())
            {
                Deflater mDeflater = new Deflater(Deflater.BEST_COMPRESSION);
                DeflaterOutputStream mStream = new DeflaterOutputStream(mMemory, mDeflater, 131072);
                mStream.Write(source, 0, source.Length);
                mStream.Close();
                return mMemory.ToArray();
            }
        }
        #endregion

        #region 解压缩字节数组到原字节数组 public static byte[] DeCompress(byte[] source)
        /// <summary>
        /// 解压缩字节数组到原字节数组
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <returns>原字节数组</returns>
        public static byte[] DeCompress(byte[] source)
        {
            using (InflaterInputStream mStream = new InflaterInputStream(new MemoryStream(source)))
            {
                using (MemoryStream mMemory = new MemoryStream())
                {
                    Int32 mSize;
                    byte[] mWriteData = new byte[4096];
                    while (true)
                    {
                        mSize = mStream.Read(mWriteData, 0, mWriteData.Length);
                        if (mSize > 0)
                            mMemory.Write(mWriteData, 0, mSize);
                        else
                            break;
                    }
                    mStream.Close();
                    return mMemory.ToArray();
                }
            }
        }
        #endregion

        #endregion

        #region byte[] = base64 字节数组到Base64字符串的压缩及反向解压

        #region 压缩原字节数组到Base64编码的结果 public static string CompressToBase64()
        /// <summary>
        /// 压缩原字节数组到Base64编码的结果
        /// </summary>
        /// <param name="source">原字节数组</param>
        /// <returns>Base64编码的结果</returns>
        public static string CompressToBase64(byte[] source)
        {
            byte[] byteDest = Compress(source);
            return Convert.ToBase64String(byteDest);
        }
        #endregion

        #region 解压缩Base64编码的结果到原字节数组 public static byte[] DeCompressFromBase64()
        /// <summary>
        /// 解压缩Base64编码的结果到原字节数组
        /// </summary>
        /// <param name="source">Base64编码的结果</param>
        /// <returns>原字节数组</returns>
        public static byte[] DeCompressFromBase64(string source)
        {
            byte[] byteSource = Convert.FromBase64String(source);
            return ZipCompress.DeCompress(byteSource);
        }
        #endregion

        #endregion

        #region byte[] = hex 字节数组到16进制字符串的压缩及反向解压

        #region 压缩原字节数组到16进制字符串的结果 public static string CompressToHex()
        /// <summary>
        /// 压缩原字节数组到16进制字符串的结果
        /// </summary>
        /// <param name="source">原字节数组</param>
        /// <returns>16进制字符串的结果</returns>
        public static string CompressToHex(byte[] source)
        {
            byte[] byteDest = Compress(source);
            return StringEncode.ByteArrayToHexString(byteDest);
        }
        #endregion

        #region 解压缩16进制字符串的结果到原字节数组 public static byte[] DeCompressFromHex()
        /// <summary>
        /// 解压缩16进制字符串的结果到原字节数组
        /// </summary>
        /// <param name="source">16进制字符串的结果</param>
        /// <returns>原字节数组</returns>
        public static byte[] DeCompressFromHex(string source)
        {
            byte[] byteSource = source.HexStringToByteArray();
            return ZipCompress.DeCompress(byteSource);
        }
        #endregion

        #endregion

        #region string = byte[] 字符串到字节数组的压缩及反向解压

        #region 压缩原字符串到字节数组 public static byte[] CompressStringToArray()
        /// <summary>
        /// 压缩原字符串到字节数组
        /// </summary>
        /// <param name="source">原字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] CompressStringToArray(string source)
        {
            byte[] byteSource = Encoding.UTF8.GetBytes(source);
            return Compress(byteSource);
        }
        #endregion

        #region 解压缩字节数组到原字符串 public static string DeCompressStringFromArray()
        /// <summary>
        /// 解压缩字节数组到原字符串
        /// </summary>
        /// <param name="source">字节数组</param>
        /// <returns>原字符串</returns>
        public static string DeCompressStringFromArray(byte[] source)
        {
            byte[] byteDest = DeCompress(source);
            return Encoding.UTF8.GetString(byteDest, 0, byteDest.Length);
        }
        #endregion

        #endregion

        #region string = base64 字符串到Base64字符串的压缩及反向解压

        #region 压缩原字符串到Base64编码的结果 public static string CompressStringToBase64()
        /// <summary>
        /// 压缩原字符串到Base64编码的结果
        /// </summary>
        /// <param name="source">原字符串</param>
        /// <returns>Base64编码的结果</returns>
        public static string CompressStringToBase64(string source)
        {
            byte[] byteDest = CompressStringToArray(source);
            return Convert.ToBase64String(byteDest);
        }
        #endregion

        #region 解压缩Base64编码的结果到原字符串 public static string DeCompressStringFromBase64()
        /// <summary>
        /// 解压缩Base64编码的结果到原字符串
        /// </summary>
        /// <param name="source">Base64编码的结果</param>
        /// <returns>原字符串</returns>
        public static string DeCompressStringFromBase64(string source)
        {
            byte[] byteSource = Convert.FromBase64String(source);
            return ZipCompress.DeCompressStringFromArray(byteSource);
        }
        #endregion

        #endregion

        #region string = hex 字符串到16进制字符串的压缩及反向解压

        #region 压缩原字符串到16进制字符串的结果 public static string CompressStringToHex()
        /// <summary>
        /// 压缩原字符串到16进制字符串的结果
        /// </summary>
        /// <param name="source">原字符串</param>
        /// <returns>Hex编码的结果</returns>
        public static string CompressStringToHex(string source)
        {
            byte[] byteDest = CompressStringToArray(source);
            return StringEncode.ByteArrayToHexString(byteDest);
        }
        #endregion

        #region 解压缩16进制字符串的结果到原字符串 public static string DeCompressStringFromHex()
        /// <summary>
        /// 解压缩16进制字符串的结果到原字符串
        /// </summary>
        /// <param name="source">Base64编码的结果</param>
        /// <returns>原字符串</returns>
        public static string DeCompressStringFromHex(string source)
        {
            byte[] byteSource = source.HexStringToByteArray();
            return ZipCompress.DeCompressStringFromArray(byteSource);
        }
        #endregion

        #endregion

    }

}
