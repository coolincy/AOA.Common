using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace AOA.Common.Utility
{

    /// <summary>
    /// 加密帮助类，来自 net91com.Core.dll
    /// </summary>
    public class Net91ComCryptoHelper
    {

        /// <summary>
        /// DES加解密的默认密钥
        /// </summary>
        public static string Key
        {
            get
            {
                return "!@#ASD12";
            }
        }

        #region 使用Get传输替换关键字符为全角和半角转换
        /// <summary>
        /// 使用Get传输替换关键字符为全角
        /// </summary>
        /// <param name="UrlParam"></param>
        /// <returns></returns>
        public static string UrlParamUrlEncodeRun(string UrlParam)
        {
            UrlParam = UrlParam.Replace("+", "＋");
            UrlParam = UrlParam.Replace("=", "＝");
            UrlParam = UrlParam.Replace("&", "＆");
            UrlParam = UrlParam.Replace("?", "？");
            return UrlParam;
        }

        /// <summary>
        /// 使用Get传输替换关键字符为半角
        /// </summary>
        /// <param name="UrlParam"></param>
        /// <returns></returns>
        public static string UrlParamUrlDecodeRun(string UrlParam)
        {
            UrlParam = UrlParam.Replace("＋", "+");
            UrlParam = UrlParam.Replace("＝", "=");
            UrlParam = UrlParam.Replace("＆", "&");
            UrlParam = UrlParam.Replace("？", "?");
            return UrlParam;
        }
        #endregion

        #region  DES 加解密

        /// <summary>
        /// 接口使用的DES加密
        /// </summary>
        /// <param name="source">原字符串</param>
        /// <param name="key">密钥字符串</param>
        /// <returns>16进制表示的加密结果</returns>
        public static string DES_Encrypt(string source, string key)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //把字符串放到byte数组中  
            byte[] inputByteArray = Encoding.Default.GetBytes(source);

            //建立加密对象的密钥和偏移量  
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法  
            //使得输入密码必须输入英文文本  
            //            des.Key = UTF8Encoding.UTF8.GetBytes(key);
            //            des.IV  = UTF8Encoding.UTF8.GetBytes(key);
            des.Key = Encoding.Default.GetBytes(key);
            des.IV = Encoding.Default.GetBytes(key);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }

            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// 使用默认key 做 DES加密 Encoding.Default
        /// </summary>
        /// <param name="source">明文</param>
        /// <returns>密文</returns>
        public static string DES_Encrypt(string source)
        {
            return DES_Encrypt(source, Key);
        }

        /// <summary>
        /// DES解密 Encoding.Default
        /// </summary>
        /// <param name="source">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string DES_Decrypt(string source, string key)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //将字符串转为字节数组  
            byte[] inputByteArray = new byte[source.Length / 2];
            for (int x = 0; x < source.Length / 2; x++)
            {
                int i = (Convert.ToInt32(source.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            //建立加密对象的密钥和偏移量，此值重要，不能修改  
            des.Key = UTF8Encoding.UTF8.GetBytes(key);
            des.IV = UTF8Encoding.UTF8.GetBytes(key);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象  
            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// 使用默认key 做 DES解密 Encoding.Default
        /// </summary>
        /// <param name="source">密文</param>
        /// <returns>明文</returns>
        public static string DES_Decrypt(string source)
        {
            return DES_Decrypt(source, Key);
        }

        #endregion

        #region  MD5加密

        /// <summary>
        /// 标准MD5加密
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <param name="addKey">附加字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source, string addKey, Encoding encoding)
        {
            if (addKey.Length > 0)
            {
                source = source + addKey;
            }

            MD5 MD5 = new MD5CryptoServiceProvider();
            byte[] datSource = encoding.GetBytes(source);
            byte[] newSource = MD5.ComputeHash(datSource);
            string byte2String = null;
            for (int i = 0; i < newSource.Length; i++)
            {
                string thisByte = newSource[i].ToString("x");
                if (thisByte.Length == 1) thisByte = "0" + thisByte;
                byte2String += thisByte;
            }
            return byte2String;
        }

        /// <summary>
        /// 标准MD5加密
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source, string encoding)
        {
            return MD5_Encrypt(source, string.Empty, Encoding.GetEncoding(encoding));
        }

        /// <summary>
        /// 标准MD5加密
        /// </summary>
        /// <param name="source">被加密的字符串</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source)
        {
            return MD5_Encrypt(source, string.Empty, Encoding.Default);
        }

        #endregion

        #region SHA1_Encrypt SHA1加密
        /// <summary>
        /// SHA1加密，等效于 PHP 的 SHA1() 代码
        /// </summary>
        /// <param name="source">被加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string SHA1_Encrypt(string source)
        {
            byte[] temp1 = Encoding.UTF8.GetBytes(source);

            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            byte[] temp2 = sha.ComputeHash(temp1);
            sha.Clear();

            //注意，不能用这个
            //string output = Convert.ToBase64String(temp2); 

            string output = BitConverter.ToString(temp2);
            output = output.Replace("-", "");
            output = output.ToLower();
            return output;
        }
        #endregion

        #region HttpBase64Encode 通过HTTP传递的Base64编码
        /// <summary>
        /// 编码 通过HTTP传递的Base64编码
        /// </summary>
        /// <param name="source">编码前的</param>
        /// <returns>编码后的</returns>
        public static string HttpBase64Encode(string source)
        {
            //空串处理
            if (source == null || source.Length == 0)
            {
                return "";
            }

            //编码
            string encodeString = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

            //过滤
            encodeString = encodeString.Replace("+", "~");
            encodeString = encodeString.Replace("/", "@");
            encodeString = encodeString.Replace("=", "$");

            //返回
            return encodeString;
        }
        #endregion

        #region HttpBase64Decode 通过HTTP传递的Base64解码
        /// <summary>
        /// 解码 通过HTTP传递的Base64解码
        /// </summary>
        /// <param name="source">解码前的</param>
        /// <returns>解码后的</returns>
        public static string HttpBase64Decode(string source)
        {
            //空串处理
            if (source == null || source.Length == 0)
            {
                return "";
            }

            //还原
            string deocdeString = source;
            deocdeString = deocdeString.Replace("~", "+");
            deocdeString = deocdeString.Replace("@", "/");
            deocdeString = deocdeString.Replace("$", "=");

            //Base64解码
            deocdeString = Encoding.UTF8.GetString(Convert.FromBase64String(deocdeString));

            //返回
            return deocdeString;
        }
        #endregion

        #region 新版本PHP加解密

        /// <summary>
        /// 91通行证帐号密码加密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt_PHP(string source, string key)
        {
            if (source == string.Empty) return string.Empty;

            string str = Std_Encrypt_MD5("128");
            int startIndex = 0;

            byte[] b_source = System.Text.Encoding.GetEncoding("gb2312").GetBytes(source);

            byte[] b_destiation = new byte[b_source.Length * 2];

            for (int i = 0; i < b_source.Length; i++)
            {
                if (startIndex == str.Length)
                {
                    startIndex = 0;
                }

                b_destiation[i * 2] = System.Text.Encoding.GetEncoding("gb2312").GetBytes(str.Substring(startIndex, 1))[0];

                b_destiation[i * 2 + 1] = (byte)(b_source[i] ^ b_destiation[i * 2]);

                startIndex++;

            }

            return EncodingToBase64(keyED(b_destiation, key));

        }

        /// <summary>
        /// 91通行证帐号密码解密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt_PHP(string source, string key)
        {
            if (source == string.Empty) return string.Empty;

            byte[] b_source = keyED(DecodingFromBase64(source), key);

            byte[] b_destiation = new byte[b_source.Length / 2];

            for (int i = 0; i < b_source.Length / 2; i++)
            {
                int num = b_source[i * 2] ^ b_source[i * 2 + 1];
                b_destiation[i] = (byte)num;
            }

            return System.Text.Encoding.GetEncoding("gb2312").GetString(b_destiation);
        }

        #region 辅助方法

        private static byte[] keyED(byte[] source, string key)
        {
            string str = Std_Encrypt_MD5(key);
            int startIndex = 0;

            for (int i = 0; i < source.Length; i++)
            {
                if (str.Length == startIndex)
                    startIndex = 0;

                int num = source[i] ^ Encoding.GetEncoding("gb2312").GetBytes(str.Substring(startIndex, 1))[0];
                source[i] = (byte)num;

                startIndex++;
            }

            return source;
        }

        /// <summary>
        /// 将B64字符串转化字节数组
        /// </summary>
        /// <param name="base64Str">需要转化的字符串</param>
        /// <returns></returns>
        private static byte[] DecodingFromBase64(string base64Str)
        {
            byte[] bytes = Convert.FromBase64String(base64Str);

            return bytes;
        }

        /// <summary>
        /// 将字节数组转化B64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string EncodingToBase64(byte[] str)
        {
            return Convert.ToBase64String(str);
        }

        /// <summary>
        /// 标准MD5加密
        /// </summary>
        /// <param name="AppKey"></param>
        /// <returns></returns>
        private static string Std_Encrypt_MD5(string AppKey)
        {
            MD5 MD5 = new MD5CryptoServiceProvider();
            byte[] datSource = Encoding.GetEncoding("gb2312").GetBytes(AppKey);
            byte[] newSource = MD5.ComputeHash(datSource);
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < newSource.Length; i++)
            {
                sb.Append(newSource[i].ToString("x").PadLeft(2, '0'));
            }
            string crypt = sb.ToString();
            return crypt;
        }

        #endregion

        #endregion

        #region 91库MD5密码加密
        /// <summary>
        /// 91库MD5密码加密，返回使用MD5加密后字符串
        /// </summary>
        /// <param name="strpwd">待加密字符串</param>
        /// <returns>加密后字符串</returns>
        public static string Encrypt_MD5_Pwd(string strpwd)
        {
            string appkey = "fdjf,jkgfkl"; //，。加一特殊的字符后再加密，这样更安全些

            using (MD5 MD5 = new MD5CryptoServiceProvider())
            {
                byte[] datSource = System.Text.Encoding.UTF8.GetBytes(strpwd);
                byte[] a = System.Text.Encoding.UTF8.GetBytes(appkey);
                byte[] b = new byte[a.Length + 4 + datSource.Length];
                int i;
                for (i = 0; i < datSource.Length; i++)
                    b[i] = datSource[i];
                b[i++] = 163;
                b[i++] = 172;
                b[i++] = 161;
                b[i++] = 163;
                for (int k = 0; k < a.Length; k++)
                {
                    b[i] = a[k];
                    i++;
                }
                byte[] newSource = MD5.ComputeHash(b);
                string byte2String = null;
                for (i = 0; i < newSource.Length; i++)
                {
                    string thisByte = newSource[i].ToString("x");
                    if (thisByte.Length == 1)
                        thisByte = "0" + thisByte;
                    byte2String += thisByte;
                }
                return byte2String;
            }
        }
        #endregion

    }

}
