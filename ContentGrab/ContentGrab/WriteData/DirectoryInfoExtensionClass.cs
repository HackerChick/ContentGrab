using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContentGrab.WriteData
{
    public static class DirectoryInfoExtensionClass
    {
        public static void CopyTo(this DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                if (!Directory.Exists(target.FullName)) Directory.CreateDirectory(target.FullName);
                foreach (FileInfo fi in source.GetFiles())
                {
                    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                }


                foreach (DirectoryInfo diSourceDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
                    diSourceDir.CopyTo(nextTargetDir);
                }
            }
            catch (IOException ie)
            {
                throw new Exception("Unable to include web files, got exception: " + ie.StackTrace);
            }
        }
    }
}
