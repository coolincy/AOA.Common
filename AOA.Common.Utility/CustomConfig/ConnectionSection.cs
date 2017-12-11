using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// 连接串配置项
    /// </summary>
    public sealed class ConnectionSection : ConfigurationSection
    {
        ConnectionElement element;

        /// <summary>
        /// 构建函数
        /// </summary>
        public ConnectionSection()
        {
            element = new ConnectionElement();
        }

        /// <summary>
        /// 所有连接串配置
        /// </summary>
        [ConfigurationProperty("connections", IsRequired = true)]
        public ConnectionElementCollection Connections
        {
            get
            {
                return (ConnectionElementCollection)base["connections"];
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="reader">System.Xml.XmlReader 对象，它从配置文件进行读取。</param>
        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            base.DeserializeSection(reader);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="parentElement">要在执行分离时充当父对象的 System.Configuration.ConfigurationElement 实例。</param>
        /// <param name="name">要创建的节的名称。</param>
        /// <param name="saveMode">写入到字符串中时要使用的 System.Configuration.ConfigurationSaveMode 实例。</param>
        /// <returns>XML 字符串，包含 System.Configuration.ConfigurationSection 对象的分离视图。</returns>
        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            return base.SerializeSection(parentElement, name, saveMode);
        }

    }

}
