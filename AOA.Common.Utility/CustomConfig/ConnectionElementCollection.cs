using System.Configuration;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// <see cref="T:ConnectionElement"/> 节点集合，该类不能被继承
    /// </summary>
    public sealed class ConnectionElementCollection : ConfigurationElementCollection
    {

        #region 索引器
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="connectionName">元素索引</param>
        /// <returns>连接串节点</returns>
        public new ConnectionElement this[string connectionName]
        {
            get
            {
                return GetByName(connectionName);
            }
        }
        #endregion

        /// <summary>
        /// 创建一个 <see cref="T:ConfigurationElement"/> 新节点
        /// </summary>
        /// <returns><see cref="T:ConfigurationElement"/>新节点</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionElement();
        }

        /// <summary>
        /// 返回一个节点的Key
        /// </summary>
        /// <param name="element">需要返回Key的 <see cref="T:ConfigurationElement"/> 节点</param>
        /// <returns>一个包含 <see cref="T:ConfigurationElement"/> 节点Key的 <see cref="T:Object"/></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            ConnectionElement ce = (ConnectionElement)element;
            return ce.ConnName;
        }

        /// <summary>
        /// 通过名称取得指定的连接串节点
        /// </summary>
        /// <param name="connectionName">连接串名称</param>
        /// <returns>连接串节点</returns>
        public ConnectionElement GetByName(string connectionName)
        {
            ConnectionElement connectionElement = null;
            foreach (ConnectionElement element in this)
            {
                if (element.ConnName.ToLower() == connectionName.ToLower())
                {
                    connectionElement = element;
                    break;
                }
            }
            return connectionElement;
        }

    }

}
