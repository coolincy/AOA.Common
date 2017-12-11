using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// 自定义配置节帮助类
    /// </summary>
    public static class CustomConfigHelper
    {

        #region Connections 连接串集合
        private static ConnectionElementCollection connections;
        /// <summary>
        /// 连接串集合
        /// </summary>
        public static ConnectionElementCollection Connections
        {
            get { return connections; }
            set { connections = value; }
        }
        #endregion

        #region CustomConfigHelper 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static CustomConfigHelper()
        {
            // Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //ConfigurationSectionGroup configGroup = config.GetSectionGroup("AOA.Common.Utility");
            //foreach (ConfigurationSection element in configGroup.Sections)
            //{
            //    if (element is ConnectionSection)
            //    {
            //        ConnectionSection connectionSection = element as ConnectionSection;
            //        connections = connectionSection.Connections;
            //        break;
            //    }
            //}

            object element = ConfigurationManager.GetSection("ConnectionConfig");
            if (element is ConnectionSection)
            {
                ConnectionSection connectionSection = element as ConnectionSection;
                connections = connectionSection.Connections;
            }
        }
        #endregion

    }

}
