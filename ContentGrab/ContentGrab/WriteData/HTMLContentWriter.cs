using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContentGrab.WriteData
{
    public abstract class HTMLContentWriter
    {
        internal readonly string directory;
        internal readonly string pageTitle;
        internal readonly bool includePicklist;
        internal StreamWriter sw;
        private string includePath;

        internal HTMLContentWriter( string htmlDirectory, string htmlIncludePath, string htmlPageTitle, bool includePicklist )
        {
            this.directory = htmlDirectory;
            this.includePath = htmlIncludePath;
            this.pageTitle = htmlPageTitle;
            this.includePicklist = includePicklist;
        }

        private HTMLContentWriter(){}

        public string Create()
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string path = directory + @"\index.html";
            if (File.Exists(path)) File.Delete(path);

            using (sw = new StreamWriter(path))
            {
                WriteStartHTML();
                WriteContentItems();
                WriteEndHTML();
            }

            return path;
        }

        public abstract void WriteContentItems();

        // Override to add content
        public virtual void WriteLinks(){}

        public void WriteStartHTML()
        {
            sw.WriteLine("<html>");

            sw.WriteLine("<body>");
            sw.WriteLine("<head>");
            sw.WriteLine("    <title>{0}</title>", pageTitle);

            sw.WriteLine("  <link href='{0}css/3DContent.css' type='text/css' rel='stylesheet' />", includePath);
            sw.WriteLine("	<script src='{0}js/jquery.js' type='text/javascript'></script>", includePath);
            sw.WriteLine("	<script src='{0}js/jquery-ui.js' type='text/javascript'></script>", includePath);
            sw.WriteLine("	<script src='{0}js/3DContent.js' type='text/javascript'></script>", includePath);
            sw.WriteLine("</head>");
            sw.WriteLine("<body>");

            sw.WriteLine("<div class='header'>");
            sw.WriteLine("<h1>{0}</h1>", pageTitle);
            sw.WriteLine("<div id='slider'></div>");

            WriteLinks();
            sw.WriteLine("</div>");

            sw.WriteLine("<div class='container' id='content-container'>");
            sw.WriteLine("    <ul class='grid' id='content-grid'> ");
        }

        public void WriteEndHTML()
        {
            sw.WriteLine("   </ul> ");
            sw.WriteLine("</div>");

            if (includePicklist) WritePicklist();

            sw.WriteLine("</body> ");
            sw.WriteLine("</html> ");
        }

        private void WritePicklist()
        {
            sw.WriteLine("<a name='PicklistBox'></a>");
            sw.WriteLine("<div class='picklist'>");
            sw.WriteLine("    <ul class='grid' id='picklist-grid'> ");
            sw.WriteLine("   </ul> ");
            sw.WriteLine("</div>");
        }
    }
}
