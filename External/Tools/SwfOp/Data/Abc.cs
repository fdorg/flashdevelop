using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SwfOp.Data
{
    #region enums
    public enum MethodFlags : byte
    {
        NeedArguments = 0x01,
        NeedActivation = 0x02,
        NeedRest = 0x04,
        HasOptional = 0x08,
        IgnoreRest = 0x10,
        Native = 0x20,
        HasParamNames = 0x80
    }

    public enum ConstantKind : byte
    {
        Utf8 = 0x01,
        Int = 0x03,
        UInt = 0x04,
        PrivateNs = 0x05, // non-shared namespace
        Double = 0x06,
        Qname = 0x07, // o.ns::name, ct ns, ct name
        Namespace = 0x08,
        Multiname = 0x09, // o.name, ct nsset, ct name
        False = 0x0A,
        True = 0x0B,
        Null = 0x0C,
        QnameA = 0x0D, // o.@ns::name, ct ns, ct attr-name
        MultinameA = 0x0E, // o.@name, ct attr-name
        RTQname = 0x0F, // o.ns::name, rt ns, ct name
        RTQnameA = 0x10, // o.@ns::name, rt ns, ct attr-name
        RTQnameL = 0x11, // o.ns::[name], rt ns, rt name
        RTQnameLA = 0x12, // o.@ns::[name], rt ns, rt attr-name
        NameL = 0x13,
        NameLA = 0x14, // o.@[], ns=public implied, rt attr-name
        NamespaceSet = 0x15,
        PackageNs = 0x16,
        PackageInternalNs = 0x17,
        ProtectedNs = 0x18,
        StaticProtectedNs = 0x19,
        StaticProtectedNs2 = 0x1a,
        MultinameL = 0x1B,
        MultinameLA = 0x1C,
        TypeName = 0x1D
    }

    public enum TraitMember: byte
    {
        Slot = 0x00,
        Method = 0x01,
        Getter = 0x02,
        Setter = 0x03,
        Class = 0x04,
        Function = 0x05,
        Const = 0x06,
        HasProtectedNS = 0x08
    }

    [Flags]
    public enum Attribute: byte
    {
        Final = 0x01, // 1=final, 0=virtual
        Override = 0x02, // 1=override, 0=new
        Metadata = 0x04, // 1=has metadata, 0=no metadata
        Public = 0x08 // 1=add public namespace
    }

    [Flags]
    public enum TraitFlag : byte
    {
        Sealed = 0x01,
        Final = 0x02,
        Interface = 0x04,
        HasProtectedNS = 0x08
    }

    public enum Op : byte
    {
        bkpt = 0x01,
        nop = 0x02,
        _throw = 0x03,
        getsuper = 0x04,
        setsuper = 0x05,
        dxns = 0x06,
        dxnslate = 0x07,
        kill = 0x08,
        label = 0x09,
        ifnlt = 0x0C,
        ifnle = 0x0D,
        ifngt = 0x0E,
        ifnge = 0x0F,
        jump = 0x10,
        iftrue = 0x11,
        iffalse = 0x12,
        ifeq = 0x13,
        ifne = 0x14,
        iflt = 0x15,
        ifle = 0x16,
        ifgt = 0x17,
        ifge = 0x18,
        ifstricteq = 0x19,
        ifstrictne = 0x1A,
        lookupswitch = 0x1B,
        pushwith = 0x1C,
        popscope = 0x1D,
        nextname = 0x1E,
        hasnext = 0x1F,
        pushnull = 0x20,
        pushundefined = 0x21,
        pushconstant = 0x22,
        nextvalue = 0x23,
        pushbyte = 0x24,
        pushshort = 0x25,
        pushtrue = 0x26,
        pushfalse = 0x27,
        pushnan = 0x28,
        pop = 0x29,
        dup = 0x2A,
        swap = 0x2B,
        pushstring = 0x2C,
        pushint = 0x2D,
        pushuint = 0x2E,
        pushdouble = 0x2F,
        pushscope = 0x30,
        pushnamespace = 0x31,
        hasnext2 = 0x32,
        newfunction = 0x40,
        call = 0x41,
        construct = 0x42,
        callmethod = 0x43,
        callstatic = 0x44,
        callsuper = 0x45,
        callproperty = 0x46,
        returnvoid = 0x47,
        returnvalue = 0x48,
        constructsuper = 0x49,
        constructprop = 0x4A,
        callsuperid = 0x4B,
        callproplex = 0x4C,
        callinterface = 0x4D,
        callsupervoid = 0x4E,
        callpropvoid = 0x4F,
        applytype = 0x53,
        newobject = 0x55,
        newarray = 0x56,
        newactivation = 0x57,
        newclass = 0x58,
        getdescendants = 0x59,
        newcatch = 0x5A,
        findpropstrict = 0x5D,
        findproperty = 0x5E,
        finddef = 0x5F,
        getlex = 0x60,
        setproperty = 0x61,
        getlocal = 0x62,
        setlocal = 0x63,
        getglobalscope = 0x64,
        getscopeobject = 0x65,
        getproperty = 0x66,
        getpropertylate = 0x67,
        initproperty = 0x68,
        setpropertylate = 0x69,
        deleteproperty = 0x6A,
        deletepropertylate = 0x6B,
        getslot = 0x6C,
        setslot = 0x6D,
        getglobalslot = 0x6E,
        setglobalslot = 0x6F,
        convert_s = 0x70,
        esc_xelem = 0x71,
        esc_xattr = 0x72,
        convert_i = 0x73,
        convert_u = 0x74,
        convert_d = 0x75,
        convert_b = 0x76,
        convert_o = 0x77,
        coerce = 0x80,
        coerce_b = 0x81,
        coerce_a = 0x82,
        coerce_i = 0x83,
        coerce_d = 0x84,
        coerce_s = 0x85,
        astype = 0x86,
        astypelate = 0x87,
        coerce_u = 0x88,
        coerce_o = 0x89,
        negate = 0x90,
        increment = 0x91,
        inclocal = 0x92,
        decrement = 0x93,
        declocal = 0x94,
        _typeof = 0x95,
        not = 0x96,
        bitnot = 0x97,
        concat = 0x9A,
        add_d = 0x9B,
        add = 0xA0,
        subtract = 0xA1,
        multiply = 0xA2,
        divide = 0xA3,
        modulo = 0xA4,
        lshift = 0xA5,
        rshift = 0xA6,
        urshift = 0xA7,
        bitand = 0xA8,
        bitor = 0xA9,
        bitxor = 0xAA,
        equals = 0xAB,
        strictequals = 0xAC,
        lessthan = 0xAD,
        lessequals = 0xAE,
        greaterthan = 0xAF,
        greaterequals = 0xB0,
        instanceof = 0xB1,
        istype = 0xB2,
        istypelate = 0xB3,
        _in = 0xB4,
        increment_i = 0xC0,
        decrement_i = 0xC1,
        inclocal_i = 0xC2,
        declocal_i = 0xC3,
        negate_i = 0xC4,
        add_i = 0xC5,
        subtract_i = 0xC6,
        multiply_i = 0xC7,
        getlocal0 = 0xD0,
        getlocal1 = 0xD1,
        getlocal2 = 0xD2,
        getlocal3 = 0xD3,
        setlocal0 = 0xD4,
        setlocal1 = 0xD5,
        setlocal2 = 0xD6,
        setlocal3 = 0xD7,
        debug = 0xEF,
        debugline = 0xF0,
        debugfile = 0xF1,
        bkptline = 0xF2
    }
    #endregion

    #region models
    public class MetaData : List<string[]>
    {
        public string name;

        public void Add(string name, string data)
        {
            Add(new string[] { name, data });
        }

        public override string ToString()
        {
            return "[Metadata "+name+"]";
        }
    }

    public class MemberInfo
    {
        public int id;
        public TraitMember kind;
        public QName name;
        public List<MetaData> metadata;

        public override string ToString()
        {
            return "[MemberInfo "+name+"]";
        }
        public virtual string[] RelatedTypes()
        {
            return null;
        }
    }

    public class LabelInfo : Dictionary<long, string>
    {
        public int count;
        public string labelFor(long target)
        {
            if (ContainsKey(target))
                return this[target];
            return this[target] = "L" + (++count);
        }
    }

    public class SlotInfo : MemberInfo
    {
        public QName type;
        public object value;

        public override string[] RelatedTypes()
        {
            return new string[] { type.ToString() };
        }
    }

    public class MethodInfo : MemberInfo
    {
        public MethodFlags flags;
        public string debugName;
        public QName[] paramTypes;
        public object[] optionalValues;
        public string[] paramNames;
        public QName returnType;
        // body
        public int local_count;
        public int max_scope;
        public int max_stack;
        public int code_length;
        public byte[] code;
        public Traits activation;
        public bool anon;

        public override string[] RelatedTypes()
        {
            List<string> types = new List<string>();
            types.Add(returnType.ToString());
            foreach (QName qn in paramTypes)
                types.Add(qn.ToString());
            return types.ToArray();
        }
    }

    public class Namespace
    {
        public string prefix;
        public string uri;

        public Namespace(string uri)
        {
            this.uri = uri;
        }
        public Namespace(string prefix, string uri)
        {
            this.uri = uri;
            this.prefix = prefix;
        }

        public string ToTypeNamespace()
        {
            if (prefix == null) return uri;
            else return prefix;
        }

        public override string ToString()
        {
            return "[Namespace "+prefix+":"+uri+"]";
        }
    }

    public class Multiname : QName
    {
        public Namespace[] nsset;

        public Multiname(Namespace[] nsset, string localName)
            : base(localName)
        {
            this.nsset = nsset;
        }

        public override string ToTypeString()
        {
            string qname = base.ToTypeString();
            string tempUri = uri;
            if (tempUri == null)
            {
                if (nsset != null && nsset.Length > 0)
                    tempUri = nsset[0].ToTypeNamespace();
            }
            return (tempUri != null && tempUri.Length > 0) ? tempUri + "." + qname : qname;
        }

        public override string ToString()
        {
            return ToTypeString();
        }
    }

    public class ParameterizedQName : QName
    {
        QName[] parameters;

        public ParameterizedQName(QName name, QName[] parameters)
            : base(name.localName)
        {
            this.parameters = parameters;
        }

        public override string ToString()
        {
            if (parameters.Length == 0) return base.ToString() + " []";
            string s = base.ToString() + " [";
            foreach (QName name in parameters) s += name.ToString() + ", ";
            return s.Substring(0, s.Length - 2) + "]";
        }

        public override string ToTypeString()
        {
            string t = (uri == "__AS3__.vec") ? localName : base.ToTypeString();
            if (parameters.Length > 0)
            {
                t += ".<";
                foreach (QName name in parameters) t += name.ToTypeString() + ", ";
                t = t.Substring(0, t.Length - 2) + ">";
            }
            return t;
        }
    }

    public class QName
    {
        public string localName;
        public string uri;

        public QName(string localName)
        {
            this.localName = localName;
        }

        public QName(Multiname mname)
        {
            this.localName = mname.localName;
        }

        public QName(QName qname)
        {
            this.localName = qname.localName;
        }

        public QName(Namespace uri, string localName)
        {
            this.localName = localName;
            this.uri = uri.uri;
        }

        public virtual string ToTypeString()
        {
            string name = localName;
            if (name.IndexOf('$') > 0) name = name.Replace("$", ".<") + ">";
            return (uri != null && uri.Length > 0) ? uri + "." + name : name;
        }

        public override string ToString()
        {
            return (uri != null && uri.Length > 0) ? uri + ":" + localName : localName;
        }
    }

    public class Traits
    {
        public QName name;
        public MethodInfo init;
        public Traits itraits;
        public QName baseName;
        public TraitFlag flags;
        public Namespace protectedNs;
        public QName[] interfaces;
        public Dictionary<QName, MemberInfo> names;
        public List<MemberInfo> slots;
        public List<MethodInfo> methods;
        public List<MemberInfo> members;

        public Traits()
        {
            names = new Dictionary<QName, MemberInfo>();
            slots = new List<MemberInfo>();
            methods = new List<MethodInfo>();
            members = new List<MemberInfo>();
        }

        public override string ToString()
        {
            return name.ToString();
            //return "[Trait "+name+" extends "+baseName+"]";
        }
    }
    #endregion

    public class Abc
    {
        #region ABC data
        internal int magic;
        internal MetaData[] metadata;

        internal object[] const_values;
        internal int[] ints;
        internal uint[] uints;
        internal double[] doubles;
        internal string[] strings;
        internal Namespace[] namespaces;
        internal Namespace[][] nssets;
        internal QName[] names;
                
        internal MethodInfo[] methods;
        public Traits[] instances;
        public Traits[] classes;
        public Traits[] scripts;
        internal long methodBodiesPosition;
        internal bool methodBodiesParsed;

        internal Namespace publicNs = new Namespace("");
        internal Namespace anyNs = new Namespace("*");
        #endregion

        public Abc(BinaryReader br)
        {
            magic = br.ReadInt32();
            if (magic != (46 << 16 | 14) && magic != (46 << 16 | 15) && magic != (46 << 16 | 16))
            {
                Console.WriteLine("-- Error: Not an ABC block ["+magic+"]");
                return;
            }

            // constants pool
            parseCpool(br);
            const_values = new object[13];
            const_values[10] = false;
            const_values[11] = true;
            const_values[12] = null;
           
            parseMethodInfos(br);
            parseMetadataInfos(br);
            parseInstanceInfos(br);
            parseClassInfos(br);
            parseScriptInfos(br);

            // store read position for optional parseMethodBodies() call
            methodBodiesPosition = br.BaseStream.Position;
        }

        /// <summary>
        /// Reading Flash 9 multi-byte integer encoding
        /// </summary>
        static public int readU32(BinaryReader br)
        {
            int result = br.ReadByte();
            if ((result & 0x00000080) == 0)
                return result;
            result = result & 0x0000007f | br.ReadByte() << 7;
            if ((result & 0x00004000) == 0)
                return result;
            result = result & 0x00003fff | br.ReadByte() << 14;
            if ((result & 0x00200000) == 0)
                return result;
            result = result & 0x001fffff | br.ReadByte() << 21;
            if ((result & 0x10000000) == 0)
                return result;
            return result & 0x0fffffff | br.ReadByte() << 28;
        }

        static public string readUTFBytes(BinaryReader br)
        {
            int len = readU32(br);
            byte[] buffer = new byte[len];
            for (int i = 0; i < len; ++i)
            {
                buffer[i] = br.ReadByte();
            }
            return UTF8Encoding.UTF8.GetString(buffer);
        }

        #region constants pool
        private void parseCpool(BinaryReader br)
        {
            long start = br.BaseStream.Position;
            int n;

            // ints
            n = readU32(br);
            ints = new int[n];
            if (n > 0) ints[0] = 0;
            for (int i=1; i < n; i++)
                ints[i] = readU32(br);

            // uints
            n = readU32(br);
            uints = new uint[n];
            if (n > 0) uints[0] = 0;
            for (int i=1; i < n; i++)
                uints[i] = (uint)readU32(br);

            // doubles
            n = readU32(br);
            doubles = new double[n];
            if (n > 0) doubles[0] = double.NaN;
            for (int i=1; i < n; i++)
                doubles[i] = br.ReadDouble();

            // sb.append("Cpool numbers size "+(data.position-start)+" "+int(100*(data.position-start)/data.length)+" %")
            //Console.WriteLine("Cpool numbers size " + (br.BaseStream.Position - start) + " " + (int)(100 * (br.BaseStream.Position - start) / br.BaseStream.Length) + " %");
            start = br.BaseStream.Position;

            // strings
            n = readU32(br);
            strings = new string[n];
            if (n > 0) strings[0] = "";
            for (int i = 1; i < n; i++)
                strings[i] = readUTFBytes(br); //br.ReadString()

            // sb.append("Cpool strings count "+ n +" size "+(data.position-start)+" "+int(100*(data.position-start)/data.length)+" %")
            //Console.WriteLine("Cpool strings count " + n + " size " + (br.BaseStream.Position - start) + " " + (int)(100 * (br.BaseStream.Position - start) / br.BaseStream.Length) + " %");
            start = br.BaseStream.Position;

            // namespaces
            n = readU32(br);
            namespaces = new Namespace[n];
            if (n > 0) namespaces[0] = publicNs;
            for (int i=1; i < n; i++)
            switch ((ConstantKind)br.ReadSByte())
            {
                case ConstantKind.Namespace:
                case ConstantKind.PackageNs:
                case ConstantKind.PackageInternalNs:
                case ConstantKind.ProtectedNs:
                case ConstantKind.StaticProtectedNs:
                case ConstantKind.StaticProtectedNs2:
                    namespaces[i] = new Namespace((string)strings[readU32(br)]);
                    // todo mark kind of namespace.
                    break;
                case ConstantKind.PrivateNs:
                    readU32(br);
                    namespaces[i] = new Namespace(null, "private");
                    break;
            }

            //Console.WriteLine("Cpool namespaces count "+ n +" size "+(br.BaseStream.Position-start)+" "+(int)(100*(br.BaseStream.Position-start)/br.BaseStream.Length)+" %");
            start = br.BaseStream.Position;
            
            // namespace sets
            n = readU32(br);
            nssets = new Namespace[n][];
            if (n > 0) nssets[0] = null;
            for (int i = 1; i < n; i++)
            {
                int count = readU32(br);
                Namespace[] nsset = new Namespace[count];
                nssets[i] = nsset;
                for (int j = 0; j < count; j++)
                    nsset[j] = (Namespace)namespaces[readU32(br)];
            }

            //Console.WriteLine("Cpool nssets count "+ n +" size "+(br.BaseStream.Position-start)+" "+(int)(100*(br.BaseStream.Position-start)/br.BaseStream.Length)+" %");
            start = br.BaseStream.Position;

            // multinames
            n = readU32(br);
            names = new QName[n];
            if (n > 0) names[0] = null;
            namespaces[0] = anyNs;
            strings[0] = "*";
            for (int i = 1; i < n; i++)
            {
                switch ((ConstantKind)br.ReadSByte())
                {
                    case ConstantKind.Qname:
                    case ConstantKind.QnameA:
                        names[i] = new QName((Namespace)namespaces[readU32(br)], (string)strings[readU32(br)]);
                        break;

                    case ConstantKind.RTQname:
                    case ConstantKind.RTQnameA:
                        names[i] = new QName((string)strings[readU32(br)]);
                        break;

                    case ConstantKind.RTQnameL:
                    case ConstantKind.RTQnameLA:
                        names[i] = null;
                        break;

                    case ConstantKind.NameL:
                    case ConstantKind.NameLA:
                        names[i] = new QName(new Namespace(""), null);
                        break;

                    case ConstantKind.Multiname:
                    case ConstantKind.MultinameA:
                        string localName = (string)strings[readU32(br)];
                        names[i] = new Multiname((Namespace[])nssets[readU32(br)], localName);
                        break;

                    case ConstantKind.MultinameL:
                    case ConstantKind.MultinameLA:
                        names[i] = new Multiname((Namespace[])nssets[readU32(br)], null);
                        break;

                    case ConstantKind.TypeName:
                        QName name = (QName)names[readU32(br)] ?? new QName("Vector");
                        int count = readU32(br);
                        QName[] parameters = new QName[count];
                        for (int j = 0; j < count; ++j)
                        {
                            int idx = readU32(br);
                            parameters[j] = (QName)names[idx] ?? new QName("*");
                        }
                        names[i] = new ParameterizedQName(name, parameters);
                        break;

                    default:
                        br.BaseStream.Position--;
                        Console.WriteLine("-- Error: Invalid constant kind [" + br.ReadSByte() + "]");
                        break;
                }
            }

            //Console.WriteLine("Cpool names count " + n + " size " + (br.BaseStream.Position - start) + " " + (int)(100 * (br.BaseStream.Position - start) / br.BaseStream.Length) + " %");

            namespaces[0] = publicNs;
            strings[0] = "*";
        }
        #endregion

        #region model parser
        
        private void parseMethodInfos(BinaryReader br)
        {
            long start = br.BaseStream.Position;
            names[0] = new QName(publicNs,"*");
            int method_count = readU32(br);
            methods = new MethodInfo[method_count];

            for (int i=0; i < method_count; i++)
            {
                MethodInfo m = new MethodInfo();
                methods[i] = m;
                int param_count = readU32(br);
                m.returnType = (QName)names[readU32(br)];
                m.paramTypes = new QName[param_count];
                for (int j=0; j < param_count; j++)
                    m.paramTypes[j] = (QName)names[readU32(br)];
                m.debugName = (string)strings[readU32(br)];
                m.flags = (MethodFlags)br.ReadSByte();

                if ((m.flags & MethodFlags.NeedRest) > 0)
                {
                    QName[] temp = m.paramTypes;
                    m.paramTypes = new QName[param_count + 1];
                    temp.CopyTo(m.paramTypes, 0);
                    m.paramTypes[param_count] = new QName("Array");
                }
                if ((m.flags & MethodFlags.HasOptional) > 0)
                {
                    // has_optional
                    int optional_count = readU32(br);
                    if ((m.flags & MethodFlags.NeedRest) > 0)
                    {
                        m.optionalValues = new object[optional_count + 1];
                        m.optionalValues[optional_count] = null;
                    }
                    else m.optionalValues = new object[optional_count];
                    for(int k = 0; k < optional_count; k++)
                    {
                        int index = readU32(br);    // optional value index
                        ConstantKind kind = (ConstantKind)br.ReadSByte(); // kind byte for each default value
                        if (index == 0)
                        {
                            // kind is ignored, default value is based on type
                            m.optionalValues[k] = null;
                        }
                        else m.optionalValues[k] = getDefaultValue(kind, index);
                    }
                }
                if ((m.flags & MethodFlags.HasParamNames) > 0)
                {
                    // has_paramnames
                    if ((m.flags & MethodFlags.NeedRest) > 0)
                    {
                        m.paramNames = new string[param_count + 1];
                        m.paramNames[param_count] = "...rest";
                    }
                    else m.paramNames = new string[param_count];
                    for(int k = 0; k < param_count; k++)//++k)
                    {
                        int index = readU32(br);
                        if (index < strings.Length && strings[index] != null)
                            m.paramNames[k] = (string)strings[index];
                    }
                }
            }
            //Console.WriteLine("MethodInfo count " +method_count+ " size "+(br.BaseStream.Position-start)+" "+(int)(100*(br.BaseStream.Position-start)/br.BaseStream.Length)+" %");
        }

        private object getDefaultValue(ConstantKind kind, int index)
        {
            switch(kind)
            {
                case ConstantKind.Utf8: return strings[index];
                case ConstantKind.Int: return ints[index];
                case ConstantKind.UInt: return uints[index];
                case ConstantKind.Double: return doubles[index];
                case ConstantKind.False:
                case ConstantKind.True:
                case ConstantKind.Null: return const_values[index];
                case ConstantKind.Namespace:
                case ConstantKind.PrivateNs:
                case ConstantKind.PackageNs:
                case ConstantKind.PackageInternalNs:
                case ConstantKind.ProtectedNs:
                case ConstantKind.StaticProtectedNs:
                case ConstantKind.StaticProtectedNs2: return namespaces[index];
                default:
                    Console.WriteLine("-- Error: unknown kind [" + kind + "]");
                    return null;
            }
        }

        private void parseMetadataInfos(BinaryReader br)
        {
            int count = readU32(br);
            metadata = new MetaData[count];

            for (int i=0; i < count; i++)
            {
                // MetadataInfo
                MetaData m = new MetaData();
                metadata[i] = m;
                m.name = (string)strings[readU32(br)];

                int values_count = readU32(br);
                string[] names = new string[values_count];
                for(int q = 0; q < values_count; ++q)
                    names[q] = (string)strings[readU32(br)]; // name
                for (int q = 0; q < values_count; ++q)
                    m.Add(names[q], (string)strings[readU32(br)]); // value
            }
        }
        
        private void parseInstanceInfos(BinaryReader br)
        {
            long start = br.BaseStream.Position;
            int count = readU32(br);
            instances = new Traits[count];

            for (int i=0; i < count; i++)
            {
                Traits t = new Traits();
                instances[i] = t;
                t.name = (QName)names[readU32(br)];
                t.baseName = (QName)names[readU32(br)];
                t.flags = (TraitFlag)br.ReadByte();
                if ((t.flags & TraitFlag.HasProtectedNS) > 0)
                    t.protectedNs = (Namespace)namespaces[readU32(br)];

                int interface_count = readU32(br);
                t.interfaces = new QName[interface_count];
                for (int j = 0; j < interface_count; j++)
                {
                    int index = readU32(br);
                    t.interfaces[j] = (QName)names[index];
                }

                MethodInfo m = t.init = (MethodInfo)methods[readU32(br)];
                m.name = t.name;
                m.kind = TraitMember.Method;
                m.id = -1;

                parseTraits(br, t);
            }
            //Console.WriteLine("InstanceInfo size " + (br.BaseStream.Position - start) + " " + (int)(100 * (br.BaseStream.Position - start) / br.BaseStream.Length) + " %");
        }

        private void parseTraits(BinaryReader br, Traits t)
        {
            int namecount = readU32(br);
            for (int i=0; i < namecount; i++)
            {
                QName name = (QName)names[readU32(br)];
                byte tag = br.ReadByte();
                TraitMember kind = (TraitMember)((byte)tag & 0xf);
                MemberInfo member = null;
                switch (kind) 
                {
                    case TraitMember.Slot:
                    case TraitMember.Const:
                    case TraitMember.Class:
                        SlotInfo slot = new SlotInfo();
                        member = slot as MemberInfo;
                        slot.id = readU32(br);
                        t.slots.Add(slot);
                        //t.slots[slot.id] = slot
                        if (kind == TraitMember.Slot || kind == TraitMember.Const)
                        {
                            slot.type = (QName)names[readU32(br)];
                            int index = readU32(br);
                            if (index > 0)
                                slot.value = getDefaultValue((ConstantKind)br.ReadByte(), index);
                        }
                        else // (kind == TraitMember.Class)
                        {
                            slot.value = classes[readU32(br)];
                        }
                        break;
                    case TraitMember.Method:
                    case TraitMember.Getter:
                    case TraitMember.Setter:
                        int disp_id = readU32(br);
                        MethodInfo method = methods[readU32(br)];
                        member = method as MemberInfo;
                        //t.methods[disp_id] = method;
                        t.methods.Add(method);
                        method.id = disp_id;
                        //Console.WriteLine("\t" + kind + " " + name + " " + disp_id + " " + method);
                        break;
                }

                if (member == null)
                    Console.WriteLine("-- Error trait kind "+kind+"\n");
                member.kind = kind;
                member.name = name;
                //t.members[i] = member;
                t.members.Add(member);
                t.names[name] = member;

                Attribute attr = (Attribute)(tag >> 4);
                if ((attr & Attribute.Metadata) > 0)
                {
                    member.metadata = new List<MetaData>();
                    int mdCount = readU32(br);
                    for(int j=0; j < mdCount; ++j)
                        member.metadata.Add((MetaData)metadata[readU32(br)]);
                }
            }
        }

        private void parseClassInfos(BinaryReader br)
        {
            long start = br.BaseStream.Position;
            int count = instances.Length;
            classes = new Traits[count];
            for (int i=0; i < count; i++)
            {
                Traits t = new Traits();
                classes[i] = t; 
                t.init = (MethodInfo)methods[readU32(br)];
                t.baseName = new QName("Class");
                t.itraits = instances[i];
                t.name = new QName(t.itraits.name.localName + "$");
                t.init.name = new QName(t.itraits.name.localName + "$cinit");
                t.init.kind = TraitMember.Method;

                parseTraits(br, t);
            }
            //Console.WriteLine("ClassInfo size " + (br.BaseStream.Position - start) + " " + (int)(100 * (br.BaseStream.Position - start) / br.BaseStream.Length) + "%");
        }

        private void parseScriptInfos(BinaryReader br)
        {
            long start = br.BaseStream.Position;
            int count = readU32(br);
            scripts = new Traits[count];
            for (int i = 0; i < count; i++)
            {
                Traits t = new Traits();
                scripts[i] = t;
                t.name = new QName("script" + i);
                t.baseName = (QName)names[0]; // Object
                t.init = (MethodInfo)methods[readU32(br)];
                t.init.name = new QName(t.name.localName + "$init");
                t.init.kind = TraitMember.Method;

                parseTraits(br, t);
            }
            //Console.WriteLine("ScriptInfo size " + (br.BaseStream.Position - start) + " " + (int)(100 * (br.BaseStream.Position - start) / br.BaseStream.Length) + " %");
        }

        /// <summary>
        /// Call this method to extract the methods information & bytecode
        /// </summary>
        /// <param name="data">AbcTag raw data</param>
        public void parseMethodBodies(byte[] data)
        {
            if (methodBodiesParsed)
                return;
            methodBodiesParsed = true;

            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            long start = methodBodiesPosition;
            br.BaseStream.Position = start;

            int count = readU32(br);
            if (count > methods.Length)
            {
                Console.WriteLine("Invalid method bodies count.");
                return;
            }
            for (int i = 0; i < count; i++)
            {
                MethodInfo m = methods[readU32(br)];
                if (m == null)
                {
                    Console.WriteLine("Invalid method body index");
                    continue;
                }
                m.max_stack = readU32(br);
                m.local_count = readU32(br);
                int initScopeDepth = readU32(br);
                int maxScopeDepth = readU32(br);
                m.max_scope = maxScopeDepth - initScopeDepth;
                int code_length = readU32(br);
                m.code = br.ReadBytes(code_length);
                int ex_count = readU32(br);
                for (int j = 0; j < ex_count; j++)
                {
                    int from = readU32(br);
                    int to = readU32(br);
                    int target = readU32(br);
                    QName type = (QName)names[readU32(br)];
                    //if (magic >= (46<<16|16))
                    QName name = (QName)names[readU32(br)];
                }
                m.activation = new Traits();
                parseTraits(br, m.activation);
                //Console.Write(AbcDump.dumpMethod(this, m, "", ""));
            }
            //Console.WriteLine("MethodBodies size " + (br.BaseStream.Position - start) + " " + (int)(100 * (br.BaseStream.Position - start) / br.BaseStream.Length) + " %");
        }
        #endregion
    }
}
