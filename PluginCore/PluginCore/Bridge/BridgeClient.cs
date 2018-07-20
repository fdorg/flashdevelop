using System;
using System.Net.NetworkInformation;
using PluginCore.Managers;

namespace PluginCore.Bridge
{
    public class BridgeClient : ServerSocket
    {
        #region configuration
        static private string ip;

        static public string BridgeIP
        {
            get
            {
                if (BridgeManager.Settings.CustomIP.Length > 0) return BridgeManager.Settings.CustomIP;
                else if (ip == null)
                {
                    ip = DetectIP();
                    if (ip == null) ip = "invalid";
                }
                return ip;
            }
        }

        static public int BridgePort { get { return BridgeManager.Settings.Port; } }

        static string DetectIP()
        {
            try
            {
                string issue = "Unable to find a gateway";
                foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
                {
                    issue += "\n" + f.Name + ", " + f.Description;
                    if (f.OperationalStatus == OperationalStatus.Up)
                        foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                        {
                            return d.Address.ToString();
                        }
                }
                throw new Exception(issue);
            }
            catch (Exception ex)
            {
                ErrorManager.AddToLog("Gateway detection", ex);
            }
            return null;
        }

        #endregion

        public bool Connected { get { return conn != null && conn.Connected; } }

        public BridgeClient()
            : base(BridgeIP, BridgePort)
        {
            if (!isInvalid) ConnectClient();
        }
    }
}

