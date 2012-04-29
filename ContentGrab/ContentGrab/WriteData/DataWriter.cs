using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContentGrab.Model;

namespace ContentGrab.WriteData
{
    class DataWriter
    {
        private List<ContentList> content = null;
        private DirectoryInfo contentDir = null;

        public DataWriter(List<ContentList> content)
        {
            this.content = content;
        }

        public string Run(string contentDirectory)
        {
            SetupContentDirectory(contentDirectory);

            var topLevelIndex = new TopLevelIndex(contentDirectory, content);
            var topLevelIndexPath = topLevelIndex.Create();

            foreach (var contentList in content)
            {
                string directory = contentDir + @"\" + contentList.DirectoryName;
                var index = new CreateListOfContentItems(directory,contentList);
                index.Create();
            }

            return topLevelIndexPath;
        }

        private void SetupContentDirectory(string contentDirectory)
        {
            contentDir = new DirectoryInfo(contentDirectory);

            var includeFiles = new DirectoryInfo("Includes");
            includeFiles.CopyTo(contentDir);
        }

    }
}
