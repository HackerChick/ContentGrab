using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ContentGrab.GetData;
using ContentGrab.Model;
using ContentGrab.WriteData;

namespace ContentGrab
{
    class Program
    {
        private const bool FAST_MODE = false;
        private const string CONTENT_DIR = @"D:\Art\3D\Content";

        static void Main(string[] args)
        {
            var contentRetrieval = new ContentRetrieval();
            var content = contentRetrieval.Run(CONTENT_DIR, FAST_MODE);

            //var content = new List<ContentList>();
            var dataWriter = new DataWriter(content);
            var index = dataWriter.Run(CONTENT_DIR);

            while (contentRetrieval.FilesDownloading > 0)
            {
                Thread.Sleep(5000);
                Console.WriteLine("Finishing downloads: {0} files remain...", contentRetrieval.FilesDownloading);
            }

            // open main index page
            Console.WriteLine("All done");
            Process.Start(index);
        }
    }
}
