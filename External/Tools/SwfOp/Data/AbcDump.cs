using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SwfOp.Data
{
    public class AbcDump
    {
        const string TAB = "\t";

        #region OP names
        static string[] opNames = 
        {
            "0x00          ",
            "bkpt          ",
            "nop           ",
            "throw         ",
            "getsuper      ",
            "setsuper      ",
            "dxns          ",
            "dxnslate      ",
            "kill          ",
            "label         ",
            "OP_0x0A       ",
            "OP_0x0B       ",
            "ifnlt         ",
            "ifnle         ",
            "ifngt         ",
            "ifnge         ",
            "jump          ",
            "iftrue        ",
            "iffalse       ",
            "ifeq          ",
            "ifne          ",
            "iflt          ",
            "ifle          ",
            "ifgt          ",
            "ifge          ",
            "ifstricteq    ",
            "ifstrictne    ",
            "lookupswitch  ",
            "pushwith      ",
            "popscope      ",
            "nextname      ",
            "hasnext       ",
            "pushnull      ",
            "pushundefined ",
            "pushconstant  ",
            "nextvalue     ",
            "pushbyte      ",
            "pushshort     ",
            "pushtrue      ",
            "pushfalse     ",
            "pushnan       ",
            "pop           ",
            "dup           ",
            "swap          ",
            "pushstring    ",
            "pushint       ",
            "pushuint      ",
            "pushdouble    ",
            "pushscope     ",
            "pushnamespace ",
            "hasnext2      ",
            "OP_0x33       ",
            "OP_0x34       ",
            "OP_0x35       ",
            "OP_0x36       ",
            "OP_0x37       ",
            "OP_0x38       ",
            "OP_0x39       ",
            "OP_0x3A       ",
            "OP_0x3B       ",
            "OP_0x3C       ",
            "OP_0x3D       ",
            "OP_0x3E       ",
            "OP_0x3F       ",
            "newfunction   ",
            "call          ",
            "construct     ",
            "callmethod    ",
            "callstatic    ",
            "callsuper     ",
            "callproperty  ",
            "returnvoid    ",
            "returnvalue   ",
            "constructsuper",
            "constructprop ",
            "callsuperid   ",
            "callproplex   ",
            "callinterface ",
            "callsupervoid ",
            "callpropvoid  ",
            "applytype     ",
            "OP_0x51       ",
            "OP_0x52       ",
            "OP_0x53       ",
            "OP_0x54       ",
            "newobject     ",
            "newarray      ",
            "newactivation ",
            "newclass      ",
            "getdescendants",
            "newcatch      ",
            "OP_0x5B       ",
            "OP_0x5C       ",
            "findpropstrict",
            "findproperty  ",
            "finddef       ",
            "getlex        ",
            "setproperty   ",
            "getlocal      ",
            "setlocal      ",
            "getglobalscope",
            "getscopeobject",
            "getproperty   ",
            "getouterscope ",
            "initproperty  ",
            "OP_0x69       ",
            "deleteproperty",
            "OP_0x6A       ",
            "getslot       ",
            "setslot       ",
            "getglobalslot ",
            "setglobalslot ",
            "convert_s     ",
            "esc_xelem     ",
            "esc_xattr     ",
            "convert_i     ",
            "convert_u     ",
            "convert_d     ",
            "convert_b     ",
            "convert_o     ",
            "checkfilter   ",
            "OP_0x79       ",
            "OP_0x7A       ",
            "OP_0x7B       ",
            "OP_0x7C       ",
            "OP_0x7D       ",
            "OP_0x7E       ",
            "OP_0x7F       ",
            "coerce        ",
            "coerce_b      ",
            "coerce_a      ",
            "coerce_i      ",
            "coerce_d      ",
            "coerce_s      ",
            "astype        ",
            "astypelate    ",
            "coerce_u      ",
            "coerce_o      ",
            "OP_0x8A       ",
            "OP_0x8B       ",
            "OP_0x8C       ",
            "OP_0x8D       ",
            "OP_0x8E       ",
            "OP_0x8F       ",
            "negate        ",
            "increment     ",
            "inclocal      ",
            "decrement     ",
            "declocal      ",
            "typeof        ",
            "not           ",
            "bitnot        ",
            "OP_0x98       ",
            "OP_0x99       ",
            "concat        ",
            "add_d         ",
            "OP_0x9C       ",
            "OP_0x9D       ",
            "OP_0x9E       ",
            "OP_0x9F       ",
            "add           ",
            "subtract      ",
            "multiply      ",
            "divide        ",
            "modulo        ",
            "lshift        ",
            "rshift        ",
            "urshift       ",
            "bitand        ",
            "bitor         ",
            "bitxor        ",
            "equals        ",
            "strictequals  ",
            "lessthan      ",
            "lessequals    ",
            "greaterthan   ",
            "greaterequals ",
            "instanceof    ",
            "istype        ",
            "istypelate    ",
            "in            ",
            "OP_0xB5       ",
            "OP_0xB6       ",
            "OP_0xB7       ",
            "OP_0xB8       ",
            "OP_0xB9       ",
            "OP_0xBA       ",
            "OP_0xBB       ",
            "OP_0xBC       ",
            "OP_0xBD       ",
            "OP_0xBE       ",
            "OP_0xBF       ",
            "increment_i   ",
            "decrement_i   ",
            "inclocal_i    ",
            "declocal_i    ",
            "negate_i      ",
            "add_i         ",
            "subtract_i    ",
            "multiply_i    ",
            "OP_0xC8       ",
            "OP_0xC9       ",
            "OP_0xCA       ",
            "OP_0xCB       ",
            "OP_0xCC       ",
            "OP_0xCD       ",
            "OP_0xCE       ",
            "OP_0xCF       ",
            "getlocal0     ",
            "getlocal1     ",
            "getlocal2     ",
            "getlocal3     ",
            "setlocal0     ",
            "setlocal1     ",
            "setlocal2     ",
            "setlocal3     ",
            "OP_0xD8       ",
            "OP_0xD9       ",
            "OP_0xDA       ",
            "OP_0xDB       ",
            "OP_0xDC       ",
            "OP_0xDD       ",
            "OP_0xDE       ",
            "OP_0xDF       ",
            "OP_0xE0       ",
            "OP_0xE1       ",
            "OP_0xE2       ",
            "OP_0xE3       ",
            "OP_0xE4       ",
            "OP_0xE5       ",
            "OP_0xE6       ",
            "OP_0xE7       ",
            "OP_0xE8       ",
            "OP_0xE9       ",
            "OP_0xEA       ",
            "OP_0xEB       ",
            "OP_0xEC       ",
            "OP_0xED       ",
            "OP_0xEE       ",
            "debug         ",
            "debugline     ",
            "debugfile     ",
            "bkptline      ",
            "timestamp     ",
            "OP_0xF4       ",
            "verifypass    ",
            "alloc         ",
            "mark          ",
            "wb            ",
            "prologue      ",
            "sendenter     ",
            "doubletoatom  ",
            "sweep         ",
            "codegenop     ",
            "verifyop      ",
            "decode        "
        };
        #endregion

        public static void formatMethod(MethodInfo m, StringBuilder sb)
        {
            if ((m.flags & MethodFlags.Native) > 0)
                sb.Append("native ");

            sb.Append(m.kind).Append(" ").Append(m.name).Append("(");
            formatParams(m, sb);
            sb.Append("):").AppendLine(m.returnType.ToString());
        }

        public static void formatParams(MethodInfo m, StringBuilder sb)
        {
            for (int i = 0; i < m.paramTypes.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                if (m.paramNames != null)
                    sb.Append(m.paramNames[i]).Append(":");
                sb.Append(m.paramTypes[i]);
            }
        }

        public static string dumpMethod(Abc abc, MethodInfo m, string indent, string attr)
        {
            StringBuilder sb = new StringBuilder();

            if (m.metadata != null)
            {
                foreach (MetaData md in m.metadata)
                    sb.Append(indent).AppendLine(md.ToString());
            }

            sb.Append(indent);
            formatMethod(m, sb);
            if (m.code != null && m.code.Length > 0)
            {
                sb.AppendLine(indent + "{");

                if ((m.flags & MethodFlags.NeedActivation) > 0)
                {
                    sb.AppendLine(indent + TAB + "// NEED ACTIVATION ???");
                    //m.activation.dump(abc, indent+TAB, "");
                }

                dumpCode(abc, m, indent + TAB, sb);

                sb.AppendLine(indent + "}\n");
            }
            return sb.ToString();
        }

        private static void dumpCode(Abc abc, MethodInfo m, string indent, StringBuilder sb)
        {
            MemoryStream ms = new MemoryStream(m.code);
            BinaryReader br = new BinaryReader(ms);

            //long totalSize = 0;
            //long[] opSizes = new long[256];
            string[] stack = new string[m.max_stack];
            LabelInfo labels = new LabelInfo();
            long len = m.code.Length;
            while (br.BaseStream.Position < len)
            {
                long start = br.BaseStream.Position;
                string s = indent + start;
                while (s.Length < 12) s += ' ';
                int opcode = br.ReadByte();

                if (opcode == (int)Op.label || labels.ContainsKey(br.BaseStream.Position - 1))
                {
                    sb.AppendLine(indent)
                        .AppendLine(indent + labels.labelFor(br.BaseStream.Position - 1) + ": ");
                }

                s += opNames[opcode];
                s += opNames[opcode].Length < 8 ? TAB + TAB : TAB;

                switch ((Op)opcode)
                {
                    case Op.debugfile:
                    case Op.pushstring:
                        s += '"' + abc.strings[Abc.readU32(br)].ToString().Replace("\n", "\\n").Replace("\t", "\\t") + '"';
                        break;
                    case Op.pushnamespace:
                        s += abc.namespaces[Abc.readU32(br)];
                        break;
                    case Op.pushint:
                        int i = (int)abc.ints[Abc.readU32(br)];
                        s += i;// + "\t// 0x" + i.ToString(16);
                        break;
                    case Op.pushuint:
                        uint u = (uint)abc.uints[Abc.readU32(br)];
                        s += u;// + "\t// 0x" + u.ToString(16);
                        break;
                    case Op.pushdouble:
                        s += abc.doubles[Abc.readU32(br)];
                        break;
                    case Op.getsuper:
                    case Op.setsuper:
                    case Op.getproperty:
                    case Op.initproperty:
                    case Op.setproperty:
                    case Op.getlex:
                    case Op.findpropstrict:
                    case Op.findproperty:
                    case Op.finddef:
                    case Op.deleteproperty:
                    case Op.istype:
                    case Op.coerce:
                    case Op.astype:
                    case Op.getdescendants:
                        s += abc.names[Abc.readU32(br)];
                        break;
                    case Op.constructprop:
                    case Op.callproperty:
                    case Op.callproplex:
                    case Op.callsuper:
                    case Op.callsupervoid:
                    case Op.callpropvoid:
                        s += abc.names[Abc.readU32(br)];
                        s += " (" + Abc.readU32(br) + ")";
                        break;
                    case Op.newfunction:
                        {
                            var method_id = Abc.readU32(br);
                            s += abc.methods[method_id];
                            abc.methods[method_id].anon = true;
                            break;
                        }
                    case Op.callstatic:
                        s += abc.methods[Abc.readU32(br)];
                        s += " (" + Abc.readU32(br) + ")";
                        break;
                    case Op.newclass:
                        s += abc.instances[Abc.readU32(br)];
                        break;
                    case Op.lookupswitch:
                        var pos = br.BaseStream.Position - 1;
                        var target = pos + readS24(br);
                        var maxindex = Abc.readU32(br);
                        s += "default:" + labels.labelFor(target); // target + "("+(target-pos)+")"
                        s += " maxcase:" + maxindex;
                        for (int j = 0; j <= maxindex; j++)
                        {
                            target = pos + readS24(br);
                            s += " " + labels.labelFor(target); // target + "("+(target-pos)+")"
                        }
                        break;
                    case Op.jump:
                    case Op.iftrue:
                    case Op.iffalse:
                    case Op.ifeq:
                    case Op.ifne:
                    case Op.ifge:
                    case Op.ifnge:
                    case Op.ifgt:
                    case Op.ifngt:
                    case Op.ifle:
                    case Op.ifnle:
                    case Op.iflt:
                    case Op.ifnlt:
                    case Op.ifstricteq:
                    case Op.ifstrictne:
                        int offset = readS24(br);
                        target = br.BaseStream.Position + offset;
                        //s += target + " ("+offset+")"
                        s += labels.labelFor(target);
                        if (!labels.ContainsKey(br.BaseStream.Position))
                            s += "\n";
                        break;
                    case Op.inclocal:
                    case Op.declocal:
                    case Op.inclocal_i:
                    case Op.declocal_i:
                    case Op.getlocal:
                    case Op.kill:
                    case Op.setlocal:
                    case Op.debugline:
                    case Op.getglobalslot:
                    case Op.getslot:
                    case Op.setglobalslot:
                    case Op.setslot:
                    case Op.pushshort:
                    case Op.newcatch:
                        s += Abc.readU32(br);
                        break;
                    case Op.debug:
                        s += br.ReadByte();
                        s += " " + Abc.readU32(br);
                        s += " " + br.ReadByte();
                        s += " " + Abc.readU32(br);
                        break;
                    case Op.newobject:
                        s += "{" + Abc.readU32(br) + "}";
                        break;
                    case Op.newarray:
                        s += "[" + Abc.readU32(br) + "]";
                        break;
                    case Op.call:
                    case Op.construct:
                    case Op.constructsuper:
                    case Op.applytype:
                        s += "(" + Abc.readU32(br) + ")";
                        break;
                    case Op.pushbyte:
                    case Op.getscopeobject:
                        s += br.ReadSByte();
                        break;
                    case Op.hasnext2:
                        s += Abc.readU32(br) + " " + Abc.readU32(br);
                        break;
                    default:
                        /*if (opNames[opcode] == ("0x"+opcode.toString(16).toUpperCase()))
                            s += " UNKNOWN OPCODE"*/
                        break;
                }
                long size = br.BaseStream.Position - start;
                //totalSize += size;
                //opSizes[opcode] = opSizes[opcode] + size;
                sb.AppendLine(s);
            }
        }

        static int readS24(BinaryReader br)
        {
            int b = br.ReadByte();
            b |= br.ReadByte() << 8;
            b |= br.ReadSByte() << 16;
            return b;
        }
    }
}
