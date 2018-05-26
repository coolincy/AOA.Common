using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Windows.Forms;

using AOA.Common.Utility;
using AOA.Common.Utility.ClassExtensions;
using AOA.Common.Utility.Compress;
using AOA.Common.Utility.CustomConfig;
using AOA.Common.Utility.Net;
using AOA.Common.Utility.Crypto;
using Microsoft.Extensions.Configuration;

namespace AOA.Common.Test
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            cbCacheCatalog.Items.Clear();
            var result = AOA.Common.Utility.IO.FileUtility.GetFullFileName("ss.txt");
            //if (CommonCache.Instance != null)
            //{
            //    foreach (KeyValuePair<string, CacheCatalog> catalogPair in CacheConfigFileManager.CurrentCacheConfiguration.CacheCatalogList)
            //        cbCacheCatalog.Items.Add(catalogPair.Value.Name);
            //    if (cbCacheCatalog.Items.Count > 0)
            //        cbCacheCatalog.SelectedIndex = 0;
            //}
        }

        private void btnLongToIP_Click(object sender, EventArgs e)
        {
            txt01.Text = IPUtility.LongToString(Convert.ToInt64(txt02.Text), chkBigEndian.Checked);
        }

        private void btnIPtoLong_Click(object sender, EventArgs e)
        {
            txt02.Text = IPUtility.StringToLong(txt01.Text, chkBigEndian.Checked).ToString();
        }

        private void btnToDotNetIP_Click(object sender, EventArgs e)
        {
            txt03.Text = new System.Net.IPAddress(Convert.ToInt64(txt02.Text)).ToString();
        }

        private void btnMD5_Click(object sender, EventArgs e)
        {
            txt12.Text = txt11.Text.HashToMD5Hex();
        }

        private void btnSHA1_Click(object sender, EventArgs e)
        {
            txt13.Text = txt11.Text.HashToSHA1Hex();
        }

        private void btnMD5Default_Click(object sender, EventArgs e)
        {
            txt14.Text = txt11.Text.HashDefaultToMD5Hex();
        }

        private void btnSHA1Default_Click(object sender, EventArgs e)
        {
            txt15.Text = txt11.Text.HashDefaultToSHA1Hex();
        }

        private void btnHex2MD5_Click(object sender, EventArgs e)
        {
            txt16.Text = txt11.Text.HexStringToByteArray().HashToMD5Hex();
        }

        private void btnZip_Click(object sender, EventArgs e)
        {
            if (rbGZip.Checked)
                txt22.Text = GZipCompress.CompressStringToBase64(txt21.Text);
            else
                txt22.Text = ZipCompress.CompressStringToBase64(txt21.Text);
        }

        private void btnUnZip_Click(object sender, EventArgs e)
        {
            if (rbGZip.Checked)
                txt23.Text = GZipCompress.DeCompressStringFromBase64(txt22.Text);
            else
                txt23.Text = ZipCompress.DeCompressStringFromBase64(txt22.Text);
        }

        private void btnZipToHex_Click(object sender, EventArgs e)
        {
            if (rbGZip.Checked)
                txt22.Text = GZipCompress.CompressStringToHex(txt21.Text);
            else
                txt22.Text = ZipCompress.CompressStringToHex(txt21.Text);
        }

        private void btnUnZipFromHex_Click(object sender, EventArgs e)
        {
            if (rbGZip.Checked)
                txt23.Text = GZipCompress.DeCompressStringFromHex(txt22.Text);
            else
                txt23.Text = ZipCompress.DeCompressStringFromHex(txt22.Text);
        }

        private void btnDes_Click(object sender, EventArgs e)
        {
            txt33.Text = Cryptography.DelphiDesEncryptToBase64(txt32.Text, txt31.Text);
        }

        private void btnUnDes_Click(object sender, EventArgs e)
        {
            txt34.Text = Cryptography.DelphiDesDecryptFromBase64(txt32.Text, txt33.Text);
        }

        private void btnN91Des_Click(object sender, EventArgs e)
        {
            txt33.Text = Net91ComCryptoHelper.DES_Encrypt(txt31.Text);
        }

        private void btnN91UnDes_Click(object sender, EventArgs e)
        {
            txt34.Text = Net91ComCryptoHelper.DES_Decrypt(txt33.Text);
        }

        private void lblGen3DesKey_DoubleClick(object sender, EventArgs e)
        {
            txt42.Text = Cryptography.GenerateHexString3DESKey();
        }

        private void btn3Des_Click(object sender, EventArgs e)
        {
            if (cbBenchMark.Checked)
            {
                byte[] input = Encoding.UTF8.GetBytes(txt41.Text);
                DateTime start = DateTime.Now;
                for (int i = 0; i < 20000; i++)
                {
                    string key = Cryptography.GenerateHexString3DESKey();
                    Cryptography.TripleDesEncrypt(key, input);
                }
                txt44.Text = (DateTime.Now - start).TotalMilliseconds.ToString();
            }
            else
            {
                if (rbHex.Checked)
                    txt43.Text = Cryptography.TripleDesEncryptToHex(txt42.Text, txt41.Text);
                else
                    txt43.Text = Cryptography.TripleDesEncryptToBase64(txt42.Text, txt41.Text);
            }
        }

        private void btnUn3Des_Click(object sender, EventArgs e)
        {
            if (cbBenchMark.Checked)
            {
                byte[] input = Encoding.UTF8.GetBytes(txt41.Text);
                DateTime start = DateTime.Now;
                for (int i = 0; i < 20000; i++)
                {
                    string key = Cryptography.GenerateHexString3DESKey();
                    byte[] output = Cryptography.TripleDesEncrypt(key, input);
                    Cryptography.TripleDesDecrypt(key, output);
                }
                txt44.Text = (DateTime.Now - start).TotalMilliseconds.ToString();
            }
            else
            {
                if (rbHex.Checked)
                    txt44.Text = Cryptography.TripleDesDecryptFromHexToString(txt42.Text, txt43.Text);
                else
                    txt44.Text = Cryptography.TripleDesDecryptFromBase64ToString(txt42.Text, txt43.Text);
            }
        }

        private void btn3DesDefault_Click(object sender, EventArgs e)
        {
            if (rbHex.Checked)
                txt43.Text = Cryptography.TripleDesEncryptToHex(txt41.Text);
            else
                txt43.Text = Cryptography.TripleDesEncryptToBase64(txt41.Text);
        }

        private void btnUn3DesDefault_Click(object sender, EventArgs e)
        {
            if (rbHex.Checked)
                txt44.Text = Cryptography.TripleDesDecryptFromHexToString(txt43.Text);
            else
                txt44.Text = Cryptography.TripleDesDecryptFromBase64ToString(txt43.Text);
        }

        private byte[] BCC3DesEncypt(bool encrypt, byte[] input, string hexKey)
        {
            //byte[] key = hexKey.HexStringToByteArray();
            //byte[] iv = Cryptography.GetDefaultIVFromKey(key);
            //DesEdeParameters keyParam = new DesEdeParameters(key);
            //ParametersWithIV ivParam = new ParametersWithIV(keyParam, iv);
            //IBufferedCipher engine = CipherUtilities.GetCipher("DESede/CBC/PKCS7PADDING");
            //engine.Init(encrypt, ivParam);
            //return engine.DoFinal(input);
            return new byte[1];
        }

        private void btnBCC3DesEncypt_Click(object sender, EventArgs e)
        {
            //byte[] input = Encoding.UTF8.GetBytes(txt41.Text);
            //if (cbBenchMark.Checked)
            //{
            //    DateTime start = DateTime.Now;
            //    for (int i = 0; i < 20000; i++)
            //    {
            //        string key = Cryptography.GenerateHexString3DESKey();
            //        BCC3DesEncypt(true, input, key);
            //    }
            //    txt44.Text = (DateTime.Now - start).TotalMilliseconds.ToString();
            //}
            //else
            //{
            //    byte[] result = BCC3DesEncypt(true, input, txt42.Text);
            //    if (rbHex.Checked)
            //        txt43.Text = StringEncode.ByteArrayToHexString(result);
            //    else
            //        txt43.Text = Convert.ToBase64String(result);
            //}
            txt43.Text = "";
        }

        private void btnBCC3DesDecypt_Click(object sender, EventArgs e)
        {
            //byte[] input;

            //if (cbBenchMark.Checked)
            //{
            //    input = Encoding.UTF8.GetBytes(txt41.Text);
            //    DateTime start = DateTime.Now;
            //    for (int i = 0; i < 20000; i++)
            //    {
            //        string key = Cryptography.GenerateHexString3DESKey();
            //        byte[] output = BCC3DesEncypt(true, input, key);
            //        BCC3DesEncypt(false, output, key);
            //    }
            //    txt44.Text = (DateTime.Now - start).TotalMilliseconds.ToString();
            //}
            //else
            //{
            //    if (rbHex.Checked)
            //        input = txt43.Text.HexStringToByteArray();
            //    else
            //        input = Convert.FromBase64String(txt43.Text);
            //    byte[] result = BCC3DesEncypt(false, input, txt42.Text);
            //    txt44.Text = Encoding.UTF8.GetString(result);
            //}
        }

        private void btnZipDes_Click(object sender, EventArgs e)
        {
            txt53.Text = Cryptography.ZipAndDelphiDesToBase64(txt52.Text, txt51.Text);
        }

        private void btnUnDesUnZip_Click(object sender, EventArgs e)
        {
            txt54.Text = Cryptography.UnDelphiDesAndUnZipFromBase64(txt52.Text, txt53.Text);
        }

        private void btnZip3Des_Click(object sender, EventArgs e)
        {
            txt63.Text = Cryptography.GZipAnd3DesToBase64(txt62.Text, txt61.Text);
        }

        private void btnUn3DesUnZip_Click(object sender, EventArgs e)
        {
            txt64.Text = Cryptography.Un3DesAndUnGZipFromBase64(txt62.Text, txt63.Text);
        }

        private void btnRsaKey_Click(object sender, EventArgs e)
        {
            string publicKey;
            string privateKey;
            Cryptography.GenRSAKeyPair(out publicKey, out privateKey, 384);
            txt72.Text = publicKey;
            txt73.Text = privateKey;
        }

        private void btnRsa_Click(object sender, EventArgs e)
        {
            txt74.Text = Cryptography.RSAEncryptString(txt71.Text, txt72.Text);
        }

        private void btnUnRsa_Click(object sender, EventArgs e)
        {
            txt75.Text = Cryptography.RSADecryptString(txt74.Text, txt73.Text);
        }

        private void btnStringToHex_Click(object sender, EventArgs e)
        {
            txt82.Text = txt81.Text.HexStringEncode();
        }

        private void btnHexToBase64_Click(object sender, EventArgs e)
        {
            txt83.Text = txt82.Text.HexStringToBase64String();
        }

        private void btnBase64ToHex_Click(object sender, EventArgs e)
        {
            txt84.Text = txt83.Text.Base64StringToHexString();
        }

        private void btnB64Encode_Click(object sender, EventArgs e)
        {
            txt85.Text = txt81.Text.Base64StringEncode();
        }

        private void btnB64Decode_Click(object sender, EventArgs e)
        {
            txt86.Text = txt85.Text.Base64StringDecode();
        }

        private void btnHtmlEncode_Click(object sender, EventArgs e)
        {
            txt87.Text = txt81.Text.HtmlEncode();
        }

        private void btnHtmlDecode_Click(object sender, EventArgs e)
        {
            txt88.Text = txt87.Text.HtmlDecode();
        }

        private void btnToMobileNumer_Click(object sender, EventArgs e)
        {
            txt92.Text = txt91.Text.ToMobileNumer();
        }

        private void btnCacheSetStr_Click(object sender, EventArgs e)
        {
            //int result = CommonCache.Instance.Set(cbCacheCatalog.Text, tbKey.Text, tbValue.Text);
            //tbLog.AppendText(String.Format("{1} Set Value = {2}, Result = {3}{0}", Environment.NewLine, DateTime.Now.ToLongTimeString(), tbValue.Text, result));
        }

        private void btnCacheGetStr_Click(object sender, EventArgs e)
        {
            //string result = CommonCache.Instance.Get<string>(cbCacheCatalog.Text, tbKey.Text);
            //tbLog.AppendText(String.Format("{1} Get Value = {2}{0}", Environment.NewLine, DateTime.Now.ToLongTimeString(), result));
        }

        private void btnCacheSetObj_Click(object sender, EventArgs e)
        {
            //SessionState obj = new SessionState();
            //obj.SessionId = tbValue.Text;
            //int result = CommonCache.Instance.Set(cbCacheCatalog.Text, tbKey.Text, obj);
            //tbLog.AppendText(String.Format("{1} Set Value = {2}, Result = {3}{0}", Environment.NewLine, DateTime.Now.ToLongTimeString(), obj.SessionId, result));
        }

        private void btnCacheGetObj_Click(object sender, EventArgs e)
        {
            //SessionState obj = CommonCache.Instance.Get<SessionState>(cbCacheCatalog.Text, tbKey.Text);
            //if (obj != null)
            //    tbLog.AppendText(String.Format("{1} Get Value = {2}{0}", Environment.NewLine, DateTime.Now.ToLongTimeString(), obj.SessionId));
            //else
            //    tbLog.AppendText(String.Format("{1} Get Value = {2}{0}", Environment.NewLine, DateTime.Now.ToLongTimeString(), "null"));
        }

        private void cbCacheCatalog_SelectedIndexChanged(object sender, EventArgs e)
        {
            //tbCatelogInfo.Text = CommonCache.GetCacheCatalog(cbCacheCatalog.Text).ToJson();
        }

        private void btnCacheClearAll_Click(object sender, EventArgs e)
        {
            //CommonCache.Instance.ClearAll();
        }

        private void btnCacheClearIn_Click(object sender, EventArgs e)
        {
            //CommonCache.Instance.ClearInternal();
        }

        private void btnCacheClearEx_Click(object sender, EventArgs e)
        {
            //CommonCache.Instance.ClearExternal();
        }

        private void btnTestLog_Click(object sender, EventArgs e)
        {
            NLogUtility.InfoLog("test message", "prefix", "subDir");
            NLogUtility.DebugLog("debug message", "prefix", "subDir");
            NLogUtility.ExceptionLog(new Exception("test excepteion"), "prefix", "subDir", "extInfo");
            Dictionary<string, string> dictVariable = new Dictionary<string, string>();
            //NLogUtility.CallInfoLog(DateTime.Now.AddMinutes(-1), DateTime.Now, 100010, "127.0.0.1", dictVariable, "call message");
            dictVariable.Add("Action", "action");
            dictVariable.Add("CallSource", "callSource");
            dictVariable.Add("SessionId", "sessionId");
            dictVariable.Add("SessionState", "1");
            dictVariable.Add("UserId", "100000");
            dictVariable.Add("UserName", "UserName");
            dictVariable.Add("ResultCode", "0");
            NLogUtility.CallInfoLog(DateTime.Now.AddMinutes(-1), DateTime.Now, 100010, "127.0.0.1", dictVariable, "call message");
            //NLogUtility.CallErrorLog(100010, DateTime.Now.AddMinutes(-1), DateTime.Now, "action", "callSource", "127.0.0.1", "sessionId", 1, 100000, "UserName", 0, new Exception("test excepteion"), "ext message");
            NLogUtility.CallErrorLog(DateTime.Now.AddMinutes(-1), DateTime.Now, 100010, "127.0.0.1", dictVariable, new Exception("test excepteion"), "ext message");
        }

        private void btnDoubleToChnMoney_Click(object sender, EventArgs e)
        {
            txt1202.Text = double.Parse(txt1201.Text).ConvertToChineseMoney();
        }

        private void btnReadConnStrs_Click(object sender, EventArgs e)
        {
            txtConnStrs.Clear();
            if (CustomConfigHelper.Connections != null)
            {
                foreach (ConnectionElement element in CustomConfigHelper.Connections)
                {
                    txtConnStrs.AppendText(String.Format("Name = {0}, Conn = {1}, Read = {2}, Write = {3}{4}", element.ConnName, element.ConnString, element.ReadString, element.WriteString, Environment.NewLine));
                    txtConnStrs.AppendText(ConfigReader.GetDefaultConnectionString(element.ConnName) + Environment.NewLine);
                }
            }
            var configroot = AppSettingsHelper.Get();
            if (configroot != null)
            {
                txtConnStrs.AppendText(configroot.GetSection("TestConfig:SubTestConfig:Item1").Value);
                txtConnStrs.AppendText(configroot.GetSection("TestConfig:SubTestConfig").GetValue<string>("Item2"));
                txtConnStrs.AppendText(configroot.GetSection("TestConfig:SubTestConfig").GetValue<string>("Item3", "Item3Value"));
            }
        }

        private void btnTestMQSend_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    MessageQueueHelper mqh = new MessageQueueHelper(txtMQName.Text);
            //    MessageQueueInfo<string> msi = new MessageQueueInfo<string>();
            //    msi.IdentityId = 12345678;
            //    msi.Label = "test";
            //    msi.Body = txtMQText.Text;
            //    mqh.SendToQueue<string>(msi, true);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void btnTestMQReceive_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    MessageQueueHelper mqh = new MessageQueueHelper(txtMQName.Text);
            //    MessageQueueInfo<string> msi = mqh.Receive<string>(0);
            //    MessageBox.Show(msi.Body);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void btnHttpGet_Click(object sender, EventArgs e)
        {
            //HttpWebClient client = new HttpWebClient();
            //client.Encoding = Encoding.UTF8;
            //txtHttpGetResult.Text = client.OpenRead(txtUrlToGet.Text);
            DateTime dtNow = DateTime.Now;
            HttpStatusCode statusCode;
            string resultText = HttpPostManager.GetStringData(txtUrlToGet.Text, out statusCode, maxReadLen: int.Parse(txtGetLength.Text));
            txtHttpGetResult.Text = String.Format("{0}:{1}{2}{3}",
                statusCode, (DateTime.Now - dtNow).TotalMilliseconds, Environment.NewLine, resultText);
        }

        private void btnListKeys_Click(object sender, EventArgs e)
        {
            //CommonCache.Instance.ClearAll(cbCacheCatalog.Text);
        }

        // 线程状态回调委托
        private delegate void SetRunningDel(string info);
        private void ThreadRunning(string info)
        {
            try
            {
                SetRunningDel srd = SetRunning;
                Invoke(srd, new object[] { info });
            }
            catch
            {
                return;
            }
        }

        private void SetRunning(string info)
        {
            try
            {
                tbLog.AppendText(info);
            }
            catch
            {
                return;
            }
        }

        const int baseValue = 1000000;
        private void btnDecrement_Click(object sender, EventArgs e)
        {
            tbLog.Text = string.Empty;
            for (int i = 0; i < 100; i++)
            {
                Thread threadGo = new Thread(DecrementValue);
                threadGo.Start();
            }
        }

        private void DecrementValue()
        {
            //MemcachedClient mClient = new MemcachedClient();
            //for (int i = 0; i < 20; i++)
            //{
            //    bool decOK = true;
            //    ulong lastValue = mClient.Decrement(tbKey.Text, baseValue, 1);
            //    if (lastValue < baseValue)
            //    {
            //        lastValue = mClient.Increment(tbKey.Text, baseValue, 1);
            //        decOK = false;
            //    }

            //    ThreadRunning(String.Format("{0}: Decrement = {1}: {2}{3}", Thread.CurrentThread.ManagedThreadId, decOK, (long)lastValue - baseValue, Environment.NewLine));
            //}
        }

        private void btnIncrement_Click(object sender, EventArgs e)
        {
            //MemcachedClient mClient = new MemcachedClient();
            //ulong lastValue = mClient.Increment(tbKey.Text, baseValue, 1000);
            //tbLog.AppendText(String.Format("Increment = true: {0}{1}", (long)lastValue - baseValue, Environment.NewLine));
        }

        private void btnToZero_Click(object sender, EventArgs e)
        {
            //MemcachedClient mClient = new MemcachedClient();
            //bool decOK = mClient.Store(StoreMode.Set, tbKey.Text, baseValue.ToString());
            //tbLog.AppendText(String.Format("ToZero = {0}: 0{1}", decOK, Environment.NewLine));
        }

        private void btnGATest_Click(object sender, EventArgs e)
        {
            //AOA.Common.GoogleAnalytics.Analytics.ReportAppEvent("test", "action", "label", 1);
        }

        private void btnGetMQMsgCount_Click(object sender, EventArgs e)
        {
            //MessageQueueHelper mqh = new MessageQueueHelper(txtMQName.Text);
            //int msgCount = -2;
            //if (mqh.QueueCount > 0)
            //    msgCount = MessageQueueHelper.GetMessageCount(mqh.Get(0));
            //MessageBox.Show(msgCount.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 代码执行时间测试，采用匿名方法
            CodeTimer.Time("NLogUtility.GetLogger", 100000000, () => { NLogUtility.GetLogger("InfoLog"); });
            CodeTimer.Time("NLog.LogManager.GetLogger", 100000000, () => { NLog.LogManager.GetLogger("ExceptionLog"); });
        }

        private void btnReadWriteText_Click(object sender, EventArgs e)
        {
            string ss = @"private void button1_Click(object sender, EventArgs e)
                          {
                          // 代码执行时间测试，采用匿名方法";

            ss.SaveToFile("D:\\test.txt");
            string tt = "D:\\test.txt".LoadFromFile();
            MessageBox.Show(tt);
        }

    }

    [Serializable]
    public class aClass
    {
        public string s1;
        public string s2;
        public int i1;
        public int i2;
        public List<string> sList;

        public aClass()
        {
            s1 = "s1";
            s1 = "s2";
            i1 = 1;
            i2 = 2;
            sList = new List<string>();
            sList.Add(s1);
            sList.Add(s2);
        }
    }

}
