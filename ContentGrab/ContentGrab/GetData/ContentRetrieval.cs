using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ContentGrab.Model;

namespace ContentGrab.GetData
{
    class ContentRetrieval
    {
        private string contentDir;
        private bool inFastMode;
        private const string LOGIN_URL = "https://www.wishpot.com/secure/signin.aspx";
        private const string CONTENT_URL = "http://www.wishpot.com/user/227540";

        private const string LISTS_START_WITH = "\"wishpot_lists\"";    // <ol id="wishpot_lists" class="fluid collections size3 imageborder containfloat ">
        private const string EACH_LIST_BEGINS = "\"border\"";           // <div class="border">
        private const string EACH_ITEM_BEGINS = "\"normalWish\"";       // <div class="normalWish" id="Wish_XXXXXXXX"> 

        public ContentRetrieval()
        {
            FilesDownloading = 0;
        }

        public List<ContentList> Run(string contentDirectory)
        {
            return Run(contentDirectory, false);
        }

        public List<ContentList> Run(string contentDirectory, bool fastMode)
        {
            this.contentDir = contentDirectory;
            this.inFastMode = fastMode;

            List<ContentList> content;
            using (var client = new WebClientEx())
            {
                Login(client);
                content = RetrieveContent(client);
            }

            return content;
        }

        private static void Login(WebClientEx client)
        {
            var loginValues = new NameValueCollection
                             {
                                 {"EmailAddress", "haxrchick@gmail.com"},
                                 {"Password", "wishing"},
                             };

            Console.WriteLine("Logging in...");
            client.UploadValues(LOGIN_URL, loginValues);
        }

        private List<ContentList> RetrieveContent(WebClientEx client)
        {
            var listOfLists = new List<ContentList>();

            string listOfListsSource = GetListOfLists(client);

            int startIndex = 0;
            int totalItems = 0;

            while (true)
            {
                startIndex = listOfListsSource.IndexOf(EACH_LIST_BEGINS, startIndex);
                if (startIndex < 0) break; startIndex += EACH_LIST_BEGINS.Length;
                var contentList = GetListInfo(listOfListsSource, startIndex);

                Console.WriteLine("Retrieving content for " + contentList.Name + "...");
                List<ContentItem> pageContent;
                int pageNumber = 1;

                do
                {
                    string pageURL = contentList.URL + "&pg=" + pageNumber++;
                    string pageSource = client.DownloadString(pageURL);
                    pageContent = GetContentItemsOnPage(pageURL, pageSource, contentList);
                    contentList.ContentItems.AddRange(pageContent); 
                    foreach (var ci in pageContent)
                    {
                        foreach (var tag in ci.Tags)
                        {
                            contentList.Tags.Add(tag);
                        }
                    }

                } while (pageContent.Count > 0 && !inFastMode); // just grab the 1st page if in fast mode
                totalItems += contentList.ContentItems.Count;
                Console.WriteLine("Found " + contentList.ContentItems.Count + " items for " + contentList.Name);

                listOfLists.Add(contentList);
            }

            Console.WriteLine("Read " + totalItems + " items (total).");

            return listOfLists;
        }

        private string GetListOfLists(WebClientEx client)
        {
            var listOfListsSource = client.DownloadString(CONTENT_URL);

            int listStartIdx = listOfListsSource.GetIndexAfter(LISTS_START_WITH);
            if (listStartIdx < 0) throw new Exception("Unable to find lists");
            return listOfListsSource.Substring(listStartIdx);
        }


        private ContentList GetListInfo(string source, int startIndex)
        {
            if (source == null) throw new Exception("Unable to get list, source is blank.");

            source = source.Substring(startIndex);

            var contentList = new ContentList();
            contentList.SetParams(source);
            return contentList;
        }

        private List<ContentItem> GetContentItemsOnPage(string pageURL, string sourceCode, ContentList contentList)
        {
            var contentItems = new List<ContentItem>();
            if (sourceCode == null) return contentItems;

            int startIndex = 0;
            while (true)
            {
                // New Item
                startIndex = sourceCode.IndexOf(EACH_ITEM_BEGINS, startIndex);
                if (startIndex < 0) break;
                startIndex += EACH_ITEM_BEGINS.Length;

                try
                {
                    contentItems.Add(CreateContentItem(pageURL, sourceCode, startIndex, contentList));
                }
                catch (Exception ex)
                {
                    Console.Beep();
                    Console.WriteLine("Caught exception: " + ex.ToString().Substring(0, 160));
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("Press any key to continue...");
                    Console.Read();
                }
            }

            return contentItems;
        }


        private ContentItem CreateContentItem(string pageURL, string source, int startIndex, ContentList contentList)
        {
            if (source == null) throw new Exception("Unable to get content, source is blank.");

            source = source.Substring(startIndex);

            var content = new ContentItem();
            content.SetParams(pageURL, source, contentDir, contentList, DownloadStartedCallback, DownloadCompletedCallback);
            return content;
        }

        public int FilesDownloading { get; private set; }
        private void DownloadStartedCallback(object sender, AsyncCompletedEventArgs e)
        {
            FilesDownloading++;
        }

        public void DownloadCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            FilesDownloading--;
        }
    }
}
