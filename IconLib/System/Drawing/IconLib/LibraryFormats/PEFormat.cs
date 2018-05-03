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
using System.Drawing.Imaging;
using System.Collections;

namespace System.Drawing.IconLib.EncodingFormats
{
    [Author("Franco, Gustavo")]
    internal class PEFormat : ILibraryFormat
    {
        #region Variables Declaration
        private static List<string> mIconsIDs;
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

                //Lets position over the "PE" header
                stream.Seek(dos_header.e_lfanew, SeekOrigin.Begin);

                //Lets read the NE header
                IMAGE_NT_HEADERS nt_header = new IMAGE_NT_HEADERS(stream);
                if (nt_header.Signature != (int) HeaderSignatures.IMAGE_NT_SIGNATURE) //PE
                    return false;

                return true;
            }
            catch(Exception){}
            return false;
        }

        public unsafe MultiIcon Load(Stream stream)
        {
            // LoadLibraryEx only can load files from File System, lets create a tmp file
            string tmpFile  = null;
            IntPtr hLib     = IntPtr.Zero;
            try
            {
                stream.Position = 0;

                // Find a tmp file where to dump the DLL stream, later we will remove this file
                tmpFile = Path.GetTempFileName();
                FileStream fs = new FileStream(tmpFile, FileMode.Create, FileAccess.Write);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();

                hLib = Win32.LoadLibraryEx(tmpFile, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
                if (hLib == IntPtr.Zero)
                    throw new InvalidFileException();

                List<string> iconsIDs;
                lock (typeof(PEFormat))
                {
                    mIconsIDs = new List<string>();
                    bool bResult = Win32.EnumResourceNames(hLib, (IntPtr) ResourceType.RT_GROUP_ICON, new Win32.EnumResNameProc(EnumResNameProc), IntPtr.Zero);
                    if (bResult == false)
                    {
                        // No Resources in this file
                    }
                    iconsIDs = new List<string>(mIconsIDs);
                }

                MultiIcon multiIcon = new MultiIcon();
                for(int index=0; index<iconsIDs.Count; index++)
                {
                    string id = iconsIDs[index];
                    IntPtr hRsrc = IntPtr.Zero;

                    if (Win32.IS_INTRESOURCE(id))
                        hRsrc = Win32.FindResource(hLib, int.Parse(id), (IntPtr) ResourceType.RT_GROUP_ICON);
                    else
                        hRsrc = Win32.FindResource(hLib, id, (IntPtr) ResourceType.RT_GROUP_ICON);

                    if (hRsrc == IntPtr.Zero)
                        throw new InvalidFileException();
                     
                    IntPtr hGlobal = Win32.LoadResource(hLib, hRsrc);
                    if (hGlobal == IntPtr.Zero)
                        throw new InvalidFileException();

                    MEMICONDIR* pDirectory = (MEMICONDIR*) Win32.LockResource(hGlobal);
                    if (pDirectory->wCount != 0)
                    {
                        MEMICONDIRENTRY* pEntry = &(pDirectory->arEntries);

                        SingleIcon singleIcon = new SingleIcon(id);
                        for(int i=0;i<pDirectory->wCount; i++)
                        {
                            IntPtr hIconInfo = Win32.FindResource(hLib, (IntPtr) pEntry[i].wId, (IntPtr) ResourceType.RT_ICON);
                            if (hIconInfo == IntPtr.Zero)
                                throw new InvalidFileException();

                            IntPtr hIconRes = Win32.LoadResource(hLib, hIconInfo);
                            if (hIconRes == IntPtr.Zero)
                                throw new InvalidFileException();

                            IntPtr dibBits = Win32.LockResource(hIconRes);
                            if (dibBits == IntPtr.Zero)
                                throw new InvalidFileException();

                            buffer = new byte[Win32.SizeofResource(hLib, hIconInfo)];
                            Marshal.Copy(dibBits, buffer, 0, buffer.Length);

                            MemoryStream ms = new MemoryStream(buffer);
                            IconImage iconImage = new IconImage(ms, buffer.Length);
                            singleIcon.Add(iconImage);
                        }
                        multiIcon.Add(singleIcon);
                    }
                }

                // If everything went well then lets return the multiIcon.
                return multiIcon;
            }
            catch(Exception)
            {
                throw new InvalidFileException();
            }
            finally
            {
                if (hLib != null)
                    Win32.FreeLibrary(hLib);
                if (tmpFile != null)
                    File.Delete(tmpFile);
            }
        }

        public unsafe void Save(MultiIcon multiIcon, Stream stream)
        {
            // LoadLibraryEx only can load files from File System, lets create a tmp file
            string tmpFile  = null;
            IntPtr hLib     = IntPtr.Zero;
            MemoryStream ms;
            bool bResult;
            try
            {
                stream.Position = 0;

                // Find a tmp file where to dump the DLL stream, later we will remove this file
                tmpFile = Path.GetTempFileName();

                FileStream fs = new FileStream(tmpFile, FileMode.Create, FileAccess.Write);
                byte[] buffer = Resource.EmptyDll;
                stream.Read(buffer, 0, buffer.Length);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();

				// Begin the injection process
			    IntPtr updPtr = Win32.BeginUpdateResource(tmpFile, false);
                if (updPtr == IntPtr.Zero)
                    throw new InvalidFileException();

                ushort iconIndex = 1;
                foreach(SingleIcon singleIcon in multiIcon)
                {
                    // Lets scan all groups
                    GRPICONDIR grpIconDir   = GRPICONDIR.Initalizated;
                    grpIconDir.idCount      = (ushort) singleIcon.Count;
                    grpIconDir.idEntries    = new GRPICONDIRENTRY[grpIconDir.idCount];

                    for(int i=0;i<singleIcon.Count; i++)
                    {
                        // Inside every Icon let update every image format
                        IconImage iconImage = singleIcon[i];
                        grpIconDir.idEntries[i]     = iconImage.GRPICONDIRENTRY;
                        grpIconDir.idEntries[i].nID = iconIndex;

                        // Buffer creation with the same size of the icon to optimize write call
                        ms = new MemoryStream((int) grpIconDir.idEntries[i].dwBytesInRes);
                        iconImage.Write(ms);
                        buffer = ms.GetBuffer();

                        // Update resource but it doesn't write to disk yet
                        bResult = Win32.UpdateResource(updPtr, (int) ResourceType.RT_ICON, iconIndex, 0,  buffer, (uint) ms.Length);
                        
                        iconIndex++;

                        // For some reason Windows will fail if there are many calls to update resource and no call to endUpdateResource
                        // It is like there some internal buffer that gets full, after that all calls fail.
                        // This workaround will save the changes every 70 icons, for big files this slow the saving process significantly
                        // but I didn't find a way to make EndUpdateResource works without save frequently
                        if ((iconIndex % 70) == 0)
                        {
                            bResult = Win32.EndUpdateResource(updPtr, false);
                            updPtr = Win32.BeginUpdateResource(tmpFile, false);
                            if (updPtr == IntPtr.Zero)
                                throw new InvalidFileException();
                        }
                    }

                    // Buffer creation with the same size of the group to optimize write call
                    ms = new MemoryStream(grpIconDir.GroupDirSize);
                    grpIconDir.Write(ms);
                    buffer = ms.GetBuffer();

                    int id;
                    if (int.TryParse(singleIcon.Name, out id))
                    {
                        // Write id as an integer
                        bResult = Win32.UpdateResource(updPtr, (int) ResourceType.RT_GROUP_ICON, (IntPtr) id, 0, buffer, (uint) ms.Length);
                    }
                    else
                    {
                        // Write id as string
                        IntPtr pName = Marshal.StringToHGlobalAnsi(singleIcon.Name.ToUpper());
                        bResult = Win32.UpdateResource(updPtr, (int) ResourceType.RT_GROUP_ICON, pName, 0, buffer, (uint) ms.Length);
                        Marshal.FreeHGlobal(pName);
                    }
                }
                
                // Last call to update the file with the rest not that was not write before
                bResult = Win32.EndUpdateResource(updPtr, false);

                // Because Windows Resource functions requiere a filepath, and we need to return an string then lets open
                // the temporary file and dump it to the stream received as parameter.
                fs = new FileStream(tmpFile, FileMode.Open, FileAccess.Read);
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                stream.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
            catch(Exception)
            {
                throw new InvalidFileException();
            }
            finally
            {
                if (hLib != null)
                    Win32.FreeLibrary(hLib);
                if (tmpFile != null)
                    File.Delete(tmpFile);
            }
        }
        #endregion

        #region Private Methods
        private static unsafe bool EnumResNameProc(IntPtr hModule, IntPtr pType, IntPtr pName, IntPtr param)
        {
            if (Win32.IS_INTRESOURCE(pName))
            {
                mIconsIDs.Add(pName.ToString()); 
            }
            else
            {
                mIconsIDs.Add(Marshal.PtrToStringUni(pName)); 
            }
            return true;
        }
        #endregion
    }
}
