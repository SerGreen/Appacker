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
using System.Drawing.IconLib.Exceptions;

namespace System.Drawing.IconLib.EncodingFormats
{
    [Author("Franco, Gustavo")]
    internal class IconFormat : ILibraryFormat
    {
        #region Methods
        public bool IsRecognizedFormat(Stream stream)
        {
            stream.Position = 0;
            try
            {
                ICONDIR iconDir = new ICONDIR(stream);
                if (iconDir.idReserved != 0)
                    return false;

                if (iconDir.idType != 1)
                    return false;

                return true;

            }
            catch(Exception){}
            return false;
        }

        public unsafe MultiIcon Load(Stream stream)
        {
            stream.Position = 0;
            SingleIcon singleIcon = new SingleIcon("Untitled");
            ICONDIR iconDir = new ICONDIR(stream);
            if (iconDir.idReserved != 0)
                throw new InvalidMultiIconFileException();

            if (iconDir.idType != 1)
                throw new InvalidMultiIconFileException();

            int entryOffset = sizeof(ICONDIR);

            // Add Icon Images one by one to the new entry created
            for(int i=0; i<iconDir.idCount; i++)
            {
                stream.Seek(entryOffset, SeekOrigin.Begin);
                ICONDIRENTRY entry = new ICONDIRENTRY(stream);

                // If there is missing information in the header... lets try to calculate it
                entry = CheckAndRepairEntry(entry);

                stream.Seek(entry.dwImageOffset, SeekOrigin.Begin);

                singleIcon.Add(new IconImage(stream, (int) (stream.Length - stream.Position)));
                entryOffset += sizeof(ICONDIRENTRY);
            }

            return new MultiIcon(singleIcon);
        }

        public unsafe void Save(MultiIcon multiIcon, Stream stream)
        {
            if (multiIcon.SelectedIndex == -1)
                return;

            SingleIcon singleIcon = multiIcon[multiIcon.SelectedIndex];

            // ICONDIR header
            ICONDIR iconDir = ICONDIR.Initalizated;
            iconDir.idCount = (ushort) singleIcon.Count;
            iconDir.Write(stream);
            
            // ICONENTRIES
            int entryPos    = sizeof(ICONDIR);
            int imagesPos   = sizeof(ICONDIR) + iconDir.idCount * sizeof(ICONDIRENTRY);
            foreach(IconImage iconImage in singleIcon)
            {
                // for some formats We don't know the size until we write, 
                // so we have to write first the image then later the header
                
                // IconImage
                stream.Seek(imagesPos, SeekOrigin.Begin);
                iconImage.Write(stream);
                long bytesInRes = stream.Position - imagesPos;

                // IconDirHeader
                stream.Seek(entryPos, SeekOrigin.Begin);
                ICONDIRENTRY iconEntry  = iconImage.ICONDIRENTRY;
                stream.Seek(entryPos, SeekOrigin.Begin);
                iconEntry.dwImageOffset= (uint) imagesPos;
                iconEntry.dwBytesInRes = (uint) bytesInRes; 
                iconEntry.Write(stream);

                entryPos    += sizeof(ICONDIRENTRY);
                imagesPos   += (int) bytesInRes;
            }
        }
        #endregion

        #region Private Methods
        private static unsafe ICONDIRENTRY CheckAndRepairEntry(ICONDIRENTRY entry)
        {
            // If there is missing information in the header... lets try to calculate it
            if (entry.wBitCount == 0)
            {
                int stride, CLSSize, palette; 
                int bmpSize  = ((ushort) entry.dwBytesInRes - sizeof(BITMAPINFOHEADER));
                int BWStride = ((entry.bWidth * 1 + 31) & ~31) >> 3;
                int BWSize   = BWStride * entry.bHeight;
                bmpSize     -= BWSize;
                
                // Lets find the value;
                byte[] bpp = {1, 4, 8, 16, 24, 32};
                int j=0;
                while(j<=5)
                {
                    stride   = ((entry.bWidth * bpp[j] + 31) & ~31) >> 3;
                    CLSSize  = entry.bHeight * stride ;
                    palette  = bpp[j]<=8 ? ((int) (1 << bpp[j]) * 4) : 0;
                    if (palette + CLSSize == bmpSize)
                    {
                        entry.wBitCount = bpp[j];
                        break;
                    }
                    j++;
                }
            }

            if (entry.wBitCount < 8 && entry.bColorCount == 0)
                entry.bColorCount = (byte) (1 << entry.wBitCount);
            if (entry.wPlanes == 0) 
                entry.wPlanes = 1;

            return entry;
        }
        #endregion
    }
}
