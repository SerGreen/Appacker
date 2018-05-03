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
using System.Diagnostics;
using System.Collections;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Drawing.IconLib.ColorProcessing
{
    [Author("Franco, Gustavo")]
    public class FloydSteinbergDithering : IDithering
    {
        #region Variables Declaration
        private const float mOffset16 = 255F / 249F;
        #endregion

        #region Constructors
        public FloydSteinbergDithering()
        {
        }
        #endregion

        #region Methods
        public unsafe void Disperse(byte* pixelSource, int x, int y, byte bpp, int stride, int width, int height, Color colorEntry)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            uint argColor = 0;

            byte* pixelColorComponent;

            GetRGB(pixelSource, bpp, x, ref r, ref g, ref b, ref argColor);
            int errorR = ((int)r) - colorEntry.R; 
            int errorG = ((int)g) - colorEntry.G; 
            int errorB = ((int)b) - colorEntry.B; 

            switch (bpp) 
            {
                case 16:
                    ushort* pixelColorComponentS;
                    if (x + 1 < width) 
                    { 
                        // Right
                        pixelColorComponentS = (ushort*) ((pixelSource) + ((x + 1) * 2));
                        r = (byte) (((*pixelColorComponentS & 0x7C00) >> 7) * mOffset16);
                        g = (byte) (((*pixelColorComponentS & 0x03E0) >> 2) * mOffset16);
                        b = (byte) (((*pixelColorComponentS & 0x001F) << 3) * mOffset16);
                        r = (byte) ((int) ((Limits(r, ((int) (errorR * 7)) >> 4))) & 0xF8); 
                        g = (byte) ((int) ((Limits(g, ((int) (errorG * 7)) >> 4))) & 0xF8); 
                        b = (byte) ((int) ((Limits(b, ((int) (errorB * 7)) >> 4))) & 0xF8); 
                        *pixelColorComponentS = (ushort) (r << 7 | g << 2 | b >> 3);
                    } 
                    if (y + 1 < height) 
                    { 
                        if (x - 1 > 0) 
                        { 
                            // Left and Down
                            pixelColorComponentS = (ushort*) ((pixelSource) + ((x - 1) * 2) + stride);
                            r = (byte) (((*pixelColorComponentS & 0x7C00) >> 7) * mOffset16);
                            g = (byte) (((*pixelColorComponentS & 0x03E0) >> 2) * mOffset16);
                            b = (byte) (((*pixelColorComponentS & 0x001F) << 3) * mOffset16);
                            r = (byte) ((int) ((Limits(r, ((int) (errorR * 3)) >> 4))) & 0xF8); 
                            g = (byte) ((int) ((Limits(g, ((int) (errorG * 3)) >> 4))) & 0xF8); 
                            b = (byte) ((int) ((Limits(b, ((int) (errorB * 3)) >> 4))) & 0xF8); 
                            *pixelColorComponentS = (ushort) (r << 7 | g << 2 | b >> 3);
                        } 

                        // Down
                        pixelColorComponentS = (ushort*) ((pixelSource) + ((x + 0) * 2) + stride);
                        r = (byte) (((*pixelColorComponentS & 0x7C00) >> 7) * mOffset16);
                        g = (byte) (((*pixelColorComponentS & 0x03E0) >> 2) * mOffset16);
                        b = (byte) (((*pixelColorComponentS & 0x001F) << 3) * mOffset16);
                        r = (byte) ((int) ((Limits(r, ((int) (errorR * 5)) >> 4))) & 0xF8); 
                        g = (byte) ((int) ((Limits(g, ((int) (errorG * 5)) >> 4))) & 0xF8); 
                        b = (byte) ((int) ((Limits(b, ((int) (errorB * 5)) >> 4))) & 0xF8); 
                        *pixelColorComponentS = (ushort) (r << 7 | g << 2 | b >> 3);

                        if (x + 1 < width) 
                        {
                            // Right and Down
                            pixelColorComponentS = (ushort*) ((pixelSource) + ((x + 1) * 2) + stride);
                            r = (byte) (((*pixelColorComponentS & 0x7C00) >> 7) * mOffset16);
                            g = (byte) (((*pixelColorComponentS & 0x03E0) >> 2) * mOffset16);
                            b = (byte) (((*pixelColorComponentS & 0x001F) << 3) * mOffset16);
                            r = (byte) ((int) ((Limits(r, ((int) (errorR * 1)) >> 4))) & 0xF8); 
                            g = (byte) ((int) ((Limits(g, ((int) (errorG * 1)) >> 4))) & 0xF8); 
                            b = (byte) ((int) ((Limits(b, ((int) (errorB * 1)) >> 4))) & 0xF8); 
                            *pixelColorComponentS = (ushort) (r << 7 | g << 2 | b >> 3);
                        } 
                    }
                    break;
                case 24:
                    if (x + 1 < width) 
                    { 
                        // Right
                        pixelColorComponent = ((pixelSource) + ((x + 1) * 3));
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 7) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 7) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 7) >> 4); 
                    } 
                    if (y + 1 < height) 
                    { 
                        if (x - 1 > 0) 
                        { 
                            // Left and Down
                            pixelColorComponent = ((pixelSource) + ((x - 1) * 3) + stride);
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 3) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 3) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 3) >> 4); 
                        } 

                        // Down
                        pixelColorComponent = ((pixelSource) + ((x + 0) * 3) + stride);
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 5) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 5) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 5) >> 4); 

                        if (x + 1 < width) 
                        {
                            // Right and Down
                            pixelColorComponent = ((pixelSource) + ((x + 1) * 3) + stride);
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 1) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 1) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 1) >> 4); 
                        } 
                    }
                    break;
                case 32:
                    if (x + 1 < width) 
                    { 
                        // Right
                        pixelColorComponent = ((pixelSource) + ((x + 1) * 4)) + 0;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 7) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 7) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 7) >> 4); 
                    } 
                    if (y + 1 < height) 
                    { 
                        if (x - 1 > 0) 
                        { 
                            // Left and Down
                            pixelColorComponent = ((pixelSource) + ((x - 1) * 4) + stride);
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 3) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 3) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 3) >> 4); 
                        } 

                        // Down
                        pixelColorComponent = ((pixelSource) + ((x + 0) * 4) + stride);
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 5) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 5) >> 4); 
                        pixelColorComponent++;
                        *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 5) >> 4); 

                        if (x + 1 < width) 
                        {
                            // Right and Down
                            pixelColorComponent = ((pixelSource) + ((x + 1) * 4) + stride);
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorB * 1) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorG * 1) >> 4); 
                            pixelColorComponent++;
                            *pixelColorComponent = Limits(*(pixelColorComponent), (errorR * 1) >> 4); 
                        } 
                    }

                    break;
            }
        }

        private byte Limits(int a, int b)
        {
            return (a + b) < 0 ? (byte) 0 : (a + b) > 255 ? (byte) 255 : (byte) (a + b);
        }

        private unsafe void GetRGB(byte* firstStridePixel, byte bpp, int x, ref byte r, ref byte g, ref byte b, ref uint ARGBColor)
        {
            byte* pixelSourceBT;
            switch (bpp) 
            {
                case 16:
                    pixelSourceBT = firstStridePixel + x * 2;
                    r = (byte) ((*((ushort*) pixelSourceBT) & 0x7C00) >> 7);
                    g = (byte) ((*((ushort*) pixelSourceBT) & 0x03E0) >> 2);
                    b = (byte) ((*((ushort*) pixelSourceBT) & 0x001F) << 3);
                    ARGBColor = *((ushort*) (pixelSourceBT));
                    break;
                case 24:
                    pixelSourceBT = firstStridePixel + x * 3;
                    r = *((byte*) pixelSourceBT + 2);
                    g = *((byte*) pixelSourceBT + 1);
                    b = *((byte*) pixelSourceBT + 0);
                    ARGBColor = (uint) (r << 16 | g << 8 | b);
                    break;
                case 32:
                    pixelSourceBT = firstStridePixel + x * 4;
                    r = *((byte*) pixelSourceBT + 2);
                    g = *((byte*) pixelSourceBT + 1);
                    b = *((byte*) pixelSourceBT + 0);
                    ARGBColor = *((uint*) (pixelSourceBT));
                    break;
            }
        }
        #endregion
    }
}
