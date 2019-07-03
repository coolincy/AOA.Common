using System;
using System.Collections.Generic;
using System.Threading;

using AOA.Common.Utility.ClassExtensions;

using NLog;

namespace AOA.Common.Utility
{

    /// <summary>
    /// NLogUtility 使用NLog进行日志记录，日志配置见 NLog.config
    /// </summary>
    public class NLogUtility
    {

        private static ReaderWriterLockSlim readWriteLockSlim;
        private static Dictionary<string, Logger> loggerList = new Dictionary<string, Logger>();

        #region NLogUtility 静态构建函数
        /// <summary>
        /// 静态构建函数
        /// </summary>
        static NLogUtility()
        {
            readWriteLockSlim = new ReaderWriterLockSlim();
            if (loggerList == null)
                loggerList = new Dictionary<string, Logger>();
        }
        #endregion

        #region GetLogger 获取Logger(使用缓存)
        /// <summary>
        /// 获取Logger(使用缓存)
        /// </summary>
        /// <param name="loggerName">Logger 名称</param>
        /// <returns></returns>
        public static Logger GetLogger(string loggerName)
        {
            bool fromCache = false;
            Logger logger = null;

            // 从缓存读取Logger
            readWriteLockSlim.EnterReadLock();
            try
            {
                if (loggerList.ContainsKey(loggerName))
                    logger = loggerList[loggerName];
                if (logger != null)
                    fromCache = true;
            }
            finally
            {
                readWriteLockSlim.ExitReadLock();
            }

            // 重新获取Logger
            if (logger == null)
                logger = LogManager.GetLogger(loggerName);

            // 添加Logger到缓存
            if (logger != null && !fromCache)
            {
                readWriteLockSlim.EnterWriteLock();
                try
                {
                    if (loggerList.ContainsKey(loggerName))
                        loggerList[loggerName] = logger;
                    else
                        loggerList.Add(loggerName, logger);
                }
                finally
                {
                    readWriteLockSlim.ExitWriteLock();
                }
            }
            return logger;
        }
        #endregion

        #region Log 写日志
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

            Dictionary<string, string> dictVariable = new Dictionary<string, string>();
            dictVariable.Add("EventPrefix", eventPrefix);
            dictVariable.Add("SubDir", subDir);
            Log("InfoLog", LogLevel.Info, message, dictVariable);
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

            Dictionary<string, string> dictVariable = new Dictionary<string, string>();
            dictVariable.Add("EventPrefix", eventPrefix);
            dictVariable.Add("SubDir", subDir);
            Log("DebugLog", LogLevel.Debug, message, dictVariable);
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
                return $@"InnerException {level} -- Message:{ex.InnerException.Message}
StackTrace:
{ex.InnerException.StackTrace}
{GetInnerExceptionInfo(ex.InnerException, level + 1)}".Replace(@"
", @"
  ").Trim();
            }
            else
                return "";
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

            Dictionary<string, string> dictVariable = new Dictionary<string, string>();
            dictVariable.Add("EventPrefix", eventPrefix);
            dictVariable.Add("SubDir", subDir);
            string message = $@"Message:{ex.Message}
StackTrace:
{ex.StackTrace}
ExtInfo:
{extInfo}
{GetInnerExceptionInfo(ex)}";
            Log("ExceptionLog", LogLevel.Error, message, dictVariable);
        }
        #endregion

        #region ExceptionLog(Exception ex, string eventPrefix, string extInfo)
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

        private static string actionIdsToWriteCallLog = ConfigReader.GetString("ActionIdsToWriteCallLog", "").Trim();
        private static string actionIdsDontWriteCallLog = ConfigReader.GetString("ActionIdsDontWriteCallLog", "").Trim();
        private static int lazyTime = ConfigReader.GetInt("CallLazyTime", 1000);

        #region CheckCanCallLog
        private static bool CheckCanCallLog(string actionId)
        {
            bool canWriteLog = true;

            try
            {
                // 检查是否被拒绝写日志
                if (!string.IsNullOrEmpty(actionIdsDontWriteCallLog))
                {
                    if (actionId.IsIntegerPositive())
                        canWriteLog = !actionIdsDontWriteCallLog.ContainsUintValue(uint.Parse(actionId));
                    else
                        canWriteLog = actionIdsDontWriteCallLog.IndexOf("actionId") < 0;
                }

                // 检查是否在写入列表，如果写入列表为空，那么不在拒绝列表里的ActionId都会写入
                if (canWriteLog && !string.IsNullOrEmpty(actionIdsToWriteCallLog))
                {
                    if (actionId.IsIntegerPositive())
                        canWriteLog = actionIdsToWriteCallLog.ContainsUintValue(uint.Parse(actionId));
                    else
                        canWriteLog = actionIdsToWriteCallLog.IndexOf("actionId") >= 0;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex, "CheckCanCallLog", "NLogUtility", actionId);
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
        /// <param name="appid">应用ID</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="dictVariable">其他字段</param>
        /// <param name="message">日志内容</param>
        /// <param name="canOverride">默认日志变量可以被传入的值重写</param>
        private static void CallLog(DateTime callBegin, DateTime callEnd,
            string loggerName, LogLevel logLevel, int appid, string ipAddress,
            Dictionary<string, string> dictVariable, string message, bool canOverride = true)
        {
            if (dictVariable == null)
                dictVariable = new Dictionary<string, string>();


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


            if (!dictVariable.ContainsKey("AppId"))
                dictVariable.Add("AppId", appid.ToString());
            else if (!canOverride)
                dictVariable["AppId"] = appid.ToString();

            if (!dictVariable.ContainsKey("IPAddress"))
                dictVariable.Add("IPAddress", ipAddress);
            else if (!canOverride)
                dictVariable["IPAddress"] = ipAddress;

            //message = message.Replace(Environment.NewLine, " ").Replace("\r", " ").Replace("\n", " ");
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
        /// <param name="appid">应用ID</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="dictVariable">其他字段</param>
        /// <param name="message">日志内容</param>
        /// <param name="canOverride">默认日志变量可以被传入的值重写</param>
        public static void CallInfoLog(DateTime callBegin, DateTime callEnd, int appid, string ipAddress,
            Dictionary<string, string> dictVariable, string message, bool canOverride = true)
        {
            CallLog(callBegin, callEnd, "CallInfoLog", LogLevel.Info, appid, ipAddress, dictVariable, message, canOverride);
        }
        #endregion

        #region CallErrorLog 记录调用异常日志
        /// <summary>
        /// 记录调用异常日志
        /// </summary>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="appid">应用ID</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="dictVariable">其他字段</param>
        /// <param name="ex">异常</param>
        /// <param name="extInfo">附加的记录信息</param>
        /// <param name="canOverride">默认日志变量可以被传入的值重写</param>
        public static void CallErrorLog(DateTime callBegin, DateTime callEnd, int appid, string ipAddress,
            Dictionary<string, string> dictVariable, Exception ex, string extInfo, bool canOverride = true)
        {
            string message = $@"Message:{ex.Message}
StackTrace:
{ex.StackTrace}
ExtInfo:
{extInfo}
{GetInnerExceptionInfo(ex)}";
            CallLog(callBegin, callEnd, "CallErrorLog", LogLevel.Error, appid, ipAddress, dictVariable, message, canOverride);
        }
        #endregion

        #region CallErrorLog 记录调用异常日志
        /// <summary>
        /// 记录调用异常日志
        /// </summary>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="appid">应用ID</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="dictVariable">其他字段</param>
        /// <param name="ex">异常</param>
        /// <param name="canOverride">默认日志变量可以被传入的值重写</param>
        public static void CallErrorLog(DateTime callBegin, DateTime callEnd, int appid, string ipAddress,
            Dictionary<string, string> dictVariable, Exception ex, bool canOverride = true)
        {
            CallErrorLog(callBegin, callEnd, appid, ipAddress, dictVariable, ex, "", canOverride);
        }
        #endregion


        #region private CallLog 记录调用日志
        /// <summary>
        /// 记录调用日志
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="action">调用操作</param>
        /// <param name="callSource">调用源</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="sessionId">SessionId</param>
        /// <param name="sessionState">Session状态</param>
        /// <param name="userId">用户Id</param>
        /// <param name="userName">用户名称</param>
        /// <param name="resultCode">接口返回值代码</param>
        /// <param name="loggerName">日志的配置名称</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="message">日志信息</param>
        [Obsolete("请使用新的CallLog重载方法代替")]
        private static void CallLog(int appid, DateTime callBegin, DateTime callEnd, string action, string callSource,
            string ipAddress, string sessionId, int sessionState, long userId, string userName, int resultCode,
            string loggerName, LogLevel logLevel, string message)
        {
            double milliseconds = (callEnd - callBegin).TotalMilliseconds;

            // 可记录的action，失败的调用，超时的调用，都记录日志
            if (CheckCanCallLog(action) || resultCode != 0 || milliseconds >= lazyTime)
            {
                Dictionary<string, string> dictVariable = new Dictionary<string, string>();
                dictVariable.Add("Action", action);
                dictVariable.Add("CallSource", callSource);
                dictVariable.Add("SessionId", sessionId);
                dictVariable.Add("SessionState", sessionState.ToString());
                dictVariable.Add("UserId", userId.ToString());
                dictVariable.Add("UserName", userName);
                dictVariable.Add("ResultCode", resultCode.ToString());
                CallLog(callBegin, callEnd, loggerName, logLevel, appid, ipAddress, dictVariable, message);
            }
        }
        #endregion

        #region CallInfoLog 记录调用信息日志
        /// <summary>
        /// 记录调用信息日志
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="action">调用操作</param>
        /// <param name="callSource">调用源</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="sessionId">SessionId</param>
        /// <param name="sessionState">Session状态</param>
        /// <param name="userId">用户Id</param>
        /// <param name="userName">用户名称</param>
        /// <param name="resultCode">接口返回值代码</param>
        /// <param name="message">日志信息</param>
        [Obsolete("请使用新的CallInfoLog重载方法代替")]
        public static void CallInfoLog(int appid, DateTime callBegin, DateTime callEnd, string action, string callSource,
            string ipAddress, string sessionId, int sessionState, long userId, string userName, int resultCode, string message)
        {
            CallLog(appid, callBegin, callEnd, action, callSource, ipAddress, sessionId, sessionState, userId, userName, resultCode, "CallInfoLog", LogLevel.Info, message);
        }
        #endregion

        #region CallErrorLog 记录调用异常日志
        /// <summary>
        /// 记录调用异常日志
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="action">调用操作</param>
        /// <param name="callSource">调用源</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="sessionId">SessionId</param>
        /// <param name="sessionState">Session状态</param>
        /// <param name="userId">用户Id</param>
        /// <param name="userName">用户名称</param>
        /// <param name="resultCode">接口返回值代码</param>
        /// <param name="ex">异常</param>
        /// <param name="extInfo">附加的记录信息</param>
        [Obsolete("请使用新的CallErrorLog重载方法代替")]
        public static void CallErrorLog(int appid, DateTime callBegin, DateTime callEnd, string action, string callSource,
            string ipAddress, string sessionId, int sessionState, long userId, string userName, int resultCode, Exception ex, string extInfo)
        {
            string message = $@"Message:{ex.Message}
StackTrace:
{ex.StackTrace}
ExtInfo:
{extInfo}
{GetInnerExceptionInfo(ex)}";
            CallLog(appid, callBegin, callEnd, action, callSource, ipAddress, sessionId, sessionState, userId, userName, resultCode, "CallErrorLog", LogLevel.Error, message);
        }
        #endregion

        #region CallErrorLog 记录调用异常日志
        /// <summary>
        /// 记录调用异常日志
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="callBegin">调用开始时间</param>
        /// <param name="callEnd">调用结束时间</param>
        /// <param name="action">调用操作</param>
        /// <param name="callSource">调用源</param>
        /// <param name="ipAddress">客户端IP地址</param>
        /// <param name="sessionId">SessionId</param>
        /// <param name="sessionState">Session状态</param>
        /// <param name="userId">用户Id</param>
        /// <param name="userName">用户名称</param>
        /// <param name="resultCode">接口返回值代码</param>
        /// <param name="ex">异常</param>
        [Obsolete("请使用新的CallErrorLog重载方法代替")]
        public static void CallErrorLog(int appid, DateTime callBegin, DateTime callEnd, string action, string callSource,
            string ipAddress, string sessionId, int sessionState, long userId, string userName, int resultCode, Exception ex)
        {
            CallErrorLog(appid, callBegin, callEnd, action, callSource, ipAddress, sessionId, sessionState, userId, userName, resultCode, ex, "");
        }
        #endregion

    }

}