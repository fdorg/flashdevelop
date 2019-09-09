// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Net.Sockets;
using System.Xml;

namespace FlashConnect
{
    public delegate void XmlReceivedEventHandler(object sender, XmlReceivedEventArgs e);
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
    
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(string text, Socket socket) 
        {
            Text = text;
            Socket = socket;
        }

        /// <summary>
        /// The message as text
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The sender of the message
        /// </summary>
        public Socket Socket { get; }
    }
    
    public class XmlReceivedEventArgs : EventArgs
    {
        public XmlReceivedEventArgs(string text, Socket socket) 
        {
            Socket = socket;
            XmlDocument = new XmlDocument();
            XmlDocument.LoadXml(text);
        }

        /// <summary>
        /// The message as xml document
        /// </summary>
        public XmlDocument XmlDocument { get; }

        /// <summary>
        /// The sender of the message
        /// </summary> 
        public Socket Socket { get; }
    }
}