using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace AOA.Common.Utility
{

    /// <summary>
    /// ���ܰ����࣬���� net91com.Core.dll
    /// </summary>
    public class Net91ComCryptoHelper
    {

        /// <summary>
        /// DES�ӽ��ܵ�Ĭ����Կ
        /// </summary>
        public static string Key
        {
            get
            {
                return "!@#ASD12";
            }
        }

        #region ʹ��Get�����滻�ؼ��ַ�Ϊȫ�ǺͰ��ת��
        /// <summary>
        /// ʹ��Get�����滻�ؼ��ַ�Ϊȫ��
        /// </summary>
        /// <param name="UrlParam"></param>
        /// <returns></returns>
        public static string UrlParamUrlEncodeRun(string UrlParam)
        {
            UrlParam = UrlParam.Replace("+", "��");
            UrlParam = UrlParam.Replace("=", "��");
            UrlParam = UrlParam.Replace("&", "��");
            UrlParam = UrlParam.Replace("?", "��");
            return UrlParam;
        }

        /// <summary>
        /// ʹ��Get�����滻�ؼ��ַ�Ϊ���
        /// </summary>
        /// <param name="UrlParam"></param>
        /// <returns></returns>
        public static string UrlParamUrlDecodeRun(string UrlParam)
        {
            UrlParam = UrlParam.Replace("��", "+");
            UrlParam = UrlParam.Replace("��", "=");
            UrlParam = UrlParam.Replace("��", "&");
            UrlParam = UrlParam.Replace("��", "?");
            return UrlParam;
        }
        #endregion

        #region  DES �ӽ���

        /// <summary>
        /// �ӿ�ʹ�õ�DES����
        /// </summary>
        /// <param name="source">ԭ�ַ���</param>
        /// <param name="key">��Կ�ַ���</param>
        /// <returns>16���Ʊ�ʾ�ļ��ܽ��</returns>
        public static string DES_Encrypt(string source, string key)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //���ַ����ŵ�byte������  
            byte[] inputByteArray = Encoding.Default.GetBytes(source);

            //�������ܶ������Կ��ƫ����  
            //ԭ��ʹ��ASCIIEncoding.ASCII������GetBytes����  
            //ʹ�����������������Ӣ���ı�  
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
        /// ʹ��Ĭ��key �� DES���� Encoding.Default
        /// </summary>
        /// <param name="source">����</param>
        /// <returns>����</returns>
        public static string DES_Encrypt(string source)
        {
            return DES_Encrypt(source, Key);
        }

        /// <summary>
        /// DES���� Encoding.Default
        /// </summary>
        /// <param name="source">����</param>
        /// <param name="key">��Կ</param>
        /// <returns>����</returns>
        public static string DES_Decrypt(string source, string key)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            //���ַ���תΪ�ֽ�����  
            byte[] inputByteArray = new byte[source.Length / 2];
            for (int x = 0; x < source.Length / 2; x++)
            {
                int i = (Convert.ToInt32(source.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            //�������ܶ������Կ��ƫ��������ֵ��Ҫ�������޸�  
            des.Key = UTF8Encoding.UTF8.GetBytes(key);
            des.IV = UTF8Encoding.UTF8.GetBytes(key);

            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            //����StringBuild����CreateDecryptʹ�õ��������󣬱���ѽ��ܺ���ı����������  
            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// ʹ��Ĭ��key �� DES���� Encoding.Default
        /// </summary>
        /// <param name="source">����</param>
        /// <returns>����</returns>
        public static string DES_Decrypt(string source)
        {
            return DES_Decrypt(source, Key);
        }

        #endregion

        #region  MD5����

        /// <summary>
        /// ��׼MD5����
        /// </summary>
        /// <param name="source">�������ַ���</param>
        /// <param name="addKey">�����ַ���</param>
        /// <param name="encoding">���뷽ʽ</param>
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
        /// ��׼MD5����
        /// </summary>
        /// <param name="source">�������ַ���</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source, string encoding)
        {
            return MD5_Encrypt(source, string.Empty, Encoding.GetEncoding(encoding));
        }

        /// <summary>
        /// ��׼MD5����
        /// </summary>
        /// <param name="source">�����ܵ��ַ���</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source)
        {
            return MD5_Encrypt(source, string.Empty, Encoding.Default);
        }

        #endregion

        #region SHA1_Encrypt SHA1����
        /// <summary>
        /// SHA1���ܣ���Ч�� PHP �� SHA1() ����
        /// </summary>
        /// <param name="source">�����ܵ��ַ���</param>
        /// <returns>���ܺ���ַ���</returns>
        public static string SHA1_Encrypt(string source)
        {
            byte[] temp1 = Encoding.UTF8.GetBytes(source);

            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            byte[] temp2 = sha.ComputeHash(temp1);
            sha.Clear();

            //ע�⣬���������
            //string output = Convert.ToBase64String(temp2); 

            string output = BitConverter.ToString(temp2);
            output = output.Replace("-", "");
            output = output.ToLower();
            return output;
        }
        #endregion

        #region HttpBase64Encode ͨ��HTTP���ݵ�Base64����
        /// <summary>
        /// ���� ͨ��HTTP���ݵ�Base64����
        /// </summary>
        /// <param name="source">����ǰ��</param>
        /// <returns>������</returns>
        public static string HttpBase64Encode(string source)
        {
            //�մ�����
            if (source == null || source.Length == 0)
            {
                return "";
            }

            //����
            string encodeString = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

            //����
            encodeString = encodeString.Replace("+", "~");
            encodeString = encodeString.Replace("/", "@");
            encodeString = encodeString.Replace("=", "$");

            //����
            return encodeString;
        }
        #endregion

        #region HttpBase64Decode ͨ��HTTP���ݵ�Base64����
        /// <summary>
        /// ���� ͨ��HTTP���ݵ�Base64����
        /// </summary>
        /// <param name="source">����ǰ��</param>
        /// <returns>������</returns>
        public static string HttpBase64Decode(string source)
        {
            //�մ�����
            if (source == null || source.Length == 0)
            {
                return "";
            }

            //��ԭ
            string deocdeString = source;
            deocdeString = deocdeString.Replace("~", "+");
            deocdeString = deocdeString.Replace("@", "/");
            deocdeString = deocdeString.Replace("$", "=");

            //Base64����
            deocdeString = Encoding.UTF8.GetString(Convert.FromBase64String(deocdeString));

            //����
            return deocdeString;
        }
        #endregion

        #region �°汾PHP�ӽ���

        /// <summary>
        /// 91ͨ��֤�ʺ��������
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
        /// 91ͨ��֤�ʺ��������
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

        #region ��������

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
        /// ��B64�ַ���ת���ֽ�����
        /// </summary>
        /// <param name="base64Str">��Ҫת�����ַ���</param>
        /// <returns></returns>
        private static byte[] DecodingFromBase64(string base64Str)
        {
            byte[] bytes = Convert.FromBase64String(base64Str);

            return bytes;
        }

        /// <summary>
        /// ���ֽ�����ת��B64�ַ���
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string EncodingToBase64(byte[] str)
        {
            return Convert.ToBase64String(str);
        }

        /// <summary>
        /// ��׼MD5����
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

        #region 91��MD5�������
        /// <summary>
        /// 91��MD5������ܣ�����ʹ��MD5���ܺ��ַ���
        /// </summary>
        /// <param name="strpwd">�������ַ���</param>
        /// <returns>���ܺ��ַ���</returns>
        public static string Encrypt_MD5_Pwd(string strpwd)
        {
            string appkey = "fdjf,jkgfkl"; //������һ������ַ����ټ��ܣ���������ȫЩ

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
