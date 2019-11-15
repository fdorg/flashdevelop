// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Globalization;
using System.Reflection;

namespace Flash.Localization
{
	/// <author>  Roger Gonzalez
	/// </author>
	public class LocalizationManager
	{
		public LocalizationManager()
		{
		}
		
		public virtual void  addLocalizer(ILocalizer localizer)
		{
			localizers.Add(localizer);
		}
		
		private ILocalizedText getLocalizedText(CultureInfo locale, String id)
		{
            foreach (ILocalizer localizer in localizers)
            {
                ILocalizedText text = localizer.getLocalizedText(locale, id);

                if (text != null)
                {
                    return text;
                }
            }

            return null;
        }
		
		protected internal static String replaceInlineReferences(String text, System.Collections.IDictionary parameters)
		{
			if (parameters == null)
				return text;
			
			int depth = 100;
			while (depth-- > 0)
			{
				int o = text.IndexOf("${");
				if (o == - 1)
					break;
				if ((o >= 1) && (text[o - 1] == '$'))
				{
					o = text.IndexOf("${", o + 2);
					if (o == - 1)
						break;
				}
				
				int c = text.IndexOf("}", o);
				
				if (c == - 1)
				{
					return null; // FIXME
				}
				String name = text.Substring(o + 2, (c) - (o + 2));
				String value = null;
				if (parameters.Contains(name) && (parameters[name] != null))
				{
					value = parameters[name].ToString();
				}
				
				if (value == null)
				{
					value = "";
				}
				text = text.Substring(0, (o) - (0)) + value + text.Substring(c + 1);
			}
			return text.Replace("$${", "${");
		}
		
		public virtual String getLocalizedTextString(String id)
		{
			return getLocalizedTextString(id, (System.Collections.IDictionary) new System.Collections.Hashtable());
		}
		
		public virtual String getLocalizedTextString(String id, System.Collections.IDictionary parameters)
		{
			return getLocalizedTextString(System.Threading.Thread.CurrentThread.CurrentCulture, id, parameters);
		}
		
		public virtual String getLocalizedTextString(CultureInfo locale, String id, System.Collections.IDictionary parameters)
		{
			ILocalizedText t = getLocalizedText(locale, id);
			
			if ((t == null) && !locale.Equals(System.Threading.Thread.CurrentThread.CurrentCulture))
			{
				t = getLocalizedText(System.Threading.Thread.CurrentThread.CurrentCulture, id);
			}
			if ((t == null) && !System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("en"))
			{
				t = getLocalizedText(new CultureInfo("en"), id);
			}
			
			return (t == null)?null:t.format(parameters);
		}
		
		public virtual String getLocalizedTextString(Object obj)
		{
			return getLocalizedTextString(System.Threading.Thread.CurrentThread.CurrentCulture, obj);
		}
		
		// todo - this is a pretty specialized helper function, hoist up to client code?
		public virtual String getLocalizedTextString(CultureInfo locale, Object obj)
		{
			String id = obj.GetType().FullName.Replace("$", ".");
			
			System.Collections.IDictionary parameters = new System.Collections.Hashtable();
			Type c = obj.GetType();
			
			while (c != typeof(Object))
			{
				FieldInfo[] fields = c.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

				foreach (FieldInfo f in fields)
				{
					if (!f.IsPublic || f.IsStatic)
					{
						continue;
					}
					
					try
					{
						parameters[f.Name] = f.GetValue(obj);
					}
					catch (Exception)
					{
					}
				}
				c = c.BaseType;
			}
			
			String s = null;
			if ((parameters.Contains("id") && parameters["id"] != null))
			{
				String subid = parameters["id"].ToString();
				if (subid.Length > 0)
				{
					// fixme - Formalize?
					s = getLocalizedTextString(locale, id + "." + subid, parameters);
				}
			}
			if (s == null)
			{
				s = getLocalizedTextString(locale, id, parameters);
			}
			
			if (s == null)
			{
				s = id;
				
				if (parameters != null)
				{
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (System.Collections.DictionaryEntry e in parameters)
					{
                        if (sb.Length > 0)
                        {
							sb.Append(", ");
                        }

						sb.Append(e.Key.ToString());
						if (e.Value != null)
						{
							sb.Append("='" + e.Value.ToString() + "'");
						}
					}
					s += "[" + sb.ToString() + "]";
				}
				return s;
			}
			
			return s;
		}
		
		private System.Collections.IList localizers = new System.Collections.ArrayList();
	}
}
