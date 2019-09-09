// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using Flash.Swf;
using ILocalizedText = Flash.Localization.ILocalizedText;
using ILocalizer = Flash.Localization.ILocalizer;
using ResourceBundleLocalizer = Flash.Localization.ResourceBundleLocalizer;

namespace Flash.Tools.Debugger
{
	/// <summary> An ILocalizer which does a couple of extra things:
	/// 
	/// <ol>
	/// <li> If the requested string is not found, rather than returning <code>null</code>, we
	/// return a default string, to avoid a crash. </li>
	/// <li> We replace any "\n" with the current platform's newline sequence. </li>
	/// </ol>
	/// 
	/// </summary>
	/// <author>  mmorearty
	/// </author>
	public class DebuggerLocalizer : ILocalizer
	{
		private class DefaultILocalizedText : ILocalizedText
		{
            public DefaultILocalizedText(String id)
			{
                this.id = id;
            }
			private String id;
			public virtual String format(System.Collections.IDictionary parameters)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append('!');
				sb.Append(id);
				sb.Append('!');
				if (parameters != null && parameters.Count != 0)
				{
					sb.Append(' ');
					sb.Append(SupportClass.CollectionToString(parameters));
				}
				return sb.ToString();
			}
		}

        private class FinalILocalizedText : ILocalizedText
		{
			private ILocalizedText finalLocalizedText;
            public FinalILocalizedText(ILocalizedText localizedText)
            {
                finalLocalizedText = localizedText;
            }

			public virtual String format(System.Collections.IDictionary parameters)
			{
				String result = finalLocalizedText.format(parameters);
				return result.Replace("\n", DebuggerLocalizer.s_newline); //$NON-NLS-1$
			}
		}

		private String m_prefix;

        private static System.Collections.Hashtable s_Localizers = new System.Collections.Hashtable();
        private static readonly String s_newline = System.Environment.NewLine; //$NON-NLS-1$
		
		public DebuggerLocalizer(String prefix)
		{
			m_prefix = prefix;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetCallingAssembly();

            if (!s_Localizers.ContainsKey(assembly))
            {
                s_Localizers.Add(assembly, new ResourceBundleLocalizer(assembly));
            }
		}
		
		public virtual ILocalizedText getLocalizedText(System.Globalization.CultureInfo locale, String id)
		{
			// We hard-code our package name in here, so that callers can use
			// a short string
            ILocalizedText localizedText = null;
			
            foreach (ILocalizer localizer in s_Localizers.Values)
            {
                localizedText = localizer.getLocalizedText(locale, m_prefix + id);

                if (localizedText != null)
                {
                    break;
                }
            }

			// If still no ILocalizedText was found, create a default one
			if (localizedText == null)
			{
                localizedText = new DefaultILocalizedText(id);
			}
			
			// If the current platform's newline sequence is something other
			// than "\n", then replace all occurrences of "\n" with this platform's
			// newline sequence.
			if (DebuggerLocalizer.s_newline.Equals("\n")) //$NON-NLS-1$
			{
				return localizedText;
			}
			else
			{
				return new FinalILocalizedText(localizedText);
			}
		}
	}
}
