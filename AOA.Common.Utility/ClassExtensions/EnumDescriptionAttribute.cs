using System;
using System.Reflection;
using System.Collections;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 把枚举值按照指定的文本显示
    /// <remarks>
    /// 一般通过枚举值的ToString()可以得到变量的文本，
    /// 但是有时候需要的到与之对应的更充分的文本，
    /// 这个类帮助达到此目的
    /// Date: 2006-3-25 
    /// Author: dearzp@hotmail.com
    /// </remarks>
    /// </summary>
    /// <example>
    /// [EnumDescriptionAttribute("中文数字")]
    /// enum MyEnum
    /// {
    ///		[EnumDescriptionAttribute("数字一")]
    /// 	One = 1, 
    /// 
    ///		[EnumDescriptionAttribute("数字二")]
    ///		Two, 
    /// 
    ///		[EnumDescriptionAttribute("数字三")]
    ///		Three
    /// }
    /// EnumDescriptionAttribute.GetEnumText(typeof(MyEnum));
    /// EnumDescriptionAttribute.GetFieldText(MyEnum.Two);
    /// EnumDescriptionAttribute.GetFieldTexts(typeof(MyEnum)); 
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        //private static object lockobj = new object();
        private string enumDisplayText;
        private int enumRank;
        private FieldInfo fieldIno;

        /// <summary>
        /// 描述枚举值
        /// </summary>
        /// <param name="enumDisplayText">描述内容</param>
        /// <param name="enumRank">排列顺序</param>
        public EnumDescriptionAttribute(string enumDisplayText, int enumRank)
        {
            this.enumDisplayText = enumDisplayText;
            this.enumRank = enumRank;
        }

        /// <summary>
        /// 描述枚举值，默认排序为5
        /// </summary>
        /// <param name="enumDisplayText">描述内容</param>
        public EnumDescriptionAttribute(string enumDisplayText)
            : this(enumDisplayText, 5) { }

        /// <summary>
        /// 描述枚举值
        /// </summary>
        protected EnumDescriptionAttribute()
        {

        }

        /// <summary>
        /// 枚举的显示文本
        /// </summary>
        public string EnumDisplayText
        {
            get { return this.enumDisplayText; }
        }

        /// <summary>
        /// 枚举的Rank
        /// </summary>
        public int EnumRank
        {
            get { return enumRank; }
        }

        /// <summary>
        /// 枚举值
        /// </summary>
        public int EnumValue
        {
            get { return (int)fieldIno.GetValue(null); }
        }

        /// <summary>
        /// 枚举的名称
        /// </summary>
        public string FieldName
        {
            get { return fieldIno.Name; }
        }

        #region 对枚举描述属性的解释相关函数

        /// <summary>
        /// 排序类型
        /// </summary>
        public enum SortType
        {
            /// <summary>
            ///按枚举顺序默认排序
            /// </summary>
            Default,
            /// <summary>
            /// 按描述值排序
            /// </summary>
            DisplayText,
            /// <summary>
            /// 按排序熵
            /// </summary>
            Rank
        }

        private static Hashtable cachedEnum = new Hashtable();

        /// <summary>
        /// 得到对枚举的描述文本
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static string GetEnumText(Type enumType)
        {
            EnumDescriptionAttribute[] eds = (EnumDescriptionAttribute[])enumType.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
            if (eds.Length != 1)
                return string.Empty;
            return eds[0].EnumDisplayText;
        }

        /// <summary>
        /// 获得指定枚举类型中，指定值的描述文本。
        /// </summary>
        /// <param name="enumValue">枚举值，不要作任何类型转换</param>
        /// <returns>描述字符串</returns>
        public static string GetFieldText(object enumValue)
        {
            try
            {
                EnumDescriptionAttribute[] descriptions = GetFieldTexts(enumValue.GetType());
                if (descriptions == null)
                    return string.Empty;

                foreach (EnumDescriptionAttribute ed in descriptions)
                {
                    if (ed.fieldIno.Name == enumValue.ToString())
                        return ed.EnumDisplayText;
                }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "GetFieldText", "EnumDescription");
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 得到枚举类型定义的所有文本，按定义的顺序返回
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="enumType">枚举类型</param>
        /// <returns>所有定义的文本</returns>
        public static EnumDescriptionAttribute[] GetFieldTexts(Type enumType)
        {
            return GetFieldTexts(enumType, SortType.Default);
        }

        /// <summary>
        /// 得到枚举类型定义的所有文本
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <param name="enumType">枚举类型</param>
        /// <param name="sortType">指定排序类型</param>
        /// <returns>所有定义的文本</returns>
        public static EnumDescriptionAttribute[] GetFieldTexts(Type enumType, SortType sortType)
        {
            EnumDescriptionAttribute[] descriptions = null;
            try
            {
                //// 加锁，防止并发下出错
                //lock (lockobj)
                //{
                try
                {
                    //缓存中没有找到，通过反射获得字段的描述信息
                    if (!cachedEnum.Contains(enumType.FullName))
                    {
                        FieldInfo[] fields = enumType.GetFields();
                        ArrayList edAL = new ArrayList();
                        foreach (FieldInfo fi in fields)
                        {
                            object[] eds = fi.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                            if (eds.Length != 1)
                                continue;
                            ((EnumDescriptionAttribute)eds[0]).fieldIno = fi;
                            edAL.Add(eds[0]);
                        }

                        descriptions = (EnumDescriptionAttribute[])edAL.ToArray(typeof(EnumDescriptionAttribute));
                        if (!cachedEnum.Contains(enumType.FullName))
                            cachedEnum.Add(enumType.FullName, descriptions);
                    }
                    else
                        descriptions = (EnumDescriptionAttribute[])cachedEnum[enumType.FullName];
                }
                catch (Exception ex)
                {
                    NLogUtility.ExceptionLog(ex, "GetFieldTexts", "EnumDescription", String.Format("通过反射获得{0}的字段的描述信息", enumType.Name));
                }
                //}
                if (descriptions.Length <= 0)
                    throw new NotSupportedException(String.Format("枚举类型[{0}]未定义属性EnumValueDescription", enumType.Name));

                // 按指定的属性冒泡排序
                for (int m = 0; m < descriptions.Length; m++)
                {
                    // 默认不排序
                    if (sortType == SortType.Default)
                        break;

                    for (int n = m; n < descriptions.Length; n++)
                    {
                        EnumDescriptionAttribute temp;
                        bool swap = false;

                        switch (sortType)
                        {
                            case SortType.Default:
                                break;
                            case SortType.DisplayText:
                                if (string.Compare(descriptions[m].EnumDisplayText, descriptions[n].EnumDisplayText) > 0)
                                    swap = true;
                                break;
                            case SortType.Rank:
                                if (descriptions[m].EnumRank > descriptions[n].EnumRank)
                                    swap = true;
                                break;
                        }

                        if (swap)
                        {
                            temp = descriptions[m];
                            descriptions[m] = descriptions[n];
                            descriptions[n] = temp;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "GetFieldTexts", "EnumDescription", enumType.Name);
            }

            return descriptions;
        }

        #endregion

    }

}
