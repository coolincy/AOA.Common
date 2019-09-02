using System.Security.Cryptography;
using System.Text;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.Crypto
{

    public static partial class Cryptography
    {

        // RSA 相关

        #region 生成RSA密钥对
        /// <summary>
        /// 生成RSA密钥对
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="dwKeySize">密钥长度</param>
        public static void GenRSAKeyPair(out string publicKey, out string privateKey, int dwKeySize = 512)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(dwKeySize))
            {
                RSAParameters rsaParams = rsa.ExportParameters(true);
                // 公钥	
                publicKey = StringEncode.ByteArrayToHexString(rsaParams.Modulus);
                // 私钥
                privateKey = rsa.ToXmlString(true);
            }
        }
        #endregion

        #region RSA 加密
        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="data">需要加密的数据</param>
        /// <param name="hexPublicKey">16进制字符串表示的公钥</param>
        /// <returns></returns>
        public static byte[] RSAEncrypt(byte[] data, string hexPublicKey)
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    byte[] exponent = { 1, 0, 1 };
                    RSAParameters rsaParamsPub = new RSAParameters()
                    {
                        Modulus = StringEncode.HexStringToByteArray(hexPublicKey),
                        Exponent = exponent,
                    };
                    rsa.ImportParameters(rsaParamsPub);
                    return rsa.Encrypt(data, false);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="hexSource">需要加密的数据以16进制字符串表示</param>
        /// <param name="hexPublicKey">16进制字符串表示的公钥</param>
        /// <returns>加的数据以16进制字符串表示</returns>
        public static string RSAEncrypt(string hexSource, string hexPublicKey)
        {
            try
            {
                byte[] dataToEncrypt = hexSource.HexStringToByteArray();
                byte[] dataEncrypted = RSAEncrypt(dataToEncrypt, hexPublicKey);
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
        /// <param name="source">源字符串</param>
        /// <param name="hexPublicKey">16进制字符串表示的公钥</param>
        /// <returns>加的数据以16进制字符串表示</returns>
        public static string RSAEncryptString(string source, string hexPublicKey)
        {
            try
            {
                byte[] dataToEncrypt = Encoding.UTF8.GetBytes(source);
                byte[] dataEncrypted = RSAEncrypt(dataToEncrypt, hexPublicKey);
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
        /// <param name="data">需要解密的数据</param>
        /// <param name="xmlPrivateKey">xml表示的私钥配置</param>
        /// <returns></returns>
        public static byte[] RSADecrypt(byte[] data, string xmlPrivateKey)
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(xmlPrivateKey);
                    RSAParameters rsaParamsPri = rsa.ExportParameters(true);
                    rsa.ImportParameters(rsaParamsPri);
                    return rsa.Decrypt(data, false);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// RSA 解密(解密的数据以16进制字符串表示)
        /// </summary>
        /// <param name="hexSource">需要解密的数据以16进制字符串表示</param>
        /// <param name="xmlPrivateKey">xml表示的私钥配置</param>
        /// <returns>解密的数据以16进制字符串表示</returns>
        public static string RSADecrypt(string hexSource, string xmlPrivateKey)
        {
            try
            {
                byte[] dataToDecrypt = hexSource.HexStringToByteArray();
                byte[] dataDecrypted = RSADecrypt(dataToDecrypt, xmlPrivateKey);
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
        /// <param name="hexSource">需要解密的数据以16进制字符串表示</param>
        /// <param name="xmlPrivateKey">xml表示的私钥配置</param>
        /// <returns>解密的字符串</returns>
        public static string RSADecryptString(string hexSource, string xmlPrivateKey)
        {
            try
            {
                byte[] dataToDecrypt = hexSource.HexStringToByteArray();
                byte[] dataDecrypted = RSADecrypt(dataToDecrypt, xmlPrivateKey);
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

    }

}
