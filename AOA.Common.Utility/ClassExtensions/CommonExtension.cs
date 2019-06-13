using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ServiceModel;

using Newtonsoft.Json;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 一些常用扩展方法
    /// </summary>
    public static class CommonExtension
    {

        /// <summary>
        /// 将Key字符串转为大写
        /// </summary>
        /// <param name="dict">Dictionary对象</param>
        /// <returns>转换后的Dictionary对象</returns>
        public static Dictionary<string, T> ToUpperKey<T>(this Dictionary<string, T> dict)
        {
            Dictionary<string, T> newDict = new Dictionary<string, T>();
            foreach (KeyValuePair<string, T> kvp in dict)
                newDict.Add(kvp.Key.ToUpper(), kvp.Value);
            return newDict;
        }

        /// <summary>
        /// 将Key字符串转为小写
        /// </summary>
        /// <param name="dict">Dictionary对象</param>
        /// <returns>转换后的Dictionary对象</returns>
        public static Dictionary<string, T> ToLowerKey<T>(this Dictionary<string, T> dict)
        {
            Dictionary<string, T> newDict = new Dictionary<string, T>();
            foreach (KeyValuePair<string, T> kvp in dict)
                newDict.Add(kvp.Key.ToLower(), kvp.Value);
            return newDict;
        }

        /// <summary>
        /// 把阿拉伯数字的金额转换为中文大写数字
        /// </summary>
        /// <param name="money">数字金额</param>
        /// <returns>中文大写数字</returns>
        public static string ConvertToChineseMoney(this double money)
        {
            string s = money.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".",
                delegate (Match m)
                {
                    return "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString();
                }
                );
        }

        /// <summary>
        /// 对象转换为Json字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>Json格式的字符串</returns>
        public static string ToJson(this object obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
            {
                Formatting = formatting,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        /// <summary>
        /// 安全关闭WCF服务连接
        /// </summary>
        /// <param name="wcfServiceClient">WCF服务连接</param>
        public static void CloseConnection(this ICommunicationObject wcfServiceClient)
        {
            if (wcfServiceClient.State != CommunicationState.Opened)
                return;

            try
            {
                wcfServiceClient.Close();
            }
            catch (CommunicationException ex)
            {
                wcfServiceClient.Abort();
                throw ex;
            }
            catch (TimeoutException ex)
            {
                wcfServiceClient.Abort();
                throw ex;
            }
            catch (Exception ex)
            {
                wcfServiceClient.Abort();
                throw ex;
            }

        }

    }

}
