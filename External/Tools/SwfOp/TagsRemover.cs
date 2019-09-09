using System;
using System.Collections.Generic;
using System.Text;
using SwfOp.Data;
using SwfOp.IO;
using System.IO;
using SwfOp.Data.Tags;

namespace SwfOp
{
    public class TagsRemover
    {
        private Swf swf;

        public Swf SwfObject { get { return swf; } }

        public TagsRemover(string fileName)
        {
            Stream stream = new FileStream(fileName, FileMode.Open);
            SwfReader reader = new SwfReader(stream);
            swf = reader.ReadSwf();
            stream.Close();
        }

        public TagsRemover(Swf swfObject)
        {
            swf = swfObject;
        }

        public Swf RemoveTags(int tagCode)
        {
            if (swf == null) return null;
            List<BaseTag> tags = new List<BaseTag>();
            foreach (BaseTag tag in swf)
                if (tag.TagCode != tagCode) tags.Add(tag);
            Swf newSwf = new Swf(swf.Header, tags.ToArray());
            newSwf.Version = swf.Version;
            swf = newSwf;
            return newSwf;
        }

        public void WriteSwf(string fileName)
        {
            Stream stream = new FileStream(fileName, FileMode.Create);
            SwfWriter writer = new SwfWriter(stream);
            writer.Write(swf);
            stream.Close();
        }
    }
}
