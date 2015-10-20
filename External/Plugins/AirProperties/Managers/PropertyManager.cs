// TODO: Some of these methods should be merged with WizardHelper

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using PluginCore.Localization;

namespace AirProperties
{
    // Air Application Properties Manager
    class PropertyManager
    {
        private static XmlDocument _descriptorFile;
        private static XmlNamespaceManager _namespaceManager;
        private static XmlNode _rootNode;
        private static Boolean _isInitialised;
        private static Exception _lastException;
        private static AirVersion _version;
        private static Boolean _unsupportedVersion;
        private const String _BaseAirNamespace = "http://ns.adobe.com/air/application/";
        private const String _MaxSupportedVersion = "20.0";

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
            V35 = 14,    // Version 3.5
            V36 = 15,    // Version 3.6
            V37 = 16,    // Version 3.7
            V38 = 17,    // Version 3.8
            V39 = 18,    // Version 3.9
            V40 = 19,    // Version 4.0
            V130 = 20,    // Version 13.0
            V140 = 21,    // Version 14.0
            V150 = 22,    // Version 15.0
            V160 = 23,    // Version 16.0
            V170 = 24,    // Version 17.0
            V180 = 25,    // Version 18.0
            V190 = 26,    // Version 19.0
            V200 = 27    // Version 20.0
        }

        public static Exception LastException
        {
            get { return _lastException; }
        }

        public static Boolean IsInitialised
        {
            get { return _isInitialised; }
        }

        public static AirVersion MajorVersion
        {
            get { return _version; }
        }

        public static String Version
        {
            get
            {
                if (_rootNode == null) return "0.0";
                else return _rootNode.NamespaceURI.Replace(_BaseAirNamespace, String.Empty);
            }
        }

        public static Boolean IsUnsupportedVersion
        {
            get { return _unsupportedVersion; }
        }

        public static String MaxSupportedVersion
        {
            get { return _MaxSupportedVersion; }
        }

        public static Boolean InitializeProperties(string filePath)
        {
            Boolean result = false;
            try
            {
                _descriptorFile = new XmlDocument();
                _descriptorFile.Load(filePath);
                _rootNode = _descriptorFile.DocumentElement;
                _unsupportedVersion = false;
                string nsuri = _rootNode.NamespaceURI.ToLower();
                // Determine if valid descriptor file, and which version of AIR is specified
                if (!nsuri.StartsWith(_BaseAirNamespace))
                {
                    throw new Exception(String.Format(TextHelper.GetString("Exception.Message.NotAirDescriptorFile"), filePath));
                }
                else
                {
                    if (nsuri.StartsWith(_BaseAirNamespace + "1.0")) _version = AirVersion.V10;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "1.1")) _version = AirVersion.V11;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "1.5.3")) _version = AirVersion.V153;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "1.5")) _version = AirVersion.V15;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "2.0")) _version = AirVersion.V20;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "2.5")) _version = AirVersion.V25;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "2.6")) _version = AirVersion.V26;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "2.7")) _version = AirVersion.V27;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.0")) _version = AirVersion.V30;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.1")) _version = AirVersion.V31;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.2")) _version = AirVersion.V32;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.3")) _version = AirVersion.V33;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.4")) _version = AirVersion.V34;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.5")) _version = AirVersion.V35;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.6")) _version = AirVersion.V36;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.7")) _version = AirVersion.V37;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.8")) _version = AirVersion.V38;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "3.9")) _version = AirVersion.V39;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "4.0")) _version = AirVersion.V40;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "13.0")) _version = AirVersion.V130;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "14.0")) _version = AirVersion.V140;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "15.0")) _version = AirVersion.V150;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "16.0")) _version = AirVersion.V160;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "17.0")) _version = AirVersion.V170;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "18.0")) _version = AirVersion.V180;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "19.0")) _version = AirVersion.V190;
                    else if (nsuri.StartsWith(_BaseAirNamespace + "20.0")) _version = AirVersion.V200;
                    else
                    {
                        // Is a valid AIR descriptor, but version not supported so default to max supported version
                        _unsupportedVersion = true;
                        _version = AirVersion.V190;
                    }
                }
                _namespaceManager = new XmlNamespaceManager(_descriptorFile.NameTable);
                _namespaceManager.AddNamespace("air", _rootNode.NamespaceURI);
                result = true;
            }
            catch (Exception ex)
            {
                _lastException = ex;
            }
            _isInitialised = result;
            return result;
        }

        public static Boolean CommitProperties(string filePath)
        {
            Boolean result = false;
            try
            {
                _descriptorFile.Save(filePath);
                result = true;
            }
            catch (Exception ex)
            {
                _lastException = ex;
            }
            _isInitialised = result;
            return result;
        }

        public static void GetAttribute(string attribute, TextBox field)
        {
            field.Text = GetAttribute(attribute);
        }

        public static String GetAttribute(string attribute)
        {
            XmlNode propertyNode;
            if (attribute.IndexOf('/') > -1)
                propertyNode = _rootNode.SelectSingleNode("air:" + attribute.Replace("/", "/air:").Replace("/air:@", "/@"), _namespaceManager);
            else
                propertyNode = _rootNode.Attributes.GetNamedItem(attribute);
            if (propertyNode != null) return propertyNode.InnerText.Trim();
            return string.Empty;
        }

        public static void SetAttribute(string attribute, TextBox field)
        {
            SetAttribute(attribute, field.Text);
        }

        public static void SetAttribute(string attribute, string value)
        {
            XmlAttribute attributeNode;
            if (attribute.IndexOf('/') > -1)
                attributeNode = _rootNode.SelectSingleNode("air:" + attribute.Replace("/", "/air:").Replace("/air:@", "/@"), _namespaceManager) as XmlAttribute;
            else
                attributeNode = _rootNode.Attributes.GetNamedItem(attribute) as XmlAttribute;
            if (attributeNode != null)
            {
                if (!string.IsNullOrEmpty(value)) attributeNode.InnerText = value;
                else
                {
                    // Remove the attribute, reverting to system default
                    XmlNode attributeParent;
                    if (attribute.IndexOf('/') > -1)
                        attributeParent = _rootNode.SelectSingleNode("air:" + attribute.Substring(0, attribute.LastIndexOf('/')).Replace("/", "/air:"), _namespaceManager);
                    else
                        attributeParent = _rootNode;
                    attributeParent.Attributes.Remove(attributeNode);
                }
            }
            else
            {
                // Only add attribute if there is a value to add
                if (!string.IsNullOrEmpty(value))
                {
                    XmlNode propertyNode;
                    string attributeName = attribute.Substring(attribute.IndexOf("@") + 1);
                    attributeNode = _descriptorFile.CreateAttribute(attributeName);
                    attributeNode.Value = value;
                    if (attribute.IndexOf('/') > -1)
                        propertyNode = _rootNode.SelectSingleNode("air:" + attribute.Substring(0, attribute.LastIndexOf('/')).Replace("/", "/air:"), _namespaceManager);
                    else
                        propertyNode = _rootNode;
                    propertyNode.Attributes.Append(attributeNode);
                }
            }
        }

        public static String GetProperty(string property)
        {
            XmlNode propertyNode;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null) return propertyNode.InnerText.Trim();
            else return "";
        }

        private static String GetProperty(string property, XmlNode rootNode)
        {
            XmlNode propertyNode;
            propertyNode = rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null) return propertyNode.InnerText.Trim();
            else return "";
        }

        public static void GetProperty(string property, TextBox field)
        {
            GetProperty(property, field, String.Empty);
        }

        public static void GetProperty(string property, TextBox field, string locale)
        {
            XmlNode propertyNode;
            XmlNode localeNode;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (locale != String.Empty && propertyNode.HasChildNodes)
                {
                    foreach (XmlNode childNode in propertyNode.ChildNodes)
                    {
                        if (childNode.Attributes != null)
                        {
                            localeNode = childNode.Attributes.GetNamedItem("xml:lang");
                            if (localeNode != null)
                            {
                                if (localeNode.InnerText.Equals(locale))
                                {
                                    field.Text = childNode.InnerText.Trim();
                                    break;
                                }
                                else field.Text = "";
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
            XmlNode propertyNode;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (propertyNode.InnerText.ToLower() == "true") field.Checked = true;
                else field.Checked = false;
            }
            else field.CheckState = CheckState.Indeterminate;
        }

        public static void GetProperty(string property, ComboBox field, int defaultIndex)
        {
            XmlNode propertyNode;
            Boolean foundListItem = false;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
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
            XmlNode propertyNode;
            String childName;
            int index;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
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
                    index = property.IndexOf('/');
                    childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    if (propertyNode != null)
                    {
                        propertyNode.InnerText = value;
                        GetParentNode(property).AppendChild(propertyNode);
                    }
                }
            }
        }

        public static void SetProperty(string property, TextBox field)
        {
            XmlNode propertyNode;
            String childName;
            int index;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
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
                    index = property.IndexOf('/');
                    childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    if (propertyNode != null)
                    {
                        propertyNode.InnerText = field.Text;
                        GetParentNode(property).AppendChild(propertyNode);
                    }
                }
            }
        }

        public static void SetPropertyCData(string property, Control field)
        {
            XmlNode propertyNode;
            String childName;
            int index;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                if (field.Text != "")
                {
                    XmlCDataSection x = _descriptorFile.CreateCDataSection(field.Text);
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
                    index = property.IndexOf('/');
                    childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    if (propertyNode != null)
                    {
                        XmlCDataSection x = _descriptorFile.CreateCDataSection(field.Text);
                        propertyNode.AppendChild(x);
                        GetParentNode(property).AppendChild(propertyNode);
                    }
                }
            }
        }

        // Searches the specified property node for an element with the specified locale, creating one if none is found. 
        // Supplied element parameter is referenced to resulting element.
        // Returns true if the element already existed, false if it was created.
        private static Boolean GetLocalizedElement(XmlNode propertyNode, String locale, ref XmlElement element)
        {
            Boolean found = false;
            XmlNode localeAttribute;
            XmlElement localeElement = null;
            if (locale != String.Empty)
            {
                if (propertyNode.HasChildNodes)
                {
                    foreach (XmlNode childNode in propertyNode.ChildNodes)
                    {
                        if (childNode.Attributes != null)
                        {
                            localeAttribute = childNode.Attributes.GetNamedItem("xml:lang");
                            if (localeAttribute != null)
                            {
                                if (localeAttribute.InnerText.Equals(locale))
                                {
                                    localeElement = (XmlElement)childNode;
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!found)
                {
                    localeElement = _descriptorFile.CreateElement("text", _namespaceManager.LookupNamespace("air"));
                    localeElement.SetAttribute("xml:lang", locale);
                }
                element = localeElement;
            }
            return found;
        }

        public static void SetProperty(string property, TextBox field, String locale)
        {
            SetProperty(property, field, locale, false);
        }

        public static void SetProperty(string property, TextBox field, String locale, Boolean isDefaultLocale)
        {
            XmlNode propertyNode;
            XmlElement localeElement = null;
            Boolean elementExists;
            String childName;
            int index;
            // If no locale then just add/edit the node
            if (locale == String.Empty)
            {
                SetProperty(property, field);
                return;
            }
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                elementExists = GetLocalizedElement(propertyNode, locale, ref localeElement);
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
                    index = property.IndexOf('/');
                    childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    if (propertyNode != null)
                    {
                        GetLocalizedElement(propertyNode, locale, ref localeElement);
                        localeElement.InnerText = field.Text;
                        propertyNode.AppendChild(localeElement);
                        GetParentNode(property).AppendChild(propertyNode);
                    }
                }
            }
        }
        
        public static void SetProperty(string property, CheckBox field)
        {
            XmlNode propertyNode;
            String childName;
            int index;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
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
                    index = property.IndexOf('/');
                    childName = property.Substring(index + 1, property.Length - (index + 1));
                    propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                    if (propertyNode != null)
                    {
                        propertyNode.InnerText = field.Checked.ToString().ToLower();
                        GetParentNode(property).AppendChild(propertyNode);
                    }
                }
            }
        }

        public static void SetProperty(string property, ComboBox field)
        {
            XmlNode propertyNode;
            ListItem selectedItem;
            String childName;
            int index;
            selectedItem = (ListItem)field.SelectedItem;
            if (selectedItem != null)
            {
                propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
                if (propertyNode != null)
                {
                    if (selectedItem.Value != String.Empty) propertyNode.InnerText = selectedItem.Value;
                    else
                    {
                        // Remove the node, reverting to system default
                        GetParentNode(property).RemoveChild(propertyNode);
                    }
                }
                else
                {
                    // Only add property if there is a value to add
                    if (selectedItem.Value != String.Empty)
                    {
                        index = property.IndexOf('/');
                        childName = property.Substring(index + 1, property.Length - (index + 1));
                        propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                        if (propertyNode != null)
                        {
                            propertyNode.InnerText = selectedItem.Value;
                            GetParentNode(property).AppendChild(propertyNode);
                        }
                    }
                }
            }
        }

        public static void RemoveLocalizedProperty(string property, String locale)
        {
            XmlNode propertyNode;
            XmlElement localeElement = null;
            Boolean elementExists;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                elementExists = GetLocalizedElement(propertyNode, locale, ref localeElement);
                if (elementExists) propertyNode.RemoveChild(localeElement);
            }
        }

        public static void CreateLocalizedProperty(string property, String locale, Boolean isDefaultLocale)
        {
            XmlNode propertyNode;
            XmlElement localeElement = null;
            Boolean elementExists;
            String childName;
            int index;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null)
            {
                elementExists = GetLocalizedElement(propertyNode, locale, ref localeElement);
                localeElement.InnerText = String.Empty;
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
                index = property.IndexOf('/');
                childName = property.Substring(index + 1, property.Length - (index + 1));
                propertyNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
                if (propertyNode != null)
                {
                    GetLocalizedElement(propertyNode, locale, ref localeElement);
                    localeElement.InnerText = String.Empty;
                    propertyNode.AppendChild(localeElement);
                    GetParentNode(property).AppendChild(propertyNode);
                }
            }
        }

        /** 
        * Simple method returns the parent node based on the supplied property name (XPATH)
        * If parent node does not exist it is created and added to the root element
        * This function will only work for a single parent/child, but that is all
        * that is required at this point as only element in AIR properties file that has more
        * than two levels is fileTypes which is handled by the Get/SetFileTypes methods.
        */
        private static XmlNode GetParentNode(string property)
        {
            XmlNode parentNode;
            String parentName;
            int index;
            index = property.IndexOf('/');
            if (index == -1) return _rootNode;
            else
            {
                parentName = property.Substring(0, index);
                parentNode = _rootNode.SelectSingleNode("air:" + parentName, _namespaceManager);
                if (parentNode == null)
                {
                    parentNode = _descriptorFile.CreateNode(XmlNodeType.Element, parentName, _namespaceManager.LookupNamespace("air"));
                    _rootNode.AppendChild(parentNode);
                }
                return parentNode;
            }
        }

        public static void GetPropertyLocales(string property, List<string> locales)
        {
            XmlNode propertyNode;
            XmlNode localeNode;
            propertyNode = _rootNode.SelectSingleNode("air:" + property.Replace("/", "/air:"), _namespaceManager);
            if (propertyNode != null && propertyNode.HasChildNodes)
            {
                foreach (XmlNode childNode in propertyNode.ChildNodes)
                {
                    if (childNode.Attributes != null)
                    {
                        localeNode = childNode.Attributes.GetNamedItem("xml:lang");
                        if (localeNode != null)
                        {
                            if (!locales.Contains(localeNode.InnerText))
                            {
                                locales.Add(localeNode.InnerText);
                            }
                        }
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
            XmlNode propertyNode;
            AirFileType fileType;
            fileTypes.Clear();
            propertyNode = _rootNode.SelectSingleNode("air:fileTypes", _namespaceManager);
            if (propertyNode != null && propertyNode.HasChildNodes)
            {
                // Loop through collection of xml fileType nodes
                foreach (XmlNode childNode in propertyNode.ChildNodes)
                {
                    // Create a new fileType business object
                    fileType = new AirFileType();
                    fileType.Name = GetProperty("name", childNode);
                    fileType.Extension = GetProperty("extension", childNode);
                    fileType.Description = GetProperty("description", childNode);
                    fileType.ContentType = GetProperty("contentType", childNode);
                    foreach (AirFileType.AirFileTypeIcon icon in fileType.Icons)
                    {
                        if (icon.MinVersion <= _version)
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
            XmlNode fileTypesNode;
            XmlNode fileTypeNode;
            // Get fileTypes node
            fileTypesNode = _rootNode.SelectSingleNode("air:fileTypes", _namespaceManager);
            if (fileTypesNode == null)
            {
                fileTypesNode = _descriptorFile.CreateNode(XmlNodeType.Element, "fileTypes", _namespaceManager.LookupNamespace("air"));
            }
            else _rootNode.RemoveChild(fileTypesNode);
            if (fileTypes.Count <= 0) return;
            if (fileTypesNode != null)
            {
                // Clear contents
                fileTypesNode.InnerText = "";
                // Loop through collection of AirFileType 
                foreach (AirFileType fileType in fileTypes)
                {
                    // Create a new fileType node
                    fileTypeNode = _descriptorFile.CreateNode(XmlNodeType.Element, "fileType", _namespaceManager.LookupNamespace("air"));
                    if (fileTypeNode != null)
                    {
                        CreateChildNode(fileTypeNode, "name", fileType.Name);
                        CreateChildNode(fileTypeNode, "extension", fileType.Extension);
                        CreateChildNode(fileTypeNode, "description", fileType.Description);
                        CreateChildNode(fileTypeNode, "contentType", fileType.ContentType);
                        foreach (AirFileType.AirFileTypeIcon icon in fileType.Icons)
                        {
                            if (icon.MinVersion <= _version)
                            {
                                CreateChildNode(fileTypeNode, "icon/image" + icon.Size + "x" + icon.Size, icon.FilePath);
                            }
                        }
                    }
                    fileTypesNode.AppendChild(fileTypeNode);
                }
                //Add the fileTypes node
                _rootNode.AppendChild(fileTypesNode);
            }
        }

        // OK, this method puts the responsibility of searching
        // for property names within the Property Manager. The key reason for
        // this is that is a lot easier from the UI's perspective to work with a 
        // collection of 'extension' business objects rather than straight XML
        public static void GetExtensions(List<AirExtension> extensions)
        {
            XmlNode propertyNode;
            AirExtension extension;
            extensions.Clear();
            propertyNode = _rootNode.SelectSingleNode("air:extensions", _namespaceManager);
            if (propertyNode != null && propertyNode.HasChildNodes)
            {
                // Loop through collection of xml fileType nodes
                foreach (XmlNode childNode in propertyNode.ChildNodes)
                {
                    if (childNode.NodeType != XmlNodeType.Element)
                        continue;

                    // Create a new extension business object
                    extension = new AirExtension();
                    extension.ExtensionId = childNode.InnerText.Trim();
                    // Add to the business object collection
                    extensions.Add(extension);
                }
            }
        }

        // Writes the collection of AirExtension objects to the extensions element in
        // the descriptor file. Again this takes the responsibility of property names
        // away from the UI, but that's the way it is.
        public static void SetExtensions(List<AirExtension> extensions)
        {
            XmlNode extensionsNode;
            // Get fileTypes node
            extensionsNode = _rootNode.SelectSingleNode("air:extensions", _namespaceManager);
            if (extensionsNode == null)
            {
                extensionsNode = _descriptorFile.CreateNode(XmlNodeType.Element, "extensions", _namespaceManager.LookupNamespace("air"));
            }
            else _rootNode.RemoveChild(extensionsNode);
            if (extensions.Count <= 0) return;
            if (extensionsNode != null)
            {
                // Clear contents
                extensionsNode.InnerText = "";
                // Loop through collection of AirExtension 
                foreach (AirExtension extension in extensions)
                {
                    // Create extensionID node
                    CreateChildNode(extensionsNode, "extensionID", extension.ExtensionId);
                }
                // Add the extensions node
                _rootNode.AppendChild(extensionsNode);
            }
        }

        // Creates a child node on the specified parent node, will also create sub-parent node (used for creating fileType properties)
        private static void CreateChildNode(XmlNode parentNode, String childNodeName, String childNodeValue)
        {
            XmlNode subParentNode;
            XmlNode childNode;
            String subParentName;
            String childName;
            int index;
            if (childNodeValue == String.Empty) return;
            // Determine if there is a sub parent
            index = childNodeName.IndexOf('/');
            if (index == -1) subParentNode = parentNode;
            else
            {
                //create a sub parent element if required
                subParentName = childNodeName.Substring(0, index);
                subParentNode = parentNode.SelectSingleNode("air:" + subParentName, _namespaceManager);
                if (subParentNode == null)
                {
                    subParentNode = _descriptorFile.CreateNode(XmlNodeType.Element, subParentName, _namespaceManager.LookupNamespace("air"));
                    if (subParentNode != null)
                    {
                        parentNode.AppendChild(subParentNode);
                    }
                }
            }
            childName = childNodeName.Substring(index + 1, childNodeName.Length - (index + 1));
            childNode = _descriptorFile.CreateNode(XmlNodeType.Element, childName, _namespaceManager.LookupNamespace("air"));
            if (childNode != null)
            {
                childNode.InnerText = childNodeValue;
                subParentNode.AppendChild(childNode);
            }
        }

        // Simple class to construct an Air Application File Type. 
        // The class requires external validation of property values.
        public class AirFileType
        {

            private String _name;
            private String _extension;
            private String _description;
            private String _contentType;
            private readonly List<AirFileTypeIcon> _icons;

            private Boolean _nameIsValid;
            private Boolean _extensionIsValid;
            private Boolean _descriptionIsValid;
            private Boolean _contentTypeIsValid;

            public AirFileType()
            {
                _name = String.Empty;
                _extension = String.Empty;
                _description = String.Empty;
                _contentType = String.Empty;
                // According to Descriptor.xsd, 57x57 is not used for file icons
                _icons = new List<AirFileTypeIcon> 
                { 
                    new AirFileTypeIcon(16, String.Empty, AirVersion.V10),
                    new AirFileTypeIcon(29, String.Empty, AirVersion.V20),
                    new AirFileTypeIcon(32, String.Empty, AirVersion.V10),
                    new AirFileTypeIcon(36, String.Empty, AirVersion.V25),
                    new AirFileTypeIcon(48, String.Empty, AirVersion.V10),
                    new AirFileTypeIcon(72, String.Empty, AirVersion.V20),
                    new AirFileTypeIcon(128, String.Empty, AirVersion.V10),
                    new AirFileTypeIcon(512, String.Empty, AirVersion.V20)
                };
            }

            public String Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public String Extension
            {
                get { return _extension; }
                set { _extension = value; }
            }

            public String Description
            {
                get { return _description; }
                set { _description = value; }
            }

            public String ContentType
            {
                get { return _contentType; }
                set { _contentType = value; }
            }

            public List<AirFileTypeIcon> Icons
            {
                get { return _icons; }
            }

            public void SetIconPath(Int16 size, string path)
            {
                foreach (AirFileTypeIcon icon in _icons)
                {
                    if (icon.Size == size)
                    {
                        icon.FilePath = path;
                        break;
                    }
                }
            }

            public void SetIconIsValid(Int16 size, Boolean isValid)
            {
                foreach (AirFileTypeIcon icon in _icons)
                {
                    if (icon.Size == size)
                    {
                        icon.IsValid = isValid;
                        break;
                    }
                }
            }

            public String GetIconPath(Int16 size)
            {
                foreach (AirFileTypeIcon icon in _icons)
                {
                    if (icon.Size == size) return icon.FilePath;
                }
                return String.Empty;
            }

            public Boolean NameIsValid
            {
                get { return _nameIsValid; }
                set { _nameIsValid = value; }
            }

            public Boolean ExtensionIsValid
            {
                get { return _extensionIsValid; }
                set { _extensionIsValid = value; }
            }

            public Boolean DescriptionIsValid
            {
                get { return _descriptionIsValid; }
                set { _descriptionIsValid = value; }
            }

            public Boolean ContentTypeIsValid
            {
                get { return _contentTypeIsValid; }
                set { _contentTypeIsValid = value; }
            }

            public Boolean IsValid
            {
                get
                {
                    foreach (AirFileTypeIcon icon in _icons)
                    {
                        if (!icon.IsValid) return false;
                    }
                    return _nameIsValid && _extensionIsValid && _descriptionIsValid && _contentTypeIsValid;
                }
            }

            public class AirFileTypeIcon
            {
                private Int16 _size;
                private Boolean _isValid = false;
                private string _filePath = String.Empty;
                private AirVersion _minVersion = AirVersion.V10;

                public AirFileTypeIcon(Int16 size, string filePath, AirVersion minVersion)
                {
                    _size = size;
                    _filePath = filePath;
                    _minVersion = minVersion;
                    _isValid = true;
                }

                public Int16 Size
                {
                    get { return _size; }
                }

                public String FilePath
                {
                    get { return _filePath; }
                    set { _filePath = value; }
                }

                public AirVersion MinVersion
                {
                    get { return _minVersion; }
                }

                public Boolean IsValid
                {
                    get { return _isValid; }
                    set { _isValid = value; }
                }
            }
        }

        public class AirApplicationIconField
        {
            private string _size = String.Empty;
            private AirVersion _minVersion = AirVersion.V10;

            public AirApplicationIconField(string size, AirVersion minVersion)
            {
                _size = size;
                _minVersion = minVersion;
            }

            public String Size
            {
                get { return _size; }
            }

            public TextBox Field { get; set; }

            public AirVersion MinVersion
            {
                get { return _minVersion; }
            }
        }

        public class AirExtension
        {
            private string _extensionId = String.Empty;

            public String ExtensionId
            {
                get { return _extensionId; }
                set { _extensionId = value; }
            }

            public Boolean IsValid { get; set; }

            public String Path { get; set; }
        }

        public class AndroidPermission
        {
            public String Constant { get; set; }

            public String Description { get; set; }

            public AndroidPermission(string constant, string description)
            {
                Constant = constant;
                Description = description;
            }
        }
     
    }

}
