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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Drawing.IconLib.BitmapEncoders
{
    [Author("Franco, Gustavo")]
    internal abstract class ImageEncoder
    {
        #region Variables Declaration
        protected BITMAPINFOHEADER  mHeader;
        protected RGBQUAD[]         mColors;
        protected byte[]            mXOR;
        protected byte[]            mAND;
        #endregion

        #region Constructors
        protected ImageEncoder()
        {
        }
        #endregion

        #region Properties
        public unsafe virtual Icon Icon
        {
            get
            {
                MemoryStream ms = new MemoryStream();

                // ICONDIR
                ICONDIR iconDir = ICONDIR.Initalizated;
                iconDir.idCount = 1;
                iconDir.Write(ms);

                // ICONDIRENTRY 
                ICONDIRENTRY iconEntry = new ICONDIRENTRY();
                iconEntry.bColorCount   = (byte) mHeader.biClrUsed;
                iconEntry.bHeight       = (byte) (mHeader.biHeight / 2);
                iconEntry.bReserved     = 0;
                iconEntry.bWidth        = (byte) mHeader.biWidth;
                iconEntry.dwBytesInRes  = (uint) (sizeof(BITMAPINFOHEADER) + 
                                            sizeof(RGBQUAD) * ColorsInPalette + 
                                            mXOR.Length + mAND.Length);
                iconEntry.dwImageOffset = (uint) (sizeof(ICONDIR) + sizeof(ICONDIRENTRY));
                iconEntry.wBitCount     = mHeader.biBitCount;
                iconEntry.wPlanes       = mHeader.biPlanes;
                iconEntry.Write(ms);

                // Image Info Header
                ms.Seek(iconEntry.dwImageOffset, SeekOrigin.Begin);
                mHeader.Write(ms);

                // Image Palette
                byte[] buffer = new byte[sizeof(RGBQUAD) * ColorsInPalette];
                GCHandle handle = GCHandle.Alloc(mColors, GCHandleType.Pinned);
                Marshal.Copy(handle.AddrOfPinnedObject(), buffer, 0, buffer.Length);
                handle.Free();
                ms.Write(buffer, 0, buffer.Length);

                // Image XOR Image
                ms.Write(mXOR, 0, mXOR.Length);

                // Image AND Image
                ms.Write(mAND, 0, mAND.Length);

                // Rewind the stream
                ms.Position = 0;
                Icon icon = new Icon(ms, iconEntry.bWidth, iconEntry.bHeight);
                ms.Dispose();
                return icon;
            }
        }

        public virtual BITMAPINFOHEADER Header
        {
            get {return mHeader;}
            set {mHeader = value;}
        }

        public virtual RGBQUAD[] Colors
        {
            get {return mColors;}
            set {mColors = value;}
        }

        public virtual byte[] XOR
        {
            get {return mXOR;}
            set 
            {
                mHeader.biSizeImage = (uint) value.Length;
                mXOR = value;
            }
        }

        public virtual byte[] AND
        {
            get {return mAND;}
            set {mAND = value;}
        }

        public unsafe virtual int ColorsInPalette
        {
            get
            {
                return (int) (mHeader.biClrUsed != 0 ? 
                                    mHeader.biClrUsed : 
                                    mHeader.biBitCount <=8 ? 
                                        (uint) (1 << mHeader.biBitCount) : 0);
            }
        }

        public unsafe virtual int ImageSize
        {
            get{return sizeof(BITMAPINFOHEADER) + sizeof(RGBQUAD) * ColorsInPalette + mXOR.Length + mAND.Length;}
        }

        public abstract IconImageFormat IconImageFormat
        {
            get;
        }
        #endregion

        #region Abstract Methods
        public abstract void Read(Stream stream, int resourceSize);
        public abstract void Write(Stream stream);
        #endregion

        #region Methods
        public void CopyFrom(ImageEncoder encoder)
        {
            this.mHeader    = encoder.mHeader;
            this.mColors    = encoder.mColors;
            this.mXOR       = encoder.mXOR;
            this.mAND       = encoder.mAND;
        }
        #endregion
    }
}
