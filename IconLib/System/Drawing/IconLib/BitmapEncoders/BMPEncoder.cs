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
    internal class BMPEncoder : ImageEncoder
    {
        #region Constructors
        public BMPEncoder()
        {
        }
        #endregion

        #region Properties
        public override IconImageFormat IconImageFormat
        {
            get {return IconImageFormat.BMP;}
        }
        #endregion

        #region Methods
        public unsafe override void Read(Stream stream, int resourceSize)
        {
            // BitmapInfoHeader
            mHeader.Read(stream);

            // Palette
            mColors             = new RGBQUAD[ColorsInPalette];
            byte[] colorBuffer  = new byte[mColors.Length * sizeof(RGBQUAD)];
            stream.Read(colorBuffer, 0, colorBuffer.Length);
            GCHandle handle = GCHandle.Alloc(mColors, GCHandleType.Pinned);
            Marshal.Copy(colorBuffer, 0, handle.AddrOfPinnedObject(), colorBuffer.Length);
            handle.Free();

            // XOR Image
            int stride = (int) ((mHeader.biWidth * mHeader.biBitCount + 31) & ~31) >> 3;
            mXOR = new byte[stride * (mHeader.biHeight / 2)];
            stream.Read(mXOR, 0, mXOR.Length);

            // AND Image
            stride = (int) ((mHeader.biWidth * 1 + 31) & ~31) >> 3;
            mAND = new byte[stride * (mHeader.biHeight / 2)];
            stream.Read(mAND, 0, mAND.Length);
        }

        public unsafe override void Write(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);

            // BitmapInfoHeader
            mHeader.Write(stream);

            // Palette
            byte[] buffer = new byte[ColorsInPalette * sizeof(RGBQUAD)];
            GCHandle handle = GCHandle.Alloc(mColors, GCHandleType.Pinned);
            Marshal.Copy(handle.AddrOfPinnedObject(), buffer, 0, buffer.Length);
            handle.Free();
            stream.Write(buffer, 0, buffer.Length);

            // XOR Image
            stream.Write(mXOR, 0, mXOR.Length);

            // AND Image
            stream.Write(mAND, 0, mAND.Length);
        }
        #endregion
    }
}
