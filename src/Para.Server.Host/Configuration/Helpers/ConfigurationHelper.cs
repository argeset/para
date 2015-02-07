using System;
using System.Configuration;
using System.ServiceModel;

namespace Para.Server.Host.Configuration.Helpers
{
    public class ConfigurationHelper
    {
        public static NetNamedPipeBinding NetNamedPipeBinding
        {
            get
            {
                return new NetNamedPipeBinding
                {
                    MaxBufferSize = 67108864,
                    MaxReceivedMessageSize = 67108864,
                    TransferMode = TransferMode.Streamed,
                    ReceiveTimeout = new TimeSpan(0, 30, 0),
                    SendTimeout = new TimeSpan(0, 30, 0)
                };
            }
        }

        public static NetTcpBinding NetTcpBinding
        {
            get
            {
                return new NetTcpBinding
                {
                    PortSharingEnabled = true,
                    Security = new NetTcpSecurity { Mode = SecurityMode.None },
                    MaxBufferSize = 67108864,
                    MaxReceivedMessageSize = 67108864,
                    TransferMode = TransferMode.Streamed,
                    ReceiveTimeout = new TimeSpan(0, 30, 0),
                    SendTimeout = new TimeSpan(0, 30, 0)
                };
            }
        }

        public static int TcpPort
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["TcpPort"]); }
        }
    }
}