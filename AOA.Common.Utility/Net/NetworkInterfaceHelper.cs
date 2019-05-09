using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.Net
{

    /// <summary>
    /// 网络接口帮助类
    /// </summary>
    public class NetworkInterfaceHelper
    {

        /// <summary>
        /// 获取所有网卡的mac物理地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPhysicalAddresss()
        {
            List<string> macList = new List<string>();
            try
            {
                IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                // Console.WriteLine("Interface information for {0}.{1}     ", computerProperties.HostName, computerProperties.DomainName);
                if (nics == null || nics.Length < 1)
                {
                    // Console.WriteLine("  No network interfaces found.");
                    return null;
                }

                // Console.WriteLine("  Number of interfaces ... : {0}", nics.Length);
                foreach (NetworkInterface adapter in nics)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    // Console.WriteLine();
                    // Console.WriteLine(adapter.Description);
                    // Console.WriteLine(string.Empty.PadLeft(adapter.Description.Length, '='));
                    // Console.WriteLine("  Interface type ......... : {0}", adapter.NetworkInterfaceType);
                    // Console.Write("  Physical address ....... : ");
                    PhysicalAddress address = adapter.GetPhysicalAddress();
                    byte[] bytes = address.GetAddressBytes();
                    var mac = StringEncode.ByteArrayToHexString(bytes);
                    if (!string.IsNullOrEmpty(mac)
                        && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        macList.Add(mac);
                    // Console.WriteLine(mac);
                }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "GetPhysicalAddresss", "NetworkInterfaceHelper");
            }
            return macList;
        }

    }

}
