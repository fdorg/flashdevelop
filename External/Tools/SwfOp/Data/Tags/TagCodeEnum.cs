/*
    swfOP is an open source library for manipulation and examination of
    Macromedia Flash (SWF) ActionScript bytecode.
    Copyright (C) 2004 Florian Krüsch.
    see Licence.cs for LGPL full text!

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

namespace SwfOp.Data.Tags {

    // action tags:
    // 12: DoAction
    // 26: PlaceObject2
    // 39: DefineSprite
    // 59: InitAction

    // button tags, not implemented
    //  7: DefineButton
    // 34: DefineButton2

    /// <summary>
    /// enumeration of tag codes of tags containing bytecode
    /// </summary>
    public enum TagCodeEnum {
        End = 00,
        ShowFrame = 01,
        DefineShape = 02,
        FreeCharacter = 03,
        PlaceObject = 04,
        RemoveObject = 05,
        DefineBits = 06,
        DefineButton = 07,
        JpegTable = 08,
        SetBackgroundColor = 09,
        DefineFont = 10,
        DefineText = 11,
        DoAction = 12,
        DefineFontInfo = 13,
        DefineSound = 14,
        StartSound = 15,
        StopSound = 16,
        DefineButtonSound = 17,
        SoundStreamHead = 18,
        SoundStreamBlock = 19,
        DefineBitsLossless = 20,
        DefineBitsJpeg2 = 21,
        DefineShape2 = 22,
        DefineButtonCxform = 23,
        Protect = 24,
        PathsArePostScript = 25,
        PlaceObject2 = 26,
        RemoveObject2 = 28,
        SyncFrame = 29,
        FreeAll = 31,
        DefineShape3 = 32,
        DefineText2 = 33,
        DefineButton2 = 34,
        DefineBitsJpeg3 = 35,
        DefineBitsLossless2 = 36,
        DefineEditText = 37,
        DefineVideo = 38,
        DefineSprite = 39,
        NameCharacter = 40,
        ProductInfo = 41,
        DefineTextFormat = 42,
        FrameLabel = 43,
        DefineBehavior = 44,
        SoundStreamHead2 = 45,
        DefineMorphShape = 46,
        FrameTag = 47,
        DefineFont2 = 48,
        GenCommand = 49,
        DefineCommandObj = 50,
        CharacterSet = 51,
        FontRef = 52,
        DefineFunction = 53,
        PlaceFunction = 54,
        GenTagObject = 55,
        ExportAssets = 56,
        ImportAssets = 57,
        EnableDebugger = 58,
        InitAction = 59,
        DefineVideoStream = 60,
        VideoFrame = 61,
        DefineFontInfo2 = 62,
        DebugID = 63,
        EnableDebugger2 = 64,
        ScriptLimit = 65,
        SetTabIndex = 66,
        //DefineShape4 = 67,
        //DefineMorphShape2 = 68,
        // Flash 8
        FileAttributes = 69,
        PlaceObject3 = 70,
        ImportAssets2 = 71,
        DefineFontAlignZones = 73,
        CSMTextSettings = 74,
        DefineFont3 = 75,
        MetaData = 77,
        DefineScalingGrid = 78,
        DefineShape4 = 83,
        DefineMorphShape2 = 84,
        // Flash 9
        DoABC = 72,
        SymbolClass = 76,
        DoABCDefine = 82,
        DefineSceneAndFrameData = 86,
        DefineBinaryData = 87,
        DefineFontName = 88
    }
}
