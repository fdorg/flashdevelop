using System;
using System.Net.Sockets;
using System.Text;

namespace FlashConnect
{
    public class StateObject
    {
        public Int32 Size; 
        public Socket Client;
        public StringBuilder Data;
        public Byte[] Buffer;
        
        public StateObject(Socket client)
        {
            this.Size = 2048;
            this.Data = new StringBuilder();
            this.Buffer = new Byte[this.Size];
            this.Client = client;
        }
        
    }
    
}
