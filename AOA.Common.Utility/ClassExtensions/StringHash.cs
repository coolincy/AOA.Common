using System;
using System.Security.Cryptography;
using System.Text;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 字符串哈希扩展
    /// </summary>
    public static class StringHash
    {

        #region 由参数指定编码

        #region HashToMD5 指定编码字符串计算MD5值(字节数组)
        /// <summary>
        /// 指定编码字符串计算MD5值(字节数组)
        /// </summary>
        /// <param name="sourceStr">指定编码的字符串</param>
        /// <param name="theEncodeing">指定编码格式</param>
        /// <returns>MD5(字节数组)</returns>
        public static byte[] HashToMD5(this string sourceStr, Encoding theEncodeing)
        {
            byte[] Bytes = theEncodeing.GetBytes(sourceStr);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] result = md5.ComputeHash(Bytes);
                return result;
            }
        }
        #endregion

        #region HashToMD5Hex 指定编码字符串计算MD5值(十六进制编码字符串)
        /// <summary>
        /// 指定编码字符串计算MD5值(十六进制编码字符串)
        /// </summary>
        /// <param name="sourceStr">指定编码的字符串</param>
        /// <param name="theEncodeing">指定编码格式</param>
        /// <returns>MD5(十六进制编码字符串)</returns>
        public static string HashToMD5Hex(this string sourceStr, Encoding theEncodeing)
        {
            return StringEncode.ByteArrayToHexString(HashToMD5(sourceStr, theEncodeing));
        }
        #endregion

        #region HashToMD5Base64 指定编码字符串计算MD5值(Base64编码字符串)
        /// <summary>
        /// 指定编码字符串计算MD5值(Base64编码字符串)
        /// </summary>
        /// <param name="sourceStr">指定编码的字符串</param>
        /// <param name="theEncodeing">指定编码格式</param>
        /// <returns>MD5(Base64编码字符串)</returns>
        public static string HashToMD5Base64(this string sourceStr, Encoding theEncodeing)
        {
            return Convert.ToBase64String(HashToMD5(sourceStr, theEncodeing));
        }
        #endregion

        #region HashToSHA1 指定编码字符串计算SHA1值(字节数组)
        /// <summary>
        /// 指定编码字符串计算SHA1值字节数组
        /// </summary>
        /// <param name="sourceStr">指定编码的字符串</param>
        /// <param name="theEncodeing">指定编码格式</param>
        /// <returns>SHA1(字节数组)</returns>
        public static byte[] HashToSHA1(this string sourceStr, Encoding theEncodeing)
        {
            byte[] Bytes = theEncodeing.GetBytes(sourceStr);
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] result = sha1.ComputeHash(Bytes);
                return result;
            }
        }
        #endregion

        #region HashToSHA1Hex 指定编码字符串计算SHA1值(十六进制编码字符串)
        /// <summary>
        /// 指定编码字符串计算SHA1值(十六进制编码字符串)
        /// </summary>
        /// <param name="sourceStr">指定编码的字符串</param>
        /// <param name="theEncodeing">指定编码格式</param>
        /// <returns>SHA1(十六进制编码字符串)</returns>
        public static string HashToSHA1Hex(this string sourceStr, Encoding theEncodeing)
        {
            return StringEncode.ByteArrayToHexString(HashToSHA1(sourceStr, theEncodeing));
        }
        #endregion

        #region HashToSHA1Base64 指定编码字符串计算SHA1值(Base64编码字符串)
        /// <summary>
        /// 指定编码字符串计算SHA1值(Base64编码字符串)
        /// </summary>
        /// <param name="sourceStr">指定编码的字符串</param>
        /// <param name="theEncodeing">指定编码格式</param>
        /// <returns>SHA1(Base64编码字符串)</returns>
        public static string HashToSHA1Base64(this string sourceStr, Encoding theEncodeing)
        {
            return Convert.ToBase64String(HashToSHA1(sourceStr, theEncodeing));
        }
        #endregion

        #endregion

        #region UTF8编码

        #region HashToMD5 UTF8编码字符串计算MD5值(字节数组)
        /// <summary>
        /// UTF8编码字符串计算MD5值(字节数组)
        /// </summary>
        /// <param name="sourceStr">UTF8编码的字符串</param>
        /// <returns>MD5(字节数组)</returns>
        public static byte[] HashToMD5(this string sourceStr)
        {
            return HashToMD5(sourceStr, Encoding.UTF8);
        }
        #endregion

        #region HashToMD5Hex UTF8编码字符串计算MD5值(十六进制编码字符串)
        /// <summary>
        /// UTF8编码字符串计算MD5值(十六进制编码字符串)
        /// </summary>
        /// <param name="sourceStr">UTF8编码的字符串</param>
        /// <returns>MD5(十六进制编码字符串)</returns>
        public static string HashToMD5Hex(this string sourceStr)
        {
            return StringEncode.ByteArrayToHexString(HashToMD5(sourceStr));
        }
        #endregion

        #region HashToMD5Base64 UTF8编码字符串计算MD5值(Base64编码字符串)
        /// <summary>
        /// UTF8编码字符串计算MD5值(Base64编码字符串)
        /// </summary>
        /// <param name="sourceStr">UTF8编码的字符串</param>
        /// <returns>MD5(Base64编码字符串)</returns>
        public static string HashToMD5Base64(this string sourceStr)
        {
            return Convert.ToBase64String(HashToMD5(sourceStr));
        }
        #endregion

        #region HashToSHA1 UTF8编码字符串计算SHA1值(字节数组)
        /// <summary>
        /// UTF8编码字符串计算SHA1值字节数组
        /// </summary>
        /// <param name="sourceStr">UTF8编码的字符串</param>
        /// <returns>SHA1(字节数组)</returns>
        public static byte[] HashToSHA1(this string sourceStr)
        {
            return HashToSHA1(sourceStr, Encoding.UTF8);
        }
        #endregion

        #region HashToSHA1Hex UTF8编码字符串计算SHA1值(十六进制编码字符串)
        /// <summary>
        /// UTF8编码字符串计算SHA1值(十六进制编码字符串)
        /// </summary>
        /// <param name="sourceStr">UTF8编码的字符串</param>
        /// <returns>SHA1(十六进制编码字符串)</returns>
        public static string HashToSHA1Hex(this string sourceStr)
        {
            return StringEncode.ByteArrayToHexString(HashToSHA1(sourceStr));
        }
        #endregion

        #region HashToSHA1Base64 UTF8编码字符串计算SHA1值(Base64编码字符串)
        /// <summary>
        /// UTF8编码字符串计算SHA1值(Base64编码字符串)
        /// </summary>
        /// <param name="sourceStr">UTF8编码的字符串</param>
        /// <returns>SHA1(Base64编码字符串)</returns>
        public static string HashToSHA1Base64(this string sourceStr)
        {
            return Convert.ToBase64String(HashToSHA1(sourceStr));
        }
        #endregion

        #endregion

        #region 系统默认编码(=GB2312?)

        #region HashDefaultToMD5 系统默认编码字符串计算MD5值(字节数组)
        /// <summary>
        /// 系统默认编码字符串计算MD5值(字节数组)
        /// </summary>
        /// <param name="sourceStr">系统默认编码的字符串</param>
        /// <returns>MD5(字节数组)</returns>
        public static byte[] HashDefaultToMD5(this string sourceStr)
        {
            return HashToMD5(sourceStr, Encoding.Default);
        }
        #endregion

        #region HashDefaultToMD5Hex 系统默认编码字符串计算MD5值(十六进制编码字符串)
        /// <summary>
        /// 系统默认编码字符串计算MD5值(十六进制编码字符串)
        /// </summary>
        /// <param name="sourceStr">系统默认编码的字符串</param>
        /// <returns>MD5(十六进制编码字符串)</returns>
        public static string HashDefaultToMD5Hex(this string sourceStr)
        {
            return StringEncode.ByteArrayToHexString(HashDefaultToMD5(sourceStr));
        }
        #endregion

        #region HashDefaultToMD5Base64 系统默认编码字符串计算MD5值(Base64编码字符串)
        /// <summary>
        /// 系统默认编码字符串计算MD5值(Base64编码字符串)
        /// </summary>
        /// <param name="sourceStr">系统默认编码的字符串</param>
        /// <returns>MD5(Base64编码字符串)</returns>
        public static string HashDefaultToMD5Base64(this string sourceStr)
        {
            return Convert.ToBase64String(HashDefaultToMD5(sourceStr));
        }
        #endregion

        #region HashDefaultToSHA1 UTF8编码字符串计算SHA1值(字节数组)
        /// <summary>
        /// 系统默认编码字符串计算SHA1值字节数组
        /// </summary>
        /// <param name="sourceStr">系统默认编码的字符串</param>
        /// <returns>SHA1(字节数组)</returns>
        public static byte[] HashDefaultToSHA1(this string sourceStr)
        {
            return HashToSHA1(sourceStr, Encoding.Default);
        }
        #endregion

        #region HashDefaultToSHA1Hex 系统默认编码字符串计算SHA1值(十六进制编码字符串)
        /// <summary>
        /// 系统默认编码字符串计算SHA1值(十六进制编码字符串)
        /// </summary>
        /// <param name="sourceStr">系统默认编码的字符串</param>
        /// <returns>SHA1(十六进制编码字符串)</returns>
        public static string HashDefaultToSHA1Hex(this string sourceStr)
        {
            return StringEncode.ByteArrayToHexString(HashDefaultToSHA1(sourceStr));
        }
        #endregion

        #region HashDefaultToSHA1Base64 系统默认编码字符串计算SHA1值(Base64编码字符串)
        /// <summary>
        /// 系统默认编码字符串计算SHA1值(Base64编码字符串)
        /// </summary>
        /// <param name="sourceStr">系统默认编码的字符串</param>
        /// <returns>SHA1(Base64编码字符串)</returns>
        public static string HashDefaultToSHA1Base64(this string sourceStr)
        {
            return Convert.ToBase64String(HashDefaultToSHA1(sourceStr));
        }
        #endregion

        #endregion

    }

}
