using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentGrab.Model;

namespace ContentGrab.WriteData
{
    public class CreateListOfContentItems : HTMLContentWriter
    {
        private const string PATH_TO_INCLUDE_FILES = "../";     // because content dirs are 1 down
        private const string OWNED_TAG = "OWN";

        private readonly ContentList contentList;

        public CreateListOfContentItems(string htmlDirectory, ContentList contentListToDisplay)
            : base(htmlDirectory, PATH_TO_INCLUDE_FILES, contentListToDisplay.Name, true)
        {
            this.contentList = contentListToDisplay;
        }

        public override void WriteLinks()
        {
            sw.WriteLine("<div class='navlinks'>");
            sw.WriteLine("<a href='../index.html'><< Back</a> | ");
            sw.WriteLine("<a href='#' id='all' class='show'>Show All</a>");
            sw.WriteLine(" | <a href='#' id='{0}' class='filter'>Owned</a>", OWNED_TAG);
            var orderedTags = contentList.Tags.ToList();
            orderedTags.Sort();
            foreach (var tag in orderedTags)
            {
                sw.WriteLine(" | <a href='#' id='{0}' class='filter'>{0}</a>", tag.Replace(" ", "-"));
            }
            sw.WriteLine(" | <a href='#PicklistBox'>Picklist >></a>");

            sw.WriteLine("</div>");
        }

        public override void WriteContentItems()
        {
            foreach (var contentItem in contentList.ContentItems)
            {
                var itemId = contentItem.GetHashCode();
                var ownedClass = contentItem.IsOwned ? (" " + OWNED_TAG) : "";
                sw.WriteLine("        <li class='contentItem {0}{1}' id='item_{2}'> ", contentItem.GetTagList(" ", true), ownedClass, itemId);
                sw.WriteLine("            <div class='block'> ");
                sw.WriteLine("              <a href='{0}'><Img class='contentPic' src='{1}'  /></a>", contentItem.ProductURL, contentItem.ImageFile);
                sw.WriteLine("                <h2><a href='{0}'>{1}</a></h2>", contentItem.ProductURL, contentItem.Name);
                sw.WriteLine("                <p class='artist'>{0}</p>", contentItem.Artist);
                sw.WriteLine("				<p class='tags'>{0}</p>", contentItem.TagList);

                sw.Write("				<p class='options'>");
                if (contentItem.IsOwned) sw.Write("OWN | ");
                sw.WriteLine("<a href='#PicklistBox' class='select' id='{0}'>select</a> | <a href='{1}'>edit</a> | <a href='{2}'>delete</a></p>", itemId, contentItem.EditURL, contentItem.DeleteURL);

                sw.WriteLine("            </div> ");
                sw.WriteLine("        </li> ");
            }
        }
    }
}
