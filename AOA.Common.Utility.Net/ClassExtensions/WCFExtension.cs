using System;
using System.ServiceModel;

namespace AOA.Common.Utility.ClassExtensions
{

    /// <summary>
    /// 一些常用扩展方法
    /// </summary>
    public static class WCFExtension
    {

        /// <summary>
        /// 安全关闭WCF服务连接
        /// </summary>
        /// <param name="wcfServiceClient">WCF服务连接</param>
        public static void CloseConnection(this ICommunicationObject wcfServiceClient)
        {
            if (wcfServiceClient.State != CommunicationState.Opened)
                return;

            try
            {
                wcfServiceClient.Close();
            }
            catch (CommunicationException ex)
            {
                wcfServiceClient.Abort();
                throw ex;
            }
            catch (TimeoutException ex)
            {
                wcfServiceClient.Abort();
                throw ex;
            }
            catch (Exception ex)
            {
                wcfServiceClient.Abort();
                throw ex;
            }

        }

    }

}
