using System;
using System.Collections.Generic;
using System.Linq;

using AOA.Common.Utility.ClassExtensions;
using Microsoft.Extensions.Configuration;
using NLog;

namespace AOA.Common.Utility
{

    /// <summary>
    /// NLogUtility 使用NLog进行日志记录，日志配置见 NLog.config
    /// </summary>
    public class NLogUtility
    {

        private static readonly object loggerLock = new object();
        private static readonly Dictionary<string, Logger> loggerList = new Dictionary<string, Logger>();

        #region GetLogger(string loggerName) 获取Logger(使用缓存)
        /// <summary>
        /// 获取Logger(使用缓存)
        /// </summary>
        /// <param name="loggerName">Logger 名称</param>
        /// <returns></returns>
        public static Logger GetLogger(string loggerName)
        {
            Logger logger = null;

            try
            {
                // 从缓存读取Logger
                lock (loggerLock)
                {
                    if (loggerList.ContainsKey(loggerName))
                        logger = loggerList[loggerName];
                }

                if (logger == null)
                {
                    // 重新获取Logger
                    logger = LogManager.GetLogger(loggerName);
                    // 添加Logger到缓存
                    if (logger != null)
                    {
                        lock (loggerLock)
                        {
                            if (loggerList.ContainsKey(loggerName))
                                loggerList[loggerName] = logger;
                            else
                                loggerList.Add(loggerName, logger);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果这里出错了，那么扔掉日志信息
                Console.WriteLine(ex.Message);
            }

            return logger;
        }
        #endregion

        #region Log(string loggerName, LogLevel logLevel, string message, Dictionary<string, string> dictVariable) 写日志
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="loggerName">日志的配置名称</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="message">日志信息</param>
        /// <param name="dictVariable">日志上下文自定义变量字典，可以以${event-context:键名称}的形式用于NLog.Config文件中</param>
        public static void Log(string loggerName, LogLevel logLevel, string message, Dictionary<string, string> dictVariable)
        {
            try
            {
                Logger logger = GetLogger(loggerName);
                if (logger != null)
                {
                    LogEventInfo logEvent = new LogEventInfo(logLevel, logger.Name, message);
                    if (dictVariable != null)
                        foreach (KeyValuePair<string, string> item in dictVariable)
                            logEvent.Properties.Add(item.Key, item.Value); // logEvent.Context.Add(item.Key, item.Value);
                    logger.Log(logEvent);
                }
                else
                    Console.WriteLine(string.Format("Logger {0} is Null", loggerName));
            }
            catch (Exception ex)
            {
                // 如果这里出错了，那么扔掉日志信息
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region InfoLog 记录信息日志

        #region InfoLog(string message, string eventPrefix, string subDir)
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="eventPrefix">日志文件名前缀</param>
        /// <param name="subDir">子目录</param>
        public static void InfoLog(string message, string eventPrefix, string subDir)
        {
            if (string.IsNullOrEmpty(eventPrefix))
                eventPrefix = "Log";

            Log("InfoLog", LogLevel.Info, message, new Dictionary<string, string>
            {
                { "EventPrefix", eventPrefix },
                { "SubDir", subDir }
            });
        }
        #endregion

        #region InfoLog(string message, string eventPrefix)
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="eventPrefix">日志文件名前缀</param>
        public static void InfoLog(string message, string eventPrefix)
        {
            InfoLog(message, eventPrefix, "");
        }
        #endregion

        #region InfoLog(string message)
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">日志信息</param>
        public static void InfoLog(string message)
        {
            InfoLog(message, "", "");
        }
        #endregion

        #endregion

        #region DebugLog 记录信息日志

        #region DebugLog(string message, string eventPrefix, string subDir)
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">调试信息</param>
        /// <param name="eventPrefix">日志文件名前缀</param>
        /// <param name="subDir">子目录</param>
        public static void DebugLog(string message, string eventPrefix, string subDir)
        {
            if (string.IsNullOrEmpty(eventPrefix))
                eventPrefix = "Log";

            Log("DebugLog", LogLevel.Debug, message, new Dictionary<string, string>
            {
                { "EventPrefix", eventPrefix },
                { "SubDir", subDir }
            });
        }
        #endregion

        #region DebugLog(string message, string eventPrefix)
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">调试信息</param>
        /// <param name="eventPrefix">日志文件名前缀</param>
        public static void DebugLog(string message, string eventPrefix)
        {
            DebugLog(message, eventPrefix, "");
        }
        #endregion

        #region DebugLog(string message)
        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">调试信息</param>
        public static void DebugLog(string message)
        {
            DebugLog(message, "", "");
        }
        #endregion

        #endregion

        #region ExceptionLog 记录异常日志

        private static string GetInnerExceptionInfo(Exception ex, int level = 1)
        {
            if (ex != null && ex.InnerException != null)
            {
                string message = $"{Environment.NewLine}InnerException {level}";
                message = $"{message} -- Message:{ex.InnerException.Message}{Environment.NewLine}";
                message = $"{message}StackTrace:{Environment.NewLine}";
                message = $"{message}{ex.InnerException.StackTrace}{Environment.NewLine}";
                message = $"{message}{GetInnerExceptionInfo(ex.InnerException, level + 1)}";
                return message.Replace(Environment.NewLine, Environment.NewLine + "  ").Trim();
            }
            else
                return "";
        }

        private static string GetExceptionMessage(Exception ex, string extInfo)
        {
            if (ex == null)
                return extInfo;

            string message = $"Message:{ex.Message}{Environment.NewLine}";
            message = $"{message}StackTrace:{Environment.NewLine}";
            message = $"{message}{ex.StackTrace}{Environment.NewLine}";
            message = $"{message}ExtInfo:{Environment.NewLine}";
            if (!string.IsNullOrEmpty(extInfo))
                message = $"{message}{extInfo}{Environment.NewLine}";
            message = $"{message}{GetInnerExceptionInfo(ex)}";
            return message;
        }

        #region ExceptionLog(Exception ex, string eventPrefix, string subDir, string extInfo)
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="eventPrefix">日志文件名前缀</param>
        /// <param name="subDir">子目录</param>
        /// <param name="extInfo">附加的记录信息</param>
        public static void ExceptionLog(Exception ex, string eventPrefix, string subDir, string extInfo)
        {
            if (ex == null)
                return;

            if (string.IsNullOrEmpty(eventPrefix))
                eventPrefix = "Log";

            Log("ExceptionLog", LogLevel.Error, GetExceptionMessage(ex, extInfo),
                new Dictionary<string, string>
                {
                    { "EventPrefix", eventPrefix },
                    { "SubDir", subDir }
                });
        }
        #endregion

        #region ExceptionLog(Exception ex, string eventPrefix, string subDir)
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="eventPrefix">日志文件名前缀</param>
        /// <param name="subDir">子目录</param>
        public static void ExceptionLog(Exception ex, string eventPrefix, string subDir)
        {
            ExceptionLog(ex, eventPrefix, subDir, "");
        }
        #endregion

        #region ExceptionLog(Exception ex, string eventPrefix)
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="eventPrefix">日志文件名前缀</param>
        public static void ExceptionLog(Exception ex, string eventPrefix)
        {
            ExceptionLog(ex, eventPrefix, "", "");
        }
        #endregion

        #region ExceptionLog(Exception ex)
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="ex">异常</param>
        public static void ExceptionLog(Exception ex)
        {
            ExceptionLog(ex, "", "", "");
        }
        #endregion

        #endregion

        //===========================================================================================================================================

        private static readonly string actionsToWriteCallLog = "";
        private static readonly string actionsDontWriteCallLog = "";
        private static readonly int lazyTime = ConfigReader.GetInt("CallLazyTime", 1000);

        #region NLogUtility 静态构建函数
        /// <summary>
        /// 静态构建函数
        /// </summary>
        static NLogUtility()
        {
            var configroot = AppSettingsHelper.Get();
            if (configroot != null)
            {
                actionsToWriteCallLog = configroot.GetSection("CallLog").GetValue("ActionsToWriteCallLog", "").Trim();
                actionsDontWriteCallLog = configroot.GetSection("CallLog").GetValue("ActionsDontWriteCallLog", "").Trim();
            }
            if (loggerList == null)
                loggerList = new Dictionary<string, Logger>();
        }
        #endregion

        #region CheckCanCallLog
        private static bool CheckCanCallLog(string action)
        {
            bool canWriteLog = true;

            try
            {
                // 检查是否白名单
                if (!string.IsNullOrEmpty(actionsToWriteCallLog))
                {
                    canWriteLog = actionsToWriteCallLog.ToLower()
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Contains(action.ToLower());
                }

                // 检查是否黑名单
                if (canWriteLog && !string.IsNullOrEmpty(actionsDontWriteCallLog))
                {
                    canWriteLog = !actionsDontWriteCallLog.ToLower()
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Contains(action.ToLower());
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex, "CheckCanCallLog", "NLogUtility", action);
            }

            return canWriteLog;
        }
        #endregion

        #region private CallLog 记录调用日志
        /// <summary>
        /// 记录调用日志
        /// </summary>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="loggerName">日志的配置名称</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="dictVariable">其他字段</param>
        /// <param name="message">日志内容</param>
        /// <param name="canOverride">默认日志变量可以被传入的值重写</param>
        private static void CallLog(
            DateTime callBegin, DateTime callEnd,
            string loggerName, LogLevel logLevel,
            Dictionary<string, string> dictVariable,
            string message, bool canOverride = true)
        {
            if (dictVariable == null)
                dictVariable = new Dictionary<string, string>();
            if (dictVariable.ContainsKey("Action")
                && !CheckCanCallLog(dictVariable["Action"]))
                return;

            if (!dictVariable.ContainsKey("CallBegin"))
                dictVariable.Add("CallBegin", callBegin.ToFullDateTimeString());
            else if (!canOverride)
                dictVariable["CallBegin"] = callBegin.ToFullDateTimeString();

            if (!dictVariable.ContainsKey("CallBeginDate"))
                dictVariable.Add("CallBeginDate", callBegin.ToFullDateString());
            else if (!canOverride)
                dictVariable["CallBeginDate"] = callBegin.ToFullDateString();

            if (!dictVariable.ContainsKey("CallBeginTime"))
                dictVariable.Add("CallBeginTime", callBegin.ToFullTimeWithMsString());
            else if (!canOverride)
                dictVariable["CallBeginTime"] = callBegin.ToFullTimeWithMsString();

            if (!dictVariable.ContainsKey("CallBeginHour"))
                dictVariable.Add("CallBeginHour", callBegin.ToString("HH"));
            else if (!canOverride)
                dictVariable["CallBeginHour"] = callBegin.ToString("HH");


            if (!dictVariable.ContainsKey("CallEnd"))
                dictVariable.Add("CallEnd", callEnd.ToFullDateTimeString());
            else if (!canOverride)
                dictVariable["CallEnd"] = callEnd.ToFullDateTimeString();

            if (!dictVariable.ContainsKey("CallEndDate"))
                dictVariable.Add("CallEndDate", callEnd.ToFullDateString());
            else if (!canOverride)
                dictVariable["CallEndDate"] = callEnd.ToFullDateString();

            if (!dictVariable.ContainsKey("CallEndTime"))
                dictVariable.Add("CallEndTime", callEnd.ToFullTimeWithMsString());
            else if (!canOverride)
                dictVariable["CallEndTime"] = callEnd.ToFullTimeWithMsString();

            if (!dictVariable.ContainsKey("CallEndHour"))
                dictVariable.Add("CallEndHour", callEnd.ToString("HH"));
            else if (!canOverride)
                dictVariable["CallEndHour"] = callEnd.ToString("HH");


            double milliseconds = (callEnd - callBegin).TotalMilliseconds;
            if (!dictVariable.ContainsKey("Milliseconds"))
                dictVariable.Add("Milliseconds", milliseconds.ToString("0.000"));
            else if (!canOverride)
                dictVariable["Milliseconds"] = milliseconds.ToString("0.000");


            Log(loggerName, logLevel, message, dictVariable);
            // 当接口调用时间超过 CallLazyTime 指定的毫秒时，需要特殊记录
            if (milliseconds >= lazyTime)
                Log("Lazy" + loggerName, logLevel, message, dictVariable);
        }
        #endregion

        #region CallInfoLog 记录调用信息日志
        /// <summary>
        /// 记录调用信息日志
        /// </summary>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="dictVariable">其他字段</param>
        /// <param name="message">日志内容</param>
        /// <param name="canOverride">默认日志变量可以被传入的值重写</param>
        public static void CallInfoLog(
            DateTime callBegin, DateTime callEnd,
            Dictionary<string, string> dictVariable,
            string message, bool canOverride = true)
        {
            CallLog(callBegin, callEnd, "CallInfoLog", LogLevel.Info,
                dictVariable, message, canOverride);
        }
        #endregion

        #region CallErrorLog 记录调用异常日志
        /// <summary>
        /// 记录调用异常日志
        /// </summary>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="dictVariable">其他字段</param>
        /// <param name="ex">异常</param>
        /// <param name="extInfo">附加的记录信息</param>
        /// <param name="canOverride">默认日志变量可以被传入的值重写</param>
        public static void CallErrorLog(
            DateTime callBegin, DateTime callEnd,
            Dictionary<string, string> dictVariable,
            Exception ex, string extInfo = "", bool canOverride = true)
        {
            CallLog(callBegin, callEnd, "CallErrorLog", LogLevel.Error,
                dictVariable, GetExceptionMessage(ex, extInfo), canOverride);
        }

        #endregion

    }

}