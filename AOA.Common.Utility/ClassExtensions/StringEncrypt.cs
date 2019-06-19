using AOA.Common.Utility.Crypto;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 字符串加密扩展
    /// </summary>
    public static class StringEncrypt
    {

        #region TripleDesEncryptToBase64 使用默认密钥加密后用Base64编码加密结果
        /// <summary>
        /// 使用默认密钥加密后用Base64编码加密结果
        /// </summary>
        /// <param name="sourceStr">UTF8编码的源字符串</param>
        /// <returns>Base64编码字符串</returns>
        public static string TripleDesEncryptToBase64(this string sourceStr)
        {
            return Cryptography.TripleDesEncryptToBase64(sourceStr);
        }
        #endregion

        #region TripleDesDecryptFromBase64ToString 用Base64解码加密结果后使用默认密钥解密
        /// <summary>
        /// 用Base64解码加密结果后使用默认密钥解密
        /// </summary>
        /// <param name="sourceStr">UTF8编码的源字符串</param>
        /// <returns>Base64编码字符串</returns>
        public static string TripleDesDecryptFromBase64ToString(this string sourceStr)
        {
            return Cryptography.TripleDesDecryptFromBase64ToString(sourceStr);
        }
        #endregion

    }

}
