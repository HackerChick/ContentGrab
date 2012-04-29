using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContentGrab.Model;

namespace ContentGrab.WriteData
{
    public class TopLevelIndex : HTMLContentWriter
    {
        private readonly List<ContentList> listOfLists;

        public TopLevelIndex(string htmlDirectory, List<ContentList> listOfListsToDisplay)
            : base(htmlDirectory, "", "Render Me", false)
        {
            listOfLists = listOfListsToDisplay;
            listOfLists.Sort(new ListOfListsSorter());
        }

        public override void WriteContentItems()
        {
            foreach (var contentList in listOfLists)
            {
                var listURL = contentList.DirectoryName + @"\index.html"; 
                var contentSubdirectory = directory + @"\" + contentList.DirectoryName;

                sw.WriteLine("        <li> ");
                sw.WriteLine("            <div class='block'> ");
                sw.WriteLine("              <a href='{0}'><img class='contentPic' src='{1}'  /></a>", listURL, GetRandomImageFile(contentSubdirectory));
                sw.WriteLine("                <h2><a href='{0}'>{1}</a></h2>", listURL, contentList.Name);
                sw.Write("				<p class='own_rate'>{0} Items</p>", contentList.ContentItems.Count);
                sw.WriteLine("            </div> ");
                sw.WriteLine("        </li> ");
            }
        }

        private static string GetRandomImageFile(string path)
        {
            try
            {
                var extensions = new string[] { ".png", ".jpg", ".gif" };

                var di = new DirectoryInfo(path);
                var imageFileName = ( di.GetFiles("*.*")
                                    .Where(f => extensions.Contains(f.Extension.ToLower()))
                                    .OrderBy(f => Guid.NewGuid())
                                    .First()).FullName;
                return imageFileName;
            }
            catch( Exception ex )
            {
                return "";
            }
        }
    }

    public class ListOfListsSorter : IComparer<ContentList>
    {
        public int Compare(ContentList x, ContentList y)
        {
            // Starting from end...
            if (x.Name.StartsWith("Tutorials")) return 1;   if (y.Name.StartsWith("Tutorials")) return -1;
            if (x.Name.StartsWith("Photoshop")) return 1;   if (y.Name.StartsWith("Photoshop")) return -1;
            if (x.Name.StartsWith("Poser")) return 1;       if (y.Name.StartsWith("Poser")) return -1;
            if (x.Name.StartsWith("Vehicles")) return 1;    if (y.Name.StartsWith("Vehicles")) return -1;
            if (x.Name.StartsWith("Scenes: Backgrounds")) return 1;    if (y.Name.StartsWith("Scenes: Backgrounds")) return -1;
            if (x.Name.StartsWith("Scenes")) return 1;    if (y.Name.StartsWith("Scenes")) return -1;
            if (x.Name.StartsWith("Character Props")) return 1; if (y.Name.StartsWith("Character Props")) return -1; 
            if (x.Name.StartsWith("Other Characters")) return 1; if (y.Name.StartsWith("Other Characters")) return -1;
            if (x.Name.StartsWith("Animals")) return 1; if (y.Name.StartsWith("Animals")) return -1;
            if (x.Name.StartsWith("Toon People")) return 1; if (y.Name.StartsWith("Toon People")) return -1;


            // V4  first
            if (x.Name.StartsWith("V4") )
            {
                if( !y.Name.StartsWith("V4")) return -1;   
            }
            if (!x.Name.StartsWith("V4") && y.Name.StartsWith("V4")) return 1;

            // M4 next
            if (x.Name.StartsWith("M4"))
            {
                if( y.Name.StartsWith("V4")) return 1;
                if( !y.Name.StartsWith("M4")) return -1;
            }
            if (!x.Name.StartsWith("M4") && y.Name.StartsWith("M4")) return 1;

            // Kids next
            if (x.Name.StartsWith("Kids"))
            {
                if (y.Name.StartsWith("V4")) return 1;
                if (y.Name.StartsWith("M4")) return 1;
                if (!y.Name.StartsWith("Kids")) return -1;
            }
            if (!x.Name.StartsWith("Kids") && y.Name.StartsWith("Kids")) return 1;

            return string.Compare(x.Name, y.Name);
        }
    }
}
