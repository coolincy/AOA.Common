using System;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 日期扩展方法
    /// </summary>
    public static class DateTimeExtension
    {

        /// <summary>
        /// 日期转换为 yyyy-MM-dd HH:mm:ss 格式的字符串
        /// </summary>
        /// <param name="theDateTime">需要转换的日期</param>
        /// <returns>yyyy-MM-dd HH:mm:ss 格式的字符串</returns>
        public static string ToFullDateTimeString(this DateTime theDateTime)
        {
            return theDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 日期转换为 yyyy-MM-dd HH:mm:ss.fff 格式的字符串
        /// </summary>
        /// <param name="theDateTime">需要转换的日期</param>
        /// <returns>yyyy-MM-dd HH:mm:ss.fff 格式的字符串</returns>
        public static string ToFullDateTimeWithMsString(this DateTime theDateTime)
        {
            return theDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 日期转换为 yyyy-MM-dd 格式的字符串
        /// </summary>
        /// <param name="theDateTime">需要转换的日期</param>
        /// <returns>yyyy-MM-dd 格式的字符串</returns>
        public static string ToFullDateString(this DateTime theDateTime)
        {
            return theDateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 日期转换为 HH:mm:ss 格式的字符串
        /// </summary>
        /// <param name="theDateTime">需要转换的日期</param>
        /// <returns>HH:mm:ss 格式的字符串</returns>
        public static string ToFullTimeString(this DateTime theDateTime)
        {
            return theDateTime.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 日期转换为 HH:mm:ss.fff 格式的字符串
        /// </summary>
        /// <param name="theDateTime">需要转换的日期</param>
        /// <returns>HH:mm:ss.fff 格式的字符串</returns>
        public static string ToFullTimeWithMsString(this DateTime theDateTime)
        {
            return theDateTime.ToString("HH:mm:ss.fff");
        }

        /// <summary>
        /// 日期转换为转换多少小时前、多少分钟前、多少秒前 格式的字符串
        /// </summary>
        /// <param name="theDateTime">需要转换的日期</param>
        /// <returns>多少小时前、多少分钟前、多少秒前 格式的字符串</returns>
        public static string ToFromNowString(this DateTime theDateTime)
        {
            TimeSpan span = DateTime.Now - theDateTime;
            if (span.TotalDays > 60)
                return theDateTime.ToFullDateTimeString();
            else if (span.TotalDays > 30)
                return "1个月前";
            else if (span.TotalDays > 14)
                return "2周前";
            else if (span.TotalDays > 7)
                return "1周前";
            else if (span.TotalDays > 1)
                return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
            else if (span.TotalHours > 1)
                return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
            else if (span.TotalMinutes > 1)
                return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
            else if (span.TotalSeconds >= 1)
                return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
            else
                return "1秒前";
        }

    }

}
