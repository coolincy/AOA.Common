using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace AOA.Common.Utility.Hardware
{

    /// <summary>
    /// 获取CPU信息
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
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("dmidecode", "-t 4")
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
                    if (item.ToLower().Contains("id:"))
                    {
                        id = item.ToLower().Replace("id:", "").Replace(" ", "");
                    }
                }
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

    }

}
