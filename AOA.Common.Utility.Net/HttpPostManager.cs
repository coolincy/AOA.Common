using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.Net
{

    /// <summary>
    /// Http Get Post 工具类
    /// 可用配置项 HttpRequestTimeOut，设置Http超时时间，默认 10000 毫秒
    /// </summary>
    public static class HttpPostManager
    {

        private static int Http_Request_TimeOut = 10000; // 10秒超时
        //private static int Http_Read_TimeOut = 10000; // 10秒超时
        private readonly static string userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; AOA.Common.Utility)";

        static HttpPostManager()
        {
            Http_Request_TimeOut = ConfigReader.GetInt("HttpRequestTimeOut", 10000); // 10秒超时
            //Http_Read_TimeOut = ConfigReader.GetInt("HttpReadTimeOut", Http_Request_TimeOut); // 10秒超时
        }

        #region private CloseRequestResponse 关闭Http请求
        /// <summary>
        /// 关闭Http请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="url"></param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        private static void CloseRequestResponse(HttpWebRequest request, HttpWebResponse response, string url, HttpStatusCode statusCode)
        {
            try
            {
                if (request != null)
                    request.Abort();
                if (response != null)
                    response.Close();
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "CloseRequestResponse", "HttpPostManager", string.Format("{0}{1}StatusCode = {2}", url, Environment.NewLine, statusCode));
            }
        }
        #endregion

        #region HttpRequest 提交Http请求，获取数据(基础方法)
        /// <summary>
        /// 提交Http请求，获取数据(基础方法)
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="httpMethod">HTTP请求的方法</param>
        /// <param name="requestContentType">HTTP 标头 Content-type 的值</param>
        /// <param name="postBytes">要发送的数据</param>
        /// <param name="responseStatusCode">Http服务器返回的状态码</param>
        /// <param name="responseTextEncoding">服务器返回的字符编码</param>
        /// <param name="responseContentType">服务器返回的Content-type 的值</param>
        /// <param name="httpRequestTimeOut">请求超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <param name="maxReadLen">最多读取的字节数</param>
        /// <returns>返回的二进制数据</returns>
        public static byte[] HttpRequest(string url, string httpMethod, string requestContentType, byte[] postBytes,
            out HttpStatusCode responseStatusCode, out Encoding responseTextEncoding, out string responseContentType,
            int httpRequestTimeOut = 0, /*int httpReadTimeOut = 0,*/ long maxReadLen = 0)
        {
            if (httpRequestTimeOut <= 0)
                httpRequestTimeOut = Http_Request_TimeOut;
            //if (httpReadTimeOut <= 0)
            //    httpReadTimeOut = Http_Read_TimeOut;

            DateTime startTime = DateTime.Now;
            responseTextEncoding = Encoding.UTF8;
            responseStatusCode = (HttpStatusCode)0;
            responseContentType = string.Empty;
            byte[] responseData = null;
            bool isWebException = false;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            #region 发送请求
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = httpRequestTimeOut;
                request.UserAgent = userAgent;
                request.Method = httpMethod;

                if (httpMethod == "POST")
                {
                    if (postBytes != null && postBytes.Length > 0)
                    {
                        request.ContentType = requestContentType;
                        request.ContentLength = postBytes.Length;
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(postBytes, 0, postBytes.Length);
                        requestStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "GetPostData", "HttpPostManager", string.Format("{0}:{1}", startTime.ToFullDateTimeWithMsString(), url));
                responseStatusCode = (HttpStatusCode)(-1);
            }
            #endregion

            if (responseStatusCode != (HttpStatusCode)(-1) && request != null)
            {
                #region 读取返回数据
                try
                {
                    WebException wex = null;
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        responseStatusCode = response.StatusCode;
                    }
                    catch (WebException ex)
                    {
                        isWebException = true;
                        if (ex.Response != null)
                        {
                            // 出现Web异常时，使用异常中的响应信息
                            response = (HttpWebResponse)ex.Response;
                            responseStatusCode = response.StatusCode;
                        }
                        else if (response != null)
                            responseStatusCode = response.StatusCode;
                        else if (ex.Status == WebExceptionStatus.Timeout)
                            responseStatusCode = HttpStatusCode.RequestTimeout;
                        else
                            responseStatusCode = (HttpStatusCode)(-1);

                        wex = ex;
                        NLogUtility.ExceptionLog(wex, "GetPostData", "HttpPostManager",
                            string.Format("WebException-1: {1}{2}{0}StatusCode = {3}", Environment.NewLine, startTime.ToFullDateTimeWithMsString(), url, responseStatusCode));

                        // 内部服务器错误，不读数据了
                        if (response != null && response.StatusCode == HttpStatusCode.InternalServerError)
                            response = null;
                    }

                    if (response != null)
                    {
                        responseContentType = response.ContentType;
                        if (!string.IsNullOrEmpty(response.CharacterSet))
                        {
                            try
                            {
                                responseTextEncoding = Encoding.GetEncoding(response.CharacterSet);
                            }
                            catch
                            {
                            }
                        }

                        using (Stream stream = response.GetResponseStream())
                        {
                            Stream streamForRead;
                            if (response.ContentEncoding == "gzip") // 处理GZip编码的数据
                                streamForRead = new GZipStream(stream, CompressionMode.Decompress);
                            else
                                streamForRead = stream;
                            // streamForRead.ReadTimeout = httpReadTimeOut;

                            //接收数据
                            long responseDataLength = response.ContentLength;
                            if (maxReadLen > 0 && (responseDataLength <= 0 || responseDataLength > maxReadLen))
                                responseDataLength = maxReadLen;

                            if (responseDataLength > 0)
                            {
                                #region 正常读取返回数据
                                responseData = new byte[responseDataLength];
                                int len = 10240; // 每次读取10K
                                int startIndex = 0;
                                int readSize = 0;
                                while (startIndex < responseDataLength)
                                {
                                    readSize = (startIndex + len < responseDataLength) ? len : (int)responseDataLength - startIndex;
                                    int intRead = streamForRead.Read(responseData, startIndex, readSize);

                                    // 没有数据读了，不再判断 startIndex < responseDataLength，防止 response.ContentLength 不正确造成死循环
                                    if (intRead <= 0)
                                        break;
                                    startIndex += intRead;
                                }
                                #endregion
                            }
                            else
                            {
                                #region 没有长度信息
                                // 使用MemoryStream来缓存后再读到数组缓存里
                                MemoryStream msTmp = new MemoryStream();
                                int readBytesTmp = 0;
                                byte[] bufTmp = new byte[10240];
                                do
                                {
                                    readBytesTmp = streamForRead.Read(bufTmp, 0, 10240);
                                    msTmp.Write(bufTmp, 0, readBytesTmp);
                                }
                                while (readBytesTmp > 0);

                                msTmp.Position = 0;
                                responseData = new byte[msTmp.Length];
                                msTmp.Read(responseData, 0, (int)msTmp.Length);

                                // 以UTF8或HTTP指定的编码读取数据
                                //using (StreamReader sr = new StreamReader(streamForRead, responseTextEncoding))
                                //{
                                //    if (sr != null)
                                //    {
                                //        string theText = sr.ReadToEnd();
                                //        responseData = responseTextEncoding.GetBytes(theText);
                                //    }
                                //}
                                #endregion
                            }

                            CloseRequestResponse(request, response, url, responseStatusCode);
                        }
                    }

                    if (isWebException && wex != null && responseData != null)
                    {
                        string exStr = string.Empty;
                        if (responseData != null)
                            exStr = responseTextEncoding.GetString(responseData);
                        NLogUtility.ExceptionLog(wex, "GetPostData", "HttpPostManager",
                            string.Format("WebException-2: {1}{0}{2}", Environment.NewLine, startTime.ToFullDateTimeWithMsString(), exStr));
                    }
                }
                catch (Exception ex)
                {
                    if (response != null)
                        responseStatusCode = response.StatusCode;
                    else
                        responseStatusCode = (HttpStatusCode)(-1);
                    responseData = null;
                    NLogUtility.ExceptionLog(ex, "GetPostData", "HttpPostManager",
                        string.Format("WebException-1: {1}{2}{0}StatusCode = {3}", Environment.NewLine, startTime.ToFullDateTimeWithMsString(), url, responseStatusCode));
                }
                #endregion
            }

            CloseRequestResponse(request, response, url, responseStatusCode);

            return responseData;
        }
        #endregion

        #region HttpRequest 提交Http请求，获取数据(基础方法)
        /// <summary>
        /// 提交Http请求，获取数据(基础方法)
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="httpMethod">HTTP请求的方法</param>
        /// <param name="requestContentType">HTTP 标头 Content-type 的值</param>
        /// <param name="postBytes">要发送的数据</param>
        /// <param name="responseStatusCode">Http服务器返回的状态码</param>
        /// <param name="responseTextEncoding">服务器返回的字符编码</param>
        /// <param name="httpRequestTimeOut">请求超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <param name="maxReadLen">最多读取的字节数</param>
        /// <returns>返回的二进制数据</returns>
        public static byte[] HttpRequest(string url, string httpMethod, string requestContentType, byte[] postBytes,
            out HttpStatusCode responseStatusCode, out Encoding responseTextEncoding,
            int httpRequestTimeOut = 0, /*int httpReadTimeOut = 0,*/ long maxReadLen = 0)
        {
            string responseContentType;
            return HttpRequest(url, httpMethod, requestContentType, postBytes,
                out responseStatusCode, out responseTextEncoding, out responseContentType,
                httpRequestTimeOut, /*httpReadTimeOut,*/ maxReadLen);
        }
        #endregion

        // 通过Get方式获取返回二进制数据

        #region GetByteData 通过Get方式获取返回二进制数据
        /// <summary>
        /// 通过Get方式获取返回二进制数据
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <param name="maxReadLen">最多读取的字节数</param>
        /// <returns>返回字节数组</returns>
        public static byte[] GetByteData(string url, out HttpStatusCode statusCode,
            int httpRequestTimeOut = 0, /*int httpReadTimeOut = 0, */long maxReadLen = 0)
        {
            Encoding encoding;
            return HttpRequest(url, "GET", "multipart/form-data", null, out statusCode, out encoding, httpRequestTimeOut, /*httpReadTimeOut, */maxReadLen);
        }

        /// <summary>
        /// 通过Get方式获取返回二进制数据
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns>返回的二进制数据</returns>
        public static byte[] GetByteData(string url, int httpRequestTimeOut = 0)
        {
            HttpStatusCode statusCode;
            return GetByteData(url, out statusCode, httpRequestTimeOut);
        }
        #endregion

        // 通过Get方式获取返回字符串

        #region GetStringData 通过Get方式获取返回字符串
        /// <summary>
        /// 通过Get方式获取返回字符串
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <param name="maxReadLen">最多读取的字节数</param>
        /// <returns>返回字符串</returns>
        public static string GetStringData(string url, out HttpStatusCode statusCode,
            int httpRequestTimeOut = 0, /*int httpReadTimeOut = 0, */long maxReadLen = 0)
        {
            Encoding encoding;
            byte[] byteData = HttpRequest(url, "GET", "multipart/form-data", null, out statusCode, out encoding, httpRequestTimeOut, /*httpReadTimeOut, */maxReadLen);
            if (byteData != null)
                return encoding.GetString(byteData);
            else
                return string.Empty;
        }

        /// <summary>
        /// 通过Get方式获取返回字符串
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns>返回字符串</returns>
        public static string GetStringData(string url, int httpRequestTimeOut = 0)
        {
            HttpStatusCode statusCode;
            return GetStringData(url, out statusCode, httpRequestTimeOut);
        }
        #endregion

        // 通过Post二进制数据并获取返回二进制数据

        #region GetPostData 通过Post二进制数据并获取返回二进制数据
        /// <summary>
        /// 通过Post二进制数据并获取返回二进制数据
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="postBytes">要发送的数据</param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        /// <param name="encoding">服务器返回的字符编码</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <param name="maxReadLen">最多读取的字节数</param>
        /// <returns>返回的二进制数据</returns>
        public static byte[] GetPostData(string url, byte[] postBytes, out HttpStatusCode statusCode, out Encoding encoding,
            int httpRequestTimeOut = 0, /*int httpReadTimeOut = 0, */long maxReadLen = 0)
        {
            return HttpRequest(url, "POST", "multipart/form-data", postBytes, out statusCode, out encoding, httpRequestTimeOut, /*httpReadTimeOut, */maxReadLen);
        }

        /// <summary>
        /// 通过Post二进制数据并获取返回二进制数据
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="postBytes">要发送的数据</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns>返回的二进制数据</returns>
        public static byte[] GetPostData(string url, byte[] postBytes, int httpRequestTimeOut = 0)
        {
            HttpStatusCode statusCode;
            Encoding encoding;
            return GetPostData(url, postBytes, out statusCode, out encoding, httpRequestTimeOut);
        }
        #endregion

        // 通过Post字符串并获取返回字符串

        #region GetPostString 通过Post字符串并获取返回字符串
        /// <summary>
        /// 通过Post字符串并获取返回字符串
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="postStr">要发送的字符串</param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <param name="maxReadLen">最多读取的字节数</param>
        /// <returns>返回的字符串</returns>
        public static string GetPostString(string url, string postStr, out HttpStatusCode statusCode,
            int httpRequestTimeOut = 0, /*int httpReadTimeOut = 0, */long maxReadLen = 0)
        {
            Encoding encoding;
            byte[] byteData = HttpRequest(url, "POST", "multipart/form-data", Encoding.UTF8.GetBytes(postStr), out statusCode, out encoding, httpRequestTimeOut, /*httpReadTimeOut, */maxReadLen);
            if (byteData != null)
                return encoding.GetString(byteData);
            else
                return string.Empty;
        }

        /// <summary>
        /// 通过Post字符串并获取返回字符串
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="postStr">要发送的字符串</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns>返回的字符串</returns>
        public static string GetPostString(string url, string postStr, int httpRequestTimeOut = 0)
        {
            HttpStatusCode statusCode;
            return GetPostString(url, postStr, out statusCode, httpRequestTimeOut);
        }
        #endregion

        #region GetStringByFormPost 通过Post模拟Request.Form请求获取返回字符串
        /// <summary>
        /// 通过Post模拟Request.Form请求获取返回字符串
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="paramStr">请求参数</param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <param name="maxReadLen">最多读取的字节数</param>
        /// <returns>返回字符串</returns>
        public static string GetStringByFormPost(string url, string paramStr, out HttpStatusCode statusCode,
            int httpRequestTimeOut = 0, /*int httpReadTimeOut = 0, */long maxReadLen = 0)
        {
            Encoding encoding;
            byte[] byteData = HttpRequest(url, "POST", "application/x-www-form-urlencoded", Encoding.UTF8.GetBytes(paramStr), out statusCode, out encoding, httpRequestTimeOut, /*httpReadTimeOut, */maxReadLen);
            if (byteData != null)
                return encoding.GetString(byteData);
            else
                return string.Empty;
        }

        /// <summary>
        /// 通过Post模拟Request.Form请求获取返回字符串
        /// </summary>
        /// <param name="url">请求地址URL</param>
        /// <param name="paramStr">请求参数</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns>返回字符串</returns>
        public static string GetStringByFormPost(string url, string paramStr, int httpRequestTimeOut = 0)
        {
            HttpStatusCode statusCode;
            return GetStringByFormPost(url, paramStr, out statusCode, httpRequestTimeOut);
        }
        #endregion

        // 通过Post提交带用户名密码验证头部的Http请求并获取返回值(仅用于调用91豆商城接口)

        #region PostRequest 通过Post提交带用户名密码验证头部的Http请求并获取返回值(仅用于调用91豆商城接口)
        /// <summary>
        /// 设置HttpRequest的用户名密码验证头部
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="request">HttpRequest</param>
        private static void SetRequestHeader(string userName, string password, HttpWebRequest request)
        {
            string checkCode = Guid.NewGuid().ToString();
            password = (password + checkCode).HashToMD5Hex();
            request.Headers.Add("AuthUserName", userName);
            request.Headers.Add("AuthPassword", password);
            request.Headers.Add("AuthCheckCode", checkCode);
        }

        /// <summary>
        /// 通过Post提交带用户名密码验证头部的Http请求并获取返回值(仅用于调用91豆商城接口)
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="paramList">调用参数</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        /// <param name="authResult">认证结果</param>
        /// <param name="isGetResponse">是否获取结果</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns>接口结果</returns>
        private static string PostRequest(string url, string paramList, string userName, string password,
            out HttpStatusCode statusCode, out string authResult, bool isGetResponse = true, int httpRequestTimeOut = 0)
        {
            DateTime startTime = DateTime.Now;
            statusCode = (HttpStatusCode)0;
            if (httpRequestTimeOut <= 0)
                httpRequestTimeOut = Http_Request_TimeOut;

            authResult = "";
            string postResult = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream webStream = null;
            StreamReader sr = null;
            try
            {
                paramList = paramList.Replace(" ", "%20");
                byte[] paramListByte = Encoding.UTF8.GetBytes(paramList);

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = httpRequestTimeOut;
                request.UserAgent = userAgent;
                request.Method = "POST";
                request.ContentType = "Application/x-www-form-urlencoded";
                request.ContentLength = paramListByte.Length;

                if (!string.IsNullOrEmpty(userName))
                    SetRequestHeader(userName, password, request);

                webStream = request.GetRequestStream();
                webStream.Write(paramListByte, 0, paramListByte.Length);
                webStream.Close();

                if (isGetResponse)
                {
                    response = (HttpWebResponse)request.GetResponse();
                    statusCode = response.StatusCode;
                    sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    if (response.Headers["AuthResult"] != null)
                        authResult = response.Headers["AuthResult"].ToString();
                    postResult = sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    // 出现Web异常时，使用异常中的响应信息
                    response = (HttpWebResponse)ex.Response;
                    statusCode = response.StatusCode;
                }
                else if (response != null)
                    statusCode = response.StatusCode;
                else if (ex.Status == WebExceptionStatus.Timeout)
                    statusCode = HttpStatusCode.RequestTimeout;
                else
                    statusCode = (HttpStatusCode)(-1);

                NLogUtility.ExceptionLog(ex, "PostRequest", "HttpPostManager",
                    string.Format("WebException-1: {1}{2}{0}StatusCode = {3}", Environment.NewLine, startTime.ToFullDateTimeWithMsString(), url, statusCode));

                // 内部服务器错误，不读数据了
                if (response != null && response.StatusCode == HttpStatusCode.InternalServerError)
                    response = null;
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "PostRequest", "HttpPostManager", url + paramList);
            }
            finally
            {
                if (webStream != null)
                    webStream.Close();
                if (sr != null)
                    sr.Close();
            }

            CloseRequestResponse(request, response, url, statusCode);
            return postResult;
        }

        /// <summary>
        /// 通过Post提交带用户名密码验证头部的Http请求并获取返回值(仅用于调用91豆商城接口)
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="paramList">调用参数</param>
        /// <param name="userName">接口用户名</param>
        /// <param name="password">接口用户密码</param>
        /// <param name="statusCode">Http服务器返回的状态码</param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns>是否Post成功</returns>
        public static string PostRequest(string url, string paramList, string userName, string password, out HttpStatusCode statusCode, int httpRequestTimeOut = 0)
        {
            string authResult = string.Empty;
            return PostRequest(url, paramList, userName, password, out statusCode, out authResult, true, httpRequestTimeOut);
        }
        #endregion

        // 来自网页监控的方法，有处理Gzip内容，回头整合到 GetPostData 中

        #region GetAllResponseContent 获取网页内容
        /// <summary>
        /// 获取网页内容，来自网页监控的方法，有处理Gzip内容，回头整合到 GetPostData 中
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encode"></param>
        /// <param name="postdate"></param>
        /// <param name="proxy"></param>
        /// <param name="errorcode"></param>
        /// <param name="processtime"></param>
        /// <param name="httpRequestTimeOut">超时时间，不传或小于等于0使用配置中的超时时间</param>
        /// <returns></returns>
        private static string GetAllResponseContent(string url, Encoding encode, string postdate, string proxy, out int errorcode, out long processtime, int httpRequestTimeOut = 0)
        {
            if (httpRequestTimeOut <= 0)
                httpRequestTimeOut = Http_Request_TimeOut;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            string result = string.Empty;
            errorcode = 0;
            processtime = 0;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //2010-06-11新增useragent
                request.Timeout = httpRequestTimeOut;
                request.UserAgent = "netdragon htmlbuilder 1.0";

                if (false == string.IsNullOrEmpty(proxy))
                {
                    string[] tmp = proxy.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    WebProxy webproxy = null;
                    if (tmp.Length == 1)
                    {
                        webproxy = new WebProxy(proxy, 80);
                    }
                    else if (tmp.Length >= 2)
                    {
                        int port;
                        if (false == int.TryParse(tmp[1], out port))
                        {
                            port = 80;
                        }

                        webproxy = new WebProxy(tmp[0], port);
                    }

                    request.Proxy = webproxy;
                }

                #region 填充要post的内容
                if (false == string.IsNullOrEmpty(postdate))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Method = "Post";
                    byte[] data = encode.GetBytes(postdate);
                    request.ContentLength = data.Length;

                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                }
                #endregion

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                sw.Stop();
                errorcode = (int)response.StatusCode;

                Stream stream;
                if (response.ContentEncoding == "gzip") // 注意内容编码
                {
                    stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    stream = response.GetResponseStream();
                }

                StreamReader reader = new StreamReader(stream, encode);
                result = reader.ReadToEnd();
                reader.Close();
                response.Close();
            }
            catch (WebException webEx)
            {
                sw.Stop();
                if (webEx.Response != null)
                {
                    HttpWebResponse response = (HttpWebResponse)webEx.Response;
                    errorcode = (int)(response).StatusCode;
                    Stream stream;
                    if (response.ContentEncoding == "gzip") // 注意内容编码
                    {
                        stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                    }
                    else
                    {
                        stream = response.GetResponseStream();
                    }

                    StreamReader reader = new StreamReader(stream, encode);
                    result = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                else
                {
                    errorcode = 600;
                    result = url + Environment.NewLine + webEx.ToString();
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                errorcode = 700;
                result = url + Environment.NewLine + ex.ToString();
            }
            processtime = sw.ElapsedMilliseconds;
            return result;
        }
        #endregion

    }

}
