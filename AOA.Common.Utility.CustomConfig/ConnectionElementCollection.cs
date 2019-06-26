using System.Configuration;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// <see cref="T:ConnectionElement"/> �ڵ㼯�ϣ����಻�ܱ��̳�
    /// </summary>
    public sealed class ConnectionElementCollection : ConfigurationElementCollection
    {

        #region ������
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="connectionName">Ԫ������</param>
        /// <returns>���Ӵ��ڵ�</returns>
        public new ConnectionElement this[string connectionName]
        {
            get
            {
                return GetByName(connectionName);
            }
        }
        #endregion

        /// <summary>
        /// ����һ�� <see cref="T:ConfigurationElement"/> �½ڵ�
        /// </summary>
        /// <returns><see cref="T:ConfigurationElement"/>�½ڵ�</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConnectionElement();
        }

        /// <summary>
        /// ����һ���ڵ��Key
        /// </summary>
        /// <param name="element">��Ҫ����Key�� <see cref="T:ConfigurationElement"/> �ڵ�</param>
        /// <returns>һ������ <see cref="T:ConfigurationElement"/> �ڵ�Key�� <see cref="T:Object"/></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            ConnectionElement ce = (ConnectionElement)element;
            return ce.ConnName;
        }

        /// <summary>
        /// ͨ������ȡ��ָ�������Ӵ��ڵ�
        /// </summary>
        /// <param name="connectionName">���Ӵ�����</param>
        /// <returns>���Ӵ��ڵ�</returns>
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
