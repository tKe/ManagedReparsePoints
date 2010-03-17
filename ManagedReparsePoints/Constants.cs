namespace ManagedReparsePoints
{
    internal static class Constants
    {
        // Our maximum reparse point name size
        internal const int MAX_NAME_LENGTH = 1024;
        internal const int MAX_PATH = 260;

        // Maximum reparse buffer info size. The max user defined reparse
        // data is 16KB, plus there's a header.
        internal const int MAX_REPARSE_SIZE = 17000;

        // winnt.h
        internal const uint IO_REPARSE_TAG_SYMBOLIC_LINK = (0);
        internal const uint IO_REPARSE_TAG_MOUNT_POINT = (0xA0000003);
        internal const uint IO_REPARSE_TAG_HSM = (0xC0000004);
        internal const uint IO_REPARSE_TAG_SIS = (0x80000007);
        internal const uint IO_REPARSE_TAG_DFS = (0x8000000A);

        internal const uint REPARSE_MOUNTPOINT_HEADER_SIZE = 8;

        // from WinIoCtl.h
        internal const uint FSCTL_SET_REPARSE_POINT = 589988;
        internal const uint FSCTL_GET_REPARSE_POINT = 589992;
        internal const uint FSCTL_DELETE_REPARSE_POINT = 0x000900ac;

        // from winbase.h
        internal const int INVALID_HANDLE_VALUE = -1;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const uint GENERIC_EXECUTE = 0x20000000;
        internal const uint GENERIC_ALL = 0x10000000;

        //Sharing
        internal const uint FILE_SHARE_NONE = 0x00000000;
        internal const uint FILE_SHARE_READ = 0x00000001;
        internal const uint FILE_SHARE_WRITE = 0x00000002;
        internal const uint FILE_SHARE_DELETE = 0x00000004;

        internal const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        internal const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;

        //Create
        internal const uint CREATE_NEW = 1;
        internal const uint CREATE_ALWAYS = 2;
        internal const uint OPEN_EXISTING = 3;
        internal const uint OPEN_ALWAYS = 4;
        internal const uint TRUNCATE_EXISTING = 5;

        //Reparse
        internal const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
        internal const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

        //Others...
        internal const uint IO_REPARSE_TAG_SYMLINK = 0xA000000C;			// SYMLINK or SYMLINKD (see http://wesnerm.blogs.com/net_undocumented/2006/10/index.html)
        internal const uint SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const string SE_BACKUP_NAME = "SeBackupPrivilege";
        internal const uint FILE_DEVICE_FILE_SYSTEM = 9;
        internal const uint FILE_ANY_ACCESS = 0;
        internal const uint METHOD_BUFFERED = 0;
        internal const int MAXIMUM_REPARSE_DATA_BUFFER_SIZE = 16 * 1024;
        internal const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;

        //Calculated
        internal const uint CALC_SET_REPARSE =
            (FILE_DEVICE_FILE_SYSTEM << 16)
          | (FILE_ANY_ACCESS << 14)
          | (FSCTL_SET_REPARSE_POINT << 2)
          | METHOD_BUFFERED;

        internal const uint CALC_GET_REPARSE =
            (FILE_DEVICE_FILE_SYSTEM << 16)
          | (FILE_ANY_ACCESS << 14)
          | (FSCTL_GET_REPARSE_POINT << 2)
          | METHOD_BUFFERED;
    }
}