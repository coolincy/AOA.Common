using System;
using System.Configuration;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// 连接配置节点，该类不能被继承
    /// </summary>
    public sealed class ConnectionElement : ConfigurationElement
    {

        /// <summary>
        /// 连接串名称
        /// </summary>
        [ConfigurationProperty("connName", IsRequired = true, IsKey = true)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\.", MinLength = 0, MaxLength = 100)]
        public string ConnName
        {
            get { return (string)base["connName"]; }
            set { base["connName"] = value; }
        }

        /// <summary>
        /// 加密方法: 0 = 不加密, 1 = 默认3DES+Base64加密, 2 = Net91Com的连接串加密
        /// </summary>
        [ConfigurationProperty("encryptType", IsRequired = false, IsKey = false, DefaultValue = 0)]
        [IntegerValidator(MinValue = 0, MaxValue = 2)]
        public int EncryptType
        {
            get { return (int)base["encryptType"]; }
            set { base["encryptType"] = value; }
        }

        /// <summary>
        /// 默认数据库连接串
        /// </summary>
        [ConfigurationProperty("connString", IsRequired = false)]
        public string ConnString
        {
            get { return DecryptConnStr((string)base["connString"]); }
            set { base["connString"] = value; }
        }

        /// <summary>
        /// 用于读取数据的数据库连接串
        /// </summary>
        [ConfigurationProperty("readString", IsRequired = false)]
        public string ReadString
        {
            get { return DecryptConnStr((string)base["readString"]); }
            set { base["readString"] = value; }
        }

        /// <summary>
        /// 用于更改数据的数据库连接串
        /// </summary>
        [ConfigurationProperty("writeString", IsRequired = false)]
        public string WriteString
        {
            get { return DecryptConnStr((string)base["writeString"]); }
            set { base["writeString"] = value; }
        }

        private string DecryptConnStr(string inStr)
        {
            try
            {
                switch (EncryptType)
                {
                    case 1:
                        inStr = inStr.TripleDesDecryptFromBase64ToString();
                        break;
                    case 2:
                        inStr = Net91ComCryptoHelper.DES_Decrypt(inStr);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "ConnectionElement_DecryptConnStr", "CustomConfig");
            }

            return inStr;
        }
    }

}
