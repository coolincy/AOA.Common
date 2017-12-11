using System;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 正则表达式字符串类
    /// </summary>
    public class RegexStrings
    {

        // ----------------------------------------------------------------------------------------
        // 数字
        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// 整数
        /// </summary>
        public const string IS_INT = @"^-?\d+$";
        //public const string IS_INT = @"^-?[1-9]\d*$"; // 整数

        /// <summary>
        /// 正整数
        /// </summary>
        public const string IS_INT_POSITIVE = @"^[0-9]*[1-9][0-9]*$";
        //public const string IS_INT_POSITIVE = @"^[1-9]\d*$"; // 正整数

        /// <summary>
        /// 负整数
        /// </summary>
        public const string IS_INT_NEGATIVE = @"^-[0-9]*[1-9][0-9]*$";
        //public const string IS_INT_NEGATIVE = @"^-[1-9]\d*$"; // 负整数

        //public const string IS_INT = @"^-[1-9]\d*|0$"; // 非正整数（负整数+0）
        //public const string IS_INT = @"^((-\d+)|(0+))$"; // 非正整数（负整数+0）

        //public const string IS_INT = @"^\d+$"; // 非负整数（正整数+0）
        //public const string IS_INT = @"^[1-9]\d*|0$"; // 非负整数（正整数+0）

        /// <summary>
        /// 浮点数
        /// </summary>
        public const string IS_FLOAT = @"^(-?\d+)(\.\d+)?$";
        //public const string IS_FLOAT = @"^-?([1-9]\d*.\d*|0.\d*[1-9]\d*|0?.0+|0)$"; // 浮点数

        //public const string IS_FLOAT = @"^[1-9]\d*.\d*|0.\d*[1-9]\d*$"; // 正浮点数
        //public const string IS_FLOAT = @"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$"; // 正浮点数

        //public const string IS_FLOAT = @"^-([1-9]\d*.\d*|0.\d*[1-9]\d*)$"; // 负浮点数
        //public const string IS_FLOAT = @"^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$"; // 负浮点数

        //public const string IS_FLOAT = @"^((-\d+(\.\d+)?)|(0+(\.0+)?))$"; // 非正浮点数（负浮点数+0）
        //public const string IS_FLOAT = @"^(-([1-9]\d*.\d*|0.\d*[1-9]\d*))|0?.0+|0$"; // 非正浮点数（负浮点数+0） 处理大量数据时有用，具体应用时注意修正

        //public const string IS_FLOAT = @"^\d+(\.\d+)?$"; // 非负浮点数（正浮点数+0）
        //public const string IS_FLOAT = @"^[1-9]\d*.\d*|0.\d*[1-9]\d*|0?.0+|0$"; // 非负浮点数（正浮点数+0）

        /// <summary>
        /// 金额
        /// </summary>
        public const string IS_MONEY = @"^[0-9]+\.[0-9]{2}$";

        // ----------------------------------------------------------------------------------------
        // 网络
        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// IP地址
        /// </summary>
        public const string IS_IP = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5]).(\d{1,2}|1\d\d|2[0-4]\d|25[0-5]).(\d{1,2}|1\d\d|2[0-4]\d|25[0-5]).(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$";
        /// <summary>
        /// 网址URL的正则表达式：这个基本可以满足需求
        /// </summary> 
        public const string IS_URL = @"[a-zA-z]+://[^\S]*";
        //public const string IS_URL = @"^[a-zA-z]+://(\w+(-\w+)*)(\.(\w+(-\w+)*))*(\?\S*)?$"; // URL

        //public const string IS_EMAIL = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$"; // EMail地址
        //public const string IS_EMAIL = @"\w+([-+.]\w+)*@\w+([-.]\w+)*.\w+([-.]\w+)*"; // Email地址的正则表达式：表单验证时很实用
        //public const string IS_EMAIL = @"^([\w-.]+)@(([[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.)|(([\w-]+.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(]?)$"; // Email

        /// <summary>
        /// HTML标记的正则表达式：对于复杂的嵌套标记依旧无能为力
        /// </summary>
        public const string IS_HTML = @"<(\S*?)[^>]*>.*?</1>|<.*?/>";
        /// <summary>
        /// 腾讯QQ号：腾讯QQ号从10000开始
        /// </summary>
        public const string IS_QQ = @"[1-9][0-9]{4,}";

        // ----------------------------------------------------------------------------------------
        // 日期
        // ----------------------------------------------------------------------------------------

        //public const string IS_DATE = @"/^(\d{2}|\d{4})-((0([1-9]{1}))|(1[1|2]))-(([0-2]([1-9]{1}))|(3[0|1]))$/"; // 年-月-日
        /// <summary>
        /// YYYY-MM-DD基本上把闰年和2月等的情况都考虑进去了
        /// </summary>
        public const string IS_DATE = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";

        // ----------------------------------------------------------------------------------------
        // 特殊字符串
        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// 空白行的正则表达式：可以用来删除空白行
        /// </summary>
        public const string IS_BLANK = @"\n\S*\r";
        /// <summary>
        /// 由26个英文字母的大写组成的字符串
        /// </summary>
        public const string IS_UPPER_CHAR = @"^[A-Z]+$";
        /// <summary>
        /// 由26个英文字母的小写组成的字符串
        /// </summary>
        public const string IS_LOWER_CHAR = @"^[a-z]+$";
        /// <summary>
        /// 由26个英文字母组成的字符串
        /// </summary>
        public const string IS_CHAR = @"^[A-Za-z]+$";
        /// <summary>
        /// 由26个英文字母和数字组成的字符串
        /// </summary>
        public const string IS_CHAR_DIGITAL = @"^[A-Za-z0-9]+$";

        //public const string IS_INT = @"^[A-Za-z][A-Za-z0-9_]{4,15}$"; // 帐号是否合法(字母开头，允许5-16字节，允许字母数字下划线)：表单验证时很实用

        //public const string IS_INT = @"^\S*|\S*$"; // 首尾空白字符的正则表达式：可以用来删除行首行尾的空白字符(包括空格、制表符、换页符等等)，非常有用的表达式
        //public const string IS_INT = @"^\w+$"; // 由数字、26个英文字母或者下划线组成的字符串

        // ----------------------------------------------------------------------------------------
        // 中文
        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// 双字节字符(包括汉字在内)：可以用来计算字符串的长度（一个双字节字符长度计2，ASCII字符计1）
        /// </summary>
        public const string IS_DOUBLE_CHAR = @"[^x00-xff]";
        /// <summary>
        /// 中文字符的正则表达式：中文还真是个头疼的事，有了这个表达式就好办了
        /// </summary>
        public const string IS_CHN_CHAR_UNICODE = @"[u4e00-u9fa5]";

        // ----------------------------------------------------------------------------------------
        // 电话号码等
        // ----------------------------------------------------------------------------------------

        /// <summary>
        /// 国内电话号码：形式如0511-4405222或021-87888822
        /// </summary>
        public const string IS_CHN_PHONE_NO = @"\d{3}-\d{8}|\d{4}-\d{7}";

        /// <summary>
        /// 国内手机号，13,14,15,18开头的11位号码
        /// </summary>
        public const string IS_CHN_MOBILEPHONE_NO = @"^1[3|4|5|8][0-9]{9}$";

        /// <summary>
        /// 中国邮政编码：中国邮政编码为6位数字
        /// </summary>
        public const string IS_CHN_POSTCODE = @"[1-9]\d{5}(?!\d)";

    }

}
