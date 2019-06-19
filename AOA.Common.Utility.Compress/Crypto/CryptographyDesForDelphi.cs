using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using AOA.Common.Utility.ClassExtensions;
using AOA.Common.Utility.Compress;

namespace AOA.Common.Utility.Crypto
{

    /// <summary>
    /// Delphi 使用的 Des 算法
    /// </summary>
    public static class CryptographyDesForDelphi
    {

        // 默认Des密钥
        private static byte[] DefaultDesKey = { 0xF0, 0xE1, 0xD2, 0xC3, 0xB4, 0xA5, 0x96, 0x87 };
        // 默认Des向量，全部为空，为了与Delphi加密兼容
        private static byte[] DefaultDesIV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        // DES 相关

        #region DelphiDesEncrypt Des加密 (用于与Delphi的加密通信)

        // 1. 基本加密 byte[] = byte[]

        #region public static byte[] DelphiDesEncrypt(string hexStringKey, string hexStringIV, byte[] encryptSource)
        /// <summary>
        /// Des加密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="encryptSource">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] DelphiDesEncrypt(string hexStringKey, string hexStringIV, byte[] encryptSource)
        {
            byte[] Key = hexStringKey.HexStringToByteArray();
            byte[] IV = hexStringIV.HexStringToByteArray();

            DESCryptoServiceProvider dsp = new DESCryptoServiceProvider();
            dsp.Mode = CipherMode.ECB; // 默认值 CBC
            dsp.Padding = PaddingMode.ANSIX923; // 默认值 PKCS7

            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dsp.CreateEncryptor(Key, IV), CryptoStreamMode.Write);
            cStream.Write(encryptSource, 0, encryptSource.Length);
            cStream.FlushFinalBlock();
            byte[] result = mStream.ToArray();

            cStream.Close();
            mStream.Close();
            return result;
        }
        #endregion

        // 2. 自带向量 string = ?

        #region public static byte[] DelphiDesEncrypt(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// Des加密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] DelphiDesEncrypt(string hexStringKey, string hexStringIV, string source)
        {
            byte[] encryptSource = Encoding.UTF8.GetBytes(source);
            return DelphiDesEncrypt(hexStringKey, hexStringIV, encryptSource);
        }
        #endregion

        #region public static string DelphiDesEncryptToBase64(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// Des加密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string DelphiDesEncryptToBase64(string hexStringKey, string hexStringIV, string source)
        {
            byte[] result = DelphiDesEncrypt(hexStringKey, hexStringIV, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string DelphiDesEncryptToHex(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// Des加密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string DelphiDesEncryptToHex(string hexStringKey, string hexStringIV, string source)
        {
            byte[] result = DelphiDesEncrypt(hexStringKey, hexStringIV, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 3. 默认向量 string = ?

        #region public static byte[] DelphiDesEncrypt(string hexStringKey, string source)
        /// <summary>
        /// Des加密(使用默认向量)，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] DelphiDesEncrypt(string hexStringKey, string source)
        {
            string hexStringIV = StringEncode.ByteArrayToHexString(DefaultDesIV);
            return DelphiDesEncrypt(hexStringKey, hexStringIV, source);
        }
        #endregion

        #region public static string DelphiDesEncryptToBase64(string hexStringKey, string source)
        /// <summary>
        /// Des加密(使用默认向量)，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string DelphiDesEncryptToBase64(string hexStringKey, string source)
        {
            byte[] result = DelphiDesEncrypt(hexStringKey, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string DelphiDesEncryptToHex(string hexStringKey, string source)
        /// <summary>
        /// Des加密(使用默认向量)，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string DelphiDesEncryptToHex(string hexStringKey, string source)
        {
            byte[] result = DelphiDesEncrypt(hexStringKey, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        #endregion

        #region DelphiDesDecrypt Des解密 (用于与Delphi的加密通信)

        // 1. 基本解密 ? = byte[]

        #region public static byte[] DelphiDesDecryptToArray(string hexStringKey, string hexStringIV, byte[] encryptSource)
        /// <summary>
        /// Des解密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] DelphiDesDecryptToArray(string hexStringKey, string hexStringIV, byte[] encryptSource)
        {
            byte[] Key = hexStringKey.HexStringToByteArray();
            byte[] IV = hexStringIV.HexStringToByteArray();

            DESCryptoServiceProvider dsp = new DESCryptoServiceProvider();
            dsp.Mode = CipherMode.ECB; // 默认值 CBC
            dsp.Padding = PaddingMode.ANSIX923; // 默认值 PKCS7

            MemoryStream mStream = new MemoryStream(encryptSource);
            CryptoStream cStream = new CryptoStream(mStream, dsp.CreateDecryptor(Key, IV), CryptoStreamMode.Read);
            byte[] result = new byte[encryptSource.Length];
            cStream.Read(result, 0, result.Length);

            cStream.Close();
            mStream.Close();
            return result;
        }
        #endregion

        #region public static byte[] DelphiDesDecryptToArrayFromBase64(string hexStringKey, string hexStringIV, string base64Source)
        /// <summary>
        /// Des解密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="base64Source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] DelphiDesDecryptToArrayFromBase64(string hexStringKey, string hexStringIV, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return DelphiDesDecryptToArray(hexStringKey, hexStringIV, encryptSource);
        }
        #endregion

        // 2. 自带向量 ? = string

        #region public static string DelphiDesDecrypt(string hexStringKey, string hexStringIV, byte[] encryptSource)
        /// <summary>
        /// Des解密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string DelphiDesDecrypt(string hexStringKey, string hexStringIV, byte[] encryptSource)
        {
            return Encoding.UTF8.GetString(DelphiDesDecryptToArray(hexStringKey, hexStringIV, encryptSource)).TrimEnd('\0');
        }
        #endregion

        #region public static string DelphiDesDecryptFromBase64(string hexStringKey, string hexStringIV, string base64Source)
        /// <summary>
        /// Des解密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="base64Source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string DelphiDesDecryptFromBase64(string hexStringKey, string hexStringIV, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return DelphiDesDecrypt(hexStringKey, hexStringIV, encryptSource);
        }
        #endregion

        #region public static string DelphiDesDecryptFromHex(string hexStringKey, string hexStringIV, string hexSource)
        /// <summary>
        /// Des解密，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="hexSource">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string DelphiDesDecryptFromHex(string hexStringKey, string hexStringIV, string hexSource)
        {
            byte[] encryptSource = hexSource.HexStringToByteArray();
            return DelphiDesDecrypt(hexStringKey, hexStringIV, encryptSource);
        }
        #endregion

        // 3. 默认向量 ? = string

        #region public static string DelphiDesDecrypt(string hexStringKey, byte[] encryptSource)
        /// <summary>
        /// Des解密(使用默认向量)，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string DelphiDesDecrypt(string hexStringKey, byte[] encryptSource)
        {
            string hexStringIV = StringEncode.ByteArrayToHexString(DefaultDesIV);
            return DelphiDesDecrypt(hexStringKey, hexStringIV, encryptSource);
        }
        #endregion

        #region public static string DelphiDesDecryptFromBase64(string hexStringKey, string base64Source)
        /// <summary>
        /// Des解密(使用默认向量)，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="base64Source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string DelphiDesDecryptFromBase64(string hexStringKey, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return DelphiDesDecrypt(hexStringKey, encryptSource);
        }
        #endregion

        #region public static string DelphiDesDecryptFromHex(string hexStringKey, string hexSource)
        /// <summary>
        /// Des解密(使用默认向量)，密钥长度必需是8字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexSource">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string DelphiDesDecryptFromHex(string hexStringKey, string hexSource)
        {
            byte[] encryptSource = hexSource.HexStringToByteArray();
            return DelphiDesDecrypt(hexStringKey, encryptSource);
        }
        #endregion

        #endregion

        #region ZipAndDelphiDes 压缩DES加密 (用于与Delphi的加密通信)

        // byte[] = byte[]
        /// <summary>
        /// 压缩后DES加密
        /// </summary>
        /// <param name="hexStringKey"></param>
        /// <param name="byteSource"></param>
        /// <returns></returns>
        public static byte[] ZipAndDelphiDes(string hexStringKey, byte[] byteSource)
        {
            string hexStringIV = StringEncode.ByteArrayToHexString(DefaultDesIV);
            byte[] zipResult = ZipCompress.Compress(byteSource);
            return DelphiDesEncrypt(hexStringKey, hexStringIV, zipResult);
        }

        // string = byte[]
        /// <summary>
        /// 压缩后DES加密
        /// </summary>
        /// <param name="hexStringKey"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] ZipAndDelphiDesToArray(string hexStringKey, string source)
        {
            byte[] byteSource = Encoding.UTF8.GetBytes(source);
            return ZipAndDelphiDes(hexStringKey, byteSource);
        }

        // string = base64
        /// <summary>
        /// 压缩后DES加密
        /// </summary>
        /// <param name="hexStringKey"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ZipAndDelphiDesToBase64(string hexStringKey, string source)
        {
            return Convert.ToBase64String(ZipAndDelphiDesToArray(hexStringKey, source));
        }

        #endregion

        #region UnDelphiDesAndUnZip DES解密解压 (用于与Delphi的加密通信)

        /// <summary>
        /// DES解密后解压
        /// </summary>
        /// <param name="hexStringKey"></param>
        /// <param name="encryptSource"></param>
        /// <returns></returns>
        public static byte[] UnDelphiDesAndUnZip(string hexStringKey, byte[] encryptSource)
        {
            string hexStringIV = StringEncode.ByteArrayToHexString(DefaultDesIV);
            byte[] desResult = DelphiDesDecryptToArray(hexStringKey, hexStringIV, encryptSource);
            return ZipCompress.DeCompress(desResult);
        }

        /// <summary>
        /// DES解密后解压
        /// </summary>
        /// <param name="hexStringKey"></param>
        /// <param name="base64Source"></param>
        /// <returns></returns>
        public static byte[] UnDelphiDesAndUnZipToArrayFromBase64(string hexStringKey, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return UnDelphiDesAndUnZip(hexStringKey, encryptSource);
        }

        /// <summary>
        /// DES解密后解压
        /// </summary>
        /// <param name="hexStringKey"></param>
        /// <param name="base64Source"></param>
        /// <returns></returns>
        public static string UnDelphiDesAndUnZipFromBase64(string hexStringKey, string base64Source)
        {
            return Encoding.UTF8.GetString(UnDelphiDesAndUnZipToArrayFromBase64(hexStringKey, base64Source)).TrimEnd('\0');
        }

        #endregion

    }

}
