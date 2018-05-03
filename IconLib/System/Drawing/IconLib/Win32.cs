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
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Drawing.IconLib
{
    [Author("Franco, Gustavo")]
    internal class Win32
    {
        #region DELEGATES
        public delegate Int32 EnumResTypeProc(IntPtr hModule, IntPtr lpszType, IntPtr lParam);
        public delegate bool EnumResNameProc(IntPtr hModule, IntPtr pType, IntPtr pName, IntPtr param);
        #endregion

        #region KERNEL32
		[DllImport("KERNEL32.DLL")]
		public unsafe static extern void CopyMemory(void* dest, void* src, int length);
		[DllImport("kernel32.dll", SetLastError=true)] 
		public static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);
		[DllImport("kernel32.dll", SetLastError=true)] 
		public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);
		[DllImport("kernel32.dll", SetLastError=true)] 
		public static extern bool UpdateResource(IntPtr hUpdate, uint lpType, ref string pName, ushort wLanguage, byte[] lpData, uint cbData);
		[DllImport("kernel32.dll", SetLastError=true, CharSet=CharSet.Ansi)] 
		public static extern bool UpdateResource(IntPtr hUpdate, uint lpType, IntPtr pName, ushort wLanguage, byte[] lpData, uint cbData);
		[DllImport("kernel32.dll", SetLastError=true)] 
		public unsafe static extern bool UpdateResource(IntPtr hUpdate, uint lpType, byte[] pName, ushort wLanguage, byte[] lpData, uint cbData);
		[DllImport("kernel32.dll", SetLastError=true)] 
		public static extern bool UpdateResource(IntPtr hUpdate, uint lpType, uint lpName, ushort wLanguage, byte[] lpData, uint cbData);
        [DllImport("kernel32.dll", EntryPoint="RtlMoveMemory")]
        public unsafe static extern void CopyMemory(RGBQUAD* dest, byte* src, int cb);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int SizeofResource(IntPtr hModule, IntPtr hResource);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern int FreeLibrary(IntPtr hModule);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr LockResource(IntPtr hGlobalResource);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResource);
        [DllImport("Kernel32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        public static extern bool EnumResourceNames(IntPtr hModule, IntPtr pType, EnumResNameProc callback, IntPtr param); 
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool EnumResourceTypes(IntPtr hModule, EnumResTypeProc callback, IntPtr lParam);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr FindResource(IntPtr hModule, string resourceID, IntPtr type);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr FindResource(IntPtr hModule, Int32 resourceID, IntPtr type);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr FindResource(IntPtr hModule, IntPtr resourceID, IntPtr type);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr FindResource(IntPtr hModule, IntPtr resourceID, string resourceName);
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string libraryName);
		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr LoadLibraryEx(string path, IntPtr hFile, LoadLibraryFlags flags);
        #endregion

        #region GDI32
        [DllImport("gdi32.dll")]
        public static extern bool MaskBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth,
           int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, IntPtr hbmMask, int xMask,
           int yMask, uint dwRop);
        [DllImport("gdi32.dll")] 
        public static extern int SetBkColor(IntPtr hDC, uint crColor);
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)] 
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, PatBltTypes dwRop);
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr DeleteDC(IntPtr hDC);
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern bool DeleteObject(IntPtr hObject);
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
		[DllImport("gdi32.dll")]
		static public extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int Width, int Heigth);
        #endregion

        #region USER32
        [DllImport("user32.dll", CharSet=CharSet.Auto)] 
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        #endregion

        #region MACROS
        public static bool IS_INTRESOURCE(IntPtr value)
        {
            if (((uint) value) > ushort.MaxValue)
                return false;
            return true;
        }

        public static bool IS_INTRESOURCE(string value)
        {
            int iResult;
            return int.TryParse(value, out iResult);
        }

		public static int MAKEINTRESOURCE(int resource)
		{
			return 0x0000FFFF & resource;
		}
        #endregion
    }
}
