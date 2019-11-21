// TODO: Some of these methods should be merged with WizardHelper

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using PluginCore;
using PluginCore.Localization;

namespace AirProperties
{
    // Air Application Properties Manager
    class PropertyManager
    {
        static XmlDocument _descriptorFile;
        static XmlNamespaceManager _namespaceManager;
        static XmlNode _rootNode;
        const string _BaseAirNamespace = "http://ns.adobe.com/air/application/";
        const string _MaxSupportedVersion = "33.0";

        public enum AirVersion
        {
            V10 = 0,    // Version 1.0
            V11 = 1,    // Version 1.1
            V15 = 2,    // Version 1.5
            V153 = 3,   // Version 1.5.3
            V20 = 4,    // Version 2.0
            V25 = 5,    // Version 2.5
            V26 = 6,    // Version 2.6
            V27 = 7,    // Version 2.7
            V30 = 9,    // Version 3.0
            V31 = 10,   // Version 3.1
            V32 = 11,   // Version 3.2
            V33 = 12,   // Version 3.3
            V34 = 13,   // Version 3.4
            V35 = 14,   // Version 3.5
            V36 = 15,   // Version 3.6
            V37 = 16,   // Version 3.7
            V38 = 17,   // Version 3.8
            V39 = 18,   // Version 3.9
            V40 = 19,   // Version 4.0
            V130 = 20,  // Version 13.0
            V140 = 21,  // Version 14.0
            V150 = 22,  // Version 15.0
            V160 = 23,  // Version 16.0
            V170 = 24,  // Version 17.0
            V180 = 25,  // Version 18.0
            V190 = 26,  // Version 19.0
            V200 = 27,  // Version 20.0
            V210 = 28,  // Version 21.0
            V220 = 29,  // Version 22.0
            V230 = 34,  // Version 23.0
            V240 = 35,  // Version 24.0
            V250 = 36,  // Version 25.0
            V260 = 37,  // Version 26.0
            V270 = 38,  // Version 27.0
            V280 = 39,  // Version 28.0
            V290 = 40,  // Version 29.0
            V300 = 41,  // Version 30.0
            V310 = 42,  // Version 31.0
            V320 = 43,  // Version 32.0
            V330 = V320,// Version 33.0
        }

        public static Exception LastException { get; private set; }

        public static bool IsInitialised { get; private set; }

        public static AirVersion MajorVersion { get; private set; }

        public static string Version => _rootNode is null ? "0.0" : _rootNode.NamespaceURI.Replace(_BaseAirNamespace, string.Empty);

        public static bool IsUnsupportedVersion { get; private set; }

        public static string MaxSupportedVersion => _MaxSupportedVersion;

        public static bool InitializeProperties(string filePath)
        {
            bool result = false;
            try
            {
                _descriptorFile = new XmlDocument();
                _descriptorFile.Load(filePath);
                _rootNode = _descriptorFile.DocumentElement;
                IsUnsupportedVersion = false;
                string nsuri = _rootNode.NamespaceURI.ToLower();
                // Determine if valid descriptor file, and which version of AIR is specified
                if (!nsuri.StartsWithOrdinal(_BaseAirNamespace))
                {
                    throw new Exception(string.Format(TextHelper.GetString("Exception.Message.NotAirDescriptorFile"), filePath));
                }
                if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "1.0")) MajorVersion = AirVersion.V10;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "1.1")) MajorVersion = AirVersion.V11;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "1.5.3")) MajorVersion = AirVersion.V153;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "1.5")) MajorVersion = AirVersion.V15;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "2.0")) MajorVersion = AirVersion.V20;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "2.5")) MajorVersion = AirVersion.V25;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "2.6")) MajorVersion = AirVersion.V26;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "2.7")) MajorVersion = AirVersion.V27;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.0")) MajorVersion = AirVersion.V30;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.1")) MajorVersion = AirVersion.V31;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.2")) MajorVersion = AirVersion.V32;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.3")) MajorVersion = AirVersion.V33;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.4")) MajorVersion = AirVersion.V34;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.5")) MajorVersion = AirVersion.V35;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.6")) MajorVersion = AirVersion.V36;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.7")) MajorVersion = AirVersion.V37;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.8")) MajorVersion = AirVersion.V38;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "3.9")) MajorVersion = AirVersion.V39;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "4.0")) MajorVersion = AirVersion.V40;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "13.0")) MajorVersion = AirVersion.V130;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "14.0")) MajorVersion = AirVersion.V140;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "15.0")) MajorVersion = AirVersion.V150;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "16.0")) MajorVersion = AirVersion.V160;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "17.0")) MajorVersion = AirVersion.V170;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "18.0")) MajorVersion = AirVersion.V180;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "19.0")) MajorVersion = AirVersion.V190;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "20.0")) MajorVersion = AirVersion.V200;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "21.0")) MajorVersion = AirVersion.V210;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "22.0")) MajorVersion = AirVersion.V220;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "23.0")) MajorVersion = AirVersion.V230;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "24.0")) MajorVersion = AirVersion.V240;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "25.0")) MajorVersion = AirVersion.V250;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "26.0")) MajorVersion = AirVersion.V260;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "27.0")) MajorVersion = AirVersion.V270;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "28.0")) MajorVersion = AirVersion.V280;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "29.0")) MajorVersion = AirVersion.V290;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "30.0")) MajorVersion = AirVersion.V300;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "31.0")) MajorVersion = AirVersion.V310;
                //else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "32.0")) MajorVersion = AirVersion.V320;
                else if (nsuri.StartsWithOrdinal(_BaseAirNamespace + "32.0")) MajorVersion = AirVersion.V330;
                else
                {
                    // Is a valid AIR descriptor, but version not supported so default to max supported version
                    IsUnsupportedVersion = true;
                    MajorVersion = AirVersion.V330;
                }
                _namespaceManager = new XmlNamespaceManager(_descriptorFile.NameTable);
                _namespaceManager.AddNamespace("air", _rootNode.NamespaceURI);
                result = true;
            }
            catch (Exception ex)
            {
                LastException = ex;
            }
            IsInitialised = result;
            return result;
        }

        public static bool CommitProperties(string filePath)
        {
            var result = false;
            try
            {
                _descriptorFile.Save(filePath);
                result = true;
            }
            catch (Exception ex)
            {
                LastException = ex;
            }
            IsInitialised = result;
            return result;
        }

        public static void GetAttribute(string attribute, TextBox field)
        {
            field.Text = GetAttribute(attribute);
        }

        public static string GetAttribute(string attribute)
        {
            var propertyNode = attribute.Contains('/')
                ? _rootNode.SelectSingleNode("air:" + attribute.Replace("/", "/air:").Replace("/air:@", "/@"), _namespaceManager)
                : _rootNode.Attributes?.GetNamedItem(attribute);
            return propertyNode?.InnerText.Trim() ?? string.Empty;
        }

        public static void SetAttribute(string attribute, TextBox field) => SetAttribute(attribute, field.Text);

        public static void SetAttribute(string attribute, string value)
        {
            var attributeNode = attribute.Contains('/')
                ? _rootNode.SelectSingleNode("air:" + attribute.Replace("/", "/air:").Replace("/air:@", "/@"), _namespaceManager) as XmlAttribute
                : _rootNode.Attributes?.GetNamedItem(attribute) as XmlAttribute;
            if (attributeNode != null)
            {
                if (!string.IsNullOrEmpty(value)) attributeNode.InnerText = value;
                else
                {
                    // Remove the attribute, reverting to system default
                    var attributeParent = attribute.Contains('/')
                        ? _rootNode.SelectSingleNode("air:" + attribute.Substring(0, attribute.LastIndexOf('/')).Replace("/", "/air:"), _namespaceManager)
                        : _rootNode;
                    attributeParent.Attributes.Remove(attributeNode);
                }
            }
            else
            {
                // Only add attribute if there is a value to add
                if (!string.IsNullOrEmpty(value))
                {
                    var attributeName = attribute.Substring(attribute.IndexOf('@') + 1);
                    attributeNode = _descriptorFile.CreateAttribute(attributeName);
                    attributeNode.Value = value;
                    var propertyNode = attribute.Contains('/')
                        ? _rootNode.SelectSingleNode("air:" + attribute.Substring(0, attribute.LastIndexOf('/')).Replace("/", "/air:"), _namespaceManager)
                        : _rootNode;
                    propertyNode.Attributes.Append(attributeNode);
                }
            }
        }

        public static string GetProperty(string property)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            return propertyNode?.InnerText.Trim() ?? string.Empty;
        }

        static string GetProperty(string property, XmlNode rootNode)
        {
            var propertyNode = rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            return propertyNode?.InnerText.Trim() ?? string.Empty;
        }

        public static void GetProperty(string property, TextBox field) => GetProperty(property, field, string.Empty);

        public static void GetProperty(string property, TextBox field, string locale)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (locale != string.Empty && propertyNode.HasChildNodes)
                {
                    foreach (XmlNode childNode in propertyNode.ChildNodes)
                    {
                        if (childNode.Attributes != null)
                        {
                            var localeNode = childNode.Attributes.GetNamedItem("xml:lang");
                            if (localeNode != null)
                            {
                                if (localeNode.InnerText.Equals(locale))
                                {
                                    field.Text = childNode.InnerText.Trim();
                                    break;
                                }

                                field.Text = "";
                            }
                        }
                        else field.Text = "";
                    }
                }
                else if (propertyNode.ChildNodes.Count > 1)
                {
                    field.Text = propertyNode.FirstChild.InnerText.Trim();
                }
                else field.Text = propertyNode.InnerText.Trim();
            }
            else field.Text = "";
        }

        public static void GetProperty(string property, CheckBox field)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                field.Checked = propertyNode.InnerText.ToLower() == "true";
            }
            else field.CheckState = CheckState.Indeterminate;
        }

        public static void GetProperty(string property, ComboBox field, int defaultIndex)
        {
            bool foundListItem = false;
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                foreach (ListItem listItem in field.Items)
                {
                    if (listItem.Value == propertyNode.InnerText.Trim().ToLower())
                    {
                        field.SelectedItem = listItem;
                        foundListItem = true;
                    }
                }
                if (!foundListItem) field.SelectedIndex = defaultIndex;
            }
            else field.SelectedIndex = defaultIndex;
        }

        public static void SetProperty(string property, string value)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (value != "") propertyNode.InnerText = value;
                else
                {
                    // Remove the node, reverting to system default
                    GetParentNode(property).RemoveChild(propertyNode);
                }
            }
            else
            {
                // Only add property if there is a value to add
                if (value != "")
                {
                    var index = property.IndexOf('/');
                    var childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    propertyNode.InnerText = value;
                    GetParentNode(property).AppendChild(propertyNode);
                }
            }
        }

        public static void SetProperty(string property, TextBox field)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (field.Text != "") propertyNode.InnerText = field.Text;
                else
                {
                    // Remove the node, reverting to system default
                    GetParentNode(property).RemoveChild(propertyNode);
                }
            }
            else
            {
                // Only add property if there is a value to add
                if (field.Text != "")
                {
                    var index = property.IndexOf('/');
                    var childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    propertyNode.InnerText = field.Text;
                    GetParentNode(property).AppendChild(propertyNode);
                }
            }
        }

        public static void SetPropertyCData(string property, Control field)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (field.Text != "")
                {
                    var x = _descriptorFile.CreateCDataSection(field.Text);
                    propertyNode.RemoveAll();
                    propertyNode.AppendChild(x);
                }
                else
                {
                    // Remove the node, reverting to system default
                    GetParentNode(property).RemoveChild(propertyNode);
                }
            }
            else
            {
                // Only add property if there is a value to add
                if (field.Text != "")
                {
                    var index = property.IndexOf('/');
                    var childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    var x = _descriptorFile.CreateCDataSection(field.Text);
                    propertyNode.AppendChild(x);
                    GetParentNode(property).AppendChild(propertyNode);
                }
            }
        }

        // Searches the specified property node for an element with the specified locale, creating one if none is found. 
        // Supplied element parameter is referenced to resulting element.
        // Returns true if the element already existed, false if it was created.
        static bool GetLocalizedElement(XmlNode propertyNode, string locale, ref XmlElement element)
        {
            if (locale == string.Empty) return false;
            var result = false;
            XmlElement localeElement = null;
            if (propertyNode.HasChildNodes)
            {
                foreach (XmlNode childNode in propertyNode.ChildNodes)
                {
                    var localeAttribute = childNode.Attributes?.GetNamedItem("xml:lang");
                    if (localeAttribute != null && localeAttribute.InnerText.Equals(locale))
                    {
                        localeElement = (XmlElement)childNode;
                        result = true;
                        break;
                    }
                }
            }
            if (!result)
            {
                localeElement = _descriptorFile.CreateElement("text", _namespaceManager.LookupNamespace("air"));
                localeElement.SetAttribute("xml:lang", locale);
            }
            element = localeElement;
            return result;
        }

        public static void SetProperty(string property, TextBox field, string locale)
        {
            SetProperty(property, field, locale, false);
        }

        public static void SetProperty(string property, TextBox field, string locale, bool isDefaultLocale)
        {
            XmlElement localeElement = null;
            // If no locale then just add/edit the node
            if (locale == string.Empty)
            {
                SetProperty(property, field);
                return;
            }
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                var elementExists = GetLocalizedElement(propertyNode, locale, ref localeElement);
                localeElement.InnerText = field.Text;
                if (!elementExists)
                {
                    // Remove any existing non-localised values
                    if (propertyNode.HasChildNodes && propertyNode.FirstChild.NodeType == XmlNodeType.Text)
                    {
                        propertyNode.InnerText = "";
                    }
                    if (isDefaultLocale)
                    {
                        // If default locale, then element has to be first child
                        propertyNode.PrependChild(localeElement);
                    }
                    else propertyNode.AppendChild(localeElement);
                }
                else
                {
                    // If default locale and element isn't the first child then move it
                    if (isDefaultLocale && localeElement != propertyNode.FirstChild)
                    {
                        propertyNode.RemoveChild(localeElement);
                        propertyNode.PrependChild(localeElement);
                    }
                }
            }
            else
            {
                // Only add property if there is a value to add
                if (field.Text != "")
                {
                    var index = property.IndexOf('/');
                    var childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    GetLocalizedElement(propertyNode, locale, ref localeElement);
                    localeElement.InnerText = field.Text;
                    propertyNode.AppendChild(localeElement);
                    GetParentNode(property).AppendChild(propertyNode);
                }
            }
        }
        
        public static void SetProperty(string property, CheckBox field)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (field.CheckState != CheckState.Indeterminate)
                {
                    propertyNode.InnerText = field.Checked.ToString().ToLower();
                }
                else
                {
                    // Remove the node, reverting to system default
                    GetParentNode(property).RemoveChild(propertyNode);
                }
            }
            else
            {
                // Only add property if there is a value to add
                if (field.CheckState != CheckState.Indeterminate)
                {
                    var index = property.IndexOf('/');
                    var childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    propertyNode.InnerText = field.Checked.ToString().ToLower();
                    GetParentNode(property).AppendChild(propertyNode);
                }
            }
        }

        public static void SetProperty(string property, ComboBox field)
        {
            var selectedItem = (ListItem)field.SelectedItem;
            if (selectedItem is null) return;
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (selectedItem.Value != string.Empty) propertyNode.InnerText = selectedItem.Value;
                else
                {
                    // Remove the node, reverting to system default
                    GetParentNode(property).RemoveChild(propertyNode);
                }
            }
            else
            {
                // Only add property if there is a value to add
                if (selectedItem.Value != string.Empty)
                {
                    var index = property.IndexOf('/');
                    var childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    propertyNode.InnerText = selectedItem.Value;
                    GetParentNode(property).AppendChild(propertyNode);
                }
            }
        }

        public static void RemoveLocalizedProperty(string property, string locale)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode is null) return;
            XmlElement localeElement = null;
            var elementExists = GetLocalizedElement(propertyNode, locale, ref localeElement);
            if (elementExists) propertyNode.RemoveChild(localeElement);
        }

        public static void CreateLocalizedProperty(string property, string locale, bool isDefaultLocale)
        {
            XmlElement localeElement = null;
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                var elementExists = GetLocalizedElement(propertyNode, locale, ref localeElement);
                localeElement.InnerText = string.Empty;
                if (!elementExists)
                {
                    // If there is any existing non-localised values, copy them to the localised element (will only occur for first localised element)
                    if (propertyNode.HasChildNodes && propertyNode.FirstChild.NodeType == XmlNodeType.Text)
                    {
                        localeElement.InnerText = propertyNode.InnerText;
                        propertyNode.InnerText = "";
                    }
                    if (isDefaultLocale)
                    {
                        // If default locale, then element has to be first child
                        propertyNode.PrependChild(localeElement);
                    }
                    else propertyNode.AppendChild(localeElement);
                }
                else
                {
                    // Should never get here as we are supposed to be creating new element, but cater for it anyways
                    // If default locale and element isn't the first child then move it
                    if (isDefaultLocale && localeElement != propertyNode.FirstChild)
                    {
                        propertyNode.RemoveChild(localeElement);
                        propertyNode.PrependChild(localeElement);
                    }
                }
            }
            else
            {
                var index = property.IndexOf('/');
                var childName = property.Substring(index + 1, property.Length - (index + 1));
                propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                GetLocalizedElement(propertyNode, locale, ref localeElement);
                localeElement.InnerText = string.Empty;
                propertyNode.AppendChild(localeElement);
                GetParentNode(property).AppendChild(propertyNode);
            }
        }

        /** 
        * Simple method returns the parent node based on the supplied property name (XPATH)
        * If parent node does not exist it is created and added to the root element
        * This function will only work for a single parent/child, but that is all
        * that is required at this point as only element in AIR properties file that has more
        * than two levels is fileTypes which is handled by the Get/SetFileTypes methods.
        */
        static XmlNode GetParentNode(string property)
        {
            var index = property.IndexOf('/');
            if (index == -1) return _rootNode;
            var parentName = property.Substring(0, index);
            var parentNode = _rootNode.SelectSingleNode("air:" + parentName, _namespaceManager);
            if (parentNode is null)
            {
                parentNode = _descriptorFile.CreateNode(XmlNodeType.Element, parentName, _namespaceManager.LookupNamespace("air"));
                _rootNode.AppendChild(parentNode);
            }
            return parentNode;
        }

        public static void GetPropertyLocales(string property, List<string> locales)
        {
            var propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null && propertyNode.HasChildNodes)
            {
                foreach (XmlNode childNode in propertyNode.ChildNodes)
                {
                    var localeNode = childNode.Attributes?.GetNamedItem("xml:lang");
                    if (localeNode != null && !locales.Contains(localeNode.InnerText))
                    {
                        locales.Add(localeNode.InnerText);
                    }
                }
            }
        }

        //OK, this method puts the responsibility of searching
        //for property names within the Property Manager. The key reason for
        //this is that is a lot easier from the UI's perspective to work with a 
        //collection of 'fileType' business objects rather than straight XML
        public static void GetFileTypes(List<AirFileType> fileTypes)
        {
            fileTypes.Clear();
            var propertyNode = _rootNode.SelectSingleNode("air:fileTypes", _namespaceManager);
            if (propertyNode != null && propertyNode.HasChildNodes)
            {
                // Loop through collection of xml fileType nodes
                foreach (XmlNode childNode in propertyNode.ChildNodes)
                {
                    // Create a new fileType business object
                    var fileType = new AirFileType();
                    fileType.Name = GetProperty("name", childNode);
                    fileType.Extension = GetProperty("extension", childNode);
                    fileType.Description = GetProperty("description", childNode);
                    fileType.ContentType = GetProperty("contentType", childNode);
                    foreach (var icon in fileType.Icons)
                    {
                        if (icon.MinVersion <= MajorVersion)
                        {
                            icon.FilePath = GetProperty("icon/image" + icon.Size + "x" + icon.Size, childNode);
                        }
                    }
                    // Add to the business object collection
                    fileTypes.Add(fileType);
                }
            }
        }

        // Writes the collection of AirFileType objects to the fileTypes element in
        // the descriptor file. Again this takes the responsibility of property names
        // away from the UI, but that's the way it is.
        public static void SetFileTypes(List<AirFileType> fileTypes)
        {
            // Get fileTypes node
            var fileTypesNode = _rootNode.SelectSingleNode("air:fileTypes", _namespaceManager);
            if (fileTypesNode is null) fileTypesNode = _descriptorFile.CreateNode(XmlNodeType.Element, "fileTypes", _namespaceManager.LookupNamespace("air"));
            else _rootNode.RemoveChild(fileTypesNode);
            if (fileTypes.Count == 0) return;
            // Clear contents
            fileTypesNode.InnerText = "";
            // Loop through collection of AirFileType 
            foreach (var fileType in fileTypes)
            {
                // Create a new fileType node
                var fileTypeNode = _descriptorFile.CreateNode(XmlNodeType.Element, "fileType", _namespaceManager.LookupNamespace("air"));
                CreateChildNode(fileTypeNode, "name", fileType.Name);
                CreateChildNode(fileTypeNode, "extension", fileType.Extension);
                CreateChildNode(fileTypeNode, "description", fileType.Description);
                CreateChildNode(fileTypeNode, "contentType", fileType.ContentType);
                foreach (var icon in fileType.Icons)
                {
                    if (icon.MinVersion <= MajorVersion)
                    {
                        CreateChildNode(fileTypeNode, "icon/image" + icon.Size + "x" + icon.Size, icon.FilePath);
                    }
                }
                fileTypesNode.AppendChild(fileTypeNode);
            }
            //Add the fileTypes node
            _rootNode.AppendChild(fileTypesNode);
        }

        // OK, this method puts the responsibility of searching
        // for property names within the Property Manager. The key reason for
        // this is that is a lot easier from the UI's perspective to work with a 
        // collection of 'extension' business objects rather than straight XML
        public static void GetExtensions(List<AirExtension> extensions)
        {
            extensions.Clear();
            var propertyNode = _rootNode.SelectSingleNode("air:extensions", _namespaceManager);
            if (propertyNode is null || !propertyNode.HasChildNodes) return;
            // Loop through collection of xml fileType nodes
            foreach (XmlNode childNode in propertyNode.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element)
                    continue;

                // Create a new extension business object
                var extension = new AirExtension {ExtensionId = childNode.InnerText.Trim()};
                // Add to the business object collection
                extensions.Add(extension);
            }
        }

        // Writes the collection of AirExtension objects to the extensions element in
        // the descriptor file. Again this takes the responsibility of property names
        // away from the UI, but that's the way it is.
        public static void SetExtensions(List<AirExtension> extensions)
        {
            // Get fileTypes node
            var extensionsNode = _rootNode.SelectSingleNode("air:extensions", _namespaceManager);
            if (extensionsNode is null)
            {
                extensionsNode = _descriptorFile.CreateNode(XmlNodeType.Element, "extensions", _namespaceManager.LookupNamespace("air"));
            }
            else _rootNode.RemoveChild(extensionsNode);
            if (extensions.Count == 0) return;
            // Clear contents
            extensionsNode.InnerText = "";
            // Loop through collection of AirExtension 
            foreach (var extension in extensions)
            {
                // Create extensionID node
                CreateChildNode(extensionsNode, "extensionID", extension.ExtensionId);
            }
            // Add the extensions node
            _rootNode.AppendChild(extensionsNode);
        }

        // Creates a child node on the specified parent node, will also create sub-parent node (used for creating fileType properties)
        static void CreateChildNode(XmlNode parentNode, string childNodeName, string childNodeValue)
        {
            XmlNode subParentNode;
            if (childNodeValue == string.Empty) return;
            // Determine if there is a sub parent
            var index = childNodeName.IndexOf('/');
            if (index == -1) subParentNode = parentNode;
            else
            {
                //create a sub parent element if required
                var subParentName = childNodeName.Substring(0, index);
                subParentNode = parentNode.SelectSingleNode("air:" + subParentName, _namespaceManager);
                if (subParentNode is null)
                {
                    subParentNode = _descriptorFile.CreateNode(XmlNodeType.Element, subParentName, _namespaceManager.LookupNamespace("air"));
                    parentNode.AppendChild(subParentNode);
                }
            }
            var childName = childNodeName.Substring(index + 1, childNodeName.Length - (index + 1));
            var childNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
            childNode.InnerText = childNodeValue;
            subParentNode.AppendChild(childNode);
        }

        // Simple class to construct an Air Application File Type. 
        // The class requires external validation of property values.
        public class AirFileType
        {
            public AirFileType()
            {
                Name = string.Empty;
                Extension = string.Empty;
                Description = string.Empty;
                ContentType = string.Empty;
                // According to Descriptor.xsd, 57x57 is not used for file icons
                Icons = new List<AirFileTypeIcon> 
                { 
                    new AirFileTypeIcon(16, string.Empty, AirVersion.V10),
                    new AirFileTypeIcon(29, string.Empty, AirVersion.V20),
                    new AirFileTypeIcon(32, string.Empty, AirVersion.V10),
                    new AirFileTypeIcon(36, string.Empty, AirVersion.V25),
                    new AirFileTypeIcon(48, string.Empty, AirVersion.V10),
                    new AirFileTypeIcon(72, string.Empty, AirVersion.V20),
                    new AirFileTypeIcon(128, string.Empty, AirVersion.V10),
                    new AirFileTypeIcon(512, string.Empty, AirVersion.V20)
                };
            }

            public string Name { get; set; }

            public string Extension { get; set; }

            public string Description { get; set; }

            public string ContentType { get; set; }

            public List<AirFileTypeIcon> Icons { get; }

            public void SetIconPath(short size, string path)
            {
                foreach (var icon in Icons)
                {
                    if (icon.Size == size)
                    {
                        icon.FilePath = path;
                        break;
                    }
                }
            }

            public void SetIconIsValid(short size, bool isValid)
            {
                foreach (var icon in Icons)
                {
                    if (icon.Size == size)
                    {
                        icon.IsValid = isValid;
                        break;
                    }
                }
            }

            public string GetIconPath(short size)
            {
                foreach (var icon in Icons)
                {
                    if (icon.Size == size) return icon.FilePath;
                }
                return string.Empty;
            }

            public bool NameIsValid { get; set; }

            public bool ExtensionIsValid { get; set; }

            public bool DescriptionIsValid { get; set; }

            public bool ContentTypeIsValid { get; set; }

            public bool IsValid
            {
                get
                {
                    foreach (var icon in Icons)
                    {
                        if (!icon.IsValid) return false;
                    }
                    return NameIsValid && ExtensionIsValid && DescriptionIsValid && ContentTypeIsValid;
                }
            }

            public class AirFileTypeIcon
            {
                public AirFileTypeIcon(short size, string filePath, AirVersion minVersion)
                {
                    Size = size;
                    FilePath = filePath;
                    MinVersion = minVersion;
                    IsValid = true;
                }

                public short Size { get; }

                public string FilePath { get; set; }

                public AirVersion MinVersion { get; }

                public bool IsValid { get; set; }
            }
        }

        public class AirApplicationIconField
        {
            public AirApplicationIconField(string size, AirVersion minVersion)
            {
                Size = size;
                MinVersion = minVersion;
            }

            public string Size { get; }

            public TextBox Field { get; set; }

            public AirVersion MinVersion { get; }
        }

        public class AirExtension
        {
            public string ExtensionId { get; set; } = string.Empty;

            public bool IsValid { get; set; }

            public string Path { get; set; }
        }

        public class AndroidPermission
        {
            public string Constant { get; set; }

            public string Description { get; set; }

            public AndroidPermission(string constant, string description)
            {
                Constant = constant;
                Description = description;
            }
        }
    }
}