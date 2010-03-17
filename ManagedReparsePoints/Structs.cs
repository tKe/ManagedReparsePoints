using System.Runtime.InteropServices;
using System;
namespace ManagedReparsePoints
{
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct ReparseDataBuffer
    {
        public uint ReparseTag;
        public ushort ReparseDataLength;
        public ushort Reserved;
        public ushort SubstituteNameOffset;
        public ushort SubstituteNameLength;
        public ushort PrintNameOffset;
        public ushort PrintNameLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x3FF0/sizeof(char))]
        public string PathBuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LUID
    {
        public UInt32 LowPart;
        public Int32 HighPart;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LUID_AND_ATTRIBUTES
    {
        public LUID Luid;
        public UInt32 Attributes;
    }

    struct TOKEN_PRIVILEGES
    {
        public UInt32 PrivilegeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]		// !! think we only need one
        public LUID_AND_ATTRIBUTES[] Privileges;
    }
}