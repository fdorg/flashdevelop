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
            Size = 2048;
            Data = new StringBuilder();
            Buffer = new byte[Size];
            Client = client;
        }
    }
}