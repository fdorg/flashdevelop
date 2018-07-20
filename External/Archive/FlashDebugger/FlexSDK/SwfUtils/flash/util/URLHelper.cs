// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JavaCompatibleClasses;

namespace Flash.Util
{
	
	public class URLHelper
	{
		/// <summary> Everything before the "query" part of the URL.  E.g. for
		/// "http://www.example.com/file?firstname=Bob&amp;lastname=Smith#foo"
		/// this would be "http://www.example.com/file".
		/// </summary>
		virtual public String EverythingBeforeQuery
		{
			get
			{
				return m_everythingBeforeQuery;
			}
			
			set
			{
				assertValidArguments(value, Query, Fragment);
				m_everythingBeforeQuery = value;
			}
			
		}
		/// <summary> Returns the "query" portion of the URL, e.g. the
		/// "?firstname=Bob&amp;lastname=Smith" part. m_query contains the query
		/// (including "?"), or "" if the URL has no query. Never null.
		/// </summary>
		/// <summary> Sets the "query" portion of the URL.  This must be either the
		/// empty string or a string that begins with "?".
		/// </summary>
		virtual public String Query
		{
			get
			{
				return m_query;
			}
			
			set
			{
				// if there is a query, make sure it starts with "?"
				if (value.Length > 0 && value[0] != '?')
					value = "?" + value; //$NON-NLS-1$
				
				assertValidArguments(EverythingBeforeQuery, value, Fragment);
				
				m_query = value;
			}
			
		}
		/// <summary> Returns the "fragment" portion of the URL, e.g. the "#foo" part, or
		/// "" if the URL has no fragment. Never null.
		/// </summary>
		/// <summary> Sets the "fragment" portion of the URL.  This must be either the
		/// empty string or a string that begins with "#".
		/// </summary>
		virtual public String Fragment
		{
			get
			{
				return m_fragment;
			}
			
			set
			{
				// if there is a fragment, make sure it starts with "#"
				if (value.Length > 0 && value[0] != '#')
					value = "#" + value; //$NON-NLS-1$
				
				assertValidArguments(EverythingBeforeQuery, Query, value);
				m_fragment = value;
			}
			
		}
		/// <summary> Returns the entire URL.</summary>
		virtual public String URL
		{
			get
			{
				return m_everythingBeforeQuery + m_query + m_fragment;
			}
			
		}
		private static Regex URL_PATTERN = new Regex(@"^(.*?)(\?.*?)?(#.*)?$"); //$NON-NLS-1$
		
		/// <summary> Everything before the "query" part of the URL.  E.g. for
		/// "http://www.example.com/file?firstname=Bob&amp;lastname=Smith#foo"
		/// this would be "http://www.example.com/file".
		/// </summary>
		private String m_everythingBeforeQuery;
		
		/// <summary> The "query" in a URL is the "?firstname=Bob&amp;lastname=Smith" part.
		/// m_query contains the query (including "?"), or contains "" if the
		/// URL has no query.  Never null.
		/// </summary>
		private String m_query;
		
		/// <summary> The "fragment" in a URL is the "#foo" part at the end of a URL.
		/// m_fragment contains the fragment (including "#"), or contains "" if the
		/// URL has no fragment. Never null.
		/// </summary>
		private String m_fragment;
		
		public URLHelper(String url)
		{
			Match match = URL_PATTERN.Match(url);
			
			if (!match.Success)
				throw new ArgumentException(url);
			
			if (match.Success)
			{
				m_everythingBeforeQuery = match.Groups[1].ToString();
				
				m_query = match.Groups[2].ToString();
				if (m_query == null)
					m_query = ""; //$NON-NLS-1$
				
				m_fragment = match.Groups[3].ToString();
				if (m_fragment == null)
					m_fragment = ""; //$NON-NLS-1$
			}
		}
		
		private static void  assertValidArguments(String everythingBeforeQuery, String query, String fragment)
		{
			Debug.Assert(areArgumentsValid(everythingBeforeQuery, query, fragment));
		}
		
		/// <summary> This will test for various error conditions, e.g. a query string that
		/// contains "#" or has incorrect contents.
		/// </summary>
		private static bool areArgumentsValid(String everythingBeforeQuery, String query, String fragment)
		{
			if (everythingBeforeQuery == null || query == null || fragment == null)
				return false;
			
			URLHelper newHelper = new URLHelper(everythingBeforeQuery + query + fragment);
			if (!newHelper.EverythingBeforeQuery.Equals(everythingBeforeQuery) || !newHelper.Query.Equals(query) || !newHelper.Fragment.Equals(fragment))
			{
				return false;
			}
			
			return true;
		}
		
		/// <summary> Returns the query portion of the URL, broken up into individual key/value
		/// pairs. Does NOT unescape the keys and values.
		/// </summary>
		public virtual LinkedHashMap getParameterMap()
		{
            LinkedHashMap map;
			
			SupportClass.Tokenizer tokens = new SupportClass.Tokenizer(Query, "?&"); //$NON-NLS-1$
			// multiply by 2 to create a sufficiently large HashMap
			map = new LinkedHashMap(tokens.Count * 2);
			
			while (tokens.HasMoreTokens())
			{
				String nameValuePair = tokens.NextToken();
				String name = nameValuePair;
				String value = ""; //$NON-NLS-1$
				int equalsIndex = nameValuePair.IndexOf('=');
				if (equalsIndex != - 1)
				{
					name = nameValuePair.Substring(0, (equalsIndex) - (0));
					if (name.Length > 0)
					{
						value = nameValuePair.Substring(equalsIndex + 1);
					}
				}
				map.Add(name, value);
			}
			
			return map;
		}
		
		/// <summary> Sets the query portion of the URL.
		/// 
		/// </summary>
		/// <param name="parameterMap">a key/value mapping; these must already be escaped!
		/// </param>
		public virtual void  setParameterMap(System.Collections.IDictionary parameterMap)
		{
			if ((parameterMap != null) && (!(parameterMap.Count == 0)))
			{
				System.Text.StringBuilder queryString = new System.Text.StringBuilder();

				foreach (System.Collections.DictionaryEntry entry in parameterMap)
				{
					String name = (String) entry.Key;
					String value = Convert.ToString(entry.Value);
					queryString.Append(name);
					if (value != null && value.Length > 0)
					{
						queryString.Append('=');
						queryString.Append(value);
					}
					queryString.Append('&');
				}

                if (queryString.Length > 0)
                {
                    queryString.Length--;
                }
				
				Query = queryString.ToString();
			}
			else
			{
				Query = ""; //$NON-NLS-1$
			}
		}
		
		// shortcut for converting spaces to %20 in URIs
		public static String escapeSpace(String uri)
		{
			return escapeCharacter(uri, ' ', "%20"); //$NON-NLS-1$
		}
		
		/// <summary> Locates characters 'c' in the scheme specific portion of a URI and
		/// translates them into 'to'
		/// </summary>
		public static String escapeCharacter(String uri, char c, String to)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			int size = uri.Length;
			int at = uri.IndexOf(':');
			int lastAt = 0;
			
			// skip the scheme
			if (at > - 1)
			{
				for (int i = 0; i <= at; i++)
					sb.Append(uri[i]);
				lastAt = ++at;
			}
			
			// while we have 'c's in uri
			while ((at = uri.IndexOf((Char) c, at)) > - 1)
			{
				// original portion
				for (int i = lastAt; i < at; i++)
					sb.Append(uri[i]);
				
				// conversion
				sb.Append(to);
				lastAt = ++at; // advance to char after conversion
			}
			
			if (lastAt < size)
			{
				for (int i = lastAt; i < size; i++)
					sb.Append(uri[i]);
			}
			return sb.ToString();
		}
	}
}
