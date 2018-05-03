using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace ChangeIcon
{
    public class IconInjector
    {

        [DllImport("kernel32.dll", SetLastError = true)]

        static extern int UpdateResource(IntPtr hUpdate, uint lpType, uint lpName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr BeginUpdateResource(string pFileName,
            [MarshalAs(UnmanagedType.Bool)]bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        public static void InjectIcon(string execFileName, string iconFileName)
        {
            InjectIcon(execFileName, iconFileName, 1, 1);
        }
        static void InjectIcon(string execFileName, string iconFileName, uint iconGroupID, uint iconBaseID)
        {
            const uint RT_ICON = 3;
            const uint RT_GROUP_ICON = 14;


            IconFile iconFile = new IconFile();
            iconFile.Load(iconFileName);

            IntPtr hUpdate = BeginUpdateResource(execFileName, false);
            Debug.Assert(hUpdate != IntPtr.Zero);


            byte[] data = iconFile.CreateIconGroupData(iconBaseID);
            UpdateResource(hUpdate, RT_GROUP_ICON, iconGroupID, 0, data, (uint)data.Length);


            for (int i = 0; i < iconFile.GetImageCount(); i++)
            {
                byte[] image = iconFile.GetImageData(i);
                UpdateResource(hUpdate, RT_ICON, (uint)(iconBaseID + i), 0, image, (uint)image.Length);
            }


            EndUpdateResource(hUpdate, false);

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ICONDIR
    {
        public ushort idReserved;
        public ushort idType;
        public ushort idCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ICONDIRENTRY
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColorCount;
        public byte bReserved;
        public ushort wPlanes;
        public ushort wBitCount;
        public uint dwBytesInRes;
        public uint dwImageOffset;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct GRPICONDIRENTRY
    {
        public byte bWidth;
        public byte bHeight;
        public byte bColorCount;
        public byte bReserved;
        public ushort wPlanes;
        public ushort wBitCount;
        public uint dwBytesInRes;
        public ushort nID;
    }



    public class IconFile
    {
        ICONDIR _iconDir = new ICONDIR();
        ArrayList _iconEntry = new ArrayList();
        ArrayList _iconImage = new ArrayList();

        public IconFile()
        {
        }


        public int GetImageCount()
        {
            return _iconDir.idCount;
        }

        public byte[] GetImageData(int index)
        {
            Debug.Assert(0 <= index && index < GetImageCount());
            return (byte[])_iconImage[index];
        }


        public unsafe void Load(string fileName)
        {
            FileStream fs = null;
            BinaryReader br = null;
            byte[] buffer = null;

            try
            {

                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);


                buffer = br.ReadBytes(sizeof(ICONDIR));
                fixed (ICONDIR* ptr = &_iconDir)
                {
                    Marshal.Copy(buffer, 0, (IntPtr)ptr, sizeof(ICONDIR));
                }


                Debug.Assert(_iconDir.idReserved == 0);
                Debug.Assert(_iconDir.idType == 1);
                Debug.Assert(_iconDir.idCount > 0);



                for (int i = 0; i < _iconDir.idCount; i++)
                {
                    ICONDIRENTRY entry = new ICONDIRENTRY();
                    buffer = br.ReadBytes(sizeof(ICONDIRENTRY));
                    ICONDIRENTRY* ptr = &entry;
                    {
                        Marshal.Copy(buffer, 0, (IntPtr)ptr, sizeof(ICONDIRENTRY));
                    }

                    _iconEntry.Add(entry);
                }


                for (int i = 0; i < _iconDir.idCount; i++)
                {
                    fs.Position = ((ICONDIRENTRY)_iconEntry[i]).dwImageOffset;
                    byte[] img = br.ReadBytes((int)((ICONDIRENTRY)_iconEntry[i]).dwBytesInRes);
                    _iconImage.Add(img);
                }

                byte[] b = (byte[])_iconImage[0];

            }
            catch (Exception ex)
            {
                Debug.Assert(false);
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }


        unsafe int SizeOfIconGroupData()
        {
            return sizeof(ICONDIR) + sizeof(GRPICONDIRENTRY) * GetImageCount();
        }


        public unsafe byte[] CreateIconGroupData(uint nBaseID)
        {

            byte[] data = new byte[SizeOfIconGroupData()];


            fixed (ICONDIR* ptr = &_iconDir)
            {
                Marshal.Copy((IntPtr)ptr, data, 0, sizeof(ICONDIR));
            }

            int offset = sizeof(ICONDIR);

            for (int i = 0; i < GetImageCount(); i++)
            {
                GRPICONDIRENTRY grpEntry = new GRPICONDIRENTRY();
                BITMAPINFOHEADER bitmapheader = new BITMAPINFOHEADER();


                BITMAPINFOHEADER* ptr = &bitmapheader;
                {
                    Marshal.Copy(GetImageData(i), 0, (IntPtr)ptr, sizeof(BITMAPINFOHEADER));
                }


                grpEntry.bWidth = ((ICONDIRENTRY)_iconEntry[i]).bWidth;
                grpEntry.bHeight = ((ICONDIRENTRY)_iconEntry[i]).bHeight;
                grpEntry.bColorCount = ((ICONDIRENTRY)_iconEntry[i]).bColorCount;
                grpEntry.bReserved = ((ICONDIRENTRY)_iconEntry[i]).bReserved;
                grpEntry.wPlanes = bitmapheader.biPlanes;
                grpEntry.wBitCount = bitmapheader.biBitCount;
                grpEntry.dwBytesInRes = ((ICONDIRENTRY)_iconEntry[i]).dwBytesInRes;
                grpEntry.nID = (ushort)(nBaseID + i);


                GRPICONDIRENTRY* ptr2 = &grpEntry;
                {
                    Marshal.Copy((IntPtr)ptr2, data, offset, Marshal.SizeOf(grpEntry));
                }

                offset += sizeof(GRPICONDIRENTRY);
            }

            return data;
        }

    }
}