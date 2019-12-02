// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2004-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
using EnableDebugger = flash.swf.tags.EnableDebugger;
using GenericTag = flash.swf.tags.GenericTag;
using ScriptLimits = flash.swf.tags.ScriptLimits;
using SetBackgroundColor = flash.swf.tags.SetBackgroundColor;
using FileAttributes = flash.swf.tags.FileAttributes;
using ProductInfo = flash.swf.tags.ProductInfo;
using Metadata = flash.swf.tags.Metadata;
using DefineSceneAndFrameLabelData = flash.swf.tags.DefineSceneAndFrameLabelData;
using FlashUUID = flash.swf.types.FlashUUID;
using Rect = flash.swf.types.Rect;
namespace flash.swf
{
	
	/// <summary> Represents a whole flash movie.  singleton tags are represented as such,
	/// and frames are as well.
	/// 
	/// </summary>
	/// <author>  Edwin Smith
	/// </author>
	public class Movie
	{
		/// <summary> file format version (1..10)</summary>
		public int version;
		
		/// <summary> product info for compiler </summary>
		public ProductInfo productInfo;
		
		/// <summary> description of the app</summary>
		
		public Metadata metadata;
		
		/// <summary> initial stage size in twips</summary>
		public Rect size;
		
		/// <summary> frames per second</summary>
		public int framerate;
		
		/// <summary> player wide execution limits</summary>
		public ScriptLimits scriptLimits;
		
		/// <summary> protect the movie from being loaded into the authortool</summary>
		public GenericTag protect;
		
		/// <summary> bgcolor for the whole movie.</summary>
		public SetBackgroundColor bgcolor;
		
		/// <summary> FileAttributes defines whole-SWF attributes (SWF 8 or later)</summary>
		public FileAttributes fileAttributes;
		
		/// <summary> if present, player will attach to a debugger</summary>
		public EnableDebugger enableDebugger;
		
		/// <summary> if present, expect and/or generate a SWD</summary>
		public FlashUUID uuid; // if set, generate a swd
		
		/// <summary> each frame contains display list tags, and actions</summary>
		public System.Collections.IList frames;
		
		/// <summary> if movie will be used as a library, certain optimizations are verboten.</summary>
		public bool isLibrary;
		
		/// <summary> top-level class name</summary>
		public System.String topLevelClass;
		
		/// <summary> movie width in pixels, not twips</summary>
		public int width;
		
		/// <summary> movie width percentage</summary>
		public System.String widthPercent;
		
		/// <summary> is width default, or user-specified?</summary>
		public bool userSpecifiedWidth;
		
		/// <summary> movie height in pixels, not twips</summary>
		public int height;
		
		/// <summary> movie height percentage</summary>
		public System.String heightPercent;
		
		/// <summary> is height default, or user-specified?</summary>
		public bool userSpecifiedHeight;
		
		/// <summary> movie page title</summary>
		public System.String pageTitle;
		
		/// <summary> maps definition names    Sou</summary>
		public System.Collections.IDictionary definitionMap;
		
		/// <summary> 8.5 scene data, only one per timeline at the moment</summary>
		public DefineSceneAndFrameLabelData sceneAndFrameLabelData;
	}
}