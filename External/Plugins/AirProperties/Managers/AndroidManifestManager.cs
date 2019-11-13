using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PluginCore;

namespace AirProperties
{
    class AndroidManifestManager
    {
        const string AndroidNamespace = "http://schemas.android.com/apk/res/android";
        readonly XmlDocument backDoc;
        XmlNode usesSdkNode;
        readonly bool removeNamespace;

        public UsesPermissionCollection UsesPermissions { get; }

        public UsesSdkElement UsesSdk { get; set; }

        public AndroidManifestManager() : this(string.Empty)
        {
        }

        public AndroidManifestManager(string manifest)
        {

            UsesPermissions = new UsesPermissionCollection(this);
            backDoc = new XmlDocument();

            if (!string.IsNullOrEmpty(manifest))
            {
                if (!manifest.StartsWithOrdinal("<manifest"))
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

                            UsesPermissions.Add(usesPermission, (XmlElement)node);

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
                var rootNode = backDoc.CreateElement("manifest");
                var installLocation = backDoc.CreateAttribute("android", "installLocation", "http://schemas.android.com/apk/res/android");
                installLocation.Value = "auto";
                rootNode.Attributes.Append(installLocation);
                backDoc.AppendChild(rootNode);
            }

        }

        void SetUsesSdk()
        {
            if (usesSdkNode is null && UsesSdk != null)
            {
                usesSdkNode = backDoc.CreateElement("uses-sdk");
                backDoc.FirstChild.PrependChild(usesSdkNode);
            }
            if (usesSdkNode is null) return;
            if (UsesSdk is null)
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
                if (maxSdkAttribute is null)
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
                if (minSdkAttribute is null)
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
                if (targetSdkAttribute is null)
                {
                    targetSdkAttribute = backDoc.CreateAttribute("targetSdkVersion", AndroidNamespace);
                    usesSdkNode.Attributes.Append((XmlAttribute)targetSdkAttribute);
                }
                targetSdkAttribute.Value = UsesSdk.TargetSdkVersion.ToString();
            }
            else if (targetSdkAttribute != null)
                usesSdkNode.Attributes.Remove((XmlAttribute)targetSdkAttribute);
        }

        public string GetManifestXml()
        {
            SetUsesSdk();

            var builder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true,
                IndentChars = "\x09" //tab
            };
            using var xw = XmlWriter.Create(builder, settings);
            backDoc.Save(xw);
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
            readonly Dictionary<string, UsesPermissionElement> backData = new Dictionary<string, UsesPermissionElement>();
            readonly Dictionary<string, XmlElement> mapping = new Dictionary<string, XmlElement>();
            readonly AndroidManifestManager owner;

            internal UsesPermissionCollection(AndroidManifestManager owner)
            {
                this.owner = owner;
            }

            public int Count => backData.Count;

            public bool IsReadOnly => false;

            public void Add(UsesPermissionElement item)
            {
                XmlElement node;
                if (backData.TryGetValue(item.Name, out _))
                {
                    node = mapping[item.Name];
                    var sdkAttribute = node.GetAttributeNode("maxSdkVersion", AndroidNamespace);
                    if (item.MaxSdkVersion > 0)
                    {
                        if (sdkAttribute is null)
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

            public bool Contains(UsesPermissionElement item) => backData.ContainsKey(item.Name);

            public bool ContainsName(string permissionName) => backData.ContainsKey(permissionName);

            public void CopyTo(UsesPermissionElement[] array, int arrayIndex) => backData.Values.CopyTo(array, arrayIndex);

            public bool Remove(UsesPermissionElement item)
            {
                if (!backData.Remove(item.Name)) return false;
                owner.backDoc.FirstChild.RemoveChild(mapping[item.Name]);
                mapping.Remove(item.Name);
                return true;
            }

            public bool RemoveByName(string permissionName)
            {
                if (!backData.Remove(permissionName)) return false;
                owner.backDoc.FirstChild.RemoveChild(mapping[permissionName]);
                mapping.Remove(permissionName);
                return true;
            }

            public UsesPermissionElement Get(string permissionName) => backData[permissionName];

            public IEnumerator<UsesPermissionElement> GetEnumerator() => backData.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => backData.Values.GetEnumerator();
        }

        public class UsesSdkElement
        {
            public int MinSdkVersion { get; set; }
            public int MaxSdkVersion { get; set; }
            public int TargetSdkVersion { get; set; }
        }
    }
}