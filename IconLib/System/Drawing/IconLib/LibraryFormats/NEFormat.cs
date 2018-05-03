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
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System.Drawing.IconLib.Exceptions;

namespace System.Drawing.IconLib.EncodingFormats
{
    [Author("Franco, Gustavo")]
    internal class NEFormat : ILibraryFormat
    {
        #region Constants Declaration
        private static byte[] MSDOS_STUB   = { 0xba, 0x10, 0x00, 0x0e, 0x1f, 0xb4, 0x09, 0xcd, 0x21, 0xb8, 0x01, 0x4c, 0xcd, 0x21, 0x90, 0x90,
                                        0x54, 0x68, 0x69, 0x73, 0x20, 0x70, 0x72, 0x6f, 0x67, 0x72, 0x61, 0x6d, 0x20, 0x6d, 0x75, 0x73,
                                        0x74, 0x20, 0x62, 0x65, 0x20, 0x72, 0x75, 0x6e, 0x20, 0x75, 0x6e, 0x64, 0x65, 0x72, 0x20, 0x4d,
                                        0x69, 0x63, 0x72, 0x6f, 0x73, 0x6f, 0x66, 0x74, 0x20, 0x57, 0x69, 0x6e, 0x64, 0x6f, 0x77, 0x73,
                                        0x2e, 0x0d, 0x0a, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
        // A factor of 9 allow to us to create a max icl file of (1 << 9) * 65536 = 32MB 
        // it is more than enough for a ICL library file.
        // but it will waste some space when entries or icons are smaller than 512 bytes, filling with 0 the empty spaces.
        // Don't change this value if you don't know what you are doing.
        // For example: a value of 4 will put entries and icons closer each other creating a smaller file (not much) 
        // but the limitation is that you can create a max icl file of (1 << 4) * 65536 = 1MB
        // bigger than 1MB you will get unknown results for a SHIFT_FACTOR = 4.
        private const byte SHIFT_FACTOR = 10;
        #endregion

        #region Methods
        public bool IsRecognizedFormat(Stream stream)
        {
            stream.Position = 0;
            try
            {
                // Has a valid MS-DOS header?
                IMAGE_DOS_HEADER dos_header = new IMAGE_DOS_HEADER(stream);
                if (dos_header.e_magic != (int) HeaderSignatures.IMAGE_DOS_SIGNATURE) //MZ
                    return false;

                //Lets position over the "NE" header
                stream.Seek(dos_header.e_lfanew, SeekOrigin.Begin);

                //Lets read the NE header
                IMAGE_OS2_HEADER os2_header = new IMAGE_OS2_HEADER(stream);
                if (os2_header.ne_magic != (int) HeaderSignatures.IMAGE_OS2_SIGNATURE) //NE
                    return false;

                return true;
            }
            catch(Exception){}
            return false;
        }

        public MultiIcon Load(Stream stream)
        {
            stream.Position = 0;
            //Lets read the MS DOS header
            IMAGE_DOS_HEADER dos_header = new IMAGE_DOS_HEADER(stream);
            if (dos_header.e_magic != (int) HeaderSignatures.IMAGE_DOS_SIGNATURE) //MZ
                throw new InvalidICLFileException();

            //Lets position over the "NE" header
            stream.Seek(dos_header.e_lfanew, SeekOrigin.Begin);

            //Lets read the NE header
            IMAGE_OS2_HEADER os2_header = new IMAGE_OS2_HEADER(stream);
            if (os2_header.ne_magic != (int) HeaderSignatures.IMAGE_OS2_SIGNATURE) //NE
                throw new InvalidICLFileException();

            //Lets position over the "Resource Table"
            stream.Seek(os2_header.ne_rsrctab + dos_header.e_lfanew, SeekOrigin.Begin);

            //Resource Table doesn't contain ICON resources
            if (os2_header.ne_restab == os2_header.ne_rsrctab)
                return new MultiIcon();

            //Lets read the Resource Table
            RESOURCE_TABLE                  resource_table  = new RESOURCE_TABLE(stream);
            Dictionary<ushort, IconImage>   icons           = resource_table.GetIcons(stream);
            List<GRPICONDIR>                groupIcons      = resource_table.GetGroupIcons(stream);
            List<string>                    groupNames      = new List<string>(resource_table.ResourceNames);
            if (groupNames[0].ToLower() == "icl")
                groupNames.RemoveAt(0);

            SingleIcon[] singleIcons = new SingleIcon[groupIcons.Count];
            for(int i=0; i<singleIcons.Length; i++)
            {
                if (i < groupNames.Count)
                    singleIcons[i] = new SingleIcon(groupNames[i]);
                else
                {
                    string freeName = FindFreeName(groupNames);
                    groupNames.Add(freeName);
                    singleIcons[i] = new SingleIcon(freeName);
                }

                foreach(GRPICONDIRENTRY iconEntry in groupIcons[i].idEntries)
                    singleIcons[i].Add(icons[iconEntry.nID]);
            }

            // If everything went well then lets create the multiIcon.
            return new MultiIcon(singleIcons);
        }

        public unsafe void Save(MultiIcon multiIcon, Stream stream)
        {
            //Lets prepare the complete file in memory, then we dump everything to a file
            IMAGE_DOS_HEADER    dos_header      = new IMAGE_DOS_HEADER();
            IMAGE_OS2_HEADER    os2_header      = new IMAGE_OS2_HEADER();
            RESOURCE_TABLE      resource_table  = new RESOURCE_TABLE();
            TYPEINFO            rscTypes_Group  = new TYPEINFO();
            TYPEINFO            rscTypes_Icon   = new TYPEINFO();
            TNAMEINFO[]         nameInfos_Group;
            TNAMEINFO[]         nameInfos_Icon;
            byte[]              resourceNames;
            List<GRPICONDIR>    groupIcons      = new List<GRPICONDIR>();
            Dictionary<ushort, IconImage> icons = new Dictionary<ushort,IconImage>();

            int offset = 0;

            // Lets set the MS DOS header
            dos_header.e_magic                  = (int) HeaderSignatures.IMAGE_DOS_SIGNATURE; // MZ
            dos_header.e_lfanew                 = 144; // NE Header location.
            dos_header.e_cblp                   = 80;
            dos_header.e_cp                     = 2;
            dos_header.e_cparhdr                = 4;
            dos_header.e_lfarlc                 = 64;
            dos_header.e_maxalloc               = 65535;
            dos_header.e_minalloc               = 15;
            dos_header.e_sp                     = 184;
            offset += (int) dos_header.e_lfanew;

            // Lets set the NE header
            os2_header.ne_magic                 = (int) HeaderSignatures.IMAGE_OS2_SIGNATURE; // NE
            os2_header.ne_ver                   = 71; 
            os2_header.ne_rev                   = 70; 
            os2_header.ne_enttab                = 178;
            os2_header.ne_cbenttab              = 10;
            os2_header.ne_crc                   = 0;
            os2_header.ne_flags                 = 33545;
            os2_header.ne_autodata              = 3;
            os2_header.ne_heap                  = 1024;
            os2_header.ne_stack                 = 0;
            os2_header.ne_csip                  = 65536;
            os2_header.ne_sssp                  = 0;
            os2_header.ne_cseg                  = 0;  // Entries in Segment Table
            os2_header.ne_cmod                  = 1;
            os2_header.ne_cbnrestab             = 26;
            os2_header.ne_segtab                = 64; // Offset to Segment Table
            os2_header.ne_rsrctab               = 64; // Offset to Resource Table
            os2_header.ne_restab                = 132;// Later will be overwriten
            os2_header.ne_modtab                = 168;
            os2_header.ne_imptab                = 170;
            os2_header.ne_nrestab               = 332;
            os2_header.ne_cmovent               = 1;
            os2_header.ne_align                 = SHIFT_FACTOR;
            os2_header.ne_cres                  = 0;
            os2_header.ne_exetyp                = 2; // OS target = Windows.
            os2_header.ne_flagsothers           = 0;
            os2_header.ne_pretthunks            = 0;
            os2_header.ne_psegrefbytes          = 0;
            os2_header.ne_swaparea              = 0;
            os2_header.ne_expver                = 768;  // OS version = 300
            offset += os2_header.ne_rsrctab;

            // Resoruce Table
            resource_table.rscAlignShift        = SHIFT_FACTOR; // 9 for now, lets split the entries every 512 bytes;
            offset += 2; // rscAlignShift

            // Type Info Groups
            rscTypes_Group.rtTypeID             = 0x8000 + (ushort) ResourceType.RT_GROUP_ICON;
            rscTypes_Group.rtResourceCount      = (ushort) multiIcon.Count;
            offset += 8; // rtTypeID + rtResourceCount + rtReserved
            nameInfos_Group                     = new TNAMEINFO[multiIcon.Count];
            offset += sizeof(TNAMEINFO) * multiIcon.Count;

            // Type Info Icons
            int iconCounter = 0;
            foreach(SingleIcon singleIcon in multiIcon)
                iconCounter += singleIcon.Count;
            rscTypes_Icon.rtTypeID              = 0x8000 + (ushort) ResourceType.RT_ICON;
            rscTypes_Icon.rtResourceCount       = (ushort) iconCounter;
            offset += 8; // rtTypeID + rtResourceCount + rtReserved
            nameInfos_Icon                      = new TNAMEINFO[iconCounter];
            offset += sizeof(TNAMEINFO) * iconCounter;

            resource_table.rscEndTypes = 0;
            offset += 2; // rscEndTypes

            // Resource Names
            os2_header.ne_restab = (ushort) (offset - dos_header.e_lfanew);
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write("ICL");
            foreach(SingleIcon singleIcon in multiIcon)
                bw.Write(singleIcon.Name);
            resourceNames = new byte[ms.Length];
            Array.Copy(ms.GetBuffer(), resourceNames, resourceNames.Length);
            ms.Dispose();
            offset += resourceNames.Length + 1; //resourceNames + rscEndNames

            // Here is the offset where we are going to start writting the directory
            int shiftOffset = (offset >> resource_table.rscAlignShift) + 1;

            // Name Infos Group
            int iconIndex = 0;
            for(int i=0; i<multiIcon.Count; i++)
            {
                SingleIcon singleIcon   = multiIcon[i];
                GRPICONDIR groupIconDir = new GRPICONDIR();
                groupIconDir.idCount    = (ushort) singleIcon.Count;
                groupIconDir.idType     = (ushort) ResourceType.RT_GROUP_ICON;

                // Name Infos Icons
                GRPICONDIRENTRY[] goupIconDirEntries = new GRPICONDIRENTRY[singleIcon.Count];
                for(int j=0; j<singleIcon.Count; j++)
                {
                    
                    nameInfos_Icon[iconIndex].rnFlags   = (ushort) (ResourceMemoryType.Moveable | ResourceMemoryType.Pure | ResourceMemoryType.Unknown);
                    nameInfos_Icon[iconIndex].rnHandle  = 0;
                    nameInfos_Icon[iconIndex].rnID      = (ushort) (0x8000 + iconIndex + 1);
                    nameInfos_Icon[iconIndex].rnUsage   = 0;
                    nameInfos_Icon[iconIndex].rnOffset  = (ushort) shiftOffset;
                    nameInfos_Icon[iconIndex].rnLength  = (ushort) Math.Ceiling(singleIcon[j].IconImageSize / (float) (1 << resource_table.rscAlignShift));
                    shiftOffset += nameInfos_Icon[iconIndex].rnLength;

                    goupIconDirEntries[j]       = singleIcon[j].GRPICONDIRENTRY;
                    goupIconDirEntries[j].nID   = (ushort) (iconIndex + 1);

                    icons.Add((ushort) (iconIndex + 1), singleIcon[j]);
                    iconIndex++;
                }

                nameInfos_Group[i].rnFlags   = (ushort) (ResourceMemoryType.Moveable | ResourceMemoryType.Pure | ResourceMemoryType.Unknown);
                nameInfos_Group[i].rnHandle  = 0;
                nameInfos_Group[i].rnID      = (ushort) (0x8000 + i + 1);
                nameInfos_Group[i].rnUsage   = 0;
                nameInfos_Group[i].rnOffset  = (ushort) shiftOffset;
                nameInfos_Group[i].rnLength  = (ushort) Math.Ceiling((6 + singleIcon.Count * sizeof(GRPICONDIRENTRY)) / (float) (1 << resource_table.rscAlignShift));
                groupIconDir.idEntries = goupIconDirEntries;
                groupIcons.Add(groupIconDir);

                shiftOffset += nameInfos_Group[i].rnLength;
            }

            resource_table.rscTypes                 = new TYPEINFO[2];
            resource_table.rscTypes[0]              = rscTypes_Group;
            resource_table.rscTypes[0].rtNameInfo   = nameInfos_Group;
            resource_table.rscTypes[1]              = rscTypes_Icon;
            resource_table.rscTypes[1].rtNameInfo   = nameInfos_Icon;
            resource_table.rscResourceNames = resourceNames;

            // HERE WE GO TO THE FS...

            // Lets write the MS DOS header
            dos_header.Write(stream);

            // Lets write first Bin segment
            stream.Write(MSDOS_STUB, 0, MSDOS_STUB.Length);

            // Lets position over where the "NE" header will be
            stream.Seek(dos_header.e_lfanew, SeekOrigin.Begin);

            // Lets write the NE header
            os2_header.Write(stream);

            // Lets position over the "Resource Table"
            stream.Seek(os2_header.ne_rsrctab + dos_header.e_lfanew, SeekOrigin.Begin);

            // Lets write the "Resource Table"
            resource_table.Write(stream);

            // Now write the Icons Directory
            resource_table.SetGroupIcons(stream, groupIcons);

            // And the Images...
            resource_table.SetIcons(stream, icons);
        }
        #endregion

        #region Private Static Methods
        private string FindFreeName(List<string> names)
        {
            int index = 1;
            while(true)
            {
                bool found = false;
                foreach(string name in names)
                {
                    if (name.ToLower() == "icon " + index)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return "Icon " + index;
                index++;
            }
        }
        #endregion
    }
}
