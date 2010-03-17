using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ManagedReparsePoints
{
    class HG<T> : IDisposable where T : struct
    {
        public IntPtr Pointer
        {
            get
            {
                return pointer;
            }
            protected set
            {
                pointer = value;
            }
        }

        public T Structure
        {
            get
            {
                if (!disposed) return (T)Marshal.PtrToStructure(Pointer, typeof(T));
                else return value;
            }
        }

        public int SizeOf { get { return sizeOf; } }

        public HG(T obj)
            : this()
        {
            Marshal.StructureToPtr(obj, Pointer, true);
        }

        public HG()
        {
            sizeOf = Marshal.SizeOf(value);
            Pointer = Marshal.AllocHGlobal(sizeOf);
        }

        public void Dispose()
        {
            value = (T)Marshal.PtrToStructure(Pointer, typeof(T));
            Marshal.FreeHGlobal(Pointer);
            disposed = true;
        }

        private IntPtr pointer;
        private T value;
        private int sizeOf;
        private bool disposed = false;
    }
}
