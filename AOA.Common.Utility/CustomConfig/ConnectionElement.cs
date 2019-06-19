using System;
using System.Configuration;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.CustomConfig
{

    /// <summary>
    /// �������ýڵ㣬���಻�ܱ��̳�
    /// </summary>
    public sealed class ConnectionElement : ConfigurationElement
    {

        /// <summary>
        /// ���Ӵ�����
        /// </summary>
        [ConfigurationProperty("connName", IsRequired = true, IsKey = true)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\.", MinLength = 0, MaxLength = 100)]
        public string ConnName
        {
            get { return (string)base["connName"]; }
            set { base["connName"] = value; }
        }

        /// <summary>
        /// ���ܷ���: 0 = ������, 1 = Ĭ��3DES+Base64����, 2 = Net91Com�����Ӵ�����
        /// </summary>
        [ConfigurationProperty("encryptType", IsRequired = false, IsKey = false, DefaultValue = 0)]
        [IntegerValidator(MinValue = 0, MaxValue = 2)]
        public int EncryptType
        {
            get { return (int)base["encryptType"]; }
            set { base["encryptType"] = value; }
        }

        /// <summary>
        /// Ĭ�����ݿ����Ӵ�
        /// </summary>
        [ConfigurationProperty("connString", IsRequired = false)]
        public string ConnString
        {
            get { return DecryptConnStr((string)base["connString"]); }
            set { base["connString"] = value; }
        }

        /// <summary>
        /// ���ڶ�ȡ���ݵ����ݿ����Ӵ�
        /// </summary>
        [ConfigurationProperty("readString", IsRequired = false)]
        public string ReadString
        {
            get { return DecryptConnStr((string)base["readString"]); }
            set { base["readString"] = value; }
        }

        /// <summary>
        /// ���ڸ������ݵ����ݿ����Ӵ�
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
