using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 字符串常用功能扩展
    /// </summary>
    public static class StringFunction
    {

        #region LoadFromFile 使用指定编码从文件中载入字符串
        /// <summary>
        /// 使用指定编码从文件中载入字符串
        /// </summary>
        /// <param name="fileName">要读取的文件名</param>
        /// <param name="encoding">字符串编码方式，默认表示UTF8</param>
        /// <returns></returns>
        public static string LoadFromFile(this string fileName, Encoding encoding = null)
        {
            if (!File.Exists(fileName))
                return null;

            if (encoding == null)
                encoding = Encoding.UTF8;

            string sourceString = "";
            using (StreamReader reader = new StreamReader(fileName, encoding))
            {
                sourceString = reader.ReadToEnd();
                reader.Close();
            }
            return sourceString;
        }
        #endregion

        #region SaveToFile 使用UTF8编码将字符串保存到文件中
        /// <summary>
        /// 使用UTF8编码将字符串保存到文件中
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="fileName">要保存的文件名</param>
        /// <returns></returns>
        public static bool SaveToFile(this string sourceString, string fileName)
        {
            if (sourceString.Trim() == "")
                return false;

            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.Write(sourceString);
                writer.Close();
            }
            return true;
        }
        #endregion

        #region AppendToFile 使用UTF8将字符串追加到文件中
        /// <summary>
        /// 使用UTF8将字符串追加到文件中
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="fileName">要保存的文件名</param>
        /// <returns></returns>
        public static bool AppendToFile(this string sourceString, string fileName)
        {
            if (sourceString.Trim() == "")
                return false;

            using (StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8))
            {
                writer.Write(sourceString);
                writer.Close();
            }
            return true;
        }
        #endregion

        #region SplitParams 将参数列表拆分为键值对词典
        /// <summary>
        /// 将转义符做替换处理
        /// 将"&amp;","="两个转义符替换为"%&amp;%","%=%"。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns> 
        public static string ReplaceSplitChar(this string source)
        {
            return source.Replace("&", "%&%").Replace("=", "%=%");
        }
        #endregion

        #region SplitParams 将参数列表拆分为键值对词典
        /// <summary>
        /// 将参数列表拆分为键值对词典。
        /// 处理过程中将"%&amp;%","%=%"两个转义符替换回"&amp;","="。
        /// 如: S1=V1&amp;S2=V2&amp;S3=V3
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns> 
        public static Dictionary<string, string> SplitParams(this string source)
        {
            Dictionary<string, string> dicKeyValue = new Dictionary<string, string>();
            source = source.Replace("%&%", "%--%");
            string[] paramPairs = source.Split("&".ToCharArray());
            for (int i = 0; i < paramPairs.Length; i++)
            {
                paramPairs[i] = paramPairs[i].Replace("%--%", "&");
                paramPairs[i] = paramPairs[i].Replace("%=%", "%--%");
                string[] keyvalues = paramPairs[i].Split("=".ToCharArray());
                if (keyvalues.Length == 2)
                {
                    string key = keyvalues[0].Replace("%--%", "=").ToUpper();
                    string value = keyvalues[1].Replace("%--%", "=");
                    dicKeyValue.Add(key, value);
                }
            }
            return dicKeyValue;
        }
        #endregion

        #region Cut 切割保留字符串的前几个字符，同时去除空格
        /// <summary>
        /// 切割保留字符串的前几个字符，同时去除空格
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Cut(this string source, int len)
        {
            source = source.Trim();
            if (source.Length > len)
                return source.Substring(0, len).Trim();
            return source;
        }
        #endregion

        #region Cut 根据提供的头部字符串列表切除起始字符串
        /// <summary>
        /// 根据提供的头部字符串列表切除起始字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Cut(this string text, params string[] args)
        {
            foreach (string arg in args)
                if (text.StartsWith(arg))
                    text = text.Substring(arg.Length);

            return text;
        }
        #endregion

        #region ToMobileNumer 获取手机号码
        /// <summary>
        /// 获取手机号码
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>空表示不合法号码</returns>
        public static string ToMobileNumer(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (source.StartsWith("+"))
                source = source.Cut("+86", "+852", "+886", "+853", "+1", "+7", "+44", "+61");

            //去除非数字，比如电话号码里有空格或者‘-’之类的 包括开头的那个+ 
            char strChar = '\0';
            for (int i = source.Length - 1; i > -1; i--)
            {
                strChar = source[i];
                if ((strChar < '0' || strChar > '9'))
                    source = source.Remove(i, 1);
            }

            //去除常用电话号码前缀 
            source = source.Cut("12593", "17909", "79458", "11618", "197202", "12591", "17911", "17951", "17996", "17969", "11808", "10131", "10193", "12520", "106", "86");

            //130、131、132、133、134、135、136、137、138、139、150、151、153、156、157、158、159、188、189
            Regex regex = new Regex("^((13[0-9]{9})|(15[0-9]{9})|18[0-9]{9})");
            if (regex.Match(source).Success == true)
                return source;
            else
                return string.Empty;
        }
        #endregion

        #region ToDateTime 转为日期类型
        /// <summary>
        /// 转为日期类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="format">原字符串的日期格式，如"yyyy-MM-dd"</param>
        /// <param name="provider">一个DateTimeFormatInfo对象，提供有关的区域性特定格式信息。</param>
        /// <returns>空表示不合法字符串</returns>
        public static DateTime? ToDateTime(this string source, string format, IFormatProvider provider)
        {
            try
            {
                return DateTime.ParseExact(source, format, provider);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region ToDateTime 转为日期类型
        /// <summary>
        /// 转为日期类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="format">原字符串的日期格式，如"yyyy-MM-dd"</param>
        /// <returns>空表示不合法字符串</returns>
        public static DateTime? ToDateTime(this string source, string format)
        {
            try
            {
                return DateTime.ParseExact(source, format, null);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region ToInteger 转为整数
        /// <summary>
        /// 转为整数
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>空表示不合法字符串</returns>
        public static int? ToInteger(this string source)
        {
            try
            {
                if (source.IsInteger())
                    return int.Parse(source);
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region ToInteger 转为整数
        /// <summary>
        /// 转为整数
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>空表示不合法字符串</returns>
        public static Guid? ToGuid(this string source)
        {
            try
            {
                return new Guid(source);
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region ToSafeSql 将字符串转为安全的Sql字符串
        /// <summary>
        /// 将字符串转为安全的Sql字符串，不建议使用。尽可能使用参数化查询来避免
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>防注入的SQL参数</returns>
        public static string ToSafeSql(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            else
            {
                return source.Replace("'", "''");
            }
        }
        #endregion

        #region ToJsonObject 将Json字符串转为对象
        /// <summary>
        /// 将Json字符串转为对象
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        /// <returns></returns>
        public static object ToJsonObject(this string jsonString)
        {
            return JsonConvert.DeserializeObject(jsonString);
        }
        #endregion

        #region ToJsonObject<T> 将Json字符串转为对象
        /// <summary>
        /// 将Json字符串转为对象
        /// </summary>
        /// <param name="jsonString">Json字符串</param>
        /// <returns></returns>
        public static T ToJsonObject<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        #endregion

        #region ToEnum<T> 将字符串转换为枚举
        /// <summary>
        /// 将字符串转换为枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="enumString">枚举字符串</param>
        /// <param name="defaultValue">默认枚举</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string enumString, T defaultValue) where T : IComparable, IFormattable, IConvertible
        {
            T theEnum;
            if (Enum.IsDefined(typeof(T), enumString))
            {
                theEnum = (T)Enum.Parse(typeof(T), enumString);
            }
            else
                theEnum = defaultValue;
            return theEnum;
        }
        #endregion

        #region ToUintList 将正整数范围字符串转换为正整数列表，如："5,6,10-15" --> "5,6,10,11,12,13,14,15"
        /// <summary>
        /// 将正整数范围字符串转换为正整数列表，如："5,6,10-15" --> "5,6,10,11,12,13,14,15"
        /// </summary>
        /// <param name="rangeString">正整数范围字符串</param>
        /// <returns>正整数列表</returns>
        public static List<uint> ToUintList(this string rangeString)
        {
            List<uint> theIntList = new List<uint>();
            string[] ranges = rangeString.Trim().ToUpper().Split(',');
            for (int i = 0; i < ranges.Length; i++)
            {
                if (ranges[i].IndexOf('-') < 0 && ranges[i].IsIntegerUnNegative())
                {
                    if (!theIntList.Contains(uint.Parse(ranges[i])))
                        theIntList.Add(uint.Parse(ranges[i]));
                }
                else if (ranges[i].IndexOf('-') >= 0)
                {
                    string[] fromto = ranges[i].Trim().ToUpper().Split('-');
                    if (fromto.Length >= 2 && fromto[0].Trim().IsIntegerUnNegative() && fromto[1].Trim().IsIntegerUnNegative())
                    {
                        uint start = uint.Parse(fromto[0].Trim());
                        uint end = uint.Parse(fromto[1].Trim());
                        for (uint j = start; j <= end; j++)
                            if (!theIntList.Contains(j))
                                theIntList.Add(j);
                    }
                }
            }
            return theIntList;
        }
        #endregion

        #region ConvertUintList 将正整数范围字符串转换为正整数列表，如："5,6,10-15" --> "5,6,10,11,12,13,14,15"
        /// <summary>
        /// 将正整数范围字符串转换为正整数列表，如："5,6,10-15" --> "5,6,10,11,12,13,14,15"
        /// </summary>
        /// <param name="rangeString">正整数范围字符串</param>
        /// <returns>正整数列表</returns>
        public static string ConvertUintList(this string rangeString)
        {
            string newRangeString = rangeString;
            if (!string.IsNullOrEmpty(rangeString) && rangeString.IndexOf('-') >= 0)
            {
                List<uint> rangeUints = rangeString.ToUintList();
                newRangeString = string.Empty;
                for (int i = 0; i < rangeUints.Count; i++)
                {
                    if (newRangeString != string.Empty)
                        newRangeString += ",";
                    newRangeString += rangeUints[i].ToString();
                }
            }
            return newRangeString;
        }
        #endregion

        #region ContainsUintValue 检查正整数范围字符串是否包含指定的正整数，如："5,6,10-15" 包含 13
        /// <summary>
        /// 检查正整数范围字符串是否包含指定的正整数，如："5,6,10-15" 包含 13
        /// </summary>
        /// <param name="rangeString">正整数范围字符串</param>
        /// <param name="checkValue">指定的正整数</param>
        /// <returns>是否包含</returns>
        public static bool ContainsUintValue(this string rangeString, uint checkValue)
        {
            return rangeString.ToUintList().Contains(checkValue);
        }
        #endregion

        #region UrlSafeAddQuery 给URL地址添加查询参数
        /// <summary>
        /// 给URL地址添加查询参数
        /// if the URL has not any query,then append the query key and value to it.
        /// if the URL has some queries, then check it if exists the query key already,replace the value, or append the key and value
        /// if the URL has any fragment, append fragments to the URL end.
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>添加参数后的URL地址</returns>
        public static string UrlSafeAddQuery(this string url, string key, string value)
        {
            int fragPos = url.LastIndexOf("#");
            string fragment = string.Empty;
            if (fragPos > -1)
            {
                fragment = url.Substring(fragPos);
                url = url.Substring(0, fragPos);
            }
            int querystart = url.IndexOf("?");
            if (querystart < 0)
            {
                url += string.Format("?{0}={1}", key, value);
            }
            else
            {
                Regex reg = new Regex(string.Format(@"(?<=[&\?]){0}=[^\s&#]*", key), RegexOptions.Compiled);
                if (reg.IsMatch(url))
                    url = reg.Replace(url, string.Format("{0}={1}", key, value));
                else
                    url += string.Format("&{0}={1}", key, value);
            }
            return url + fragment;
        }
        #endregion

        #region UrlSafeRemoveQuery 从URL地址中移除查询参数
        /// <summary>
        /// 从URL地址中移除查询参数
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="key">参数名称</param>
        /// <returns>添加参数后的URL地址</returns>
        public static string UrlSafeRemoveQuery(this string url, string key)
        {
            Regex reg = new Regex(string.Format(@"[&\?]{0}=[^\s&#]*&?", key), RegexOptions.Compiled);
            return reg.Replace(url, PutAwayGarbageFromURL);
        }

        private static string PutAwayGarbageFromURL(Match match)
        {
            string value = match.Value;
            if (value.EndsWith("&"))
                return value.Substring(0, 1);
            else
                return string.Empty;
        }
        #endregion

        #region CompareToVersion 比较版本号
        /// <summary>
        /// 比较版本号，以版本号中的点进行分割后，逐渐段比较数字大小，最多支持 a.b.c.d 四段版本号
        /// newVersion 大于 oldVersion 返回 1
        /// newVersion 等于 oldVersion 返回 0
        /// newVersion 小于 oldVersion 返回 -1
        /// </summary>
        /// <example>
        /// 举例如下：
        /// newVersion = "1.12", oldVersion="1.11", 那么返回 1
        /// newVersion = "1.12", oldVersion="1.110", 那么返回 -1
        /// newVersion = "1.12", oldVersion="1.12", 那么返回 0
        /// newVersion = "1.12", oldVersion="1.120", 那么返回 -1
        /// newVersion = "1.12", oldVersion="1.012", 那么返回 0
        /// newVersion = "1.12", oldVersion="1.1.3", 那么返回 1
        /// </example>
        /// <param name="newVersion">新版本</param>
        /// <param name="oldVersion">旧版本</param>
        /// <returns>比较结果，如果出错，返回-2</returns>
        public static int CompareToVersion(this string newVersion, string oldVersion)
        {
            int result = -1;

            string[] oldVersions = oldVersion.Split('.');
            int[] verOlds = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (oldVersions.Length > i)
                {
                    try
                    {
                        if (oldVersions[i].IsInteger())
                            verOlds[i] = int.Parse(oldVersions[i]);
                    }
                    catch
                    {
                        verOlds[i] = 0;
                    }
                }
            }

            string[] newVersions = newVersion.Split('.');
            int[] verNews = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (newVersions.Length > i)
                {
                    try
                    {
                        if (newVersions[i].IsInteger())
                            verNews[i] = int.Parse(newVersions[i]);
                    }
                    catch
                    {
                        verNews[i] = 0;
                    }
                }
            }

            if (verNews[0] > verOlds[0] || (verNews[0] == verOlds[0] && verNews[1] > verOlds[1])
               || (verNews[0] == verOlds[0] && verNews[1] == verOlds[1] && verNews[2] > verOlds[2])
               || (verNews[0] == verOlds[0] && verNews[1] == verOlds[1] && verNews[2] == verOlds[2] && verNews[3] > verOlds[3])
                )
                result = 1;
            else if ((verNews[0] == verOlds[0] && verNews[1] == verOlds[1] && verNews[2] == verOlds[2] && verNews[3] == verOlds[3]))
                result = 0;

            return result;
        }
        #endregion

        #region CompareVersion 比较版本号
        /// <summary>
        /// 比较版本号，以版本号中的点进行分割后，逐渐段比较数字大小，最多支持 a.b.c.d 四段版本号
        /// newVersion 大于 oldVersion 返回 1
        /// newVersion 等于 oldVersion 返回 0
        /// newVersion 小于 oldVersion 返回 -1
        /// </summary>
        /// <example>
        /// 举例如下：
        /// newVersion = "1.12", oldVersion="1.11", 那么返回 1
        /// newVersion = "1.12", oldVersion="1.110", 那么返回 -1
        /// newVersion = "1.12", oldVersion="1.12", 那么返回 0
        /// newVersion = "1.12", oldVersion="1.120", 那么返回 -1
        /// newVersion = "1.12", oldVersion="1.012", 那么返回 0
        /// newVersion = "1.12", oldVersion="1.1.3", 那么返回 1
        /// </example>
        /// <param name="oldVersion">旧版本</param>
        /// <param name="newVersion">新版本</param>
        /// <returns>比较结果，如果出错，返回-2</returns>
        [Obsolete("该方法设计时候欠考虑，返回值容易混淆，请使用CompareToVersion")]
        public static int CompareVersion(this string oldVersion, string newVersion)
        {
            return CompareToVersion(oldVersion, newVersion);
        }
        #endregion

        /// <summary>
        /// 如果是空字符串转为null
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AsNullIfEmpty(this string str)
        {
            return string.IsNullOrEmpty(str) ? null : str;
        }

        /// <summary>
        /// 转换为Bool
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static bool ToBool(this string str, bool defaultVal = false)
        {
            if (!string.IsNullOrEmpty(str))
            {
                bool val;
                if (bool.TryParse(str, out val))
                    return val;
            }

            return defaultVal;
        }

        /// <summary>
        /// 转换为Byte
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static byte ToByte(this string str, byte defaultVal = 0)
        {

            if (!string.IsNullOrEmpty(str))
            {
                byte val;
                if (byte.TryParse(str, out val))
                    return val;
            }

            return defaultVal;
        }

        /// <summary>
        /// 转换为Short
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static short ToShort(this string str, short defaultVal = 0)
        {
            if (!string.IsNullOrEmpty(str))
            {
                short val;
                if (short.TryParse(str, out val))
                    return val;
            }
            return defaultVal;
        }

        /// <summary>
        /// 转换为Int32
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static Int32 ToInt32(this string str, int defaultVal = 0)
        {

            if (!string.IsNullOrEmpty(str))
            {
                Int32 val;
                if (Int32.TryParse(str, out val))
                    return val;
            }

            return defaultVal;
        }

        /// <summary>
        /// 转换为Int64
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static Int64 ToInt64(this string str, long defaultVal = 0)
        {

            if (!string.IsNullOrEmpty(str))
            {
                Int64 val;
                if (Int64.TryParse(str, out val))
                    return val;
            }

            return defaultVal;
        }

        /// <summary>
        /// 转换为Float
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static float ToFloat(this string str, float defaultVal = 0)
        {

            if (!string.IsNullOrEmpty(str))
            {
                float val;
                if (float.TryParse(str, out val))
                    return val;
            }

            return defaultVal;
        }

        /// <summary>
        /// 转换为Double
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static double ToDouble(this string str, double defaultVal = 0)
        {

            if (!string.IsNullOrEmpty(str))
            {
                double val;
                if (double.TryParse(str, out val))
                    return val;
            }

            return defaultVal;
        }

        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string str, DateTime? defaultVal = null)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime val;
                if (DateTime.TryParse(str, out val))
                    return val;
            }
            if (defaultVal.HasValue)
                return defaultVal.Value;
            else
                return DateTime.MinValue;
        }

        /// <summary>
        /// 获取当天的最后时刻 yyyy-MM-dd 23:59:59.999
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToDayLastTime(this DateTime date)
        {
            DateTime dayLastTime = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
            return dayLastTime;
        }

    }

}
