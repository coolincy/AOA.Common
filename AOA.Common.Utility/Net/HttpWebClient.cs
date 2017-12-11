// 文件名：HttpWebClient.cs
// 描　述：实现HTTP协议中的GET、POST请求
// 使　用：
//         HttpWebClient client = new HttpWebClient();
//         client.Encoding = System.Text.Encoding.Default; //默认编码方式，根据需要设置其他类型
//         client.OpenRead("http://www.baidu.com"); //普通get请求
//         MessageBox.Show(client.RespText); //获取返回的文本
//         client.DownloadFile("http://www.codepub.com/upload/163album.rar",@"C:\163album.rar"); //下载文件
//         client.OpenRead("http://passport.baidu.com/?login","username=zhangsan&password=123456"); //提交表单，此处是登录百度的示例
//         client.UploadFile("http://hiup.baidu.com/zhangsan/upload", @"file1=D:\1.mp3"); //上传文件
//         client.UploadFile("http://hiup.baidu.com/zhangsan/upload", "folder=myfolder&size=4003550",@"file1=D:\1.mp3"); //提交含文本域和文件域的表单

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace AOA.Common.Utility.Net
{

    #region 事件委托
    /// <summary>
    /// 上传事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WebClientUploadEvent(object sender, UploadEventArgs e);

    /// <summary>
    /// 下载事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WebClientDownloadEvent(object sender, DownloadEventArgs e);
    #endregion

    #region 事件参数
    /// <summary>
    /// 上传事件参数
    /// </summary>
    public struct UploadEventArgs
    {
        /// <summary>
        /// 上传数据总大小
        /// </summary>
        public long totalBytes;
        /// <summary>
        /// 已发数据大小
        /// </summary>
        public long bytesSent;
        /// <summary>
        /// 发送进度(0-1)
        /// </summary>
        public double sendProgress;
        /// <summary>
        /// 发送速度Bytes/s
        /// </summary>
        public double sendSpeed;
    }

    /// <summary>
    /// 下载事件参数
    /// </summary>
    public struct DownloadEventArgs
    {
        /// <summary>
        /// 下载数据总大小
        /// </summary>
        public long totalBytes;
        /// <summary>
        /// 已接收数据大小
        /// </summary>
        public long bytesReceived;
        /// <summary>
        /// 接收数据进度(0-1)
        /// </summary>
        public double ReceiveProgress;
        /// <summary>
        /// 当前缓冲区数据
        /// </summary>
        public byte[] receivedBuffer;
        /// <summary>
        /// 接收速度Bytes/s
        /// </summary>
        public double receiveSpeed;
    }
    #endregion

    /// <summary>
    /// 实现向WEB服务器发送和接收数据
    /// </summary>
    public class HttpWebClient
    {

        // 常量定义
        private const string BOUNDARY = "--AOAUTILITY--";
        private const int SEND_BUFFER_SIZE = 10245;
        private const int RECEIVE_BUFFER_SIZE = 10245;

        // 私有变量
        private TcpClient clientSocket;
        private MemoryStream postStream;
        private bool isCanceled = false;

        #region 公共事件

        /// <summary>
        /// 上传进度事件
        /// </summary>
        public event WebClientUploadEvent UploadProgressChanged;

        /// <summary>
        /// 下载进度事件
        /// </summary>
        public event WebClientDownloadEvent DownloadProgressChanged;

        #endregion

        #region 公共属性

        #region RequestHeaders 请求头
        private WebHeaderCollection requestHeaders;
        /// <summary>
        /// 获取或设置请求头
        /// </summary>
        public WebHeaderCollection RequestHeaders
        {
            set { requestHeaders = value; }
            get { return requestHeaders; }
        }
        #endregion

        #region ResponseHeaders 响应头集合
        private WebHeaderCollection responseHeaders;
        /// <summary>
        /// 获取响应头集合
        /// </summary>
        public WebHeaderCollection ResponseHeaders
        {
            get { return responseHeaders; }
        }
        #endregion

        #region StrRequestHeaders 请求头文本
        private string strRequestHeaders = "";
        /// <summary>
        /// 获取请求头文本
        /// </summary>
        public string StrRequestHeaders
        {
            get { return strRequestHeaders; }
        }
        #endregion

        #region StrResponseHeaders 响应头文本
        private string strResponseHeaders = "";
        /// <summary>
        /// 获取响应头文本
        /// </summary>
        public string StrResponseHeaders
        {
            get { return strResponseHeaders; }
        }
        #endregion

        #region Cookie
        private string cookie = "";
        /// <summary>
        /// 获取或设置Cookie
        /// </summary>
        public string Cookie
        {
            set { cookie = value; }
            get { return cookie; }
        }
        #endregion

        #region Encoding 编码方式
        private Encoding encoding = Encoding.Default;
        /// <summary>
        /// 获取或设置编码方式(默认为系统默认编码方式)
        /// </summary>
        public Encoding Encoding
        {
            set { encoding = value; }
            get { return encoding; }
        }
        #endregion

        #region RespText 服务器响应文本
        private string respText = "";
        /// <summary>
        /// 获取服务器响应文本
        /// </summary>
        public string RespText
        {
            get { return respText; }
        }
        #endregion

        #region StatusCode 服务器响应状态码
        private HttpStatusCode statusCode = 0;
        /// <summary>
        /// 获取服务器响应状态码
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return statusCode; }
        }
        #endregion

        #endregion

        #region 构建函数
        /// <summary>
        /// 初始化WebClient类
        /// </summary>
        public HttpWebClient()
        {
            responseHeaders = new WebHeaderCollection();
            requestHeaders = new WebHeaderCollection();
        }
        #endregion

        #region private GetRequestHeaders 获取请求头字节数组
        /// <summary>
        /// 获取请求头字节数组
        /// </summary>
        /// <param name="request">POST或GET请求</param>
        /// <returns>请求头字节数组</returns>
        private byte[] GetRequestHeaders(string request)
        {
            requestHeaders.Add("Accept", "*/*");
            requestHeaders.Add("Accept-Language", "zh-cn");
            requestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; AOA.Common.Utility)");

            string headers = String.Format("{0}\r\n", request);

            foreach (string key in requestHeaders)
            {
                headers += String.Format("{0}:{1}\r\n", key, requestHeaders[key]);
            }

            //有Cookie就带上Cookie
            if (cookie != "")
                headers += String.Format("Cookie:{0}\r\n", cookie);

            //空行，请求头结束
            headers += "\r\n";

            strRequestHeaders = headers;
            requestHeaders.Clear();
            return encoding.GetBytes(headers);
        }
        #endregion

        #region private GetResponseHeader 分析响应流，去掉响应头
        /// <summary>
        /// 分析响应流，去掉响应头
        /// </summary>
        /// <param name="buffer">缓冲</param>
        /// <param name="startIndex">起始位置</param>
        private void GetResponseHeader(byte[] buffer, out int startIndex)
        {
            responseHeaders.Clear();
            string html = encoding.GetString(buffer);
            StringReader sr = new StringReader(html);

            int start = html.IndexOf("\r\n\r\n") + 4;//找到空行位置
            strResponseHeaders = html.Substring(0, start);//获取响应头文本

            //获取响应状态码
            if (sr.Peek() > -1)
            {
                //读第一行字符串
                string line = sr.ReadLine();

                //分析此行字符串,获取服务器响应状态码
                Match M = Regex.Match(line, @"\d\d\d");
                if (M.Success)
                {
                    statusCode = (HttpStatusCode)(int.Parse(M.Value));
                }
            }

            //获取响应头
            while (sr.Peek() > -1)
            {
                //读一行字符串
                string line = sr.ReadLine();

                //若非空行
                if (line != "")
                {
                    //分析此行字符串，获取响应标头
                    Match M = Regex.Match(line, "([^:]+):(.+)");
                    if (M.Success)
                    {
                        try
                        {
                            //添加响应标头到集合
                            responseHeaders.Add(M.Groups[1].Value.Trim(), M.Groups[2].Value.Trim());
                        }
                        catch
                        {
                        }

                        //获取Cookie
                        if (M.Groups[1].Value == "Set-Cookie")
                        {
                            M = Regex.Match(M.Groups[2].Value, "[^=]+=[^;]+");
                            cookie += M.Value.Trim() + ";";
                        }
                    }

                }
                else //若是空行，代表响应头结束响应实体开始。（响应头和响应实体间用一空行隔开）
                {
                    //如果响应头中没有实体大小标头，尝试读响应实体第一行获取实体大小
                    if (responseHeaders["Content-Length"] == null && sr.Peek() > -1)
                    {
                        //读响应实体第一行
                        line = sr.ReadLine();

                        //分析此行看是否包含实体大小
                        Match M = Regex.Match(line, "[0-9a-fA-F]{1,15}");

                        if (M.Success)
                        {
                            //将16进制的实体大小字符串转换为10进制
                            int length = int.Parse(M.Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                            responseHeaders.Add("Content-Length", length.ToString());//添加响应标头
                            strResponseHeaders += M.Value + "\r\n";
                        }
                    }
                    break;//跳出循环 
                }//End If
            }//End While

            sr.Close();

            //实体开始索引
            startIndex = encoding.GetBytes(strResponseHeaders).Length;
        }
        #endregion

        #region private SendRequestData 向服务器发送请求
        /// <summary>
        /// 向服务器发送请求
        /// </summary>
        /// <param name="URL">请求地址</param>
        /// <param name="method">POST或GET</param>
        /// <param name="showProgress">是否显示上传进度</param>
        private void SendRequestData(string URL, string method, bool showProgress)
        {
            clientSocket = new TcpClient();
            Uri URI = new Uri(URL);
            clientSocket.SendTimeout = ConfigReader.GetInt("HttpRequestTimeOut", 10000); // 10秒超时
            clientSocket.ReceiveTimeout = ConfigReader.GetInt("HttpRequestTimeOut", 10000); // 10秒超时
            clientSocket.Connect(URI.Host, URI.Port);

            requestHeaders.Add("Host", URI.Host);
            byte[] request = GetRequestHeaders(String.Format("{0} {1} HTTP/1.1", method, URI.PathAndQuery));
            clientSocket.Client.Send(request);

            //若有实体内容就发送它
            if (postStream != null)
            {
                byte[] buffer = new byte[SEND_BUFFER_SIZE];
                int count = 0;
                Stream sm = clientSocket.GetStream();
                postStream.Position = 0;

                UploadEventArgs e = new UploadEventArgs();
                e.totalBytes = postStream.Length;
                Stopwatch timer = new Stopwatch();//计时器
                timer.Start();
                do
                {
                    //如果取消就推出
                    if (isCanceled)
                        break;

                    //读取要发送的数据
                    count = postStream.Read(buffer, 0, buffer.Length);
                    //发送到服务器
                    sm.Write(buffer, 0, count);

                    //是否显示进度
                    if (showProgress)
                    {
                        //触发事件
                        e.bytesSent += count;
                        e.sendProgress = (double)e.bytesSent / (double)e.totalBytes;
                        double t = timer.ElapsedMilliseconds / 1000;
                        t = t <= 0 ? 1 : t;
                        e.sendSpeed = (double)e.bytesSent / t;
                        if (UploadProgressChanged != null)
                        {
                            UploadProgressChanged(this, e);
                        }
                    }
                } while (count > 0);
                timer.Stop();
                postStream.Close();
                postStream = null;
            }
        }
        #endregion

        #region private SendRequestData 向服务器发送请求
        /// <summary>
        /// 向服务器发送请求
        /// </summary>
        /// <param name="URL">请求URL地址</param>
        /// <param name="method">POST或GET</param>
        private void SendRequestData(string URL, string method)
        {
            SendRequestData(URL, method, false);
        }
        #endregion

        #region private SaveNetworkStream 将网络流保存到指定流
        /// <summary>
        /// 将网络流保存到指定流
        /// </summary>
        /// <param name="toStream">保存位置</param>
        /// <param name="showProgress">是否显示进度</param>
        private void SaveNetworkStream(Stream toStream, bool showProgress)
        {
            //获取要保存的网络流
            NetworkStream NetStream = clientSocket.GetStream();

            byte[] buffer = new byte[RECEIVE_BUFFER_SIZE];
            int count = 0;
            int startIndex = 0;

            MemoryStream ms = new MemoryStream();
            for (int i = 0; i < 3; i++)
            {
                count = NetStream.Read(buffer, 0, 500);
                ms.Write(buffer, 0, count);
            }

            if (ms.Length == 0)
            {
                NetStream.Close();
                throw new Exception("远程服务器没有响应");
            }

            buffer = ms.GetBuffer();
            count = (int)ms.Length;

            GetResponseHeader(buffer, out startIndex);//分析响应，获取响应头和响应实体
            count -= startIndex;
            toStream.Write(buffer, startIndex, count);

            DownloadEventArgs e = new DownloadEventArgs();

            if (responseHeaders["Content-Length"] != null)
            {
                e.totalBytes = long.Parse(responseHeaders["Content-Length"]);
            }
            else
            {
                e.totalBytes = -1;
            }

            //启动计时器
            Stopwatch timer = new Stopwatch();
            timer.Start();

            do
            {
                //如果取消就推出
                if (isCanceled)
                    break;

                //显示下载进度
                if (showProgress)
                {
                    e.bytesReceived += count;
                    e.ReceiveProgress = (double)e.bytesReceived / (double)e.totalBytes;

                    byte[] tempBuffer = new byte[count];
                    Array.Copy(buffer, startIndex, tempBuffer, 0, count);
                    e.receivedBuffer = tempBuffer;

                    double t = (timer.ElapsedMilliseconds + 0.1) / 1000;
                    e.receiveSpeed = (double)e.bytesReceived / t;

                    startIndex = 0;
                    if (DownloadProgressChanged != null) { DownloadProgressChanged(this, e); }
                }

                //读取网路数据到缓冲区
                count = NetStream.Read(buffer, 0, buffer.Length);

                //将缓存区数据保存到指定流
                toStream.Write(buffer, 0, count);
            } while (count > 0);

            timer.Stop();//关闭计时器

            if (responseHeaders["Content-Length"] != null)
            {
                toStream.SetLength(long.Parse(responseHeaders["Content-Length"]));
            }
            //else
            //{
            //    toStream.SetLength(toStream.Length);
            //    responseHeaders.Add("Content-Length", toStream.Length.ToString());//添加响应标头
            //}

            toStream.Position = 0;

            //关闭网络流和网络连接
            NetStream.Close();
            clientSocket.Close();
        }
        #endregion

        #region private SaveNetworkStream 将网络流保存到指定流
        /// <summary>
        /// 将网络流保存到指定流
        /// </summary>
        /// <param name="toStream">保存位置</param>
        private void SaveNetworkStream(Stream toStream)
        {
            SaveNetworkStream(toStream, false);
        }
        #endregion

        #region private GetText 获取服务器响应文本
        /// <summary>
        /// 获取服务器响应文本
        /// </summary>
        /// <returns>服务器响应文本</returns>
        private string GetText()
        {
            MemoryStream ms = new MemoryStream();
            SaveNetworkStream(ms);//将网络流保存到内存流
            StreamReader sr = new StreamReader(ms, encoding);
            respText = sr.ReadToEnd();
            sr.Close(); ms.Close();
            return respText;
        }
        #endregion

        #region private WriteTextField 分析文本域，添加到请求流
        /// <summary>
        /// 分析文本域，添加到请求流
        /// </summary>
        /// <param name="textField">文本域</param>
        private void WriteTextField(string textField)
        {
            string[] strArr = Regex.Split(textField, "&");
            textField = "";
            foreach (string var in strArr)
            {
                Match M = Regex.Match(var, "([^=]+)=(.+)");
                textField += "--" + BOUNDARY + "\r\n";
                textField += "Content-Disposition: form-data; name=\"" + M.Groups[1].Value + "\"\r\n\r\n" + M.Groups[2].Value + "\r\n";
            }
            byte[] buffer = encoding.GetBytes(textField);
            postStream.Write(buffer, 0, buffer.Length);
        }
        #endregion

        #region private WriteFileField 分析文件域，添加到请求流
        /// <summary>
        /// 分析文件域，添加到请求流
        /// </summary>
        /// <param name="fileField">文件域</param>
        private void WriteFileField(string fileField)
        {
            string filePath = "";
            int count = 0;
            string[] strArr = Regex.Split(fileField, "&");
            foreach (string var in strArr)
            {
                Match M = Regex.Match(var, "([^=]+)=(.+)");
                filePath = M.Groups[2].Value;
                fileField = "--" + BOUNDARY + "\r\n";
                fileField += "Content-Disposition: form-data; name=\"" + M.Groups[1].Value + "\"; filename=\"" + Path.GetFileName(filePath) + "\"\r\n";
                fileField += "Content-Type: image/jpeg\r\n\r\n";

                byte[] buffer = encoding.GetBytes(fileField);
                postStream.Write(buffer, 0, buffer.Length);

                //添加文件数据
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                buffer = new byte[50000];

                do
                {
                    count = fs.Read(buffer, 0, buffer.Length);
                    postStream.Write(buffer, 0, count);

                } while (count > 0);

                fs.Close();
                fs.Dispose();
                fs = null;

                buffer = encoding.GetBytes("\r\n");
                postStream.Write(buffer, 0, buffer.Length);
            }
        }
        #endregion


        #region OpenRead Get 读取指定URL的文本
        /// <summary>
        /// Get 读取指定URL的文本
        /// </summary>
        /// <param name="URL">请求的地址</param>
        /// <returns>服务器响应文本</returns>
        public string OpenRead(string URL)
        {
            requestHeaders.Add("Connection", "close");
            SendRequestData(URL, "GET");
            return GetText();
        }
        #endregion

        #region OpenRead Post数据并读取指定URL的文本
        /// <summary>
        /// Post数据并读取指定URL的文本
        /// </summary>
        /// <param name="URL">请求的地址</param>
        /// <param name="postData">向服务器发送的文本数据</param>
        /// <returns>服务器响应文本</returns>
        public string OpenRead(string URL, string postData)
        {
            byte[] sendBytes = encoding.GetBytes(postData);
            postStream = new MemoryStream();
            postStream.Write(sendBytes, 0, sendBytes.Length);

            requestHeaders.Add("Content-Length", postStream.Length.ToString());
            requestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            requestHeaders.Add("Connection", "close");

            SendRequestData(URL, "POST");
            return GetText();
        }
        #endregion


        #region GetStream 读取指定URL的流
        /// <summary>
        /// 读取指定URL的流
        /// </summary>
        /// <param name="URL">请求的地址</param>
        /// <param name="postData">向服务器发送的数据</param>
        /// <returns>服务器响应流</returns>
        public Stream GetStream(string URL, string postData)
        {
            byte[] sendBytes = encoding.GetBytes(postData);
            postStream = new MemoryStream();
            postStream.Write(sendBytes, 0, sendBytes.Length);

            requestHeaders.Add("Content-Length", postStream.Length.ToString());
            requestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            requestHeaders.Add("Connection", "close");

            SendRequestData(URL, "POST");

            MemoryStream ms = new MemoryStream();
            SaveNetworkStream(ms);
            return ms;
        }
        #endregion

        #region UploadFile 上传文件和数据到服务器
        /// <summary>
        /// 上传文件和数据到服务器
        /// </summary>
        /// <param name="URL">请求地址</param>
        /// <param name="textField">文本域(格式为:name1=value1&amp;name2=value2)</param>
        /// <param name="fileField">文件域(格式如:file1=C:\test.mp3&amp;file2=C:\test.jpg)</param>
        /// <returns>服务器响应文本</returns>
        public string UploadFile(string URL, string textField, string fileField)
        {
            postStream = new MemoryStream();

            if (textField != "" && fileField != "")
            {
                WriteTextField(textField);
                WriteFileField(fileField);
            }
            else if (fileField != "")
            {
                WriteFileField(fileField);
            }
            else if (textField != "")
            {
                WriteTextField(textField);
            }
            else
                throw new Exception("文本域和文件域不能同时为空。");

            //写入结束标记
            byte[] buffer = encoding.GetBytes(String.Format("--{0}--\r\n", BOUNDARY));
            postStream.Write(buffer, 0, buffer.Length);

            //添加请求标头
            requestHeaders.Add("Content-Length", postStream.Length.ToString());
            requestHeaders.Add("Content-Type", "multipart/form-data; boundary=" + BOUNDARY);
            requestHeaders.Add("Connection", "Keep-Alive");

            //发送请求数据
            SendRequestData(URL, "POST", true);

            //返回响应文本
            return GetText();
        }
        #endregion

        #region UploadFile 上传文件到服务器
        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="URL">请求的地址</param>
        /// <param name="fileField">文件域(格式如:file1=C:\test.mp3&amp;file2=C:\test.jpg)</param>
        /// <returns>服务器响应文本</returns>
        public string UploadFile(string URL, string fileField)
        {
            return UploadFile(URL, "", fileField);
        }
        #endregion

        #region DownloadData 从指定URL下载数据流
        /// <summary>
        /// 从指定URL下载数据流
        /// </summary>
        /// <param name="URL">请求地址</param>
        /// <returns>数据流</returns>
        public Stream DownloadData(string URL)
        {
            requestHeaders.Add("Connection", "close");
            SendRequestData(URL, "GET");
            MemoryStream ms = new MemoryStream();
            SaveNetworkStream(ms, true);
            return ms;
        }
        #endregion

        #region DownloadFile 从指定URL下载文件
        /// <summary>
        /// 从指定URL下载文件
        /// </summary>
        /// <param name="URL">文件URL地址</param>
        /// <param name="fileName">文件保存路径,含文件名(如:C:\test.jpg)</param>
        public void DownloadFile(string URL, string fileName)
        {
            requestHeaders.Add("Connection", "close");
            SendRequestData(URL, "GET");
            FileStream fs = new FileStream(fileName, FileMode.Create);
            SaveNetworkStream(fs, true);
            fs.Close();
            fs = null;
        }
        #endregion


        #region Start 启动上传或下载，要取消请调用Cancel方法
        /// <summary>
        /// 启动上传或下载，要取消请调用Cancel方法
        /// </summary>
        public void Start()
        {
            isCanceled = false;
        }
        #endregion

        #region Cancel 取消上传或下载,要继续开始请调用Start方法
        /// <summary>
        /// 取消上传或下载,要继续开始请调用Start方法
        /// </summary>
        public void Cancel()
        {
            isCanceled = true;
        }
        #endregion

    }

}
