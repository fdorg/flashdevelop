// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace flash.localization
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class XLRLocalizer : ILocalizer
	{
		public XLRLocalizer()
		{
			// only resources
		}
		public XLRLocalizer(System.String path)
		{
			findFiles(new System.IO.FileInfo(path), null);
		}
		
		public virtual XLRTargetNode loadNode(System.Globalization.CultureInfo fileLocale, System.String fileId, System.Globalization.CultureInfo locale, System.String id)
		{
			System.String key = getKey(fileLocale, fileId);
			XLRFile f = (XLRFile) filedict[key];
			
			if (f == null)
			{
				System.String resource = key.replaceAll("\\.", "/") + ".xlr";
				//UPGRADE_ISSUE: Method 'java.lang.ClassLoader.getResource' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassLoader'"
				//UPGRADE_ISSUE: Method 'java.lang.Class.getClassLoader' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangClassgetClassLoader'"
				System.Uri url = GetType().getClassLoader().getResource(resource);
				
				if (url != null)
				{
					f = new XLRFile(this, fileId, url);
					filedict[key] = f;
				}
			}
			if (f != null)
			{
				f.load();
				XLRMessageNode messageNode = (XLRMessageNode) nodedict[id];
				if (messageNode != null)
				{
					XLRTargetNode targetNode = messageNode.getTarget(locale.ToString());
					return targetNode;
				}
			}
			
			return null;
		}
		
		public virtual XLRTargetNode checkPrefix(System.Globalization.CultureInfo fileLocale, System.String fileId, System.Globalization.CultureInfo locale, System.String id)
		{
			XLRTargetNode t = loadNode(fileLocale, fileId, locale, id);
			if (t == null)
			{
				int sep = fileId.LastIndexOf('$');
				
				if (sep == - 1)
					sep = fileId.LastIndexOf('.');
				
				if (sep != - 1)
					t = checkPrefix(fileLocale, fileId.Substring(0, (sep) - (0)), locale, id);
			}
			return t;
		}
		
		public virtual XLRTargetNode checkLocales(System.Globalization.CultureInfo locale, System.String id)
		{
			XLRTargetNode t = checkPrefix(locale, id, locale, id);
			
			//UPGRADE_ISSUE: Method 'java.util.Locale.getVariant' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilLocalegetVariant'"
			if ((t == null) && (new System.Globalization.RegionInfo(locale.LCID).TwoLetterISORegionName.Length > 0) && (locale.getVariant().Length > 0))
			{
				//UPGRADE_WARNING: Constructor 'java.util.Locale.Locale' was converted to 'System.Globalization.CultureInfo' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
				//UPGRADE_TODO: Method 'java.util.Locale.getLanguage' was converted to 'System.Globalization.CultureInfo.TwoLetterISOLanguageName' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilLocalegetLanguage'"
				t = checkPrefix(new System.Globalization.CultureInfo(locale.TwoLetterISOLanguageName + "-" + new System.Globalization.RegionInfo(locale.LCID).TwoLetterISORegionName), id, locale, id);
			}
			
			if ((t == null) && (new System.Globalization.RegionInfo(locale.LCID).TwoLetterISORegionName.Length > 0))
			{
				//UPGRADE_TODO: Method 'java.util.Locale.getLanguage' was converted to 'System.Globalization.CultureInfo.TwoLetterISOLanguageName' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilLocalegetLanguage'"
				t = checkPrefix(new Locale(locale.TwoLetterISOLanguageName), id, locale, id);
			}
			
			if ((t == null))
				t = checkPrefix(null, id, locale, id);
			
			return t;
		}
		
		
		public virtual ILocalizedText getLocalizedText(System.Globalization.CultureInfo locale, System.String id)
		{
			XLRMessageNode messageNode = (XLRMessageNode) nodedict[id];
			XLRTargetNode targetNode = null;
			if (messageNode != null)
			{
				targetNode = messageNode.getTarget(locale.ToString());
			}
			
			if (targetNode == null)
			{
				targetNode = checkLocales(locale, id);
			}
			
			if (targetNode == null)
			{
				return null;
			}
			
			return new XLRLocalizedText(this, targetNode);
		}
		
		private System.String getKey(System.Globalization.CultureInfo locale, System.String id)
		{
			System.String key = id;
			if (locale != null)
			{
				//UPGRADE_TODO: Method 'java.util.Locale.getLanguage' was converted to 'System.Globalization.CultureInfo.TwoLetterISOLanguageName' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilLocalegetLanguage'"
				if (locale.TwoLetterISOLanguageName.Length > 0)
				{
					//UPGRADE_TODO: Method 'java.util.Locale.getLanguage' was converted to 'System.Globalization.CultureInfo.TwoLetterISOLanguageName' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilLocalegetLanguage'"
					key += ("_" + locale.TwoLetterISOLanguageName);
					if (new System.Globalization.RegionInfo(locale.LCID).TwoLetterISORegionName.Length > 0)
					{
						key += ("_" + new System.Globalization.RegionInfo(locale.LCID).TwoLetterISORegionName);
						
						//UPGRADE_ISSUE: Method 'java.util.Locale.getVariant' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilLocalegetVariant'"
						if (locale.getVariant().Length > 0)
						{
							//UPGRADE_ISSUE: Method 'java.util.Locale.getVariant' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javautilLocalegetVariant'"
							key += ("_" + locale.getVariant());
						}
					}
				}
			}
			return key;
		}
		
		private System.Collections.IDictionary filedict = new System.Collections.Hashtable();
		private System.Collections.IDictionary nodedict = new System.Collections.Hashtable();
		
		private class XLRFile
		{
			public XLRFile(System.String prefix, System.Uri url)
			{
				this.prefix = prefix;
				this.url = url;
			}
			
			public virtual void  load()
			{
				if (loaded)
				{
					return;
				}
				try
				{
					System.IO.Stream inStream = new System.IO.BufferedStream(System.Net.WebRequest.Create(this.url).GetResponse().GetResponseStream());
					XmlSAXDocumentManager factory = XmlSAXDocumentManager.NewInstance();
					factory.NamespaceAllowed = false; // FIXME
					
					XLRHandler xmlHandler = new XLRHandler(Enclosing_Instance.nodedict, prefix);
					CDATAHandler cdataHandler = new CDATAHandler(xmlHandler);
					
					XmlSAXDocumentManager parser = XmlSAXDocumentManager.CloneInstance(factory);
					//UPGRADE_TODO: Method 'javax.xml.parsers.SAXParser.setProperty' was converted to 'XmlSAXDocumentManager.setProperty' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaxxmlparsersSAXParsersetProperty_javalangString_javalangObject'"
					parser.setProperty("http://xml.org/sax/properties/lexical-handler", cdataHandler);
                    parser.parse(inStream, xmlHandler);
				}
				catch (System.Exception e)
				{
					SupportClass.WriteStackTrace(e, Console.Error);
				}
				loaded = true;
			}
			
			private bool loaded = false;
			private System.Uri url;
			private System.String prefix;
		}
		
		private void  findFiles(System.IO.FileInfo f, System.String relative)
		{
			try
			{
				bool tmpBool;
				if (System.IO.File.Exists(f.FullName))
					tmpBool = true;
				else
					tmpBool = System.IO.Directory.Exists(f.FullName);
				if (!tmpBool)
					return ;
				
				if (System.IO.Directory.Exists(f.FullName))
				{
					System.IO.FileInfo[] files = SupportClass.FileSupport.GetFiles(f);
					
					for (int i = 0; i < files.Length; ++i)
					{
						findFiles(new System.IO.FileInfo(files[i].FullName), ((relative == null)?"":(relative + ".")) + files[i].Name);
					}
				}
				else
				{
					if (!f.Name.EndsWith(".xlr"))
						return ;
					else
					{
						System.String id = relative.Substring(0, (relative.Length - ".xlr".Length) - (0));
						
						System.String prefix = id;
						int dot = id.LastIndexOf('.');
						int underscore = - 1;
						if (dot != - 1)
						{
							//UPGRADE_WARNING: Method 'java.lang.String.indexOf' was converted to 'System.String.IndexOf' which may throw an exception. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1101'"
							underscore = id.IndexOf('_', dot);
						}
						else
						{
							underscore = id.IndexOf('_');
						}
						if (underscore != - 1)
						{
							prefix = id.Substring(0, (underscore) - (0));
						}
						
						filedict[id] = new XLRFile(this, prefix, SupportClass.FileSupport.ToUri(f));
					}
				}
			}
			catch (System.Exception e)
			{
				SupportClass.WriteStackTrace(e, Console.Error);
			}
		}
		
		private class XLRLocalizedText : ILocalizedText
		{
			public XLRLocalizedText(XLRTargetNode node)
			{
				this.node = node;
			}
			public virtual System.String format(System.Collections.IDictionary parameters)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder();
				System.String s = node.execute(buffer, node.locale, parameters)?buffer.ToString():null;
				if (s != null)
				{
					s = LocalizationManager.replaceInlineReferences(s, parameters);
				}
				return s;
			}
			private XLRTargetNode node;
		}
		
		//UPGRADE_NOTE: The access modifier for this class or class field has been changed in order to prevent compilation errors due to the visibility level. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1296'"
		abstract public class XLRNode
		{
			//UPGRADE_TODO: Class 'java.util.LinkedList' was converted to 'System.Collections.ArrayList' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilLinkedList'"
			public System.Collections.ArrayList children = new System.Collections.ArrayList();
			public virtual bool execute(System.Text.StringBuilder buffer, System.String locale, System.Collections.IDictionary parameters)
			{
				bool success = false;
				//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratorhasNext'"
				for (System.Collections.IEnumerator it = children.GetEnumerator(); it.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilIteratornext'"
					XLRNode child = (XLRNode) it.Current;
					
					if (child.execute(buffer, locale, parameters))
					{
						success = true;
					}
				}
				return success;
			}
		}
		
		private class XLRChoiceNode:XLRNode
		{
			public override bool execute(System.Text.StringBuilder buffer, System.String locale, System.Collections.IDictionary parameters)
			{
				for (System.Collections.IEnumerator it = children.GetEnumerator(); it.MoveNext(); )
				{
					XLRNode child = (XLRNode) it.Current;
					
					if (child.execute(buffer, locale, parameters))
					{
						return true;
					}
				}
				return false;
			}
		}
		
		private class XLRMessageNode:XLRChoiceNode
		{
			public XLRMessageNode(System.String id)
			{
				this.id = id;
			}
			public virtual XLRTargetNode getTarget(System.String locale)
			{
				for (System.Collections.IEnumerator it = children.GetEnumerator(); it.MoveNext(); )
				{
					XLRNode node = (XLRNode) it.Current;
					
					if ((node is XLRTargetNode) && ((XLRTargetNode) node).matchesLocale(locale))
					{
						return (XLRTargetNode) node;
					}
				}
				return null;
			}
			public System.String id;
		}
		
		public class XLRTargetNode:XLRNode
		{
			public XLRTargetNode(System.String locale)
			{
				this.locale = locale;
			}
			public virtual bool matchesLocale(System.String locale)
			{
				return (((this.locale == null) && (locale == null)) || locale.ToUpper().Equals(this.locale.ToUpper()));
			}
			public override bool execute(System.Text.StringBuilder buffer, System.String locale, System.Collections.IDictionary parameters)
			{
				if (matchesLocale(locale))
				{
					return base.execute(buffer, locale, parameters);
				}
				return false;
			}
			public System.String locale;
		}
		
		private class XLRTextNode:XLRNode
		{
			public XLRTextNode(System.String text)
			{
				this.text = text;
			}
			public override bool execute(System.Text.StringBuilder buffer, System.String locale, System.Collections.IDictionary parameters)
			{
				bool success = false;
				if (text != null)
				{
					success = true;
					buffer.Append(text);
				}
				bool result = base.execute(buffer, locale, parameters);
				return success || result;
			}
			public System.String text;
		}
		
		private class XLRVariableNode:XLRNode
		{
			public XLRVariableNode(System.String name)
			{
				this.varname = name;
			}
			public override bool execute(System.Text.StringBuilder buffer, System.String locale, System.Collections.IDictionary parameters)
			{
				bool success = false;
				if (varname != null)
				{
					success = parameters.Contains(varname) && (parameters[varname] != null);
					if (success)
					{
						buffer.Append(parameters[varname].ToString());
					}
				}
				success |= base.execute(buffer, locale, parameters);
				return success;
			}
			public System.String varname;
		}
		
		private class XLRMatchNode:XLRNode
		{
			public System.String varname;
			public System.String text = null;
			public System.String pattern = null;
			public XLRMatchNode(System.String varname, System.String pattern)
			{
				this.varname = varname;
				this.pattern = pattern;
			}
			public override bool execute(System.Text.StringBuilder buffer, System.String locale, System.Collections.IDictionary parameters)
			{
				System.String value_Renamed = null;
				
				if ((varname != null) && parameters.Contains(varname) && parameters[varname] != null)
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
					value_Renamed = parameters[varname].ToString();
				}
				if (value_Renamed == null)
				{
					value_Renamed = "";
				}
				// match based on the value being non-zero length, non-zero, or not "false" if pattern isn't set
				
				bool matched = false;
				if (pattern == null)
				{
					if ((value_Renamed != null) && (value_Renamed.Length > 0))
					{
						matched = !(value_Renamed.ToUpper().Equals("false".ToUpper()) || value_Renamed.Equals("0"));
					}
					else
					{
						matched = false; // null string
					}
				}
				else
				{
					// to match an empty string, try pattern of "^$"
					matched = value_Renamed.matches(pattern);
				}
				
				if (matched)
				{
					base.execute(buffer, locale, parameters);
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		public class XLRHandler:XmlSaxDefaultHandler
		{
			public XLRHandler(System.Collections.IDictionary nodedict, System.String base_Renamed)
			{
				this.nodedict = nodedict; // id -> messagenode
				this.base_Renamed = base_Renamed;
			}
			public System.Collections.ArrayList context = new System.Collections.ArrayList();
			private System.String fileLocale = null;
			private System.String base_Renamed = null;
			private System.Collections.IDictionary nodedict;
			internal System.Text.StringBuilder textBuffer = new System.Text.StringBuilder(128);
			protected internal bool inCDATA = false;
			public override void  startElement(System.String uri, System.String localName, System.String qName, SaxAttributesSupport attributes)
			{
				XLRNode current = null;
				if (context.Count > 0)
				{
					current = (XLRNode) context[context.Count - 1];
				}
				
				// common shortcuts...
				System.String locale = attributes.GetValue("locale");
				if (locale == null)
					locale = fileLocale;
				System.String text = attributes.GetValue("text");
				
				XLRNode node = null;
				if ("messages".Equals(qName))
				{
					fileLocale = attributes.GetValue("locale");
					if (attributes.GetValue("idbase") != null)
						base_Renamed = attributes.GetValue("idbase");
				}
				else if ("message".Equals(qName))
				{
					System.String id = attributes.GetValue("id");
					
					if (base_Renamed != null)
						id = base_Renamed + "." + id;
					
					node = (XLRMessageNode) nodedict[id];
					if (node == null)
					{
						node = new XLRMessageNode(id);
						nodedict[id] = node;
					}
					if ((text != null) && (locale != null))
					// check errors
					{
						XLRTargetNode targetNode = new XLRTargetNode(locale);
						node.children.Add(targetNode);
						XLRTextNode textNode = new XLRTextNode(text);
						targetNode.children.Add(textNode);
					}
					
					
					context.Add(node);
				}
				else if ("target".Equals(qName))
				{
					node = new XLRTargetNode(locale);
					if (text != null)
						node.children.Add(new XLRTextNode(text));
					
					current.children.Add(node);
					context.Add(node);
				}
				else if ("text".Equals(qName))
				{
					System.String value_Renamed = attributes.GetValue("value");
					
					node = new XLRTextNode(value_Renamed);
					
					current.children.Add(node);
					context.Add(node);
				}
				else if ("variable".Equals(qName))
				{
					System.String name = attributes.GetValue("name");
					
					node = new XLRVariableNode(name);
					current.children.Add(node);
					context.Add(node);
				}
				else if ("match".Equals(qName))
				{
					node = new XLRMatchNode(attributes.GetValue("variable"), attributes.GetValue("pattern"));
					if (text != null)
						node.children.Add(new XLRTextNode(text));
					
					current.children.Add(node);
					context.Add(node);
				}
				else if ("select".Equals(qName))
				{
					node = new XLRChoiceNode();
					current.children.Add(node);
					context.Add(node);
				}
				else
				{
					//UPGRADE_ISSUE: Constructor 'org.xml.sax.SAXParseException.SAXParseException' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_orgxmlsaxSAXParseExceptionSAXParseException_javalangString_orgxmlsaxLocator'"
					throw new SAXParseException("blorp", null); // fixme
				}
			}
			
			public override void  endElement(System.String uri, System.String localName, System.String qName)
			{
				XLRNode current = null;
				if (context.Count > 0)
					current = (XLRNode) SupportClass.StackSupport.Pop(context);
				
				if ("messages".Equals(qName))
				{
					// done
				}
				else if ("text".Equals(qName))
				{
					if (textBuffer.Length > 0)
					{
						current.children.Add(new XLRTextNode(textBuffer.ToString()));
					}
				}
				else if ("variable".Equals(qName))
				{
					if (textBuffer.Length > 0)
					{
						((XLRVariableNode) current).varname = textBuffer.ToString();
					}
				}
				textBuffer.Length = 0;
			}
			public override void  characters(System.Char[] ch, int start, int length)
			{
				if (inCDATA)
				{
					textBuffer.Append(ch, start, length);
				}
				else
				{
					System.String s = new System.String(ch, start, length).Trim();
					
					if (s.Length > 0)
						textBuffer.Append(s);
				}
			}
			
			
			//UPGRADE_TODO: Method 'org.xml.sax.helpers.DefaultHandler.ignorableWhitespace' was converted to 'XmlSaxDefaultHandler.ignorableWhitespace' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
			public override void ignorableWhitespace(System.Char[] ch, int start, int length)
			{
				// no op
			}
			//UPGRADE_TODO: Class 'org.xml.sax.SAXParseException' was converted to 'System.xml.XmlException' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
			public override void  warning(System.Xml.XmlException e)
			{
				// no op
			}
			
			
			//UPGRADE_TODO: Class 'org.xml.sax.SAXParseException' was converted to 'System.xml.XmlException' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
			public override void  error(System.Xml.XmlException e)
			{
				// no op
			}
			
			
			//UPGRADE_TODO: Class 'org.xml.sax.SAXParseException' was converted to 'System.xml.XmlException' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
			public override void  fatalError(System.Xml.XmlException e)
			{
				throw e;
			}
		}
		private class CDATAHandler : XmlSaxLexicalHandler
		{
			private XLRHandler parentHandler;
			public CDATAHandler(XLRHandler h)
			{
				parentHandler = h;
			}
			public virtual void  startCDATA()
			{
				parentHandler.inCDATA = true;
			}
			
			public virtual void  endCDATA()
			{
				parentHandler.inCDATA = false;
			}
			
			public virtual void  startDTD(System.String s, System.String s1, System.String s2)
			{
			}
			
			public virtual void  endDTD()
			{
			}
			
			public virtual void  startEntity(System.String s)
			{
			}
			
			public virtual void  endEntity(System.String s)
			{
			}
			
			public virtual void  comment(System.Char[] chars, int i, int i1)
			{
			}
		}
	}
}
