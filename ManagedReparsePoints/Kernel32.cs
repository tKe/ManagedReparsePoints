using System.Runtime.InteropServices;
using System;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.Text;
namespace ManagedReparsePoints
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetLastError();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern int FormatMessage(int dwFlags,
            string lpSource,
            int dwMessageId,
            int dwLanguageId,
            StringBuilder lpBuffer,
            int nSize,
            string[] Arguments);

        internal static string GetLastErrorMessage()
        {
            StringBuilder strLastErrorMessage = new StringBuilder(255);
            int ret2 = GetLastError();
            int dwFlags = 4096;

            int ret3 = FormatMessage(dwFlags,
                null,
                ret2,
                0,
                strLastErrorMessage,
                strLastErrorMessage.Capacity,
                null);

            return String.Format("0x{0:X} : {1}", ret2, strLastErrorMessage);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(
              string lpFileName,
              uint dwDesiredAccess,
              uint dwShareMode,
              uint SecurityAttributes,
              uint dwCreationDisposition,
              uint dwFlagsAndAttributes,
              int hTemplateFile
              );

        [DllImport("kernel32.dll")]
        internal static extern SafeHandleImpl GetCurrentProcess();

        [DllImport("kernel32.dll")]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            EIOControlCode dwIoControlCode,
            IntPtr inBuffer,
            int nInBufferSize,
            IntPtr outBuffer,
            int nOutBufferSize,
            ref int pBytesReturned,
            IntPtr lpOverlapped
        );
    }

    internal static class Advapi32
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LookupPrivilegeValue(string lpSystemName, string lpName,
            out LUID lpLuid);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(SafeHandle TokenHandle,
            [MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            Int32 BufferLength,
            //ref TOKEN_PRIVILEGES PreviousState,					!! for some reason this won't accept null
            IntPtr PreviousState,
            IntPtr ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool OpenProcessToken(SafeHandle ProcessHandle,
            UInt32 DesiredAccess, out SafeHandleImpl TokenHandle);
    }
}