using System;
using System.Collections.Generic;
using System.Text;

namespace ManagedReparsePoints
{
    public class ReparseDataBufferBuilder
    {
        public uint ReparseTag { get; set; }
        public string SubstituteName { get; set; }
        public string PrintName { get; set; }
        public ReparseDataBuffer Build()
        {
            ReparseDataBuffer buffer = new ReparseDataBuffer();
            Write(ref buffer);
            return buffer;
        }
        public void Write(ref ReparseDataBuffer buffer)
        {
            // set simple fields
            buffer.ReparseTag = ReparseTag;
            buffer.Reserved = 0;

            StringBuilder sb = new StringBuilder();
            sb.Append(SubstituteName);
            sb.Append('\0');
            sb.Append(PrintName);
            sb.Append('\0');

            // set subname
            buffer.SubstituteNameOffset = 0;
            buffer.SubstituteNameLength = (ushort)(SubstituteName != null ? SubstituteName.Length * sizeof(char) : 0);

            // set printname
            buffer.PrintNameOffset = (ushort)(buffer.SubstituteNameLength + 2);
            buffer.PrintNameLength = (ushort)(PrintName != null ? PrintName.Length * sizeof(char) : 0);

            // set length
            buffer.ReparseDataLength = (ushort)(sb.Length * sizeof(char) + 8);

            buffer.PathBuffer = sb.ToString();
        }
        public void Read(ref ReparseDataBuffer buffer)
        {
            ReparseTag = buffer.ReparseTag;
            SubstituteName = buffer.PathBuffer.Substring(buffer.SubstituteNameOffset, buffer.SubstituteNameLength / sizeof(char));
            PrintName = buffer.PathBuffer.Substring(buffer.PrintNameOffset, buffer.PrintNameLength / sizeof(char));
        }
    }
}
