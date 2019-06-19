using System;
using System.Text;

using AOA.Common.Utility.Compress;

namespace AOA.Common.Utility.Crypto
{

    /// <summary>
    /// 加密解密工具类
    /// </summary>
    public static class CryptographyTripleDesCompress
    {

        #region Zip压缩后3DES加密

        // byte[] = byte[]

        /// <summary>
        /// Zip压缩后3DES加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ZipAnd3Des(string hexStringKey, byte[] source)
        {
            byte[] zipResult = ZipCompress.Compress(source);
            return Cryptography.TripleDesEncrypt(hexStringKey, zipResult);
        }

        // string = byte[]

        /// <summary>
        /// Zip压缩后3DES加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ZipAnd3DesToArray(string hexStringKey, string source)
        {
            byte[] byteSource = Encoding.UTF8.GetBytes(source);
            return ZipAnd3Des(hexStringKey, byteSource);
        }

        // string = base64

        /// <summary>
        /// Zip压缩后3DES加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ZipAnd3DesToBase64(string hexStringKey, string source)
        {
            return Convert.ToBase64String(ZipAnd3DesToArray(hexStringKey, source));
        }

        #endregion

        #region 3DES解密后Zip解压缩

        // byte[] = byte[]

        /// <summary>
        /// 3DES解密后Zip解压缩，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="encryptSource"></param>
        /// <returns></returns>
        public static byte[] Un3DesAndUnZip(string hexStringKey, byte[] encryptSource)
        {
            byte[] desResult = Cryptography.TripleDesDecrypt(hexStringKey, encryptSource);
            return ZipCompress.DeCompress(desResult);
        }

        // byte[] = string

        /// <summary>
        /// 3DES解密后Zip解压缩，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="base64Source"></param>
        /// <returns></returns>
        public static byte[] Un3DesAndUnZipToArrayFromBase64(string hexStringKey, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return Un3DesAndUnZip(hexStringKey, encryptSource);
        }

        // base64 = string

        /// <summary>
        /// 3DES解密后Zip解压缩，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="base64Source"></param>
        /// <returns></returns>
        public static string Un3DesAndUnZipFromBase64(string hexStringKey, string base64Source)
        {
            return Encoding.UTF8.GetString(Un3DesAndUnZipToArrayFromBase64(hexStringKey, base64Source)).TrimEnd('\0');
        }

        #endregion

        #region GZip压缩后3DES加密

        // byte[] = byte[]

        /// <summary>
        /// GZip压缩后3DES加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] GZipAnd3Des(string hexStringKey, byte[] source)
        {
            byte[] gZipResult = GZipCompress.Compress(source);
            return Cryptography.TripleDesEncrypt(hexStringKey, gZipResult);
        }

        // string = byte[]

        /// <summary>
        /// GZip压缩后3DES加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] GZipAnd3DesToArray(string hexStringKey, string source)
        {
            byte[] byteSource = Encoding.UTF8.GetBytes(source);
            return GZipAnd3Des(hexStringKey, byteSource);
        }

        // string = base64

        /// <summary>
        /// GZip压缩后3DES加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GZipAnd3DesToBase64(string hexStringKey, string source)
        {
            return Convert.ToBase64String(GZipAnd3DesToArray(hexStringKey, source));
        }

        #endregion

        #region 3DES解密后GZip解压缩

        // byte[] = byte[]

        /// <summary>
        /// 3DES解密后GZip解压缩，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="encryptSource"></param>
        /// <returns></returns>
        public static byte[] Un3DesAndUnGZip(string hexStringKey, byte[] encryptSource)
        {
            byte[] desResult = Cryptography.TripleDesDecrypt(hexStringKey, encryptSource);
            return GZipCompress.DeCompress(desResult);
        }

        // byte[] = string

        /// <summary>
        /// 3DES解密后GZip解压缩，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="base64Source"></param>
        /// <returns></returns>
        public static byte[] Un3DesAndUnGZipToArrayFromBase64(string hexStringKey, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return Un3DesAndUnGZip(hexStringKey, encryptSource);
        }

        // base64 = string

        /// <summary>
        /// 3DES解密后GZip解压缩，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">密钥串</param>
        /// <param name="base64Source"></param>
        /// <returns></returns>
        public static string Un3DesAndUnGZipFromBase64(string hexStringKey, string base64Source)
        {
            return Encoding.UTF8.GetString(Un3DesAndUnGZipToArrayFromBase64(hexStringKey, base64Source)).TrimEnd('\0');
        }

        #endregion

    }
}
