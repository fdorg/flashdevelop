using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using FlashDebugger;

namespace FdbPlugin
{
    public class FdbRegex
    {
        private string locate;

        private List<MsgPattern> msglist;
        private List<RegexPattern> regexlist;

        public string Locate
        {
            get { return locate; }
            set { locate = value; }
        }

        public List<MsgPattern> MsgList
        {
            get { return msglist; }
            set { msglist = value; }
        }

        public List<RegexPattern> RegexList
        {
            get { return regexlist; }
            set { regexlist = value; }
        }

        public FdbRegex()
        {
            regexlist = new List<RegexPattern>();
        }
    }

    public class MsgPattern : RegexPattern
    {
        public MsgPattern() { }

        public MsgPattern(string name, string pattern)
        {
            this.Name = name;
            this.Pattern = pattern;
        }
    }

    public class RegexPattern
    {
        private string name;
        private string pattern;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Pattern
        {
            get { return pattern; }
            set { pattern = value; }
        }

        public RegexPattern() { }

        public RegexPattern(string name, string pattern)
        {
            this.name = name;
            this.pattern = pattern;
        }
    }

    class RegexManager
    {
        public static Regex RegexNameValue = new Regex(@"(?<name>.*).*?( = )(?<value>.*)", RegexOptions.Compiled);
        public static Regex RegexObject = new Regex(@".*\[Object\s\d*, class='.*'\]", RegexOptions.Compiled);

        private Dictionary<string, Regex> RegexDic = new Dictionary<string, Regex>();
        private FdbRegex fdbRegex;

        public void SetRegex(object obj)
        {
            foreach (FieldInfo info in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (info.FieldType == typeof(Regex))
                {
                    RegexPattern p = fdbRegex.RegexList.Find(rp => rp.Name == info.Name);

                    if (p != null)
                    {
                        Regex reg = new Regex(p.Pattern, RegexOptions.Compiled);
                        info.SetValue(obj, reg);
                    }
                }
            }

            foreach (FieldInfo info in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                
                if (info.FieldType == typeof(Regex))
                {
                    RegexPattern p = fdbRegex.RegexList.Find(rp => rp.Name == info.Name);

                    if (p != null)
                    {
                        Regex reg = new Regex(p.Pattern, RegexOptions.Compiled);
                        info.SetValue(obj, reg);
                    }
                }
                else if (info.FieldType == typeof(string))
                {
                    MsgPattern p = fdbRegex.MsgList.Find(mp => mp.Name == info.Name);

                    if (p != null)
                        info.SetValue(obj, p.Pattern);
                }
            }
        }

        public void Load(string s) => fdbRegex = Util.SerializeXML<FdbRegex>.LoadString(s);
    }
}
