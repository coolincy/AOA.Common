﻿using System;
using System.Configuration;

using AOA.Common.Utility.ClassExtensions;
using AOA.Common.Utility.CustomConfig;

namespace AOA.Common.Utility
{

    /// <summary>
    /// 配置文件读取工具类
    /// </summary>
    public static class ConfigReader
    {

        #region GetString 获取配置字符串值
        /// <summary>
        /// 获取配置字符串值
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <param name="defaultStr">没有配置项时返回的字符串</param>
        /// <returns>字符串值</returns>
        public static string GetString(string configStr, string defaultStr)
        {
            string result = ConfigurationManager.AppSettings[configStr];
            if (result == null)
                result = defaultStr;
            return result;
        }
        #endregion

        #region GetString 获取配置字符串值
        /// <summary>
        /// 获取配置字符串值
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <returns>字符串值</returns>
        public static string GetString(string configStr)
        {
            return GetString(configStr, null);
        }
        #endregion

        #region GetInt 获取配置整数值
        /// <summary>
        /// 获取配置整数值
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="minValue">最小值，默认int.MinValue</param>
        /// <param name="maxValue">最大值，默认int.MaxValue</param>
        /// <returns>整数值</returns>
        public static int GetInt(string configStr, int defaultValue, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            string tmp = ConfigurationManager.AppSettings[configStr];
            int rValue = defaultValue;
            if (!string.IsNullOrEmpty(tmp))
            {
                if (tmp.IsInteger())
                {
                    rValue = Convert.ToInt32(tmp);
                    if (minValue > int.MinValue && rValue < minValue)
                        rValue = minValue;
                    if (maxValue < int.MaxValue && rValue > maxValue)
                        rValue = maxValue;
                }
            }

            return rValue;
        }
        #endregion

        #region GetInt 获取配置整数值，无值返回 -1
        /// <summary>
        /// 获取配置整数值，无值返回 -1
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <returns>整数值</returns>
        public static int GetInt(string configStr)
        {
            return GetInt(configStr, -1);
        }
        #endregion

        #region GetDecimal 获取配置浮点值
        /// <summary>
        /// 获取配置浮点值
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>浮点值</returns>
        public static decimal GetDecimal(string configStr, decimal defaultValue)
        {
            string tmp = ConfigurationManager.AppSettings[configStr];
            if (!string.IsNullOrEmpty(tmp))
            {
                if (tmp.IsFloat())
                    return Convert.ToDecimal(tmp);
                else
                    return defaultValue;
            }
            else
                return defaultValue;
        }
        #endregion

        #region GetDecimal 获取配置浮点值，无值返回-1
        /// <summary>
        /// 获取配置浮点值，无值返回-1
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <returns>浮点值</returns>
        public static decimal GetDecimal(string configStr)
        {
            return GetDecimal(configStr, -1);
        }
        #endregion

        #region GetBool 获取配置布尔值
        /// <summary>
        /// 获取配置布尔值(1或true为真，不区分大小写)
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>布尔值</returns>
        public static bool GetBool(string configStr, bool defaultValue)
        {
            string tmp = ConfigurationManager.AppSettings[configStr];
            if (!string.IsNullOrEmpty(tmp))
                return (tmp == "1" || tmp.ToLower() == "true");
            else
                return defaultValue;
        }
        #endregion

        #region GetBool 获取配置布尔值，无值返回 false
        /// <summary>
        /// 获取配置布尔值，无值返回 false
        /// </summary>
        /// <param name="configStr">配置名称</param>
        /// <returns>布尔值</returns>
        public static bool GetBool(string configStr)
        {
            return GetBool(configStr, false);
        }
        #endregion

        #region GetConnectionString 获取已经解密后的数据库连接串
        /// <summary>
        /// 获取已经解密后的数据库连接串，如果解密过程失败，返回原串
        /// </summary>
        /// <param name="connectionName">连接串名称</param>
        /// <returns>连接串</returns>
        public static string GetConnectionString(string connectionName)
        {
            string encryptConnectionString = string.Empty;
            string decryptConnectionString = string.Empty;

            if (ConfigurationManager.ConnectionStrings[connectionName] != null)
                encryptConnectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            if (!string.IsNullOrEmpty(encryptConnectionString))
            {
                try
                {
                    decryptConnectionString = encryptConnectionString.TripleDesDecryptFromBase64ToString();
                }
                catch
                {
                    decryptConnectionString = encryptConnectionString;
                }
            }
            return decryptConnectionString;
        }
        #endregion

        #region GetDefaultConnectionString 获取默认数据库连接串
        /// <summary>
        /// 获取默认数据库连接串，来自配置 AOA.Common.Utility.CustomConfig.ConnectionSection
        /// </summary>
        /// <param name="connectionName">连接串名称</param>
        /// <returns>连接串(读取用)</returns>
        public static string GetDefaultConnectionString(string connectionName)
        {
            ConnectionElement connectionElement = CustomConfigHelper.Connections[connectionName];
            if (connectionElement != null)
                return connectionElement.ConnString;
            else
                return GetConnectionString(connectionName);
        }
        #endregion

        #region GetReadConnectionString 获取读取用数据库连接串
        /// <summary>
        /// 获取读取用数据库连接串，来自配置 AOA.Common.Utility.CustomConfig.ConnectionSection
        /// </summary>
        /// <param name="connectionName">连接串名称</param>
        /// <returns>连接串(读取用)</returns>
        public static string GetReadConnectionString(string connectionName)
        {
            ConnectionElement connectionElement = CustomConfigHelper.Connections[connectionName];
            if (connectionElement != null)
                return string.IsNullOrEmpty(connectionElement.ReadString) ? connectionElement.ConnString : connectionElement.ReadString;
            else
                return GetConnectionString(connectionName);
        }
        #endregion

        #region GetWriteConnectionString 获取写入用数据库连接串
        /// <summary>
        /// 获取写入用数据库连接串，来自配置 AOA.Common.Utility.CustomConfig.ConnectionSection
        /// </summary>
        /// <param name="connectionName">连接串名称</param>
        /// <returns>连接串(写入用)</returns>
        public static string GetWriteConnectionString(string connectionName)
        {
            ConnectionElement connectionElement = CustomConfigHelper.Connections[connectionName];
            if (connectionElement != null)
                return string.IsNullOrEmpty(connectionElement.WriteString) ? connectionElement.ConnString : connectionElement.ReadString;
            else
                return GetConnectionString(connectionName);
        }
        #endregion

    }
}
