using System.Text.RegularExpressions;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 字符串判别功能扩展
    /// </summary>
    public static class StringJudgement
    {

        #region IsIPAddress 判断是否是IP格式
        /// <summary>
        /// 判断是否是IP地址格式 0.0.0.0-255.255.255.255
        /// </summary>
        /// <param name="sourceString">待判断的IP地址</param>
        /// <returns>true or false</returns>
        public static bool IsIPAddress(this string sourceString)
        {
            if (sourceString == null || sourceString == string.Empty)
                return false;

            return new Regex(RegexStrings.IS_IP, RegexOptions.IgnoreCase).IsMatch(sourceString);
        }
        #endregion

        #region IsNumber 检查字符串是否都是数字字符
        /// <summary>
        /// 检查字符串是否都是数字字符
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        public static bool IsNumber(this string sourceString)
        {
            for (int i = 0; i < sourceString.Length; i++)
            {
                if (!char.IsNumber(sourceString, i))
                    return false;
            }
            return true;
        }
        #endregion

        #region IsInteger 检查字符串是否是整数
        /// <summary>
        /// 检查字符串是否是整数
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        public static bool IsInteger(this string sourceString)
        {
            if (sourceString == null || sourceString == string.Empty)
                return false;

            return new Regex(RegexStrings.IS_INT, RegexOptions.IgnoreCase).IsMatch(sourceString);
        }
        #endregion

        #region IsIntegerPositive 检查字符串是否是正整数
        /// <summary>
        /// 检查字符串是否是正整数
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        public static bool IsIntegerPositive(this string sourceString)
        {
            if (sourceString == null || sourceString == string.Empty)
                return false;

            return new Regex(RegexStrings.IS_INT_POSITIVE, RegexOptions.IgnoreCase).IsMatch(sourceString);
        }
        #endregion

        #region IsIntegerUnNegative 检查字符串是否是非负整数
        /// <summary>
        /// 检查字符串是否是非负整数
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        public static bool IsIntegerUnNegative(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
                return false;
            if (sourceString.Trim() == "0")
                return true;

            return new Regex(RegexStrings.IS_INT_POSITIVE, RegexOptions.IgnoreCase).IsMatch(sourceString);
        }
        #endregion

        #region IsFloat 检查字符串是否是浮点数
        /// <summary>
        /// 检查字符串是否是浮点数
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        public static bool IsFloat(this string sourceString)
        {
            if (sourceString == null || sourceString == string.Empty)
                return false;

            return new Regex(RegexStrings.IS_FLOAT, RegexOptions.IgnoreCase).IsMatch(sourceString);
        }
        #endregion

        #region IsMoney 检查字符串是否是金额
        /// <summary>
        /// 检查字符串是否是金额
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <returns></returns>
        public static bool IsMoney(this string sourceString)
        {
            if (sourceString == null || sourceString == string.Empty)
                return false;

            return new Regex(RegexStrings.IS_MONEY, RegexOptions.IgnoreCase).IsMatch(sourceString);
        }
        #endregion

        #region IsSomething 检查字符串是否符合正则表达式
        /// <summary>
        /// 检查字符串是否符合正则表达式
        /// </summary>
        /// <param name="sourceString">源字符串</param>
        /// <param name="regexStr">正则表达式字符串</param>
        /// <param name="options">A bitwise combination of the enumeration values that modify the regular expression.</param>
        /// <returns></returns>
        public static bool IsSomething(this string sourceString, string regexStr, RegexOptions options = RegexOptions.IgnoreCase)
        {
            if (sourceString == null || sourceString == string.Empty)
                return false;

            return new Regex(regexStr, options).IsMatch(sourceString);
        }
        #endregion

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 是否Int32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt32(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                int tempInt;
                return int.TryParse(str, out tempInt);
            }
            return false;
        }

        /// <summary>
        /// 是否Int64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt64(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                long tempInt;
                return long.TryParse(str, out tempInt);
            }
            return false;
        }

        /// <summary>
        /// 是否Double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDouble(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                double tempInt;
                return double.TryParse(str, out tempInt);
            }
            return false;
        }

    }

}
