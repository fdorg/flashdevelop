////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2007 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using Tag = Flash.Swf.Tag;
using TagHandler = Flash.Swf.TagHandler;
namespace Flash.Swf.Tags
{
	
	/// <author>  Paul Reilly
	/// </author>
	
	public class ProductInfo:Tag
	{
		virtual public long Build
		{
			get
			{
				return build;
			}
			
		}
		virtual public long CompileDate
		{
			get
			{
				return compileDate;
			}
			
		}
		virtual public int Edition
		{
			get
			{
				return edition;
			}
			
			set
			{
				this.edition = value;
			}
			
		}
		virtual public String EditionString
		{
			get
			{
				return editions[edition];
			}
			
		}
		virtual public int Product
		{
			get
			{
				return product;
			}
			
		}
		virtual public String ProductString
		{
			get
			{
				return products[product];
			}
			
		}
		virtual public byte MajorVersion
		{
			get
			{
				return majorVersion;
			}
			
		}
		virtual public byte MinorVersion
		{
			get
			{
				return minorVersion;
			}
			
		}
		private long build;
		private int product;
		private byte majorVersion;
		private byte minorVersion;
		private int edition;
		private long compileDate;
		
		public const int UNKNOWN = 0;
		public const int J2EE_PRODUCT = 1;
		public const int NET_PRODUCT = 2;
		public const int ABOBE_FLEX_PRODUCT = 3;
		
		protected internal static readonly String[] products = new String[]{"unknown", "Macromedia Flex for J2EE", "Macromedia Flex for .NET", "Adobe Flex"};
		
		protected internal const int DEVELOPER_EDITION = 0;
		protected internal const int FULL_COMMERCIAL_EDITION = 1;
		protected internal const int NON_COMMERCIAL_EDITION = 2;
		protected internal const int EDUCATIONAL_EDITION = 3;
		protected internal const int NFR_EDITION = 4;
		protected internal const int TRIAL_EDITION = 5;
		protected internal const int NO_EDITION = 6; // not part of any edition scheme      
		
		public static readonly String[] editions = new String[]{"Developer Edition", "Full Commercial Edition", "Non-Commercial Edition", "Educational Edition", "NFR Edition", "Trial Edition", ""};
		
		public ProductInfo(int product, int edition, byte majorVersion, byte minorVersion, long build, long compileDate):base(Flash.Swf.TagValues.stagProductInfo)
		{
			this.product = product;
			this.edition = edition;
			this.majorVersion = majorVersion;
			this.minorVersion = minorVersion;
			this.build = build;
			this.compileDate = compileDate;
		}
		
		public ProductInfo(int product, byte majorVersion, byte minorVersion, long build, long compileDate):base(Flash.Swf.TagValues.stagProductInfo)
		{
			this.product = product;
			this.majorVersion = majorVersion;
			this.minorVersion = minorVersion;
			this.build = build;
			this.compileDate = compileDate;
		}
		
		public ProductInfo(long compileDate):base(Flash.Swf.TagValues.stagProductInfo)
		{
			this.compileDate = compileDate;
		}
		
		public  override bool Equals(System.Object object_Renamed)
		{
			bool isEqual = false;
			
			if (base.Equals(object_Renamed) && (object_Renamed is ProductInfo))
			{
				ProductInfo productInfo = (ProductInfo) object_Renamed;
				
				if (product == productInfo.product && edition == productInfo.edition && majorVersion == productInfo.majorVersion && minorVersion == productInfo.minorVersion && build == productInfo.build && compileDate == productInfo.compileDate)
				{
					isEqual = true;
				}
			}
			
			return isEqual;
		}
		
		public override void  visit(TagHandler tagHandler)
		{
			tagHandler.productInfo(this);
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
