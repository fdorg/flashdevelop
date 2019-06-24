using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using PluginCore.Managers;

namespace PluginCore.Bridge
{
    public class ServerSocket
    {
        protected static readonly byte[] EOL = { 13 };
        protected static bool bridgeNotFound;
        
        protected IPAddress ipAddress;
        protected int portNum;
        protected Socket conn;
        protected bool isInvalid;
        
        public event DataReceivedEventHandler DataReceived;
        
        public ServerSocket(string address, int port)
        {
            if (address == null || address == "invalid")
            {
                isInvalid = true;
                return;
            }
            try
            {
                ipAddress = IPAddress.Parse(address);
            }
            catch (Exception ex)
            {
                ErrorManager.AddToLog("Invalid IP address: " + address, ex);
            }
            portNum = port;
        }
        
        public IPAddress IP => ipAddress;

        public int PortNum => portNum;


        #region SERVER
        
        public bool StartServer()
        {
            try
            {
                conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                conn.Bind(new IPEndPoint(ipAddress, portNum));
                conn.Listen(10);
                conn.BeginAccept(OnConnectRequest, conn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Accepts the connection request and sets a listener for the next one
        /// </summary>
        protected void OnConnectRequest(IAsyncResult result)
        {
            try
            {
                Socket server = (Socket)result.AsyncState;
                Socket client = server.EndAccept(result);
                SetupReceiveCallback(client);
                server.BeginAccept(OnConnectRequest, server);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        #endregion
        
        
        #region CLIENT
        
        public bool ConnectClient()
        {
            if (bridgeNotFound) return false; // don't fail repeatedly if bridge is not running

            try
            {
                conn = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                conn.Connect(new IPEndPoint(ipAddress, portNum));
                SetupReceiveCallback(conn);
            }
            catch (Exception ex)
            {
                ErrorManager.AddToLog("Failed to connect to: " + ipAddress + ":" + portNum, ex);
                bridgeNotFound = true;
                return false;
            }
            return true;
        }
        
        public int Send(string message)
        {
            return SendTo(conn, message);
        }

        public void Disconnect()
        {
            if (conn != null)
            {
                conn.Disconnect(false);
                conn = null;
            }
        }
        
        #endregion
        
        
        #region DATA
        
        public static int SendTo(Socket socket, string message)
        {
            int len = socket.Send(Encoding.UTF8.GetBytes(message));
            return len + socket.Send(EOL);
        }
        
        /// <summary>
        /// Sets up the receive callback for the accepted connection
        /// </summary>
        protected void SetupReceiveCallback(Socket client)
        {
            StateObject so = new StateObject(client);
            try
            {
                AsyncCallback receiveData = OnReceivedData;
                so.Client.BeginReceive(so.Buffer, 0, so.Size, SocketFlags.None, receiveData, so);
            }
            catch (SocketException)
            {
                so.Client.Shutdown(SocketShutdown.Both);
                so.Client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        /// <summary>
        /// Handles the received data and fires DataReceived event
        /// </summary>
        protected void OnReceivedData(IAsyncResult result)
        {
            StateObject so = (StateObject)result.AsyncState;
            try
            {
                int bytesReceived = so.Client.EndReceive(result);
                if (bytesReceived > 0)
                {
                    string chunk = Encoding.UTF8.GetString(so.Buffer, 0, bytesReceived);
                    if (chunk.Contains('*'))
                    {
                        so.Data.Append(chunk);
                        string[] lines = so.Data.ToString().Split('*');
                        foreach (string line in lines)
                        {
                            if (line.Length > 0)
                                DataReceived?.Invoke(this, new DataReceivedEventArgs(line, so.Client));
                        }
                        so.Data = new StringBuilder();
                    }
                    else so.Data.Append(chunk);
                    
                    so.Client.BeginReceive(so.Buffer, 0, so.Size, SocketFlags.None, OnReceivedData, so);
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
                Console.WriteLine(ex.Message);
            }
        }
        
        #endregion
        
    }
    
    
    #region STRUCTS
    
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
    
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(string text, Socket socket) 
        {
            Text = text;
            Socket = socket;
        }

        public string Text { get; }

        public Socket Socket { get; }
    }
    
    
    public class StateObject
    {
        public int Size; 
        public Socket Client;
        public StringBuilder Data;
        public byte[] Buffer;
        
        public StateObject(Socket client)
        {
            Size = 1024;
            Data = new StringBuilder();
            Buffer = new byte[Size];
            Client = client;
        }
        
    }
    
    #endregion
    
}
