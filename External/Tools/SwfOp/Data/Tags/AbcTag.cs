using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SwfOp.Data.Tags
{
    public class AbcTag : BaseTag
    {
        public Abc abc
        {
            get { return _abc; }
        }

        private string _name;
        private Abc _abc;

        public AbcTag(byte[] data, int offset, bool namedTag): base(data)
        {
            MemoryStream m = new MemoryStream(_data);
            BinaryReader br = new BinaryReader(m);
            br.BaseStream.Position += offset;
            if (namedTag)
            {
                br.ReadUInt32();
                ReadName(br);
            }
            try
            {
                _abc = new Abc(br);
                //_abc.parseMethodBodies(_data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("-- Error: ABC parser exception [" + ex.Message + "]");
            }
        }

        /// <summary>
        /// Dump ABC bytecode to file
        /// </summary>
        /// <param name="filename"></param>
        public void Dump(string filename)
        {
            Stream stream = new FileStream(filename, FileMode.Create);
            int headerLength = 6;
            stream.Write(_data, headerLength, _data.Length - headerLength);
            stream.Close();
        }

        private void ReadName(BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            while ((c = br.ReadChar()) > 0) sb.Append(c);
            _name = sb.ToString();
        }
    }
}
