using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace ManagedReparsePoints
{
    public static class ReparseExtensionMethods
    {
        public static bool IsEmpty(this DirectoryInfo dir) { return IsEmpty(dir, false); }
        public static bool IsEmpty(this DirectoryInfo dir, bool ignoreReparsePoints)
        {
            foreach (DirectoryInfo subDir in dir.GetDirectories())
                if ((subDir.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    return false;

            foreach (FileInfo file in dir.GetFiles())
                if ((file.Attributes & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                    return false;

            return true;
        }

        public static SafeFileHandle GetHandle(this FileSystemInfo dir)
        {
            bool success;
            SafeHandleImpl token;
            TOKEN_PRIVILEGES tokenPrivileges = new TOKEN_PRIVILEGES();
            tokenPrivileges.Privileges = new LUID_AND_ATTRIBUTES[1];
            SafeHandle processHandle = Kernel32.GetCurrentProcess();
            success = Advapi32.OpenProcessToken(
                processHandle,
                Constants.TOKEN_ADJUST_PRIVILEGES,
                out token);

            if (success)
            {
                // null for local system

                success = Advapi32.LookupPrivilegeValue(null, Constants.SE_BACKUP_NAME,
                                               out tokenPrivileges.Privileges[0].Luid);
                if (success)
                {
                    tokenPrivileges.PrivilegeCount = 1;
                    tokenPrivileges.Privileges[0].Attributes = Constants.SE_PRIVILEGE_ENABLED;
                    success = Advapi32.AdjustTokenPrivileges(
                        token,
                        false,
                        ref tokenPrivileges,
                        Marshal.SizeOf(tokenPrivileges),
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
                token.Close();
            }

            SafeFileHandle h = Kernel32.CreateFile(
                dir.FullName,
                Constants.GENERIC_WRITE,
                Constants.FILE_SHARE_READ | Constants.FILE_SHARE_WRITE,
                0,
                (uint)FileMode.Open,
                Constants.FILE_FLAG_BACKUP_SEMANTICS | Constants.FILE_FLAG_OPEN_REPARSE_POINT,
                (int)IntPtr.Zero
                );
            return h; 
        }

        public static bool GetReparseInformation(this FileSystemInfo dir, out ReparseDataBuffer buffer)
        {
            using (SafeFileHandle hFile = dir.GetHandle())
            {
                using (HG<ReparseDataBuffer> hg = new HG<ReparseDataBuffer>())
                {
                    int bytesReturned = 0;
                    bool rc = Kernel32.DeviceIoControl(
                        hFile,
                        EIOControlCode.FsctlGetReparsePoint,
                        IntPtr.Zero,
                        0,
                        hg.Pointer,
                        hg.SizeOf,
                        ref bytesReturned,
                        IntPtr.Zero);
                    buffer = hg.Structure;
                    return rc;
                }
            }
        }

        public static bool SetReparseInformation(this FileSystemInfo dir, ReparseDataBuffer buffer)
        {
            using (SafeFileHandle hFile = dir.GetHandle())
            {
                using (HG<ReparseDataBuffer> hg = new HG<ReparseDataBuffer>(buffer))
                {
                    int bytesReturned = 0;
                    bool rc = Kernel32.DeviceIoControl(
                        hFile,
                        EIOControlCode.FsctlSetReparsePoint,
                        hg.Pointer,
                        (int)(buffer.ReparseDataLength + Constants.REPARSE_MOUNTPOINT_HEADER_SIZE),
                        IntPtr.Zero,
                        0,
                        ref bytesReturned,
                        IntPtr.Zero);
                    return rc;
                }
            }
        }

        public static bool DeleteReparseInformation(this FileSystemInfo dir, uint reparseTag)
        {
            using (SafeFileHandle hFile = dir.GetHandle())
            {
                ReparseDataBuffer buffer;
                dir.GetReparseInformation(out buffer);
                buffer.ReparseDataLength = 0;
                using (HG<ReparseDataBuffer> hg = new HG<ReparseDataBuffer>(buffer))
                {
                    int bytesReturned = 0;
                    bool rc = Kernel32.DeviceIoControl(
                        hFile,
                        EIOControlCode.FsctlDeleteReparsePoint,
                        hg.Pointer,
                        (ushort)Constants.REPARSE_MOUNTPOINT_HEADER_SIZE,
                        IntPtr.Zero,
                        0,
                        ref bytesReturned,
                        IntPtr.Zero);
                    return rc;
                }
            }
        }

        //public static string SubstituteName(this ReparseDataBuffer buffer)
        //{
        //    return Encoding.Unicode.GetString(buffer.PathBuffer, buffer.SubstituteNameOffset, buffer.SubstituteNameLength);
        //}

        //public static string PrintName(this ReparseDataBuffer buffer)
        //{
        //    return Encoding.Unicode.GetString(buffer.PathBuffer, buffer.PrintNameOffset, buffer.PrintNameLength);
        //}
    }
}
