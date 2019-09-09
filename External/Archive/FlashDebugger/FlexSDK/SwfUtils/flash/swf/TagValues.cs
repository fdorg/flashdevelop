// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
////////////////////////////////////////////////////////////////////////////////
//
//  ADOBE SYSTEMS INCORPORATED
//  Copyright 2003-2006 Adobe Systems Incorporated
//  All Rights Reserved.
//
//  NOTICE: Adobe permits you to use, modify, and distribute this file
//  in accordance with the terms of the license agreement accompanying it.
//
////////////////////////////////////////////////////////////////////////////////
using System;
namespace Flash.Swf
{
	
	/// <summary> Tag values that represent actions or data in a Flash script.</summary>
	/// <author>  Edwin Smith
	/// </author>
	public class TagValues {
		// Flash 1 tags
		public const int stagEnd = 0;
		public const int stagShowFrame = 1;
		public const int stagDefineShape = 2;
		public const int stagFreeCharacter = 3;
		public const int stagPlaceObject = 4;
		public const int stagRemoveObject = 5;
		public const int stagDefineBits = 6; // id,w,h,colorTab,bits - bitmap referenced by a fill(s)
		public const int stagDefineButton = 7; // up obj, down obj, action (URL, Page, ???)
		public const int stagJPEGTables = 8; // id,w,h,colorTab,bits - bitmap referenced by a fill(s)
		public const int stagSetBackgroundColor = 9;
		public const int stagDefineFont = 10;
		public const int stagDefineText = 11;
		public const int stagDoAction = 12;
		public const int stagDefineFontInfo = 13;
		public const int stagDefineSound = 14; // Event sound tags.
		public const int stagStartSound = 15;
		// int stagStopSound			= 16;
		public const int stagDefineButtonSound = 17;
		public const int stagSoundStreamHead = 18;
		public const int stagSoundStreamBlock = 19;
		// Flash 2 tags
		public const int stagDefineBitsLossless = 20; // A bitmap using lossless zlib compression.
		public const int stagDefineBitsJPEG2 = 21; // A bitmap using an internal JPEG compression table.
		public const int stagDefineShape2 = 22;
		public const int stagDefineButtonCxform = 23;
		public const int stagProtect = 24; // This file should not be importable for editing.
		public const int stagPathsArePostScript = 25; // assume shapes are filled as PostScript style paths
		// Flash 3 tags
		public const int stagPlaceObject2 = 26; // The new style place w/ alpha color transform and name.
		public const int stagRemoveObject2 = 28; // A more compact remove object that omits the character tag (just depth).
		// This tag is used for RealMedia only
		// int stagSyncFrame			= 29; // OBSOLETE...Handle a synchronization of the display list
		// int stagFreeAll				= 31; // OBSOLETE...Free all of the characters
		
		public const int stagDefineShape3 = 32; // A shape V3 includes alpha values.
		public const int stagDefineText2 = 33; // A text V2 includes alpha values.
		public const int stagDefineButton2 = 34; // a Flash 3 button that contains color transform and sound info
		// int stagMoveObject			= 34;	// OBSOLETE
		public const int stagDefineBitsJPEG3 = 35; // A JPEG bitmap with alpha info.
		public const int stagDefineBitsLossless2 = 36; // A lossless bitmap with alpha info.
		// int stagDefineButtonCxform2	= 37;	// OBSOLETE...a button color transform with alpha info
		// int stagDefineMouseTarget	= 38;	// define a sequence of tags that describe the behavio
		public const int stagDefineSprite = 39; // Define a sequence of tags that describe the behavior of a sprite.
		// int stagNameCharacter		= 40;	// OBSOLETE...name a character definition, character id and a string, (used for buttons, bitmaps, sprites and sounds)
		// int stagNameObject			= 41;	// OBSOLETE...name an object instance layer, layer number and a string, clear the name when no longer valid
		public const int stagProductInfo = 41; // a tag command for the Flash Generator customer serial id and cpu information.  [preilly] Repurposed for Flex Audit info.
		// int stagDefineTextFormat		= 42;	// OBSOLETE...define the contents of a text block with formating information
		public const int stagFrameLabel = 43; // A string label for the current frame.
		// int stagDefineButton2		= 44,	// unused, this is defined as 34 above
		public const int stagSoundStreamHead2 = 45; // For lossless streaming sound; should not have needed this...
		public const int stagDefineMorphShape = 46; // A morph shape definition
		// int stagFrameTag				= 47;	// OBSOLETE...a tag command for the Flash Generator (WORD duration, STRING label)
		public const int stagDefineFont2 = 48; // defines a font with extended information
		// int stagGenCommand			= 49;	// OBSOLETE...a tag command for the Flash Generator intrinsic
		// int stagDefineCommandObj		= 50;	// OBSOLETE...a tag command for the Flash Generator intrinsic Command
		// int stagCharacterSet			= 51;	// OBSOLETE...defines the character set used to store strings
		// int stagFontRef				= 52;   // OBSOLETE...defines a reference to an external font source
		// Flash 4 tags
		public const int stagDefineEditText = 37; // an edit text object (bounds; width; font, variable name)
		// int stagDefineVideo			= 38;	// OBSOLETE...a reference to an external video stream
		// int stagGenTagObject			= 55;	// OBSOLETE...a generator tag object written to the swf.
		
		public const int stagExportAssets = 56; // export assets from this swf file
		public const int stagImportAssets = 57; // import assets into this swf file
		public const int stagEnableDebugger = 58; // OBSOLETE...this movie may be debugged
		// Flash 6 tags
		public const int stagDoInitAction = 59;
		public const int stagDefineVideoStream = 60;
		public const int stagVideoFrame = 61;
		public const int stagDefineFontInfo2 = 62; // just like a font info except adds a language tag
		public const int stagDebugID = 63; // unique id to match up swf to swd
		public const int stagEnableDebugger2 = 64; //this movie may be debugged (see 59)
		public const int stagScriptLimits = 65; // Allow authoring tool to override some AS limits
		// Flash 7 tags
		public const int stagSetTabIndex = 66; // allows us to set .tabindex via tags, not actionscript
		// Flash 8 tags
		public const int stagDefineShape4 = 67; // includes enhanced line style and gradient properties
		public const int stagFileAttributes = 69; // FileAttributes defines whole-SWF attributes
		// (must be the FIRST tag after the SWF header)
		public const int stagPlaceObject3 = 70; // includes optional surface filter list for object
		public const int stagImportAssets2 = 71; // import assets into this swf file using the SHA-1 digest to
		// enable cached cross domain RSL downloads.
		public const int stagDoABC = 72; // embedded .abc (AVM+) bytecode
		public const int stagDefineFontAlignZones = 73; // ADF alignment zones
		public const int stagCSMTextSettings = 74;
		public const int stagDefineFont3 = 75; // defines a font with saffron information
		public const int stagSymbolClass = 76;
		public const int stagMetadata = 77; // XML blob with comments, description, copyright, etc
		public const int stagDefineScalingGrid = 78; // Scale9 grid
		public const int stagDoABC2 = 82; // revised ABC version with a name
		public const int stagDefineShape6 = 83;
		public const int stagDefineMorphShape2 = 84; // includes enhanced line style abd gradient properties
		public const int stagDefineSceneAndFrameLabelData = 86; // new in 8.5, only works on root timeline
		public const int stagDefineBinaryData = 87;
		public const int stagDefineFontName = 88; // adds name and copyright information for a font
		public const int stagDefineFont4 = 91; // new in 10, embedded cff fonts
		// NOTE: If tag values exceed 255 we need to expand SCharacter::tagCode from a BYTE to a WORD
		public const int stagDefineBitsPtr = 1023; // a special tag used only in the editor
		public static String[] names = new String[]{"End", "ShowFrame", "DefineShape", "FreeCharacter", "PlaceObject", "RemoveObject", "DefineBits", "DefineButton", "JPEGTables", "SetBackgroundColor", "DefineFont", "DefineText", "DoAction", "DefineFontInfo", "DefineSound", "StartSound", "StopSound", "DefineButtonSound", "SoundStreamHead", "SoundStreamBlock", "DefineBitsLossless", "DefineBitsJPEG2", "DefineShape2", "DefineButtonCxform", "Protect", "PathsArePostScript", "PlaceObject2", "27 (invalid)", "RemoveObject2", "SyncFrame", "30 (invalid)", "FreeAll", "DefineShape3", "DefineText2", "DefineButton2", "DefineBitsJPEG3", "DefineBitsLossless2", "DefineEditText", "DefineVideo", "DefineSprite", "NameCharacter", "ProductInfo", "DefineTextFormat", "FrameLabel", "DefineBehavior", "SoundStreamHead2", "DefineMorphShape", "FrameTag", "DefineFont2", "GenCommand", "DefineCommandObj", "CharacterSet", "FontRef", "DefineFunction", "PlaceFunction", "GenTagObject", "ExportAssets", "ImportAssets", "EnableDebugger", "DoInitAction", "DefineVideoStream", "VideoFrame", "DefineFontInfo2", "DebugID", "EnableDebugger2", "ScriptLimits", "SetTabIndex", "DefineShape4", "68 (invalid)", "FileAttributes", "PlaceObject3", "ImportAssets2", "DoABC", "DefineFontAlignZones", "CSMTextSettings", "DefineFont3", "SymbolClass", "Metadata", "ScalingGrid", "79 (invalid)", "80 (invalid)", "81 (invalid)", "DoABC2", "DefineShape6", "DefineMorphShape2", "85 (invalid)", "DefineSceneAndFrameLabelData", "DefineBinaryData", "DefineFontName", "89 (unknown)  ", "90 (unknown)  ", "DefineFont4", "(invalid)"};
		
		// Flash 5 tags
		// int stagDefineBehavior		= 44;   // OBSOLETE...a behavior which can be attached to a movie clip
		// int stagDefineFunction		= 53;   // OBSOLETE...defines a refernece to internals of a function
		// int stagPlaceFunction		= 54;   // OBSOLETE...creates an instance of a function in a thread
		
	}
}
