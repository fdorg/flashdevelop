using System;
using System.Net.Sockets;
using System.Xml;

namespace FlashConnect
{
    public delegate void XmlReceivedEventHandler(Object sender, XmlReceivedEventArgs e);
    public delegate void DataReceivedEventHandler(Object sender, DataReceivedEventArgs e);
    
    public class DataReceivedEventArgs : EventArgs
    {
        private String text;
        private Socket socket;

        public DataReceivedEventArgs(String text, Socket socket) 
        {
            this.text = text;
            this.socket = socket;
        }

        /// <summary>
        /// The message as text
        /// </summary>
        public String Text 
        {
            get { return this.text; }
        }

        /// <summary>
        /// The sender of the message
        /// </summary>
        public Socket Socket
        {
            get { return this.socket; }
        }
        
    }
    
    public class XmlReceivedEventArgs : EventArgs
    {      
        private XmlDocument document;
        private Socket socket;

        public XmlReceivedEventArgs(String text, Socket socket) 
        {
            this.socket = socket;
            this.document = new XmlDocument();
            this.document.LoadXml(text);
        }

        /// <summary>
        /// The message as xml document
        /// </summary>
        public XmlDocument XmlDocument 
        {
            get { return this.document; }
        }
        
        /// <summary>
        /// The sender of the message
        /// </summary> 
        public Socket Socket
        {
            get { return this.socket; }
        }
        
    }
    
}
