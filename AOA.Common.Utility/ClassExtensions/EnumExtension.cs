using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 接口返回值错误码扩展类
    /// </summary>
    public static class EnumExtension
    {

        /// <summary>
        /// 获取枚举 EnumDescriptionAttribute 描述
        /// </summary>
        /// <param name="enumCode"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum enumCode)
        {
            return EnumDescriptionAttribute.GetFieldText(enumCode);
        }

        /// <summary>
        /// 获取枚举 EnumDescriptionAttribute 描述
        /// </summary>
        /// <param name="enumCode"></param>
        /// <returns></returns>
        public static string ToFullDescription(this Enum enumCode)
        {
            return String.Format("{0}: {1}", enumCode, EnumDescriptionAttribute.GetFieldText(enumCode));
        }

        /// <summary>
        /// 通过枚举值取枚举 EnumDescriptionAttribute 描述
        /// </summary>
        /// <param name="enumCode"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetItemDescriptionByValue(this Enum enumCode, int value)
        {
            foreach (var enObj in Enum.GetValues(enumCode.GetType()))
            {
                if (value == (int)enObj)
                    return EnumDescriptionAttribute.GetFieldText(enObj);
            }
            return null;
        }

        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="enumCode">枚举变量</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this Enum enumCode)
        {
            return GetDescription(enumCode, false);
        }

        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="enumCode">枚举变量</param>
        /// <param name="isTop">是否改变为返回该类、枚举类型的头 Description 属性，而不是当前的属性或枚举变量值的 Description 属性</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(this Enum enumCode, bool isTop)
        {
            if (enumCode == null)
                return string.Empty;

            try
            {
                Type enumType = enumCode.GetType();
                DescriptionAttribute dna = null;
                if (isTop)
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute));
                else
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType.GetField(Enum.GetName(enumType, enumCode)), typeof(DescriptionAttribute));

                if (dna != null && !string.IsNullOrEmpty(dna.Description))
                    return dna.Description;
            }
            catch
            {
            }
            return enumCode.ToString();
        }

    }

}
