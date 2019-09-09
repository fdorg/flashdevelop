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
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Flash.Localization
{
	
	/// <author>  Roger Gonzalez
	/// </author>
	public class ResourceBundleLocalizer : ILocalizer
	{
        private ResourceManager m_ResourceManager;

        public ResourceBundleLocalizer(Assembly assembly)
        {
            String name = assembly.GetName().Name;

            m_ResourceManager = new ResourceManager(name + "." + name, assembly);
        }

		public virtual ILocalizedText getLocalizedText(CultureInfo locale, String id)
        {
            try
            {
                return new ResourceBundleText(m_ResourceManager.GetString(id, locale));
            }
            catch (MissingManifestResourceException)
            {
            }

            return null;
        }
		
		private class ResourceBundleText : ILocalizedText
		{
			public ResourceBundleText(String text)
			{
				this.text = text;
			}
			public virtual String format(System.Collections.IDictionary parameters)
			{
				return LocalizationManager.replaceInlineReferences(text, parameters);
			}
			private String text;
		}
	}
}
