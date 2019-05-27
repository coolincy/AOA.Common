using System;
using System.Security.Cryptography;
using System.Text;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.Crypto
{

    /// <summary>
    /// RSA加密帮助类，支持Linux
    /// </summary>
    public class RSAHelper
    {

        #region 生成RSA密钥对
        /// <summary>
        /// 生成RSA密钥对，公钥就是私钥的Modulus
        /// </summary>
        /// <param name="dwKeySize">密钥长度，默认2048</param>
        /// <returns>RSA密钥对(公钥, 私钥)</returns>
        public static (string publicKey, string privateKey) GenRSAKeyPair(int dwKeySize = 2048)
        {
            RSA rsa = RSA.Create();
            rsa.KeySize = dwKeySize;

            RSAParameters rsaParams = rsa.ExportParameters(true);
            // 公钥	
            var publicKey = Convert.ToBase64String(rsaParams.Modulus);
            // 私钥
            var privateKey = rsaParams.ToJson();

            return (publicKey, privateKey);
        }
        #endregion

        #region 获取RSA加密服务

        /// <summary>
        /// 获取公钥RSA加密服务
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <returns></returns>
        public static RSA GetPublicRSA(string publicKey)
        {
            RSA rsa = RSA.Create();
            rsa.ImportParameters(new RSAParameters()
            {
                Modulus = Convert.FromBase64String(publicKey),
                Exponent = new byte[] { 1, 0, 1 }
            });
            return rsa;
        }

        /// <summary>
        /// 获取私钥RSA加密服务
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <returns></returns>
        public static RSA GetPrivateRSA(string privateKey)
        {
            RSA rsa = RSA.Create();
            rsa.ImportParameters(privateKey.ToJsonObject<RSAParameters>());
            return rsa;
        }

        #endregion

        #region RSA 加密

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <param name="data">需要加密的数据</param>
        /// <returns></returns>
        public static byte[] Encrypt(string publicKey, byte[] data)
        {
            try
            {
                RSA rsa = GetPublicRSA(publicKey);
                return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <param name="hexSource">需要加密的数据以16进制字符串表示</param>
        /// <returns>加的数据以16进制字符串表示</returns>
        public static string Encrypt(string publicKey, string hexSource)
        {
            try
            {
                byte[] dataToEncrypt = hexSource.HexStringToByteArray();
                byte[] dataEncrypted = Encrypt(publicKey, dataToEncrypt);
                if (dataEncrypted != null)
                    return StringEncode.ByteArrayToHexString(dataEncrypted);
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <param name="source">源字符串</param>
        /// <returns>加的数据以16进制字符串表示</returns>
        public static string EncryptString(string publicKey, string source)
        {
            try
            {
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(source);
                byte[] dataEncrypted = Encrypt(publicKey, dataToEncrypt);
                if (dataEncrypted != null)
                    return StringEncode.ByteArrayToHexString(dataEncrypted);
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region RSA 解密

        /// <summary>
        /// RSA 解密
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <param name="data">需要解密的数据</param>
        /// <returns></returns>
        public static byte[] Decrypt(string privateKey, byte[] data)
        {
            try
            {
                RSA rsa = GetPrivateRSA(privateKey);
                return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// RSA 解密(解密的数据以16进制字符串表示)
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <param name="hexSource">需要解密的数据以16进制字符串表示</param>
        /// <returns>解密的数据以16进制字符串表示</returns>
        public static string Decrypt(string privateKey, string hexSource)
        {
            try
            {
                byte[] dataToDecrypt = hexSource.HexStringToByteArray();
                byte[] dataDecrypted = Decrypt(privateKey, dataToDecrypt);
                if (dataDecrypted != null)
                    return StringEncode.ByteArrayToHexString(dataDecrypted);
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// RSA 解密(解密的数据以普通字符串表示)
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <param name="hexSource">需要解密的数据以16进制字符串表示</param>
        /// <returns>解密的字符串</returns>
        public static string DecryptString(string privateKey, string hexSource)
        {
            try
            {
                byte[] dataToDecrypt = hexSource.HexStringToByteArray();
                byte[] dataDecrypted = Decrypt(privateKey, dataToDecrypt);
                if (dataDecrypted != null)
                    return Encoding.UTF8.GetString(dataDecrypted);
                else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region RSA签名

        /// <summary>
        /// 使用私钥签名
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="hashAlgorithmName">哈希算法</param>
        /// <returns></returns>
        public static string Sign(string privateKey, byte[] data, HashAlgorithmName hashAlgorithmName)
        {
            RSA rsa = GetPrivateRSA(privateKey);
            var signatureBytes = rsa.SignData(data, hashAlgorithmName, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// 使用私钥签名
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="hashAlgorithmName">哈希算法</param>
        /// <returns></returns>
        public static string Sign(string privateKey, string data, HashAlgorithmName hashAlgorithmName)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            return Sign(privateKey, dataBytes, hashAlgorithmName);
        }

        /// <summary>
        /// 使用私钥签名(SHA256)
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="hashAlgorithmName">哈希算法</param>
        /// <returns></returns>
        public static string Sign(string privateKey, byte[] data)
        {
            RSA rsa = GetPrivateRSA(privateKey);
            var signatureBytes = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// 使用私钥签名(SHA256)
        /// </summary>
        /// <param name="privateKey">Json表示的私钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="hashAlgorithmName">哈希算法</param>
        /// <returns></returns>
        public static string Sign(string privateKey, string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            return Sign(privateKey, dataBytes);
        }

        #endregion

        #region RSA验证签名

        /// <summary>
        /// 使用公钥验证签名
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <param name="hashAlgorithmName">哈希算法</param>
        /// <returns></returns>
        public static bool Verify(string publicKey, byte[] data, string sign, HashAlgorithmName hashAlgorithmName)
        {
            RSA rsa = GetPublicRSA(publicKey);
            byte[] signBytes = Convert.FromBase64String(sign);
            return rsa.VerifyData(data, signBytes, hashAlgorithmName, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// 使用公钥验证签名
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <param name="hashAlgorithmName">哈希算法</param>
        /// <returns></returns>
        public static bool Verify(string publicKey, string data, string sign, HashAlgorithmName hashAlgorithmName)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            return Verify(publicKey, dataBytes, sign, hashAlgorithmName);
        }

        /// <summary>
        /// 使用公钥验证签名(SHA256)
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <returns></returns>
        public static bool Verify(string publicKey, byte[] data, string sign)
        {
            RSA rsa = GetPublicRSA(publicKey);
            byte[] signBytes = Convert.FromBase64String(sign);
            return rsa.VerifyData(data, signBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// 使用公钥验证签名(SHA256)
        /// </summary>
        /// <param name="publicKey">Base64表示的公钥</param>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <returns></returns>
        public static bool Verify(string publicKey, string data, string sign)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            return Verify(publicKey, dataBytes, sign);
        }

        #endregion

    }

}
