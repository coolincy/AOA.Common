using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace AOA.Common.Utility.Hardware
{

    /// <summary>
    /// 获取CPU信息
    /// 参考 https://www.cnblogs.com/pengze0902/p/5977247.html
    /// </summary>
    public class CPUInfoHelper
    {

        /// <summary>
        /// Linux下通过dmidecode获取CPUID
        /// </summary>
        /// <returns></returns>
        public static string GetCPUId()
        {
            string id = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                id = GetFromDmiDecode(4, "id:");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    id = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
            }
            return id;
        }

        /// <summary>
        /// Linux下通过dmidecode获取系统UUID，Windows获取主板序列号
        /// </summary>
        /// <returns></returns>
        public static string GetSystemUUID()
        {
            string id = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                id = GetFromDmiDecode(1, "uuid:");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var mos = new ManagementObjectSearcher("Select * from Win32_BaseBoard");
                foreach (var o in mos.Get())
                {
                    var mo = (ManagementObject)o;
                    id = mo["SerialNumber"].ToString();
                }
            }
            return id;
        }

        private static string GetFromDmiDecode(int type, string idName)
        {
            string id = "";
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("dmidecode", "-t " + type)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            var hddInfo = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();

            var lines = hddInfo.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in lines)
            {
                if (item.ToLower().Contains(idName))
                {
                    id = item.ToLower().Replace(idName, "").Replace(" ", "");
                }
            }
            return id;
        }

    }

}
