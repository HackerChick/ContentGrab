using System;
using System.Collections.Generic;

namespace ContentGrab.Model
{
    public class ContentItem
    {
        public String Name { get; set; }
        public String Artist { get; set; }
        public List<String> Tags { get; set; }
        public bool IsOwned { get; set; }
        public String ProductURL { get; set; }
        public String EditURL { get; set; }
        public String DeleteURL { get; set; }
        public String ImageFile { get; set; }
        public int? Rating { get; set; }

        public ContentItem()
        {
            IsOwned = false;
            Rating = null;
            Tags = new List<String>();
        }

        public string GetTagList(string seperator, bool removeSpaces )
        {
            string tagList = "";

            int count = 0;
            foreach (var tag in Tags)
            {
                if (count++ > 0) tagList += seperator;
                var tagToUse = tag;
                if (removeSpaces) tagToUse = tag.Replace(" ", "-");
                tagList += tagToUse;
            }

            return tagList;
        }

        public string TagList
        {
            get
            {
                return GetTagList(",", false);
            }
        }

        public override string ToString()
        {
            return (
                "Name = '" + Name + "'\r\n" +
                "Artist = '" + Artist + "'\r\n" +
                "Tags = '" + TagList + "'\r\n" +
                "ProductURL = '" + ProductURL + "'\r\n" +
                "EditURL = '" + EditURL + "'\r\n" +
                "DeleteURL = '" + DeleteURL + "'\r\n" +
                "ImageFile = '" + ImageFile + "'\r\n" +
                "Rating = " + Rating + "\r\n" +
                "Is Owned? " + IsOwned);
        }
    }
}
