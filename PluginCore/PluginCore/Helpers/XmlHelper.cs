using System;
using System.Xml;
using PluginCore.Managers;

namespace PluginCore.Helpers
{
    public class XmlHelper
    {
        /// <summary>
        /// Gets the value of the specified XmlNode.
        /// </summary>
        public static string GetValue(XmlNode node)
        {
            if (node?.FirstChild != null) return node.FirstChild.Value;
            return null;
        }

        /// <summary>
        /// Gets the specified attribute from the specified XmlNode.
        /// </summary>
        public static string GetAttribute(XmlNode node, string attName)
        {
            if (node?.Attributes[attName] != null) return node.Attributes[attName].Value;
            return null;
        }
        
        /// <summary>
        /// Checks that if the XmlNode has a value.
        /// </summary>
        public static bool HasValue(XmlNode node)
        {
            return (node?.FirstChild?.Value != null);
        }

        /// <summary>
        /// Checks if the XmlNode has the specified attribute.
        /// </summary>
        public static bool HasAttribute(XmlNode node, string attName)
        {
            return (node?.Attributes[attName] != null);
        }
        
        /// <summary>
        /// Reads a xml file and returns it as a XmlNode. Returns null on failure.
        /// </summary>
        public static XmlNode LoadXmlDocument(string file)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.PreserveWhitespace = false;
                document.Load(file); 
                try
                {
                    XmlNode declNode = document.FirstChild;
                    XmlNode rootNode = declNode.NextSibling;
                    return rootNode;
                }
                catch (Exception ex1)
                {
                    ErrorManager.ShowError(ex1);
                    return null;
                }
            }
            catch (Exception ex2)
            {
                ErrorManager.ShowError(ex2);
                return null;
            }
        }
        
    }
    
}
