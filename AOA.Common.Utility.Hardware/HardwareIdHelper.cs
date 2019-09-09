using AOA.Common.Utility.Crypto;

using System;
using System.Runtime.InteropServices;

namespace AOA.Common.Utility.Hardware
{

    /// <summary>
    /// 硬件唯一Id帮助类，使用3DES加密相关内容
    /// </summary>
    public class HardwareIdHelper
    {

        private static readonly byte[] default3DesKey =
        {
            0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88,
            0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x11,
            0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x11, 0x22
        };
        private static readonly byte[] default3DesIV = { 0x11, 0x22, 0x33, 0x44, 0x11, 0x22, 0x33, 0x44 };

        private static void ConfirmKeyIV(ref byte[] tDesKey, ref byte[] tDesIV)
        {
            if (tDesKey == null)
                tDesKey = default3DesKey;
            if (tDesIV == null)
                tDesIV = default3DesIV;
        }

        private static string GetOriginHwId()
        {
            var sysId = CPUInfoHelper.GetSystemUUID();
            var cpuId = CPUInfoHelper.GetCPUId();
            var pCnt = Environment.ProcessorCount;
            var pArch = RuntimeInformation.ProcessArchitecture;
            var s = $"{sysId}|{cpuId}|{pCnt}|{pArch}";
            var macList = NetworkInterfaceHelper.GetPhysicalAddresss();
            foreach (var item in macList)
            {
                s = $"{s}|{item}";
            }
            return s;
        }

        /// <summary>
        /// 获取系统硬件唯一Id(依据 UUID, CPUID, CPU核数, CPU架构, MAC地址列表)(16进制字符串)
        /// </summary>
        /// <param name="tDesKey">3DES Key, 24字节数组</param>
        /// <param name="tDesIV">3DES IV, 8字节数组</param>
        /// <returns>系统硬件唯一Id(16进制字符串)</returns>
        public static string GetHardwareId(byte[] tDesKey = null, byte[] tDesIV = null)
        {
            ConfirmKeyIV(ref tDesKey, ref tDesIV);
            return Cryptography.TripleDesEncryptToHex(tDesKey, tDesIV, GetOriginHwId());
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
        /// 获取系统硬件唯一Id(依据 UUID, CPUID, CPU核数, CPU架构, MAC地址列表)(16进制字符串)(Base64)
        /// </summary>
        /// <param name="tDesKey">3DES Key, 24字节数组</param>
        /// <param name="tDesIV">3DES IV, 8字节数组</param>
        /// <returns>系统硬件唯一Id(Base64)</returns>
        public static string GetHardwareIdBase64(byte[] tDesKey = null, byte[] tDesIV = null)
        {
            ConfirmKeyIV(ref tDesKey, ref tDesIV);
            return Cryptography.TripleDesEncryptToBase64(tDesKey, tDesIV, GetOriginHwId());
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
