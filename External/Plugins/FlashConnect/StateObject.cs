using System;
using System.Net.Sockets;
using System.Text;

namespace FlashConnect
{
    public class StateObject
    {
        public int Size; 
        public Socket Client;
        public StringBuilder Data;
        public byte[] Buffer;
        
        public StateObject(Socket client)
        {
            this.Size = 2048;
            this.Data = new StringBuilder();
            this.Buffer = new byte[this.Size];
            this.Client = client;
        }
        
    }
    
}
