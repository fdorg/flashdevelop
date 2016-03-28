using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashConnect
{
    public class XmlSocket
    {
        private Socket server;
        private Socket client;
        private StringBuilder packets;
        public event XmlReceivedEventHandler XmlReceived;
        public event DataReceivedEventHandler DataReceived;
        private readonly String INCORRECT_PKT = TextHelper.GetString("Info.IncorrectPacket");
        private readonly String CONNECTION_FAILED = TextHelper.GetString("Info.ConnectionFailed");

        public XmlSocket(String address, Int32 port)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(address);
                this.server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.server.Bind(new IPEndPoint(ipAddress, port));
                this.server.Listen(10);
                this.server.BeginAccept(new AsyncCallback(this.OnConnectRequest), this.server);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048) TraceManager.Add("FlashConnect: " + String.Format(CONNECTION_FAILED, port));
                else ErrorManager.ShowError(ex);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }
        
        /// <summary>
        /// Accepts the connection request and sets a listener for the next one
        /// </summary>
        public void OnConnectRequest(IAsyncResult result)
        {
            try
            {
                Socket server = (Socket)result.AsyncState;
                this.client = server.EndAccept(result);
                this.SetupReceiveCallback(client);
                server.BeginAccept(new AsyncCallback(this.OnConnectRequest), server);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }
        
        /// <summary>
        /// Sets up the receive callback for the accepted connection
        /// </summary>
        public void SetupReceiveCallback(Socket client)
        {
            StateObject so = new StateObject(client);
            try
            {
                AsyncCallback receiveData = new AsyncCallback(this.OnReceivedData);
                client.BeginReceive(so.Buffer, 0, so.Size, SocketFlags.None, receiveData, so);
            }
            catch (SocketException)
            {
                so.Client.Shutdown(SocketShutdown.Both);
                so.Client.Close();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }
        
        /// <summary>
        /// Handles the received data and fires XmlReceived event
        /// </summary>
        public void OnReceivedData(IAsyncResult result)
        {
            StateObject so = (StateObject)result.AsyncState;
            try
            {
                Int32 bytesReceived = so.Client.EndReceive(result);
                if (bytesReceived > 0)
                {
                    /**
                    * Recieve data 
                    */
                    so.Data.Append(Encoding.ASCII.GetString(so.Buffer, 0, bytesReceived));
                    String contents = so.Data.ToString();
                    if (this.DataReceived != null) this.DataReceived(this, new DataReceivedEventArgs(contents, so.Client));
                    /**
                    * Check packet
                    */
                    if (packets != null) packets.Append(contents);
                    else if (contents.StartsWith('<')) packets = new StringBuilder(contents);
                    else ErrorManager.ShowWarning(INCORRECT_PKT + contents, null);
                    /**
                    * Validate message
                    */
                    if (packets != null && contents.EndsWith('\0'))
                    {
                        String msg = packets.ToString(); packets = null; 
                        if (msg == "<policy-file-request/>\0") 
                        {
                            String policy = "<cross-domain-policy><site-control permitted-cross-domain-policies=\"master-only\"/><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0";
                            so.Client.Send(Encoding.ASCII.GetBytes(policy));
                        }
                        else if (msg.EndsWithOrdinal("</flashconnect>\0")) this.XmlReceived(this, new XmlReceivedEventArgs(msg, so.Client));
                        else ErrorManager.ShowWarning(INCORRECT_PKT + msg, null);
                    }
                    this.SetupReceiveCallback(so.Client);
                }
                else
                {
                    so.Client.Shutdown(SocketShutdown.Both);
                    so.Client.Close();
                }
            }
            catch (SocketException)
            {
                so.Client.Shutdown(SocketShutdown.Both);
                so.Client.Close();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }
        
    }
    
}
