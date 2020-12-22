using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using FlashDebugger;

namespace FdbPlugin
{
    public class FdbRegex
    {
        public string Locate { get; set; }

        public List<MsgPattern> MsgList { get; set; }

        public List<RegexPattern> RegexList { get; set; } = new List<RegexPattern>();
    }

    public class MsgPattern : RegexPattern
    {
        public MsgPattern() { }

        public MsgPattern(string name, string pattern)
        {
            Name = name;
            Pattern = pattern;
        }
    }

    public class RegexPattern
    {
        public string Name { get; set; }

        public string Pattern { get; set; }

        public RegexPattern() { }

        public RegexPattern(string name, string pattern)
        {
            Name = name;
            Pattern = pattern;
        }
    }

    internal class RegexManager
    {
        public static Regex RegexNameValue = new Regex(@"(?<name>.*).*?( = )(?<value>.*)", RegexOptions.Compiled);
        public static Regex RegexObject = new Regex(@".*\[Object\s\d*, class='.*'\]", RegexOptions.Compiled);

        Dictionary<string, Regex> RegexDic = new Dictionary<string, Regex>();
        FdbRegex fdbRegex;

        public void SetRegex(object obj)
        {
            foreach (FieldInfo info in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
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
