using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ContentGrab.Model
{
    public class ContentList
    {
        public String Name { get; set; }
        public String URL { get; set; }
        public List<ContentItem> ContentItems { get; set; }
        public HashSet<String> Tags { get; set; }

        public ContentList()
        {
            ContentItems = new List<ContentItem>();
            Tags = new HashSet<String>();
        }

        public override string ToString()
        {
            return (
                "Name = '" + Name + "'\r\n" +
                "URL = '" + URL + "'");
        }

        public String DirectoryName{ get { return Name.CleanFileName(); } }
    }
}
