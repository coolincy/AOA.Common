using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.Crypto
{

    /// <summary>
    /// 加密解密工具类
    /// </summary>
    public static partial class Cryptography
    {

        // 默认3Des密钥
        private static byte[] Default3DesKey = { 0xF0, 0xE1, 0xD2, 0xC3, 0xB4, 0xA5, 0x96, 0x87, 0x78, 0x69, 0x5A, 0x4B, 0x3C, 0x2D, 0x1E, 0x0F, 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };
        // 默认3Des密钥(加密密码使用)
        private static byte[] Default3DesKeyForPassword = { 0x0F, 0x1E, 0x2D, 0x3C, 0x4B, 0x5A, 0x69, 0x78, 0x87, 0x96, 0xA5, 0xB4, 0xC3, 0xD2, 0xE1, 0xF0, 0x10, 0x32, 0x54, 0x76, 0x98, 0xBA, 0xDC, 0xFE };
        // 默认3Des向量
        private static byte[] Default3DesIV = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// 静态构建函数
        /// </summary>
        static Cryptography()
        {
            // 读取配置的加密3DES密钥
            string default3DesKey = ConfigReader.GetString("Default3DesKey", string.Empty);
            if (!string.IsNullOrEmpty(default3DesKey))
                Default3DesKey = default3DesKey.HexStringToByteArray();

            // 读取配置的密码加密3DES密钥
            string default3DesKeyForPassword = ConfigReader.GetString("Default3DesKeyForPassword", string.Empty);
            if (!string.IsNullOrEmpty(default3DesKeyForPassword))
                Default3DesKeyForPassword = default3DesKeyForPassword.HexStringToByteArray();
        }

        // 3DES 相关

        #region 生成随机3DES密钥

        #region public static byte[] GenerateByteArray3DESKey()
        /// <summary>
        /// 生成随机3DES密钥
        /// </summary>
        /// <returns>字节数组表示的密钥</returns>
        public static byte[] GenerateByteArray3DESKey()
        {
            byte[] buf = null;
            TripleDESCryptoServiceProvider dsp = new TripleDESCryptoServiceProvider();
            dsp.GenerateKey();
            buf = dsp.Key;
            return buf;
        }
        #endregion

        #region public static string GenerateHexString3DESKey()
        /// <summary>
        /// 生成随机3DES密钥
        /// </summary>
        /// <returns>十六进制字符串表示的密钥</returns>
        public static string GenerateHexString3DESKey()
        {
            byte[] byteArray = GenerateByteArray3DESKey();
            return StringEncode.ByteArrayToHexString(byteArray);
        }
        #endregion

        #endregion

        #region 生成随机3DES加密向量

        #region public static byte[] GenerateByteArray3DESIV()
        /// <summary>
        /// 生成随机3DES加密向量
        /// </summary>
        /// <returns>字节数组表示的加密向量</returns>
        public static byte[] GenerateByteArray3DESIV()
        {
            byte[] buf = null;
            TripleDESCryptoServiceProvider dsp = new TripleDESCryptoServiceProvider();
            dsp.GenerateIV();
            buf = dsp.IV;
            return buf;
        }
        #endregion

        #region public static string GenerateHexString3DESIV()
        /// <summary>
        /// 生成随机3DES加密向量
        /// </summary>
        /// <returns>十六进制字符串表示的加密向量</returns>
        public static string GenerateHexString3DESIV()
        {
            byte[] byteArray = GenerateByteArray3DESIV();
            return StringEncode.ByteArrayToHexString(byteArray);
        }
        #endregion

        #region private static byte[] GetDefaultIVFromKey(byte[] key)
        /// <summary>
        /// 取密钥前8字节作为向量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] GetDefaultIVFromKey(byte[] key)
        {
            byte[] iv = new byte[8];
            if (key.Length < 8)
                iv = Default3DesIV;
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    //iv[i] = key[key.Length - 8 + i]; // 后8位
                    iv[i] = key[i]; // 前8位
                }
            }
            return iv;
        }
        #endregion

        #endregion

        #region TripleDesEncrypt 3Des加密

        // 1. 自带向量 byte[] = ?

        #region public static byte[] TripleDesEncrypt(byte[] key, byte[] iv, byte[] source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(byte[] key, byte[] iv, byte[] source)
        {
            TripleDESCryptoServiceProvider dsp = new TripleDESCryptoServiceProvider();
            dsp.Mode = CipherMode.CBC; // 默认值
            dsp.Padding = PaddingMode.PKCS7; // 默认值
            using (MemoryStream mStream = new MemoryStream())
            {
                CryptoStream cStream = new CryptoStream(mStream, dsp.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                cStream.Write(source, 0, source.Length);
                cStream.FlushFinalBlock();
                byte[] result = mStream.ToArray();
                cStream.Close();
                mStream.Close();
                return result;
            }
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(byte[] key, byte[] iv, byte[] source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(byte[] key, byte[] iv, byte[] source)
        {
            byte[] result = TripleDesEncrypt(key, iv, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(byte[] key, byte[] iv, byte[] source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(byte[] key, byte[] iv, byte[] source)
        {
            byte[] result = TripleDesEncrypt(key, iv, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion


        #region public static byte[] TripleDesEncrypt(string hexStringKey, string hexStringIV, byte[] source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(string hexStringKey, string hexStringIV, byte[] source)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            byte[] iv = hexStringIV.HexStringToByteArray();
            return TripleDesEncrypt(key, iv, source);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(string hexStringKey, string hexStringIV, byte[] source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(string hexStringKey, string hexStringIV, byte[] source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, hexStringIV, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(string hexStringKey, string hexStringIV, byte[] source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(string hexStringKey, string hexStringIV, byte[] source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, hexStringIV, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 2. 自带向量 string = ?

        #region public static byte[] TripleDesEncrypt(byte[] key, byte[] iv, string source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(byte[] key, byte[] iv, string source)
        {
            byte[] encryptSource = Encoding.UTF8.GetBytes(source);
            return TripleDesEncrypt(key, iv, encryptSource);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(byte[] key, byte[] iv, string source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(byte[] key, byte[] iv, string source)
        {
            byte[] result = TripleDesEncrypt(key, iv, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(byte[] key, byte[] iv, string source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(byte[] key, byte[] iv, string source)
        {
            byte[] result = TripleDesEncrypt(key, iv, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion


        #region public static byte[] TripleDesEncrypt(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(string hexStringKey, string hexStringIV, string source)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            byte[] iv = hexStringIV.HexStringToByteArray();
            return TripleDesEncrypt(key, iv, source);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(string hexStringKey, string hexStringIV, string source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, hexStringIV, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// 3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(string hexStringKey, string hexStringIV, string source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, hexStringIV, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 3. 密钥后8字节作为向量 byte[] = ?

        #region public static byte[] TripleDesEncrypt(byte[] key, byte[] source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(byte[] key, byte[] source)
        {
            byte[] iv = GetDefaultIVFromKey(key);
            return TripleDesEncrypt(key, iv, source);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(byte[] key, byte[] source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(byte[] key, byte[] source)
        {
            byte[] result = TripleDesEncrypt(key, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(byte[] key, byte[] source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(byte[] key, byte[] source)
        {
            byte[] result = TripleDesEncrypt(key, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion


        #region public static byte[] TripleDesEncrypt(string hexStringKey, byte[] source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(string hexStringKey, byte[] source)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            return TripleDesEncrypt(key, source);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(string hexStringKey, byte[] source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(string hexStringKey, byte[] source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(string hexStringKey, byte[] source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(string hexStringKey, byte[] source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 4. 密钥后8字节作为向量 string = ?

        #region public static byte[] TripleDesEncrypt(byte[] key, string source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(byte[] key, string source)
        {
            byte[] encryptSource = Encoding.UTF8.GetBytes(source);
            return TripleDesEncrypt(key, encryptSource);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(byte[] key, string source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(byte[] key, string source)
        {
            byte[] result = TripleDesEncrypt(key, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(byte[] key, string source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(byte[] key, string source)
        {
            byte[] result = TripleDesEncrypt(key, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion


        #region public static byte[] TripleDesEncrypt(string hexStringKey, string source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(string hexStringKey, string source)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            return TripleDesEncrypt(key, source);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(string hexStringKey, string source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(string hexStringKey, string source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(string hexStringKey, string source)
        /// <summary>
        /// 3Des加密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(string hexStringKey, string source)
        {
            byte[] result = TripleDesEncrypt(hexStringKey, source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 5. 默认密钥，默认向量 byte[] = ?

        #region public static byte[] TripleDesEncrypt(byte[] source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(byte[] source)
        {
            byte[] iv = GetDefaultIVFromKey(Default3DesKey);
            return TripleDesEncrypt(Default3DesKey, iv, source);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(byte[] source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(byte[] source)
        {
            byte[] result = TripleDesEncrypt(source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(byte[] source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(byte[] source)
        {
            byte[] result = TripleDesEncrypt(source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 6. 默认密钥，默认向量 string = ?

        #region public static byte[] TripleDesEncrypt(string source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncrypt(string source)
        {
            byte[] encryptSource = Encoding.UTF8.GetBytes(source);
            return TripleDesEncrypt(encryptSource);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(string source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(string source)
        {
            byte[] result = TripleDesEncrypt(source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(string source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptToHex(string source)
        {
            byte[] result = TripleDesEncrypt(source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 7. 默认密钥，默认向量 byte[] = ?

        #region public static byte[] TripleDesEncrypt(byte[] source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncryptForPassword(byte[] source)
        {
            byte[] iv = GetDefaultIVFromKey(Default3DesKeyForPassword);
            return TripleDesEncrypt(Default3DesKeyForPassword, iv, source);
        }
        #endregion

        #region public static string TripleDesEncryptToBase64(byte[] source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptForPasswordToBase64(byte[] source)
        {
            byte[] result = TripleDesEncryptForPassword(source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptToHex(byte[] source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptForPasswordToHex(byte[] source)
        {
            byte[] result = TripleDesEncryptForPassword(source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        // 8. 默认密钥，默认向量 string = ?

        #region public static byte[] TripleDesEncryptForPassword(string source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] TripleDesEncryptForPassword(string source)
        {
            byte[] encryptSource = Encoding.UTF8.GetBytes(source);
            return TripleDesEncryptForPassword(encryptSource);
        }
        #endregion

        #region public static string TripleDesEncryptForPasswordToBase64(string source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的Base64编码字符串</returns>
        public static string TripleDesEncryptForPasswordToBase64(string source)
        {
            byte[] result = TripleDesEncryptForPassword(source);
            return Convert.ToBase64String(result);
        }
        #endregion

        #region public static string TripleDesEncryptForPasswordToHex(string source)
        /// <summary>
        /// 3Des加密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的十六进制字符串</returns>
        public static string TripleDesEncryptForPasswordToHex(string source)
        {
            byte[] result = TripleDesEncryptForPassword(source);
            return StringEncode.ByteArrayToHexString(result);
        }
        #endregion

        #endregion

        #region TripleDesDecrypt 3Des解密

        // 1. 自带向量 ? = byte[]

        #region public static byte[] TripleDesDecrypt(byte[] key, byte[] iv, byte[] source, out int dataLen))
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">加密后的字节数组</param>
        /// <param name="dataLen">解密后的数据长度</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecrypt(byte[] key, byte[] iv, byte[] source, out int dataLen)
        {
            TripleDESCryptoServiceProvider dsp = new TripleDESCryptoServiceProvider();
            dsp.Mode = CipherMode.CBC; // 默认值
            dsp.Padding = PaddingMode.PKCS7; // 默认值
            using (MemoryStream mStream = new MemoryStream(source))
            {
                CryptoStream cStream = new CryptoStream(mStream, dsp.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                byte[] result = new byte[source.Length];
                //byte[] result = new byte[cStream.Length]; // cStream.Length 不可读取
                dataLen = cStream.Read(result, 0, result.Length);
                cStream.Close();
                mStream.Close();
                return result;
            }
        }
        #endregion

        #region public static byte[] TripleDesDecrypt(byte[] key, byte[] iv, byte[] source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecrypt(byte[] key, byte[] iv, byte[] source)
        {
            int dataLen = 0;

            byte[] result = TripleDesDecrypt(key, iv, source, out dataLen);

            if (result.Length != dataLen)
            {
                // 如果数组长度不是解密后的实际长度，需要截断多余的数据，用来解决Gzip的"Magic byte doesn't match"的问题
                byte[] resultToReturn = new byte[dataLen];
                Array.Copy(result, resultToReturn, dataLen);
                return resultToReturn;
            }
            else
                return result;
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromBase64(byte[] key, byte[] iv, string base64Source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="base64Source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromBase64(byte[] key, byte[] iv, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return TripleDesDecrypt(key, iv, encryptSource);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromHex(byte[] key, byte[] iv, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromHex(byte[] key, byte[] iv, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecrypt(key, iv, encryptSource);
        }
        #endregion


        #region public static byte[] TripleDesDecrypt(string hexStringKey, string hexStringIV, byte[] source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecrypt(string hexStringKey, string hexStringIV, byte[] source)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            byte[] iv = hexStringIV.HexStringToByteArray();
            return TripleDesDecrypt(key, iv, source);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromBase64(string hexStringKey, string hexStringIV, string base64Source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="base64Source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromBase64(string hexStringKey, string hexStringIV, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            byte[] key = hexStringKey.HexStringToByteArray();
            byte[] iv = hexStringIV.HexStringToByteArray();
            return TripleDesDecrypt(key, iv, encryptSource);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromHex(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromHex(string hexStringKey, string hexStringIV, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            byte[] key = hexStringKey.HexStringToByteArray();
            byte[] iv = hexStringIV.HexStringToByteArray();
            return TripleDesDecrypt(key, iv, encryptSource);
        }
        #endregion

        // 2. 自带向量 ? = string

        #region public static string TripleDesDecryptToString(byte[] key, byte[] iv, byte[] encryptSource)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptToString(byte[] key, byte[] iv, byte[] encryptSource)
        {
            int dataLen;
            byte[] result = TripleDesDecrypt(key, iv, encryptSource, out dataLen);
            return Encoding.UTF8.GetString(result, 0, dataLen).TrimEnd('\0');
        }
        #endregion

        #region public static string TripleDesDecryptFromBase64ToString(byte[] key, byte[] iv, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromBase64ToString(byte[] key, byte[] iv, string source)
        {
            byte[] encryptSource = Convert.FromBase64String(source);
            return TripleDesDecryptToString(key, iv, encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptFromHexToString(byte[] key, byte[] iv, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromHexToString(byte[] key, byte[] iv, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecryptToString(key, iv, encryptSource);
        }
        #endregion


        #region public static string TripleDesDecryptToString(string hexStringKey, string hexStringIV, byte[] encryptSource)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptToString(string hexStringKey, string hexStringIV, byte[] encryptSource)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            byte[] iv = hexStringIV.HexStringToByteArray();
            return TripleDesDecryptToString(key, iv, encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptFromBase64ToString(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromBase64ToString(string hexStringKey, string hexStringIV, string source)
        {
            byte[] encryptSource = Convert.FromBase64String(source);
            return TripleDesDecryptToString(hexStringKey, hexStringIV, encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptFromHexToString(string hexStringKey, string hexStringIV, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="hexStringIV">十六进制向量字符串</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromHexToString(string hexStringKey, string hexStringIV, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecryptToString(hexStringKey, hexStringIV, encryptSource);
        }
        #endregion

        // 3. 密钥后8字节作为向量 ? = byte[]

        #region public static byte[] TripleDesDecrypt(byte[] key, byte[] source)
        /// <summary>
        /// 3Des解密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecrypt(byte[] key, byte[] source)
        {
            byte[] iv = GetDefaultIVFromKey(key);
            return TripleDesDecrypt(key, iv, source);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromBase64(byte[] key, string base64Source)
        /// <summary>
        /// 3Des解密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="base64Source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromBase64(byte[] key, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return TripleDesDecrypt(key, encryptSource);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromHex(byte[] key, string source)
        /// <summary>
        /// 3Des解密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromHex(byte[] key, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecrypt(key, encryptSource);
        }
        #endregion


        #region public static byte[] TripleDesDecrypt(string hexStringKey, byte[] source)
        /// <summary>
        /// 3Des解密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecrypt(string hexStringKey, byte[] source)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            return TripleDesDecrypt(key, source);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromBase64(string hexStringKey, string base64Source)
        /// <summary>
        /// 3Des解密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="base64Source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromBase64(string hexStringKey, string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            byte[] key = hexStringKey.HexStringToByteArray();
            return TripleDesDecrypt(key, encryptSource);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromHex(string hexStringKey, string source)
        /// <summary>
        /// 3Des解密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromHex(string hexStringKey, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            byte[] key = hexStringKey.HexStringToByteArray();
            return TripleDesDecrypt(key, encryptSource);
        }
        #endregion

        // 4. 密钥后8字节作为向量 ? = string

        #region public static string TripleDesDecryptToString(byte[] key, byte[] encryptSource)
        /// <summary>
        /// 3Des解密(默认向量)，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptToString(byte[] key, byte[] encryptSource)
        {
            byte[] result = TripleDesDecrypt(key, encryptSource);
            return Encoding.UTF8.GetString(result).TrimEnd('\0');
        }
        #endregion

        #region public static string TripleDesDecryptFromBase64ToString(byte[] key, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromBase64ToString(byte[] key, string source)
        {
            byte[] encryptSource = Convert.FromBase64String(source);
            return TripleDesDecryptToString(key, encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptFromHexToString(byte[] key, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromHexToString(byte[] key, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecryptToString(key, encryptSource);
        }
        #endregion


        #region public static string TripleDesDecryptToString(string hexStringKey, byte[] encryptSource)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptToString(string hexStringKey, byte[] encryptSource)
        {
            byte[] key = hexStringKey.HexStringToByteArray();
            return TripleDesDecryptToString(key, encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptFromBase64ToString(string hexStringKey, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromBase64ToString(string hexStringKey, string source)
        {
            byte[] encryptSource = Convert.FromBase64String(source);
            return TripleDesDecryptToString(hexStringKey, encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptFromHexToString(string hexStringKey, string source)
        /// <summary>
        /// 3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="hexStringKey">十六进制密钥字符串</param>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromHexToString(string hexStringKey, string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecryptToString(hexStringKey, encryptSource);
        }
        #endregion

        // 5. 默认密钥，默认向量 ? = byte[]

        #region public static byte[] TripleDesDecrypt(byte[] source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecrypt(byte[] source)
        {
            byte[] iv = GetDefaultIVFromKey(Default3DesKey);
            return TripleDesDecrypt(Default3DesKey, iv, source);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromBase64(string base64Source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="base64Source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromBase64(string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return TripleDesDecrypt(encryptSource);
        }
        #endregion

        #region public static byte[] TripleDesDecryptFromHex(string source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptFromHex(string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecrypt(encryptSource);
        }
        #endregion

        // 6. 默认密钥，默认向量 ? = string

        #region public static string TripleDesDecryptToString(byte[] encryptSource)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptToString(byte[] encryptSource)
        {
            byte[] result = TripleDesDecrypt(encryptSource);
            return Encoding.UTF8.GetString(result).TrimEnd('\0');
        }
        #endregion

        #region public static string TripleDesDecryptFromBase64ToString(string source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromBase64ToString(string source)
        {
            byte[] encryptSource = Convert.FromBase64String(source);
            return TripleDesDecryptToString(encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptFromHexToString(string source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptFromHexToString(string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecryptToString(encryptSource);
        }
        #endregion

        // 7. 默认密钥，默认向量 ? = byte[]

        #region public static byte[] TripleDesDecryptForPassword(byte[] source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptForPassword(byte[] source)
        {
            byte[] iv = GetDefaultIVFromKey(Default3DesKeyForPassword);
            return TripleDesDecrypt(Default3DesKeyForPassword, iv, source);
        }
        #endregion

        #region public static byte[] TripleDesDecryptForPasswordFromBase64(string base64Source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="base64Source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptForPasswordFromBase64(string base64Source)
        {
            byte[] encryptSource = Convert.FromBase64String(base64Source);
            return TripleDesDecryptForPassword(encryptSource);
        }
        #endregion

        #region public static byte[] TripleDesDecryptForPasswordFromHex(string source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>解密后的字节数组</returns>
        public static byte[] TripleDesDecryptForPasswordFromHex(string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecryptForPassword(encryptSource);
        }
        #endregion

        // 8. 默认密钥，默认向量 ? = string

        #region public static string TripleDesDecryptForPasswordToString(byte[] encryptSource)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="encryptSource">加密后的字节数组</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptForPasswordToString(byte[] encryptSource)
        {
            byte[] result = TripleDesDecryptForPassword(encryptSource);
            return Encoding.UTF8.GetString(result).TrimEnd('\0');
        }
        #endregion

        #region public static string TripleDesDecryptForPasswordFromBase64ToString(string source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的Base64编码字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptForPasswordFromBase64ToString(string source)
        {
            byte[] encryptSource = Convert.FromBase64String(source);
            return TripleDesDecryptToString(encryptSource);
        }
        #endregion

        #region public static string TripleDesDecryptForPasswordFromHexToString(string source)
        /// <summary>
        /// 3Des解密(默认密钥，默认向量)
        /// </summary>
        /// <param name="source">加密后的十六进制字符串</param>
        /// <returns>源字符串</returns>
        public static string TripleDesDecryptForPasswordFromHexToString(string source)
        {
            byte[] encryptSource = source.HexStringToByteArray();
            return TripleDesDecryptToString(encryptSource);
        }
        #endregion

        #endregion

    }

}
