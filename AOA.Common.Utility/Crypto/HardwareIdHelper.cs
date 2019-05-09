using System;
using System.Runtime.InteropServices;

using AOA.Common.Utility.Net;

namespace AOA.Common.Utility.Crypto
{

    /// <summary>
    /// 硬件唯一Id帮助类
    /// </summary>
    public class HardwareIdHelper
    {

        private static byte[] default3DesKey =
        {
            0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
            0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x11,
            0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x11, 0x22
        };
        private static byte[] default3DesIV = { 0x11, 0x22, 0x33, 0x44, 0x11, 0x22, 0x33, 0x44 };

        private static void ConfirmKeyIV(ref byte[] tDesKey, ref byte[] tDesIV)
        {
            if (tDesKey == null)
                tDesKey = default3DesKey;
            if (tDesIV == null)
                tDesIV = default3DesIV;
        }

        /// <summary>
        /// 获取系统硬件唯一Id(依据CPU、MAC地址等)(16进制字符串)
        /// </summary>
        /// <param name="tDesKey">3DES Key, 24字节数组</param>
        /// <param name="tDesIV">3DES IV, 8字节数组</param>
        /// <returns>系统硬件唯一Id(16进制字符串)</returns>
        public static string GetHardwareId(byte[] tDesKey = null, byte[] tDesIV = null)
        {
            ConfirmKeyIV(ref tDesKey, ref tDesIV);
            var pCnt = Environment.ProcessorCount;
            var pArch = RuntimeInformation.ProcessArchitecture;
            var s = $"{pCnt}|{pArch}";
            var macList = NetworkInterfaceHelper.GetPhysicalAddresss();
            foreach (var item in macList)
            {
                s = $"{s}|{item}";
            }
            return Cryptography.TripleDesEncryptToHex(tDesKey, tDesIV, s);
        }

        /// <summary>
        /// 解析系统硬件唯一Id
        /// </summary>
        /// <param name="hardwareId">系统硬件唯一Id(16进制字符串)</param>
        /// <param name="tDesKey">3DES Key, 24字节数组</param>
        /// <param name="tDesIV">3DES IV, 8字节数组</param>
        /// <returns></returns>
        public static string[] GetInfoFromHardwareId(string hardwareId, byte[] tDesKey = null, byte[] tDesIV = null)
        {
            ConfirmKeyIV(ref tDesKey, ref tDesIV);
            var hid = Cryptography.TripleDesDecryptFromHexToString(tDesKey, tDesIV, hardwareId);
            return hid?.Split(new char[] { '|' });
        }

        /// <summary>
        /// 获取系统硬件唯一Id(依据CPU、MAC地址等)(Base64)
        /// </summary>
        /// <param name="tDesKey">3DES Key, 24字节数组</param>
        /// <param name="tDesIV">3DES IV, 8字节数组</param>
        /// <returns>系统硬件唯一Id(Base64)</returns>
        public static string GetHardwareIdBase64(byte[] tDesKey = null, byte[] tDesIV = null)
        {
            ConfirmKeyIV(ref tDesKey, ref tDesIV);
            var pCnt = Environment.ProcessorCount;
            var pArch = RuntimeInformation.ProcessArchitecture;
            var s = $"{pCnt}|{pArch}";
            var macList = NetworkInterfaceHelper.GetPhysicalAddresss();
            foreach (var item in macList)
            {
                s = $"{s}|{item}";
            }
            return Cryptography.TripleDesEncryptToBase64(tDesKey, tDesIV, s);
        }

        /// <summary>
        /// 解析系统硬件唯一Id
        /// </summary>
        /// <param name="hardwareId">系统硬件唯一Id(Base64)</param>
        /// <param name="tDesKey">3DES Key, 24字节数组</param>
        /// <param name="tDesIV">3DES IV, 8字节数组</param>
        /// <returns></returns>
        public static string[] GetInfoFromHardwareIdBase64(string hardwareId, byte[] tDesKey = null, byte[] tDesIV = null)
        {
            ConfirmKeyIV(ref tDesKey, ref tDesIV);
            var hid = Cryptography.TripleDesDecryptFromBase64ToString(tDesKey, tDesIV, hardwareId);
            return hid?.Split(new char[] { '|' });
        }

    }

}
