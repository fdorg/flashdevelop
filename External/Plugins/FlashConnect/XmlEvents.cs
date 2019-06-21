using System;
using System.Net.Sockets;
using System.Xml;

namespace FlashConnect
{
    public delegate void XmlReceivedEventHandler(object sender, XmlReceivedEventArgs e);
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
    
    public class DataReceivedEventArgs : EventArgs
    {
        private readonly string text;
        private readonly Socket socket;

        public DataReceivedEventArgs(string text, Socket socket) 
        {
            this.text = text;
            this.socket = socket;
        }

        /// <summary>
        /// The message as text
        /// </summary>
        public string Text => this.text;

        /// <summary>
        /// The sender of the message
        /// </summary>
        public Socket Socket => this.socket;
    }
    
    public class XmlReceivedEventArgs : EventArgs
    {      
        private readonly XmlDocument document;
        private readonly Socket socket;

        public XmlReceivedEventArgs(string text, Socket socket) 
        {
            this.socket = socket;
            this.document = new XmlDocument();
            this.document.LoadXml(text);
        }

        /// <summary>
        /// The message as xml document
        /// </summary>
        public XmlDocument XmlDocument => this.document;

        /// <summary>
        /// The sender of the message
        /// </summary> 
        public Socket Socket => this.socket;
    }
    
}
