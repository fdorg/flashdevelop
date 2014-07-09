using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AirProperties
{
    class AndroidManifestManager
    {
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";

        private XmlDocument backDoc;
        private UsesPermissionCollection _usesPermissions;

        private bool removeNamespace;

        public UsesPermissionCollection UsesPermissions
        {
            get { return _usesPermissions; }
        }

        public AndroidManifestManager()
            : this(string.Empty)
        {
        }

        public AndroidManifestManager(string manifest)
        {

            _usesPermissions = new UsesPermissionCollection(this);
            backDoc = new XmlDocument();

            if (!string.IsNullOrEmpty(manifest))
            {
                if (!manifest.StartsWith("<manifest"))
                    throw new ArgumentException("Not valid manifest string");
                if (!manifest.Contains("xmlns:android"))
                {
                    removeNamespace = true;
                    manifest = "<manifest xmlns:android=\"" + AndroidNamespace + "\"" + manifest.Substring(9);
                }
                backDoc.LoadXml(manifest);

                foreach (XmlNode node in backDoc.FirstChild.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element) continue;

                    switch (node.Name)
                    {
                        case "uses-permission":
                            var usesPermission = new UsesPermission();
                            usesPermission.Name = node.Attributes.GetNamedItem("name", AndroidNamespace).Value;
                            var maxSdkAttribute = node.Attributes.GetNamedItem("maxSdkVersion", AndroidNamespace);

                            if (maxSdkAttribute != null)
                                usesPermission.MaxSdkVersion = int.Parse(maxSdkAttribute.Value);

                            _usesPermissions.Add(usesPermission, (XmlElement)node);

                            break;
                    }
                }
            }
            else
            {
                XmlElement rootNode = backDoc.CreateElement("manifest");
                XmlAttribute installLocation = backDoc.CreateAttribute("android", "installLocation", "http://schemas.android.com/apk/res/android");
                installLocation.Value = "auto";
                rootNode.Attributes.Append(installLocation);
                backDoc.AppendChild(rootNode);
            }

        }

        public string GetManifestXml()
        {
            string xml = backDoc.OuterXml;
            var builder = new StringBuilder();
            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true,
                IndentChars = "\x09" //tab
            };
            using (XmlWriter xw = XmlWriter.Create(builder, settings))
            {
                backDoc.Save(xw);
            }

            if (removeNamespace) builder.Remove(9, 17 + AndroidNamespace.Length);
            return builder.ToString();
        }

        public class UsesPermission
        {
            public string Name { get; set; }
            public int MaxSdkVersion { get; set; }
        }

        public class UsesPermissionCollection : ICollection<UsesPermission>
        {

            private Dictionary<string, UsesPermission> backData = new Dictionary<string, UsesPermission>();
            private Dictionary<string, XmlElement> mapping = new Dictionary<string, XmlElement>();
            private AndroidManifestManager owner;

            internal UsesPermissionCollection(AndroidManifestManager owner)
            {
                this.owner = owner;
            }

            public int Count
            {
                get { return backData.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public void Add(UsesPermission item)
            {
                UsesPermission prevItem = null;
                XmlElement node;

                if (backData.TryGetValue(item.Name, out prevItem))
                {
                    node = mapping[item.Name];
                    var sdkAttribute = node.GetAttributeNode("maxSdkVersion", AndroidNamespace);
                    if (item.MaxSdkVersion > 0)
                    {
                        if (sdkAttribute == null)
                        {
                            sdkAttribute = owner.backDoc.CreateAttribute("maxSdkVersion", AndroidNamespace);
                            node.Attributes.Append(sdkAttribute);
                        }
                        sdkAttribute.Value = item.MaxSdkVersion.ToString();
                    }
                    else if (sdkAttribute != null)
                    {
                        node.RemoveAttributeNode(sdkAttribute);
                    }
                }
                else
                {
                    node = owner.backDoc.CreateElement("uses-permission");
                    var nameAttribute = owner.backDoc.CreateAttribute("name", AndroidNamespace);
                    nameAttribute.Value = item.Name;
                    node.Attributes.Append(nameAttribute);
                    if (item.MaxSdkVersion > 0)
                    {
                        var sdkAttribute = owner.backDoc.CreateAttribute("maxSdkVersion", AndroidNamespace);
                        sdkAttribute.Value = item.MaxSdkVersion.ToString();
                        node.Attributes.Append(sdkAttribute);
                    }
                    if (owner.backDoc.FirstChild.ChildNodes.Count > 0)
                        owner.backDoc.FirstChild.InsertBefore(node, owner.backDoc.FirstChild.FirstChild);
                    else
                        owner.backDoc.FirstChild.AppendChild(node);
                }
                backData[item.Name] = item;
                mapping[item.Name] = node;
            }

            internal void Add(UsesPermission item, XmlElement node)
            {
                backData[item.Name] = item;
                mapping[item.Name] = node;
            }

            public void Clear()
            {
                backData.Clear();
                foreach (var element in mapping.Values)
                {
                    owner.backDoc.FirstChild.RemoveChild(element);
                }
                mapping.Clear();
            }

            public bool Contains(UsesPermission item)
            {
                return backData.ContainsKey(item.Name);
            }

            public bool ContainsName(string permissionName)
            {
                return backData.ContainsKey(permissionName);
            }

            public void CopyTo(UsesPermission[] array, int arrayIndex)
            {
                backData.Values.CopyTo(array, arrayIndex);
            }

            public bool Remove(UsesPermission item)
            {
                if (backData.Remove(item.Name))
                {
                    owner.backDoc.FirstChild.RemoveChild(mapping[item.Name]);
                    mapping.Remove(item.Name);

                    return true;
                }

                return false;
            }

            public bool RemoveByName(string permissionName)
            {
                if (backData.Remove(permissionName))
                {
                    owner.backDoc.FirstChild.RemoveChild(mapping[permissionName]);
                    mapping.Remove(permissionName);

                    return true;
                }

                return false;
            }

            public UsesPermission Get(string permissionName)
            {
                return backData[permissionName];
            }

            public IEnumerator<UsesPermission> GetEnumerator()
            {
                return backData.Values.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return backData.Values.GetEnumerator();
            }

        }

    }
}
