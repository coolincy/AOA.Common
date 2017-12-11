using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Web;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.Net
{

    /// <summary>
    /// IP地址工具类
    /// </summary>
    public static class IPUtility
    {

        #region 把IP地址字符串转换为长整数 StringToLong
        /// <summary>
        /// 把IP地址字符串转换为长整数，a.b.c.d = a * 16777216 + b * 65536 + c * 256 + d
        /// </summary>
        /// <param name="ipStr">IP地址字符串</param>
        /// <returns>长整数</returns>
        public static long StringToLong(string ipStr)
        {
            return StringToLong(ipStr, true);
        }
        #endregion

        #region 把IP地址字符串转换为长整数 StringToLong
        /// <summary>
        /// 把IP地址字符串转换为长整数
        /// </summary>
        /// <param name="ipStr">IP地址字符串</param>
        /// <param name="bigEndian">True: a.b.c.d = a * 16777216 + b * 65536 + c * 256 + d, false 则相反</param>
        /// <returns>长整数</returns>
        public static long StringToLong(string ipStr, bool bigEndian)
        {
            // 分离出ip中的四个数字位
            string[] IPArray = ipStr.Trim().Split('.');

            // 取得各个数字
            int[] ipNum = new int[4];
            for (int i = 0; i < 4; i++)
            {
                ipNum[i] = int.Parse(IPArray[i].Trim());

                if (ipNum[i] > 255)
                    ipNum[i] = 255;
                else if (ipNum[i] < 0)
                    ipNum[i] = 0;
            }

            if (bigEndian) // 各个数字乘以相应的数量级(高位在前)
                return (uint)(ipNum[0] << 24) + (uint)(ipNum[1] << 16) + (uint)(ipNum[2] << 8) + (uint)(ipNum[3]);
            else
                return (uint)(ipNum[3] << 24) + (uint)(ipNum[2] << 16) + (uint)(ipNum[1] << 8) + (uint)(ipNum[0]);
        }
        #endregion

        #region 把IP地址长整数转换为字符串 LongToString
        /// <summary>
        /// 把IP地址长整数转换为字符串，a.b.c.d = a * 16777216 + b * 65536 + c * 256 + d
        /// </summary>
        /// <param name="ipLong">IP地址长整数</param>
        /// <returns>字符串IP地址</returns>
        public static string LongToString(long ipLong)
        {
            // 各个数字乘以相应的数量级
            return LongToString(ipLong, true);
        }
        #endregion

        #region 把IP地址长整数转换为字符串 LongToString
        /// <summary>
        /// 把IP地址长整数转换为字符串
        /// </summary>
        /// <param name="ipLong">IP地址长整数</param>
        /// <param name="bigEndian">True: a.b.c.d = a * 16777216 + b * 65536 + c * 256 + d, false 则相反</param>
        /// <returns>字符串IP地址</returns>
        public static string LongToString(long ipLong, bool bigEndian)
        {
            // 取得各个数字
            int[] ipNum = new int[4];
            ipNum[0] = (int)((ipLong & (255 << 24)) >> 24);
            ipNum[1] = (int)((ipLong & (255 << 16)) >> 16);
            ipNum[2] = (int)((ipLong & (255 << 8)) >> 8);
            ipNum[3] = (int)(ipLong & 255);

            // 各个数字乘以相应的数量级
            if (bigEndian) // 各个数字乘以相应的数量级(高位在前)
                return String.Format("{0}.{1}.{2}.{3}", ipNum[0], ipNum[1], ipNum[2], ipNum[3]);
            else
                return String.Format("{0}.{1}.{2}.{3}", ipNum[3], ipNum[2], ipNum[1], ipNum[0]);
        }
        #endregion

        #region IP是否在指定范围内 InRange
        /// <summary>
        /// IP是否在指定范围内
        /// </summary>
        /// <param name="checkIP">待检查的IP地址</param>
        /// <param name="startIP">起始IP地址，如“0.0.0.0”</param>
        /// <param name="endIP">终止IP地址，如“255.255.255.255”</param>
        /// <returns>是否在指定范围内</returns>
        public static bool InRange(string checkIP, string startIP, string endIP)
        {
            if (startIP.Trim() == "")
                return (StringToLong(checkIP) <= StringToLong(endIP));
            else if (endIP.Trim() == "")
                return (StringToLong(checkIP) >= StringToLong(startIP));
            else
            {
                long sIP = StringToLong(startIP);
                long eIP = StringToLong(endIP);
                long cIP = StringToLong(checkIP);

                if (sIP <= eIP)
                    return (sIP <= cIP && cIP <= eIP);
                else
                    return (eIP <= cIP && cIP <= sIP);
            }
        }
        #endregion

        #region 检查开始IP是否小于等于结束IP NotGreaterThan
        /// <summary>
        /// 检查开始IP是否小于等于结束IP
        /// </summary>
        /// <param name="startIP">起始IP地址，如“0.0.0.0”</param>
        /// <param name="endIP">终止IP地址，如“255.255.255.255”</param>
        /// <returns>开始IP是否小于等于结束IP</returns>
        public static bool NotGreaterThan(string startIP, string endIP)
        {
            return (StringToLong(startIP) <= StringToLong(endIP));
        }
        #endregion

        #region 获取本机所有IPV4地址列表 List<string> GetSelfIpv4List()
        /// <summary>
        /// 获取本机所有IPV4地址列表
        /// </summary>
        /// <returns>本机所有IPV4地址列表</returns>
        public static List<string> GetSelfIpv4List()
        {
            List<string> ips = new List<string>();
            try
            {
                IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    IPAddress ipa = IpEntry.AddressList[i];
                    if (ipa.AddressFamily == AddressFamily.InterNetwork)
                        ips.Add(ipa.ToString());
                }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "GetSelfIpv4List", "IPUtility");
            }
            return ips;
        }
        #endregion

        #region 获取本机所有IPV6地址列表 List<string> GetSelfIpv6List()
        /// <summary>
        /// 获取本机所有IPV6地址列表
        /// </summary>
        /// <returns>本机所有IPV6地址列表</returns>
        public static List<string> GetSelfIpv6List()
        {
            List<string> ips = new List<string>();
            try
            {
                IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    IPAddress ipa = IpEntry.AddressList[i];
                    if (ipa.AddressFamily == AddressFamily.InterNetworkV6)
                        ips.Add(ipa.ToString());
                }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "GetSelfIpv6List", "IPUtility");
            }
            return ips;
        }
        #endregion

    }

}
