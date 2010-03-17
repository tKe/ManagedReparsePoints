using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ManagedReparsePoints.TestApp
{
    class Program
    {
        static FileSystemAccessRule rule = new FileSystemAccessRule(
            "Everyone",
            FileSystemRights.Write | FileSystemRights.Delete | FileSystemRights.DeleteSubdirectoriesAndFiles,
            InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
            PropagationFlags.None,
            AccessControlType.Deny
            );

        static int Main(string[] args)
        {
            List<DirectoryInfo> srcs = new List<DirectoryInfo>();
            DirectoryInfo root = null;
            Regex filter = null;
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                if (arg == "-r")
                    filter = new Regex(args[++i]);
                else if (root == null)
                    root = new DirectoryInfo(arg);
                else if (Directory.Exists(arg))
                    srcs.Add(new DirectoryInfo(arg));
                else
                {
                    Console.WriteLine("Source Directory '{0}' does not exist", arg);
                    return -1;
                }
            }

            if (root == null && srcs.Count < 1)
            {
                Console.WriteLine("Not enough arguments");
                return -1;
            }

            if (root.Exists && root.IsEmpty())
            {
                Console.WriteLine("Removing Security");
                RemoveDeleteACL(root);
                Console.Write("Cleaning Directory");
                DirectoryInfo[] subDirs = root.GetDirectories();
                int lastperc = 0, dc = 0, mx = subDirs.Length, perc = 0;
                foreach (DirectoryInfo subDir in subDirs)
                {
                    if ((subDir.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                        if (subDir.DeleteReparseInformation(0xA0000003))
                            subDir.Delete();
                        else
                        {
                            Console.WriteLine("Failed to delete reparse point information for {0}", subDir.FullName);
                            return -1;
                        }
                    DoPerc(ref lastperc, ref dc, mx, ref perc);
                }
            }
            else root.Create();

            ReparseDataBufferBuilder builder = new ReparseDataBufferBuilder();
            builder.ReparseTag = 0xA0000003;

            foreach (DirectoryInfo src in srcs)
            {
                DirectoryInfo[] subDirs = src.GetDirectories();
                int lastperc = 0, dc = 0, mx = subDirs.Length, perc = 0;
                Console.Write("Processing '{0}'", src.FullName);
                foreach (DirectoryInfo subDir in subDirs)
                {
                    if (filter != null && filter.IsMatch(subDir.Name))
                    {
                        string path = Path.Combine(root.FullName, subDir.Name);
                        int c = 0;
                        while (Directory.Exists(path))
                            path = Path.Combine(root.FullName, subDir.Name + "(" + (++c) + ")");
                        builder.SubstituteName = @"\??\" + subDir.FullName;
                        DirectoryInfo dest = new DirectoryInfo(path);
                        dest.Create();
                        if (!dest.SetReparseInformation(builder.Build()))
                        {
                            Console.WriteLine("Failed for {0} -> {1}", path, subDir.FullName);
                            return -1;
                        }
                    }
                    DoPerc(ref lastperc, ref dc, mx, ref perc);
                }
            }

            Console.WriteLine("Applying Security");
            AddDeleteACL(root);
            Console.WriteLine("Finished");
            return 0;
        }

        private static void DoPerc(ref int lastperc, ref int dc, int mx, ref int perc)
        {
            ++dc;
            perc = (dc * 100) / mx;
            if (perc > lastperc)
            {
                Console.CursorLeft = Console.WindowWidth - 7;
                if (perc == 100)
                    Console.WriteLine("[Done]");
                else
                    Console.Write("[{0,3}%]", perc);
                lastperc = perc;
            }
        }

        private static void RemoveDeleteACL(DirectoryInfo root)
        {
            DirectorySecurity ds = Directory.GetAccessControl(root.FullName);
            ds.RemoveAccessRule(rule);
            Directory.SetAccessControl(root.FullName, ds);
        }

        private static void AddDeleteACL(DirectoryInfo root)
        {
            DirectorySecurity ds = Directory.GetAccessControl(root.FullName);
            ds.AddAccessRule(rule);
            Directory.SetAccessControl(root.FullName, ds);
        }
    }
}
