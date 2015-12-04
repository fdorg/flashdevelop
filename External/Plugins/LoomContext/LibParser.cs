using System;
using System.Collections.Generic;
using System.IO;
using ASCompletion.Model;
using LitJson;
using PluginCore.Managers;

namespace LoomContext
{
    public class LibParser
    {
        public PathModel path;
        Context context;
        Dictionary<string, FileModel> files;

        static public void Parse(PathModel path, Context context)
        {
            LibParser inst = new LibParser(path, context);
            inst.Run();
        }

        public LibParser(PathModel path, Context context)
        {
            this.path = path;
            this.context = context;
        }

        private void Run()
        {
            using (StreamReader sr = new StreamReader(path.Path))
            {
                string raw = sr.ReadToEnd();
                sr.Close();
                JsonReader reader = new JsonReader(raw);
                try
                {
                    if (reader.Read() && reader.Token == JsonToken.ObjectStart)
                    {
                        files = new Dictionary<string, FileModel>();
                        BuildModel(reader);
                        path.SetFiles(files);
                    }
                }
                catch (Exception ex)
                {
                    TraceManager.AddAsync(ex.Message);
                }
            }
        }

        private void BuildModel(JsonReader reader)
        {
            // root/
            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    reader.SkipObject();
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    if (prop == "modules") ReadModules(reader);
                    else reader.SkipArray();
                }
            }
        }

        private void ReadModules(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.ObjectStart) ReadModule(reader);
            }
        }

        private void ReadModule(JsonReader reader)
        {
            // root/modules
            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    if (prop == "types") ReadTypes(reader);
                    else reader.SkipArray();
                }
                else if (reader.Token == JsonToken.ObjectStart) {
                    reader.SkipObject();
                }
            }
        }

        private void ReadTypes(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.ObjectStart) ReadType(reader);
            }
        }

        private void ReadType(JsonReader reader)
        {
            // root/modules/types
            FileModel cFile;
            ClassModel cClass;
            cFile = new FileModel();
            cFile.HasPackage = true;
            cFile.Version = 3;
            cFile.Context = context;
            cClass = new ClassModel();
            cClass.Flags = FlagType.Class;
            MemberModel cDelegate = new MemberModel();
            List<string> names;

            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "name": cClass.Name = val; break;
                        case "source":
                            if (val.IndexOf('/') > 0) val = val.Substring(val.IndexOf('/') + 1);
                            cFile.FileName = path.Path + "::" + val;
                            break;
                        case "package":
                            if (val.ToLower() == "cocos2d") val = "cocos2d"; // random casing through source
                            cFile.Package = val; 
                            break;
                        case "baseType": cClass.ExtendsType = val; break;
                        case "delegateReturnType": cDelegate.Type = CleanType(val); break;
                        case "type":
                            switch (val)
                            {
                                case "CLASS": break;
                                case "INTERFACE": cClass.Flags |= FlagType.Interface; break;
                                case "STRUCT": cClass.Flags |= FlagType.Struct; break;
                                case "DELEGATE": cDelegate.Name = cClass.Name; break;
                            }
                            break;
                        case "docstring": cClass.Comments = ExtractDoc(val); break;
                    }
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    switch (prop)
                    {
                        case "classattributes": ReadAttributes(reader, cClass); break;
                        case "fields": ReadFields(reader, cClass); break;
                        case "methods": ReadMethods(reader, cClass); break;
                        case "properties": ReadProperties(reader, cClass); break;
                        case "metainfo": ReadMetas(reader, cClass); break;
                        case "imports": 
                            names = ReadNames(reader);
                            foreach (string name in names)
                                cFile.Imports.Add(new MemberModel(name, name, FlagType.Import, Visibility.Public)); 
                            break;
                        case "interfaces": ReadInterfaces(reader, cClass); break;
                        case "delegateTypes":
                            names = ReadNames(reader);
                            if (names.Count > 0)
                            {
                                cDelegate.Parameters = new List<MemberModel>();
                                foreach (string argType in names)
                                {
                                    cDelegate.Parameters.Add(
                                        new MemberModel("p" + cDelegate.Parameters.Count, argType, FlagType.ParameterVar, Visibility.Public));
                                }
                            }
                            break;
                        default: reader.SkipArray(); break;
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    switch (prop)
                    {
                        case "constructor": 
                            MemberModel ctor = ReadMethod(reader, cClass);
                            cClass.Constructor = ctor.Name;
                            break;
                        default: reader.SkipObject(); break;
                    }
                }
            }

            if (cFile.FileName == null) return;
            string key = cFile.FileName.ToUpper();
            if (files.ContainsKey(key)) cFile = files[key];
            else files.Add(key, cFile);

            if (cFile.Package.ToLower() == "system") // System classes tweaks
            {
                cFile.Package = "";
                if (cClass.Name == "Vector") cClass.Name = "Vector.<T>";
                if (cClass.Name == "Object") cClass.ExtendsType = "void";
            }
            if (cClass.Access == Visibility.Private) cClass.Access = Visibility.Public;

            if (cDelegate.Name != null)
            {
                if (cDelegate.Parameters == null) cDelegate.Parameters = new List<MemberModel>();
                cDelegate.Access = Visibility.Public;
                cDelegate.Flags = FlagType.Function | FlagType.Delegate;
                cDelegate.InFile = cFile;
                cDelegate.IsPackageLevel = true;
                cFile.Members.Add(cDelegate);
            }
            else
            {
                cClass.Type = CleanType(String.IsNullOrEmpty(cFile.Package) ? cClass.Name : cFile.Package + "." + cClass.Name);
                cClass.InFile = cFile;
                cFile.Classes.Add(cClass);
            }
        }

        private string ExtractDoc(string comment)
        {
            if (string.IsNullOrEmpty(comment)) return "";
            if (comment.StartsWith("/**")) return comment.Substring(3, comment.Length - 5).Trim();
            if (comment.StartsWith("/*")) return comment.Substring(2, comment.Length - 4).Trim();
            if (comment.StartsWith("//")) return comment.Substring(2).Trim();
            return comment;
        }

        private void ReadInterfaces(JsonReader reader, ClassModel cClass)
        {
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.String)
                {
                    string type = reader.Value.ToString();
                    if (cClass.Implements == null) cClass.Implements = new List<string>();
                    cClass.Implements.Add(type);
                }
            }
        }

        private List<String> ReadNames(JsonReader reader)
        {
            List<string> values = new List<string>();
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.String)
                {
                    string val = CleanType(reader.Value.ToString());
                    values.Add(val);
                }
            }
            return values;
        }

        private void ReadFields(JsonReader reader, ClassModel cClass)
        {
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.ObjectStart) ReadField(reader, cClass);
            }
        }

        private void ReadMethods(JsonReader reader, ClassModel cClass)
        {
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.ObjectStart) ReadMethod(reader, cClass);
            }
        }

        private void ReadProperties(JsonReader reader, ClassModel cClass)
        {
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.ObjectStart) ReadProperty(reader, cClass);
            }
        }

        private MemberModel ReadField(JsonReader reader, ClassModel cClass)
        {
            MemberModel member = new MemberModel();
            member.Flags = FlagType.Variable;

            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "name": member.Name = val; cClass.Members.Add(member); break;
                        case "type": if (member.Type == null) member.Type = CleanType(val); break;
                        case "docstring": member.Comments = ExtractDoc(val); break;
                    }
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    switch (prop)
                    {
                        case "fieldattributes": 
                            ReadAttributes(reader, member);
                            if ((member.Flags & FlagType.Static) == 0) member.Flags |= FlagType.Dynamic;
                            break;
                        case "metainfo": ReadMetas(reader, member); break;
                        default: reader.SkipArray(); break;
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    switch (prop)
                    {
                        case "templatetypes": ReadTemplateTypes(reader, member); break;
                        default: reader.SkipObject(); break;
                    }
                }
            }
            return member;
        }

        private MemberModel ReadMethod(JsonReader reader, ClassModel cClass)
        {
            MemberModel member = new MemberModel();
            member.Flags = FlagType.Function;

            string prop = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "name": 
                            if (!val.StartsWith("__op_")) {
                                member.Name = val; 
                                cClass.Members.Add(member); 
                            }
                            break;
                        case "returntype": if (member.Type == null) member.Type = CleanType(val); break;
                        case "docstring": member.Comments = ExtractDoc(val); break;
                    }
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    switch (prop)
                    {
                        case "methodattributes": 
                            ReadAttributes(reader, member); 
                            if ((member.Flags & FlagType.Static) == 0) member.Flags |= FlagType.Dynamic;
                            break;
                        case "parameters": ReadParameters(reader, member); break;
                        case "metainfo": ReadMetas(reader, member); break;
                        default: reader.SkipArray(); break;
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    switch (prop)
                    {
                        case "templatetypes": ReadTemplateTypes(reader, member); break;
                        default: reader.SkipObject(); break;
                    }
                }
            }
            return member;
        }

        private void ReadProperty(JsonReader reader, ClassModel cClass)
        {
            MemberModel member = new MemberModel();
            member.Flags = FlagType.Variable;

            MemberModel getter = null;
            MemberModel setter = null;
            string prop = null;
            string name = null;
            string doc = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "name": name = val; break;
                        case "docstring": doc = ExtractDoc(val); break;
                    }
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    switch (prop)
                    {
                        //case "propertyattributes": ReadAttributes(reader, member); break;
                        //case "metainfo": reader.SkipArray(); break;
                        default: reader.SkipArray(); break;
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    switch (prop)
                    {
                        case "getter": 
                            getter = ReadMethod(reader, cClass);
                            getter.Flags |= FlagType.Getter;
                            break;
                        case "setter":
                            setter = ReadMethod(reader, cClass);
                            setter.Flags |= FlagType.Setter;
                            break;
                        default: reader.SkipObject(); break;
                    }
                }
            }
            if (getter != null)
            {
                getter.Name = name;
                getter.Comments = doc;
                getter.Flags &= ~FlagType.Function;
            }
            if (setter != null)
            {
                setter.Name = name;
                if (getter == null) setter.Comments = doc;
                setter.Flags &= ~FlagType.Function;
            }
        }

        private void ReadTemplateTypes(JsonReader reader, MemberModel member)
        {
            string prop = null;
            List<string> names = null;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "type": member.Type = CleanType(val); break;
                    }
                }
                else if (reader.Token == JsonToken.ArrayStart)
                {
                    switch (prop)
                    {
                        case "types": names = ReadNames(reader); break;
                        default: reader.SkipArray(); break;
                    }
                }
            }
            if (names != null) 
                member.Type += ".<" + String.Join(",", names.ToArray()) + ">";
        }

        private void ReadParameters(JsonReader reader, MemberModel member)
        {
            member.Parameters = new List<MemberModel>();

            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.ObjectStart) ReadParameter(reader, member);
            }
        }

        private MemberModel ReadParameter(JsonReader reader, MemberModel member)
        {
            MemberModel para = new MemberModel("???", null, FlagType.Variable | FlagType.ParameterVar, Visibility.Public);
            member.Parameters.Add(para);

            string prop = null;
            bool isRest = false;
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd) break;
                if (reader.Token == JsonToken.PropertyName) prop = reader.Value.ToString();
                else if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (prop)
                    {
                        case "hasdefault": para.Value = "?"; break; // TODO "hasdefault" is that used?
                        case "isvarargs": isRest = true; break;  // TODO "isvarargs" is that used?
                        case "name": para.Name = val; break;
                        case "type": if (para.Type == null) para.Type = CleanType(val); break;
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart)
                {
                    switch (prop)
                    {
                        case "templatetypes": ReadTemplateTypes(reader, para); break;
                        default: reader.SkipObject(); break;
                    }
                }
            }
            if (isRest) para.Type = "..." + para.Type;
            return para;
        }

        private string CleanType(string name)
        {
            if (name.StartsWith("System.")) 
                return name.Substring(7);
            if (name.StartsWith("Cocos2D."))
                return "cocos2d." + name.Substring(8);
            return name;
        }

        private void ReadMetas(JsonReader reader, MemberModel member)
        {
            reader.SkipArray();
        }

        private void ReadAttributes(JsonReader reader, MemberModel decl)
        {
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ArrayEnd) break;
                if (reader.Token == JsonToken.String)
                {
                    string val = reader.Value.ToString();
                    switch (val)
                    {
                        case "native": decl.Flags |= FlagType.Native; break;
                        case "static": decl.Flags |= FlagType.Static; break;
                        case "public": decl.Access |= Visibility.Public; break;
                        case "internal": decl.Access |= Visibility.Internal; break;
                        case "protected": decl.Access |= Visibility.Protected; break;
                        case "private": decl.Access |= Visibility.Private; break;
                        case "final": decl.Flags |= FlagType.Final; break;
                        case "operator": break;
                        case "supercall": break;
                        default: break;
                    }
                }
            }
        }
    }
}
