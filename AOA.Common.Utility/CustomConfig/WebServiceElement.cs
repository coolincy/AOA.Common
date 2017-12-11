using System;
using System.Configuration;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// 连接配置节点，该类不能被继承
    /// </summary>
    public sealed class WebServiceElement : ConfigurationElement
    {

        /// <summary>
        /// 添加WebService引用后的完整类名+方法名称
        /// </summary>
        [ConfigurationProperty("serviceName", IsRequired = true, IsKey = true)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MaxLength = 200)]
        public string ServiceName
        {
            get { return (string)base["serviceName"]; }
            set { base["connName"] = value; }
        }

        /// <summary>
        /// WebService的Url地址
        /// </summary>
        [ConfigurationProperty("url", IsRequired = false)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["connString"] = value; }
        }

    }

}
