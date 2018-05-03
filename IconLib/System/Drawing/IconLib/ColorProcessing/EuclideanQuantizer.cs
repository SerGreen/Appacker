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
using System.Drawing.IconLib;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace System.Drawing.IconLib.ColorProcessing
{
    [Author("Franco, Gustavo")]
    public class EuclideanQuantizer : IColorQuantizer
    {
        #region Variables Declaration
        private IPaletteQuantizer   mQuantizer = null;
        private IDithering          mDithering = null;
        private Dictionary<uint, byte> mColorMap; 
        #endregion

        #region Constructors
        public EuclideanQuantizer() : this(new OctreeQuantizer(), new FloydSteinbergDithering())
        {
        }

        public EuclideanQuantizer(IPaletteQuantizer quantizer, IDithering dithering)
        {
            if (quantizer == null)
                throw new Exception("param 'quantizer' cannot be null");

            mQuantizer  = quantizer;
            mDithering  = dithering;
        }
        #endregion

        #region Methods
        public unsafe Bitmap Convert(Bitmap source, PixelFormat outputFormat)
        {
            DateTime dt1 = DateTime.Now;

            if ((outputFormat & PixelFormat.Indexed) != PixelFormat.Indexed)
                throw new Exception("Output format must be one of the indexed formats");

            Bitmap bmpTrg = new Bitmap(source.Width, source.Height, outputFormat);

            //Hashtable to chache the mapped colors.
            mColorMap = new Dictionary<uint, byte>();

            ColorPalette newPalette;
            switch(outputFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    Bitmap bmp = new Bitmap(1,1, PixelFormat.Format1bppIndexed);
                    newPalette = bmp.Palette; 
                    bmp.Dispose();

			        newPalette.Entries[0] = Color.FromArgb(255,0,0,0); 
			        newPalette.Entries[1] = Color.FromArgb(255,255,255,255); 
                    break;
                case PixelFormat.Format4bppIndexed:
                    newPalette = mQuantizer.CreatePalette(source, 16, 4);                
                    break;
                case PixelFormat.Format8bppIndexed:
                    newPalette = mQuantizer.CreatePalette(source, 256, 8);                
                    break;
                default:
                    throw new Exception("Indexed format not supported");
            }

            DateTime dt2 = DateTime.Now;

            // Pointers to the source and target bitmaps
			BitmapData bitmapDataSource = source.LockBits(new Rectangle(0,0, source.Width, source.Height), ImageLockMode.ReadWrite, source.PixelFormat);
			BitmapData bitmapDataTarget = bmpTrg.LockBits(new Rectangle(0,0, bmpTrg.Width, bmpTrg.Height), ImageLockMode.WriteOnly, bmpTrg.PixelFormat);

            try
            {
			    uint* pixelSource	= (uint*)bitmapDataSource.Scan0.ToPointer();
			    byte* pixelSourceB;
                byte* pixelTarget   = (byte*)bitmapDataTarget.Scan0.ToPointer();
                byte* pixelTargetB;
                int Width           = source.Width;
                int Height          = source.Height;
                byte bpp            = (byte) Bitmap.GetPixelFormatSize(source.PixelFormat);
                byte r              = 0;
                byte g              = 0;
                byte b              = 0;
                uint colorMatch     = 0;

                bmpTrg.Palette = newPalette;

                DateTime dt3 = DateTime.Now;
                for(int y = 0; y < Height; y++) 
                {
                    pixelSourceB = ((byte*) pixelSource) + (y * bitmapDataSource.Stride);
                    pixelTargetB = pixelTarget + (y * bitmapDataTarget.Stride);

                    for(int x = 0; x < Width; x++) 
                    {
                        GetRGB(pixelSourceB, bpp, x, ref r, ref g, ref b, ref colorMatch);
 
                        // To get better performace after find a near color, lets put on a hashtable
                        // if the color is found in the hash table, we just get it form there
                        byte index = 0;
                        if (!(mColorMap.TryGetValue(colorMatch, out index)))
                        {
                            index = (byte) FindNearestColor(r, g, b, newPalette.Entries);
                            mColorMap.Add(colorMatch, index);
                        }

                        // Lets assign the index found in the palette to the final pixel.
                        switch(outputFormat)
                        {
                            case PixelFormat.Format1bppIndexed:
					            byte mask = (byte) (0x80 >> ((x - 1) & 0x7)); 
					            if (index == 1) 
						            *(pixelTargetB) |= mask; 
					            else 
						            *(pixelTargetB) &= (byte)(mask^0xff);

                                pixelTargetB+= (x % 8) == 0 && x != 0 ? 1 : 0;
                                break;
                            case PixelFormat.Format4bppIndexed:
                                *(pixelTargetB) |= (byte) (index << (((x - 1) & 1) << 2));
                                pixelTargetB+= (x & 1);
                                break;
                            case PixelFormat.Format8bppIndexed:
                                *(pixelTargetB) = index;
                                pixelTargetB++;
                                break;
                        }

                        //Dithering
                        if (mDithering != null)
                            mDithering.Disperse(pixelSourceB, x, y, bpp, bitmapDataSource.Stride, Width, Height, newPalette.Entries[index]);
                    }
                }
                DateTime dt4 = DateTime.Now;

                TimeSpan ts = dt4.Subtract(dt3);
                Debug.WriteLine(ts.TotalMilliseconds);
                ts = dt3.Subtract(dt2);
                Debug.WriteLine(ts.TotalMilliseconds);
                ts = dt2.Subtract(dt1);
                Debug.WriteLine(ts.TotalMilliseconds);
                //MessageBox.Show(ts.TotalMilliseconds.ToString());
            }
            finally
            {
                if (source != null)
			        source.UnlockBits(bitmapDataSource);

                if (bmpTrg != null)
                    bmpTrg.UnlockBits(bitmapDataTarget);
            }
         
            return bmpTrg;
        }

        private int FindNearestColor(byte R, byte G, byte B, Color[] paletteEntries) 
        {
            int distanceSquared;
            int minDistanceSquared = 195076; // 255 * 255 + 255 * 255 + 255 * 255 + 1 
            int bestIndex = 0;

            int Rdiff;
            int Gdiff;
            int Bdiff;

            for (int i=0; i<paletteEntries.Length; i++) 
            {
                Rdiff = R - paletteEntries[i].R;
                Gdiff = G - paletteEntries[i].G;
                Bdiff = B - paletteEntries[i].B;

                distanceSquared = Rdiff * Rdiff + Gdiff * Gdiff + Bdiff * Bdiff;

                if (distanceSquared < minDistanceSquared) 
                {
                    minDistanceSquared = distanceSquared;
                    bestIndex = i;
                }
            }

            return bestIndex;
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
