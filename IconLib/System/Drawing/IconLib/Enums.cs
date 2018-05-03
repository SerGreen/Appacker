//  Copyright (c) 2006, Gustavo Franco
//  Email:  gustavo_franco@hotmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Drawing.IconLib
{
    #region MultiIconFormat
    [Author("Franco, Gustavo")]
    public enum MultiIconFormat
    {
        // Read-Write
        ICO   = 1,
        ICL   = 2,
        DLL   = 3,
        // Read-Only
        EXE   = 4,
        OCX   = 5,
        CPL   = 6,
        SRC   = 7
    }
    #endregion

    #region HeaderSignatures
    [Author("Franco, Gustavo")]
    internal enum HeaderSignatures
    {
        IMAGE_DOS_SIGNATURE = 0x5A4D,      // MZ
        IMAGE_OS2_SIGNATURE = 0x454E,      // NE
        IMAGE_NT_SIGNATURE  = 0x00004550   // PE00
    }
    #endregion

	#region LoadLibraryFlags
    [Author("Franco, Gustavo")]
	internal enum LoadLibraryFlags
	{
		DONT_RESOLVE_DLL_REFERENCES    =  0x00000001,
		LOAD_LIBRARY_AS_DATAFILE       =  0x00000002,
		LOAD_WITH_ALTERED_SEARCH_PATH  =  0x00000008,
		LOAD_IGNORE_CODE_AUTHZ_LEVEL   =  0x00000010
	}
	#endregion

    #region ResourceType
    [Author("Franco, Gustavo")]
    internal enum ResourceType : uint
    {
        RT_CURSOR           = 1,
        RT_BITMAP           = 2,
        RT_ICON             = 3,
        RT_MENU             = 4,
        RT_DIALOG           = 5,
        RT_STRING           = 6,
        RT_FONTDIR          = 7,
        RT_FONT             = 8,
        RT_ACCELERATOR      = 9,
        RT_RCDATA           = 10,
        RT_MESSAGETABLE     = 11,
        RT_GROUP_CURSOR     = 12,
        RT_GROUP_ICON       = 14,
        RT_VERSION          = 16,
        RT_DLGINCLUDE       = 17,
        RT_PLUGPLAY         = 19,
        RT_VXD              = 20,
        RT_ANICURSOR        = 21,
        RT_ANIICON          = 22,
        RT_HTML             = 23
    }
    #endregion

    #region ResourceMemoryType
    [Flags]
    [Author("Franco, Gustavo")]
    internal enum ResourceMemoryType : ushort
    {
        None        = 0,
        Moveable    = 0x10, 
        Pure        = 0x20,
        PreLoad     = 0x40,
        Unknown     = 7168
    }
    #endregion

    #region BitmapCompression
    [Author("Franco, Gustavo")]
    public enum IconImageFormat : int
    {
	    //BI_RGB        = 0,
	    //BI_RLE8       = 1,
	    //BI_RLE4       = 2,
	    //BI_BITFIELDS  = 3,
	    //BI_JPEG       = 4,
        BMP         = 0,
	    PNG         = 5,
        UNKNOWN     = 255
    }
    #endregion

	#region PatBltTypes
    [Author("Franco, Gustavo")]
	internal enum PatBltTypes
	{
		SRCCOPY          =   0x00CC0020,
		SRCPAINT         =   0x00EE0086,
		SRCAND           =   0x008800C6,
		SRCINVERT        =   0x00660046,
		SRCERASE         =   0x00440328,
		NOTSRCCOPY       =   0x00330008,
		NOTSRCERASE      =   0x001100A6,
		MERGECOPY        =   0x00C000CA,
		MERGEPAINT       =   0x00BB0226,
		PATCOPY          =   0x00F00021,
		PATPAINT         =   0x00FB0A09,
		PATINVERT        =   0x005A0049,
		DSTINVERT        =   0x00550009,
		BLACKNESS        =   0x00000042,
		WHITENESS        =   0x00FF0062
	}
	#endregion

    #region Icon OS Format
    [Author("Franco, Gustavo")]
    [Flags]
    public enum IconOutputFormat
    {
        None            = 0,
        Vista           = 1,
        WinXP           = 2,
        WinXPUnpopular  = 4,
        Win95           = 8,
        Win95Unpopular  = 16,
        Win31           = 32,
        Win31Unpopular  = 64,
        Win30           = 128,
        Win30Unpopular  = 256,
        FromWinXP       = WinXP | Vista,
        FromWin95       = Win95 | FromWinXP,
        FromWin31       = Win31 | FromWin95,
        FromWin30       = Win30 | FromWin31,
        All             = FromWin31 | Win31Unpopular | Win95Unpopular | WinXPUnpopular
    }
    #endregion
}
