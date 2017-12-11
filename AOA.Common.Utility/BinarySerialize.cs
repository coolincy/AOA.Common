using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AOA.Common.Utility
{

    /// <summary>
    /// 二进制序列化
    /// </summary>
    public static class BinarySerialize
    {

        /// <summary>
        /// 把对象序列化为字节数组
        /// </summary>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            return bytes;
        }

        /// <summary>
        /// 把字节数组反序列化成对象
        /// </summary>
        public static object DeserializeObject(byte[] bytes)
        {
            if (bytes == null)
                return null;

            object obj = null;
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);
            ms.Close();
            return obj;
        }

    }

}
