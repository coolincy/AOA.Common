using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace AOA.Common.Utility
{

    /// <summary>
    /// appsettings.json帮助类
    /// </summary>
    public static class AppSettingsHelper
    {

        private static readonly ConcurrentDictionary<string, IConfigurationRoot> ConfigurationCache;

        static AppSettingsHelper()
        {
            ConfigurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
        }

        /// <summary>
        /// 获取配置信息根节点
        /// </summary>
        /// <param name="path">appsettings.json所在路径，如果没有指定，那么使用AOA.Common.Utility.dll文件所在路径</param>
        /// <param name="environmentName">环境配置名称</param>
        /// <returns></returns>
        public static IConfigurationRoot Get(string path = null, string environmentName = null)
        {
            var cacheKey = path + "#" + environmentName;
            return ConfigurationCache.GetOrAdd(cacheKey, _ => BuildConfiguration(path, environmentName));
        }

        private static IConfigurationRoot BuildConfiguration(string path = null, string environmentName = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (!string.IsNullOrWhiteSpace(environmentName))
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            builder = builder.AddEnvironmentVariables();

            return builder.Build();
        }

    }

}
