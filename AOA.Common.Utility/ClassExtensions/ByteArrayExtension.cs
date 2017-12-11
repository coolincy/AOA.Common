using System;
using System.Security.Cryptography;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 字节数组扩展方法
    /// </summary>
    public static class ByteArrayExtension
    {

        /// <summary>
        /// 计算数据的MD5值，以16进制字符串表示结果
        /// </summary>
        /// <param name="data">数据字节数组</param>
        /// <returns>以16进制字符串表示的MD5值</returns>
        public static string HashToMD5Hex(this byte[] data)
        {
            using (MD5CryptoServiceProvider md5sp = new MD5CryptoServiceProvider())
            {
                return StringEncode.ByteArrayToHexString(md5sp.ComputeHash(data));
            }
        }

        /// <summary>
        /// 计算数据的MD5值，以16进制字符串表示结果
        /// </summary>
        /// <param name="data">数据字节数组</param>
        /// <param name="offset">字节数组中的偏移量，从该位置开始使用数据</param>
        /// <param name="count">数组中用作数据的字节数</param>
        /// <returns>以16进制字符串表示的MD5值</returns>
        public static string HashToMD5Hex(this byte[] data, int offset, int count)
        {
            using (MD5CryptoServiceProvider md5sp = new MD5CryptoServiceProvider())
            {
                return StringEncode.ByteArrayToHexString(md5sp.ComputeHash(data, offset, count));
            }
        }

        /// <summary>
        /// 将字节数组转换为Base64编码的字符串
        /// </summary>
        /// <param name="data">数据字节数组</param>
        /// <returns>Base64编码的字符串</returns>
        public static string Base64Encode(this byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 将字节数组转换为Base64编码的字符串
        /// </summary>
        /// <param name="data">数据字节数组</param>
        /// <param name="offset">字节数组中的偏移量，从该位置开始使用数据</param>
        /// <param name="count">数组中用作数据的字节数</param>
        /// <returns>Base64编码的字符串</returns>
        public static string Base64Encode(this byte[] data, int offset, int count)
        {
            return Convert.ToBase64String(data, offset, count);
        }

    }
}
