using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AirProperties
{
    class AndroidManifestManager
    {
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";

        private XmlDocument backDoc;
        private XmlNode usesSdkNode;
        private UsesPermissionCollection _usesPermissions;

        private bool removeNamespace;

        public UsesPermissionCollection UsesPermissions
        {
            get { return _usesPermissions; }
        }

        public UsesSdkElement UsesSdk { get; set; }

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
                            var usesPermission = new UsesPermissionElement();
                            usesPermission.Name = node.Attributes.GetNamedItem("name", AndroidNamespace).Value;
                            var maxSdkAttribute = node.Attributes.GetNamedItem("maxSdkVersion", AndroidNamespace);

                            if (maxSdkAttribute != null)
                                usesPermission.MaxSdkVersion = int.Parse(maxSdkAttribute.Value);

                            _usesPermissions.Add(usesPermission, (XmlElement)node);

                            break;

                        case "uses-sdk":
                            var usesSdk = new UsesSdkElement();
                            var minSdkAttribute = node.Attributes.GetNamedItem("minSdkVersion", AndroidNamespace);
                            var targetSdkAttribute = node.Attributes.GetNamedItem("targetSdkVersion", AndroidNamespace);
                            maxSdkAttribute = node.Attributes.GetNamedItem("maxSdkVersion", AndroidNamespace);

                            if (maxSdkAttribute != null)
                                usesSdk.MaxSdkVersion = int.Parse(maxSdkAttribute.Value);

                            if (minSdkAttribute != null)
                                usesSdk.MinSdkVersion = int.Parse(minSdkAttribute.Value);

                            if (targetSdkAttribute != null)
                                usesSdk.TargetSdkVersion = int.Parse(targetSdkAttribute.Value);

                            UsesSdk = usesSdk;
                            usesSdkNode = node;

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

        private void SetUsesSdk()
        {
            if (usesSdkNode == null && UsesSdk != null)
            {
                usesSdkNode = backDoc.CreateElement("uses-sdk");
                backDoc.FirstChild.PrependChild(usesSdkNode);
            }
            if (usesSdkNode != null)
            {
                if (UsesSdk == null)
                {
                    backDoc.FirstChild.RemoveChild(usesSdkNode);
                    usesSdkNode = null;

                    return;
                }

                var minSdkAttribute = usesSdkNode.Attributes.GetNamedItem("minSdkVersion", AndroidNamespace);
                var targetSdkAttribute = usesSdkNode.Attributes.GetNamedItem("targetSdkVersion", AndroidNamespace);
                var maxSdkAttribute = usesSdkNode.Attributes.GetNamedItem("maxSdkVersion", AndroidNamespace);

                if (UsesSdk.MaxSdkVersion > 0)
                {
                    if (maxSdkAttribute == null)
                    {
                        maxSdkAttribute = backDoc.CreateAttribute("maxSdkVersion", AndroidNamespace);
                        usesSdkNode.Attributes.Append((XmlAttribute)maxSdkAttribute);
                    }
                    maxSdkAttribute.Value = UsesSdk.MaxSdkVersion.ToString();
                }
                else if (maxSdkAttribute != null)
                    usesSdkNode.Attributes.Remove((XmlAttribute) maxSdkAttribute);

                if (UsesSdk.MinSdkVersion > 0)
                {
                    if (minSdkAttribute == null)
                    {
                        minSdkAttribute = backDoc.CreateAttribute("minSdkVersion", AndroidNamespace);
                        usesSdkNode.Attributes.Append((XmlAttribute)minSdkAttribute);
                    }
                    minSdkAttribute.Value = UsesSdk.MinSdkVersion.ToString();
                }
                else if (minSdkAttribute != null)
                    usesSdkNode.Attributes.Remove((XmlAttribute)minSdkAttribute);

                if (UsesSdk.TargetSdkVersion > 0)
                {
                    if (targetSdkAttribute == null)
                    {
                        targetSdkAttribute = backDoc.CreateAttribute("targetSdkVersion", AndroidNamespace);
                        usesSdkNode.Attributes.Append((XmlAttribute)targetSdkAttribute);
                    }
                    targetSdkAttribute.Value = UsesSdk.TargetSdkVersion.ToString();
                }
                else if (targetSdkAttribute != null)
                    usesSdkNode.Attributes.Remove((XmlAttribute)targetSdkAttribute);
            }
        }

        public string GetManifestXml()
        {
            SetUsesSdk();

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

        public class UsesPermissionElement
        {
            public string Name { get; set; }
            public int MaxSdkVersion { get; set; }
        }

        public class UsesPermissionCollection : ICollection<UsesPermissionElement>
        {

            private Dictionary<string, UsesPermissionElement> backData = new Dictionary<string, UsesPermissionElement>();
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

            public void Add(UsesPermissionElement item)
            {
                UsesPermissionElement prevItem = null;
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

            internal void Add(UsesPermissionElement item, XmlElement node)
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

            public bool Contains(UsesPermissionElement item)
            {
                return backData.ContainsKey(item.Name);
            }

            public bool ContainsName(string permissionName)
            {
                return backData.ContainsKey(permissionName);
            }

            public void CopyTo(UsesPermissionElement[] array, int arrayIndex)
            {
                backData.Values.CopyTo(array, arrayIndex);
            }

            public bool Remove(UsesPermissionElement item)
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

            public UsesPermissionElement Get(string permissionName)
            {
                return backData[permissionName];
            }

            public IEnumerator<UsesPermissionElement> GetEnumerator()
            {
                return backData.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return backData.Values.GetEnumerator();
            }

        }

        public class UsesSdkElement
        {
            public int MinSdkVersion { get; set; }
            public int MaxSdkVersion { get; set; }
            public int TargetSdkVersion { get; set; }
        }

    }
}
