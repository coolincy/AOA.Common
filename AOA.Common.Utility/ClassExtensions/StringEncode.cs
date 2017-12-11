using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 字符串编码扩展
    /// </summary>
    public static class StringEncode
    {

        #region HexCharToByte 十六进制字符转字节
        /// <summary>
        /// 十六进制字符转字节
        /// </summary>
        /// <param name="sourceChar">十六进制字符(0-9,A-F)</param>
        /// <returns>字节</returns>
        public static byte HexCharToByte(string sourceChar)
        {
            if (sourceChar.Length > 1)
                sourceChar = sourceChar.Substring(0, 1);

            switch (sourceChar.ToUpper())
            {
                case "0":
                    return 0x00;
                case "1":
                    return 0x01;
                case "2":
                    return 0x02;
                case "3":
                    return 0x03;
                case "4":
                    return 0x04;
                case "5":
                    return 0x05;
                case "6":
                    return 0x06;
                case "7":
                    return 0x07;
                case "8":
                    return 0x08;
                case "9":
                    return 0x09;
                case "A":
                    return 0x0a;
                case "B":
                    return 0x0b;
                case "C":
                    return 0x0c;
                case "D":
                    return 0x0d;
                case "E":
                    return 0x0e;
                case "F":
                    return 0x0f;
                default:
                    return 0x00;
            }
        }
        #endregion

        #region ByteArrayToHexString 字节数组转十六进制字符串
        /// <summary>
        /// 字节数组转十六进制字符串(字母为小写)
        /// </summary>
        /// <param name="sourceArray">源字节数组</param>
        /// <returns>十六进制字符串</returns>
        public static string ByteArrayToHexString(IEnumerable<byte> sourceArray)
        {
            StringBuilder sBuilder = new StringBuilder();

            foreach (byte item in sourceArray)
                sBuilder.Append(item.ToString("x2"));

            return sBuilder.ToString();
        }
        #endregion

        #region CharArrayToHexString 字节数组转十六进制字符串
        /// <summary>
        /// 字节数组转十六进制字符串(字母为小写)
        /// </summary>
        /// <param name="sourceArray">源字节数组</param>
        /// <returns>十六进制字符串</returns>
        public static string CharArrayToHexString(char[] sourceArray)
        {
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < sourceArray.Length; i++)
            {
                byte bb = Convert.ToByte(sourceArray[i]);
                sBuilder.Append(bb.ToString("x2"));
            }

            return sBuilder.ToString();
        }
        #endregion

        #region HexStringToByteArray 十六进制字符串转字节数组
        /// <summary>
        /// 十六进制字符串转字节数组
        /// </summary>
        /// <param name="sourceStr">十六进制字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] HexStringToByteArray(this string sourceStr)
        {
            Byte[] buf = new byte[sourceStr.Length / 2];
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(HexCharToByte(sourceStr.Substring(i * 2, 1)) * 0x10 + HexCharToByte(sourceStr.Substring(i * 2 + 1, 1)));
            }
            return buf;
        }
        #endregion

        #region HexStringEncode 十六进制字符串编码
        /// <summary>
        /// 十六进制字符串编码
        /// </summary>
        /// <param name="sourceStr">普通字符串</param>
        /// <returns>十六进制字符串</returns>
        public static string HexStringEncode(this string sourceStr)
        {
            byte[] data = Encoding.UTF8.GetBytes(sourceStr);
            return ByteArrayToHexString(data);
        }
        #endregion

        #region HexStringDecode 十六进制字符串解码
        /// <summary>
        /// 十六进制字符串解码
        /// </summary>
        /// <param name="sourceStr">十六进制字符串</param>
        /// <returns>普通字符串</returns>
        public static string HexStringDecode(this string sourceStr)
        {
            byte[] data = sourceStr.HexStringToByteArray();
            return Encoding.UTF8.GetString(data);
        }
        #endregion

        #region Base64StringEncode 用Base64编码字符串
        /// <summary>
        /// 用Base64编码字符串
        /// </summary>
        /// <param name="sourceStr">UTF8编码的源字符串</param>
        /// <returns>Base64编码字符串</returns>
        public static string Base64StringEncode(this string sourceStr)
        {
            byte[] buf = Encoding.UTF8.GetBytes(sourceStr);
            return Convert.ToBase64String(buf);
        }
        #endregion

        #region Base64StringDecode 用Base64解码字符串
        /// <summary>
        /// 用Base64解码字符串
        /// </summary>
        /// <param name="sourceStr">Base64编码字符串</param>
        /// <returns>UTF8编码的源字符串</returns>
        public static string Base64StringDecode(this string sourceStr)
        {
            byte[] buf = Convert.FromBase64String(sourceStr);
            return Encoding.UTF8.GetString(buf);
        }
        #endregion

        #region Base64StringDecodeToByteArray 用Base64解码字符串到字节数组
        /// <summary>
        /// 用Base64解码字符串到字节数组
        /// </summary>
        /// <param name="sourceStr">Base64编码字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] Base64StringDecodeToByteArray(this string sourceStr)
        {
            return Convert.FromBase64String(sourceStr);
        }
        #endregion

        #region Base64StringToHexString Base64编码字符串转十六进制字符串
        /// <summary>
        /// Base64编码字符串转十六进制字符串
        /// </summary>
        /// <param name="sourceStr">Base64编码字符串</param>
        /// <returns>十六进制字符串</returns>
        public static string Base64StringToHexString(this string sourceStr)
        {
            byte[] buf = Convert.FromBase64String(sourceStr);
            return ByteArrayToHexString(buf);
        }
        #endregion

        #region HexStringToBase64String 十六进制字符串转Base64编码字符串
        /// <summary>
        /// 十六进制字符串转Base64编码字符串
        /// </summary>
        /// <param name="sourceStr">十六进制字符串</param>
        /// <returns>Base64编码字符串</returns>
        public static string HexStringToBase64String(this string sourceStr)
        {
            byte[] buf = HexStringToByteArray(sourceStr);
            return Convert.ToBase64String(buf);
        }
        #endregion

        #region UnicodeToDefault Unicode编码字符串转为系统默认编码字符串
        /// <summary>
        /// Unicode编码字符串转为系统默认编码字符串
        /// </summary>
        /// <param name="uniString"></param>
        /// <returns></returns>
        public static string UnicodeToDefault(this string uniString)
        {
            byte[] uniBytes = Encoding.Unicode.GetBytes(uniString);
            byte[] defBytes = Encoding.Convert(Encoding.Unicode, Encoding.Default, uniBytes);
            return Encoding.Default.GetString(defBytes);
        }
        #endregion

        #region HtmlEncode 编码文本中的特殊字符为Html转义字符
        /// <summary>
        /// 编码文本中的特殊字符为Html转义字符
        /// </summary>
        /// <param name="sourceStr">普通字符串</param>
        /// <returns>编码过的文本</returns>
        public static string HtmlEncode(this string sourceStr)
        {
            return HttpUtility.HtmlEncode(sourceStr).Replace("|", "&#124;");
        }
        #endregion

        #region HtmlDecode 解码文本中的Html转义字符为正常文本
        /// <summary>
        /// 解码文本中的Html转义字符为正常文本
        /// </summary>
        /// <param name="sourceStr">普通字符串</param>
        /// <returns>解码过的文本</returns>
        public static string HtmlDecode(this string sourceStr)
        {
            return HttpUtility.HtmlDecode(sourceStr);
        }
        #endregion

        #region UrlEncode 编码文本中的特殊字符为Url转义字符
        /// <summary>
        /// 编码文本中的特殊字符为Url转义字符
        /// </summary>
        /// <param name="sourceStr">普通字符串</param>
        /// <returns>编码过的文本</returns>
        public static string UrlEncode(this string sourceStr)
        {
            return HttpUtility.UrlEncode(sourceStr);
        }
        #endregion

        #region UrlDecode 解码文本中的Url转义字符为正常文本
        /// <summary>
        /// 解码文本中的Url转义字符为正常文本
        /// </summary>
        /// <param name="sourceStr">普通字符串</param>
        /// <returns>解码过的文本</returns>
        public static string UrlDecode(this string sourceStr)
        {
            return HttpUtility.UrlDecode(sourceStr);
        }
        #endregion

    }

}
