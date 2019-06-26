using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

using AOA.Common.Utility.ClassExtensions;

namespace AOA.Common.Utility.Net
{

    /// <summary>
    /// 发送邮件帮助类，Config配置文件中，需要添加SMTP_Server, SMTP_Port, SenderEmail, SenderPwd 等几个参数
    /// </summary>
    public class MailHelper
    {

        static readonly string smtp = ConfigReader.GetString("SMTP_Server");
        static readonly int port = ConfigReader.GetInt("SMTP_Port", 25);
        static readonly string senderUserMail = ConfigReader.GetString("SenderEmail");
        static readonly string senderUserName = ConfigReader.GetString("SenderName", senderUserMail);
        static readonly string senderPassword = ConfigReader.GetString("SenderPwd").TripleDesDecryptFromBase64ToString();

        #region SendMail 发送邮件
        /// <summary>
        /// 发送邮件，所有参数
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="priority">邮件优先级</param>
        /// <param name="isHtml">是否Html</param>
        /// <param name="mailTo">收件人列表</param>
        /// <param name="mailBCC">密送人列表</param>
        /// <param name="attetchments">附件地址列表</param>
        /// <returns></returns>
        public static bool SendMail(string subject, string body, MailPriority priority, bool isHtml, string mailTo, string mailBCC, string[] attetchments)
        {
            SmtpClient client = new SmtpClient(smtp)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderUserMail, senderPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            MailMessage message = new MailMessage();

            message.Sender = new MailAddress(senderUserMail, senderUserName);
            message.From = new MailAddress(senderUserMail, senderUserName);

            message.IsBodyHtml = isHtml;
            message.Priority = priority;
            message.BodyEncoding = Encoding.UTF8;

            // 收件人列表
            mailTo = mailTo ?? string.Empty;
            string[] mails = mailTo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (mails.Length == 0)
                return false;
            foreach (string item in mails)
            {
                try
                {
                    message.To.Add(new MailAddress(item, item));
                }
                catch (Exception ex)
                {
                    NLogUtility.ExceptionLog(ex, "SendMail", "MailHelper", string.Format("TO:{0}", item));
                }
            }

            // 密送人列表
            mailBCC = mailBCC ?? string.Empty;
            string[] ccmails = mailBCC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in ccmails)
            {
                try
                {
                    message.Bcc.Add(new MailAddress(item, item));
                }
                catch (Exception ex)
                {
                    NLogUtility.ExceptionLog(ex, "SendMail", "MailHelper", string.Format("BCC:{0}", item));
                }
            }

            message.Subject = subject;
            message.Body = body;

            //添加附件
            if (attetchments != null)
            {
                foreach (string file in attetchments)
                {
                    if (File.Exists(file))
                        message.Attachments.Add(new Attachment(file));
                }
            }

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                NLogUtility.ExceptionLog(ex, "SendMail", "MailHelper", string.Format("邮件内容:{1}{0}{2}", Environment.NewLine, subject, body));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 发送邮件，正常优先级
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="isHtml">是否Html</param>
        /// <param name="mailTo">收件人列表</param>
        /// <param name="mailBCC">密送人列表</param>
        /// <param name="attetchments">附件地址列表</param>
        /// <returns></returns>
        public static bool SendMail(string subject, string body, bool isHtml, string mailTo, string mailBCC, string[] attetchments)
        {
            return SendMail(subject, body, MailPriority.Normal, isHtml, mailTo, mailBCC, attetchments);
        }

        /// <summary>
        /// 发送邮件，正常优先级，非Html
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="mailTo">收件人列表</param>
        /// <param name="mailBCC">密送人列表</param>
        /// <param name="attetchments">附件地址列表</param>
        /// <returns></returns>
        public static bool SendMail(string subject, string body, string mailTo, string mailBCC, string[] attetchments)
        {
            return SendMail(subject, body, MailPriority.Normal, false, mailTo, mailBCC, attetchments);
        }

        /// <summary>
        /// 发送邮件，正常优先级，无附件
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="isHtml">是否Html</param>
        /// <param name="mailTo">收件人列表</param>
        /// <param name="mailBCC">密送人列表</param>
        /// <returns></returns>
        public static bool SendMail(string subject, string body, bool isHtml, string mailTo, string mailBCC)
        {
            return SendMail(subject, body, MailPriority.Normal, isHtml, mailTo, mailBCC, null);
        }

        /// <summary>
        /// 发送邮件，正常优先级，非Html，无附件
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="mailTo">收件人列表</param>
        /// <param name="mailBCC">密送人列表</param>
        /// <returns></returns>
        public static bool SendMail(string subject, string body, string mailTo, string mailBCC)
        {
            return SendMail(subject, body, MailPriority.Normal, false, mailTo, mailBCC, null);
        }

        /// <summary>
        /// 发送邮件，正常优先级，非Html，无抄送人，无附件
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="mailTo">收件人列表</param>
        /// <returns></returns>
        public static bool SendMail(string subject, string body, string mailTo)
        {
            return SendMail(subject, body, MailPriority.Normal, false, mailTo, string.Empty, null);
        }

        /// <summary>
        /// 发送邮件，高优先级，非Html，无抄送人，无附件
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="mailTo">收件人列表</param>
        /// <returns></returns>
        public static bool SendMailHigh(string subject, string body, string mailTo)
        {
            return SendMail(subject, body, MailPriority.High, false, mailTo, string.Empty, null);
        }
        #endregion

        #region GetGreatestCommonMeasure 求最大公约数
        /// <summary>
        /// 求最大公约数
        /// 辗转相除法
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        public static int GetGreatestCommonMeasure(int num1, int num2)
        {
            if (num1 == 0) return Math.Abs(num2);
            if (num2 == 0) return Math.Abs(num1);
            int num3 = 0;
            //insure: num1 >= num2
            if (num1 < num2)
            {
                num3 = num1;
                num1 = num2;
                num2 = num3;
            }
            num3 = num1 % num2;
            while (num3 != 0)
            {
                num1 = num2;
                num2 = num3;
                num3 = num1 % num2;
            }
            return Math.Abs(num2);
        }
        #endregion

        #region GetLeastCommonMultiple 求最小公倍数
        /// <summary>
        /// 求最小公倍数
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        public static int GetLeastCommonMultiple(int num1, int num2)
        {
            int GCM = GetGreatestCommonMeasure(num1, num2);
            if (GCM == 0) return 0;
            return num1 * num2 / GCM;
        }
        #endregion

    }

}
