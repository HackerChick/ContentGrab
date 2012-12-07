using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ContentGrab.Model;

namespace ContentGrab.GetData
{
    public static class ContentItemExtensions
    {
        public static void SetParams(this ContentItem content, string pageURL, string source, string contentDir, ContentList contentList, EventHandler<AsyncCompletedEventArgs> downloadStartedCallback, EventHandler<AsyncCompletedEventArgs> downloadCompletedCallback)
        {
            string nameAndArtist = content.SetContentNameAndArtist(source);
            content.SetContentTagsAndOwnership(source);
            content.SetProductURL(source);
            content.SetEditURL(source);
            content.DeleteURL = pageURL;    // no way to delete except to load list page and click delete button there
            content.SetAndRetrieveImage(source, contentDir + @"\" + contentList.DirectoryName, downloadStartedCallback, downloadCompletedCallback );
            content.SetRating(source);
        }

        public static string SetContentNameAndArtist(this ContentItem content, string source)
        {
            // <h3 class="big"><a href="/my/wishes/details.aspx?wid=21208898&pid=21158144"  >Abbi (Mihrelle)</a></h3>
            string nameAndArtist = source.GetTextBetween("</a>", "<h3", ">", ">");

            if (nameAndArtist != null && nameAndArtist.Contains("("))
            {
                content.Name = nameAndArtist.GetTextBetween("(");
                content.Artist = nameAndArtist.GetTextBetween(")", "(");
            }
            else
            {
                content.Name = nameAndArtist;
                content.Artist = null;
            }

            return nameAndArtist;
        }

        // BUG: If an item has no tags this will find the NEXT item's tags
        public static void SetContentTagsAndOwnership(this ContentItem content, string source)
        {
            // <div id="PageFrame_Html_ctl01_ctl12_WishView_IntRepeater_WishSummary_0_TagsPanel_0">
            //   <span class="wishProp">Tags: </span>&nbsp;clothes, casual
            // </div>
            string tags = source.GetTextBetween("</div>", "TagsPanel", "<span", "Tags:", "/span>");
            if (tags == null) return;
            var untrimmedTags = tags.Split(',').ToList<string>();
            foreach (var tag in untrimmedTags)
            {
                content.Tags.Add(tag.Trim());
            }

            if( content.Tags.Contains("OWN"))
            {
                content.IsOwned = true;
                content.Tags.Remove("OWN");
            }
        }

        public static void SetProductURL(this ContentItem content, string source)
        {
            // <div id="PageFrame_Html_ctl01_ctl12_WishView_IntRepeater_WishSummary_0_GoToMerchantPanel_0">
            // See it at &nbsp;<a href="/public/merchant.aspx?uid=227540&wid=21208898&pid=21158144&pcat=&pr=14.9500&pspid=2b50e56c7bc540c49fe484db36a838eb&ps=4&url=http%3A%2F%2Fwww.renderosity.com%2Fmod%2Fbcs%2Fmrl-abbi%2F85705" target="_blank">renderosity.com</a>
            // </div>
            string productURL = source.GetTextBetween("\"", "GoToMerchantPanel", "<a href", "&url=");
            if (productURL != null) content.ProductURL = Uri.UnescapeDataString(productURL);
        }

        public static void SetEditURL(this ContentItem content, string source)
        {
            // <div class="buttons" style="font-size: 85%;">
            // ...
            // <a href="/my/wishes/edit.aspx?wid=20122328&ru=http%3a%2f%2fwww.wishpot.com%2flist.aspx%3fuid%3d227540%26list%3d394150%26pg%3d3#focus" class="icon-edit"  >edit</a>
            content.EditURL = "http://www.wishpot.com" + source.GetTextBetween("\"", "\"buttons\"", "<a href", "\"");
        }

        public static void SetAndRetrieveImage(this ContentItem content, string source, string contentDir, EventHandler<AsyncCompletedEventArgs> downloadStartedCallback, EventHandler<AsyncCompletedEventArgs> downloadCompletedCallback)
        {
            if (!Directory.Exists(contentDir)) Directory.CreateDirectory(contentDir);

            // <img class="wishPic" width="100" height="100 "  src="http://www.renderosity.com/mod/bcs/photos/Full85705.jpg" alt="" style="max-height:100px; max-width:100px;" />
            string imageURL = source.GetTextBetween("\"", "\"wishPic\"", "src", "\"");
            if (imageURL != null)
            {
                imageURL = imageURL.FormProperURL();
                var extension = ".jpg"; // default
                int indexOfExtension = imageURL.LastIndexOf(".");
                if (indexOfExtension > 0) extension = imageURL.Substring(indexOfExtension);

                // Only download file if doesn't exist yet
                // (acceptable to assume that within same list, won't be same exact name+artist for 2 different content types, 
                //  use convention to append "alt" if you do multiple images for the same item)
                var uncleanedFileName = content.Name + " - " + content.Artist;
                content.ImageFile = uncleanedFileName.CleanFileName() + extension;
                string imageFilePath = contentDir + @"\" + content.ImageFile;
                if (!File.Exists(imageFilePath))
                {
                    var wc = new WebClient();
                    downloadStartedCallback(null, null);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler (downloadCompletedCallback);
                    wc.DownloadFileAsync(new Uri(imageURL), imageFilePath);
                }
            }
        }


        public static void SetRating(this ContentItem content, string source)
        {
            // <img class="png" src="/img/rating_star_small0.png" width="100" height="20" alt=""/>
            content.Rating = Int32.Parse(source.GetTextBetween(".png", "img/rating_star_small"));
            if (content.Rating == 0) content.Rating = null;
        }
    }
}
