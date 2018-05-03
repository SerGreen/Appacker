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
using System.Drawing.Imaging;

namespace System.Drawing.IconLib.ColorProcessing
{
    [Author("Franco, Gustavo")]
    public class OctreeQuantizer : IPaletteQuantizer
    {
        #region Constructors
        public OctreeQuantizer()
        {
        }
        #endregion

        #region Methods
        public unsafe ColorPalette CreatePalette(Bitmap image, int maxColors, int bitsPerPixel)
        {
            int nPad;
            byte* pbBits;
            ushort* pwBits;
            uint rmask, gmask, bmask;
            int rright, gright, bright;
            int rleft, gleft, bleft;
            byte r, g, b;
            ushort wColor;
            uint dwColor;
            ColorPalette newPalette;
            Node tree;
            int  nLeafCount, nIndex;
            Node[] reducibleNodes = new Node[9];

            if (maxColors > Math.Pow(2, bitsPerPixel))
                throw new Exception("param maxColors out of range, maximum " + Math.Pow(2, bitsPerPixel) + " colors for " + bitsPerPixel + " bits");

            //// Initialize octree variables
            tree = null;
            nLeafCount = 0;
            if (bitsPerPixel > 8) // Just in case
                return null;
            for (int i=0; i<=(int) bitsPerPixel; i++)
                reducibleNodes[i] = null;

			BitmapData bitmapDataSource = image.LockBits(new Rectangle(0,0, image.Width, image.Height), ImageLockMode.ReadWrite, image.PixelFormat);
            
            try
            {
                //// Scan the DIB and build the octree
                nPad = bitmapDataSource.Stride - (((image.Width * Bitmap.GetPixelFormatSize(image.PixelFormat)) + 7) / 8);

                switch (Bitmap.GetPixelFormatSize(image.PixelFormat)) 
                {
                    case 16: // One case for 16-bit DIBs
                    {
                        rmask = 0x7C00;
                        gmask = 0x03E0;
                        bmask = 0x001F;

                        rright = GetRightShiftCount (rmask);
                        gright = GetRightShiftCount (gmask);
                        bright = GetRightShiftCount (bmask);

                        rleft = GetLeftShiftCount (rmask);
                        gleft = GetLeftShiftCount (gmask);
                        bleft = GetLeftShiftCount (bmask);

                        pwBits = (ushort*) bitmapDataSource.Scan0.ToPointer();
                        for (int i=0; i<image.Height; i++) 
                        {
                            for (int j=0; j<image.Width; j++) 
                            {
                                wColor = *pwBits++;
                                b = (byte) (((wColor & (ushort) bmask) >> bright) << bleft);
                                g = (byte) (((wColor & (ushort) gmask) >> gright) << gleft);
                                r = (byte) (((wColor & (ushort) rmask) >> rright) << rleft);
                                
                                AddColor (ref tree, r, g, b, bitsPerPixel, 0, ref nLeafCount, ref reducibleNodes);

                                while (nLeafCount > maxColors)
                                    ReduceTree (bitsPerPixel, ref nLeafCount, ref reducibleNodes);
                            }
                            pwBits = (ushort*) (((byte*) pwBits) + nPad);
                        }
                        break;
                    }
                    case 24: // Another for 24-bit DIBs
                    {
                        pbBits = (byte*) bitmapDataSource.Scan0.ToPointer();
                        for (int i=0; i<image.Height; i++) 
                        {
                            for (int j=0; j<image.Width; j++) 
                            {
                                b = *pbBits++;
                                g = *pbBits++;
                                r = *pbBits++;

                                AddColor (ref tree, r, g, b, bitsPerPixel, 0, ref nLeafCount, ref reducibleNodes);

                                while (nLeafCount > maxColors)
                                    ReduceTree (bitsPerPixel, ref nLeafCount, ref reducibleNodes);
                            }
                            pbBits += nPad;
                        }
                        break;
                    }
                    case 32: // And another for 32-bit DIBs
                    {
                        rmask = 0x00FF0000;
                        gmask = 0x0000FF00;
                        bmask = 0x000000FF;

                        rright = GetRightShiftCount (rmask);
                        gright = GetRightShiftCount (gmask);
                        bright = GetRightShiftCount (bmask);

                        uint* pdwBits = (uint*) bitmapDataSource.Scan0.ToPointer();
                        for (int i=0; i<image.Height; i++) 
                        {
                            for (int j=0; j<image.Width; j++) 
                            {
                                dwColor = *pdwBits++;
                                b = (byte) ((dwColor & bmask) >> bright);
                                g = (byte) ((dwColor & gmask) >> gright);
                                r = (byte) ((dwColor & rmask) >> rright);
                                
                                AddColor (ref tree, r, g, b, bitsPerPixel, 0, ref nLeafCount, ref reducibleNodes);

                                while (nLeafCount > maxColors)
                                    ReduceTree (bitsPerPixel, ref nLeafCount, ref reducibleNodes);
                            }
                            pdwBits = (uint*) (((byte*) pdwBits) + nPad);
                        }
                        break;
                    }
                    default: // Image must be 16, 24, or 32-bit
                        return null;
                }

                if (nLeafCount > maxColors) 
                { 
                    // Sanity check
                    tree = null;
                }

                Bitmap bmp = null;
                switch(bitsPerPixel)
                {
                    case 1:
                        bmp = new Bitmap(1,1, PixelFormat.Format1bppIndexed);
                        break;
                    case 4:
                        bmp = new Bitmap(1,1, PixelFormat.Format4bppIndexed);
                        break;
                    case 8:
                        bmp = new Bitmap(1,1, PixelFormat.Format8bppIndexed);
                        break;
                }
                newPalette = bmp.Palette;
                bmp.Dispose();

                nIndex = 0;
                GetPaletteColors (tree, ref newPalette, ref nIndex);
                
                // Fill the rest of the palette with 0s...
                Color[] entries = newPalette.Entries;
                for(int i=nIndex + 1; i<entries.Length; i++)
                    entries[i] = Color.FromArgb(0,0,0,0);
            }
            finally
            {
                if (bitmapDataSource != null)
		            image.UnlockBits(bitmapDataSource);
            }

            return newPalette;
        }

        private int GetRightShiftCount (uint dwVal)
        {
            int i;

            for (i=0; i<sizeof (uint) * 8; i++) 
            {
                if ((dwVal & 1) == 1)
                    return i;
                dwVal >>= 1;
            }
            return -1;
        }

        private int GetLeftShiftCount (uint dwVal)
        {
            int nCount, i;

            nCount = 0;
            for (i=0; i<sizeof (uint) * 8; i++) 
            {
                if ((dwVal & 1) == 1)
                    nCount++;
                dwVal >>= 1;
            }
            return (8 - nCount);
        }

        private void GetPaletteColors (Node tree, ref ColorPalette palEntries, ref int index)
        {
            int i;

            Color[] entries = palEntries.Entries;
            
            if (tree.bIsLeaf) 
            {
                entries[index] = Color.FromArgb(
                                    (byte) ((tree.nRedSum)  / (tree.nPixelCount)),
                                    (byte) ((tree.nGreenSum)/ (tree.nPixelCount)),
                                    (byte) ((tree.nBlueSum) / (tree.nPixelCount)));
                index++;
            }
            else 
            {
                for (i=0; i<8; i++) 
                {
                    if (tree.Child[i] != null)
                        GetPaletteColors (tree.Child[i], ref palEntries, ref index);
                }
            }
        }

        private void ReduceTree (int nColorBits, ref int leafCount, ref Node[] reducibleNodes)
        {
            int i;
            Node node;
            uint nRedSum, nGreenSum, nBlueSum;
            int nChildren;

            // Find the deepest level containing at least one reducible node
            for (i=nColorBits - 1; (i>0) && (reducibleNodes[i] == null); i--);

            // Reduce the node most recently added to the list at level i
            node = reducibleNodes[i];
            reducibleNodes[i] = node.Next;

            nRedSum = nGreenSum = nBlueSum = 0;
            nChildren = 0;
            for (i=0; i<8; i++) 
            {
                if (node.Child[i] != null) 
                {
                    nRedSum += node.Child[i].nRedSum;
                    nGreenSum += node.Child[i].nGreenSum;
                    nBlueSum += node.Child[i].nBlueSum;
                    node.nPixelCount += node.Child[i].nPixelCount;
                    node.Child[i] = null;
                    nChildren++;
                }
            }

            node.bIsLeaf = true;
            node.nRedSum = nRedSum;
            node.nGreenSum = nGreenSum;
            node.nBlueSum = nBlueSum;
            leafCount -= (nChildren - 1);
        }

        private void AddColor (ref Node node, byte r, byte g, byte b, int nColorBits, int nLevel, ref int leafCount, ref Node[] reducibleNodes)
        {
            int nIndex, shift;
            byte[] mask = new byte[8] { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

            // If the node doesn't exist, create it
            if (node == null)
                node = CreateNode (nLevel, nColorBits, ref leafCount, ref reducibleNodes);

            // Update color information if it's a leaf node
            if (node.bIsLeaf) 
            {
                node.nPixelCount++;
                node.nRedSum    += r;
                node.nGreenSum  += g;
                node.nBlueSum   += b;
            }
            // Recurse a level deeper if the node is not a leaf
            else 
            {
                shift = 7 - nLevel;
                nIndex = (((r & mask[nLevel]) >> shift) << 2) |
                    (((g & mask[nLevel]) >> shift) << 1) |
                    ((b & mask[nLevel]) >> shift);
                AddColor (ref node.Child[nIndex], r, g, b, nColorBits, nLevel + 1, ref leafCount, ref reducibleNodes);
            }
        }

        private Node CreateNode (int nLevel, int nColorBits, ref int leafCount, ref Node[] reducibleNodes)
        {
            Node newNode = new Node();

            newNode.bIsLeaf = (nLevel == nColorBits);
            if (newNode.bIsLeaf)
                leafCount++;
            else 
            { 
                // Add the node to the reducible list for this level
                newNode.Next = reducibleNodes[nLevel];
                reducibleNodes[nLevel] = newNode;
            }
            return newNode;
        }
        #endregion

        #region Inner Classes
        private class Node
        {
            public bool bIsLeaf;               // TRUE if node has no children
            public int nPixelCount;            // Number of pixels represented by this leaf
            public uint nRedSum;               // Sum of red components
            public uint nGreenSum;             // Sum of green components
            public uint nBlueSum;              // Sum of blue components
            public Node[] Child = new Node[8]; // Pointers to child nodes
            public Node Next;                  // Pointer to next reducible node
        }
        #endregion
    }
}
