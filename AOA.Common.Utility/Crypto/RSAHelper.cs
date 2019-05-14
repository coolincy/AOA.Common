using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
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
        /// 生成RSA密钥对
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <param name="dwKeySize"></param>
        public static void GenRSAKeyPair(out string publicKey, out string privateKey, int dwKeySize = 512)
        {
            RSA rsa = RSA.Create();
            rsa.KeySize = dwKeySize;

            RSAParameters rsaParams = rsa.ExportParameters(true);
            // 公钥	
            publicKey = StringEncode.ByteArrayToHexString(rsaParams.Modulus);
            // 私钥
            privateKey = rsa.ToXmlString(true);
        }
        #endregion

    }

}
