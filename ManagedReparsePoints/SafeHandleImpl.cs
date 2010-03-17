using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace ManagedReparsePoints
{
    internal class SafeHandleImpl : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal static SafeHandleImpl Zero = new SafeHandleImpl();

        private SafeHandleImpl() : base(true) { }
        internal SafeHandleImpl(bool ownsHandle) : base(ownsHandle) { }

        protected override bool ReleaseHandle()
        {
            return Kernel32.CloseHandle(handle);
        }
    }
}
