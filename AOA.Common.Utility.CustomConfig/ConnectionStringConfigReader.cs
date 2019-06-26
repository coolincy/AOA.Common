using System.Configuration;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// 配置文件读取工具类
    /// </summary>
    public static class ConnectionStringConfigReader
    {

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
