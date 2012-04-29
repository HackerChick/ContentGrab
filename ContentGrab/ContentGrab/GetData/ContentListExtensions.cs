using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ContentGrab.Model;

namespace ContentGrab.GetData
{
    public static class ContentListExtensions
    {
        public static void SetParams( this ContentList contentList, string source)
        {
            contentList.SetName(source);
            contentList.SetURL(source);
        }

        public static void SetName( this ContentList contentList, string source )
        {
            // <div class="info1"><a href="/my/list.aspx?list=394150">V4</a></div>
            contentList.Name = source.GetTextBetween("</a>", "\"info1\"", "<a href", ">");
        }

        public static void SetURL(this ContentList contentList, string source)
        {
            // <a href="/my/list.aspx?list=394150" class="image handleError"
            contentList.URL = "http://wishpot.com" + source.GetTextBetween("\"", "<a href", "\"");
        }
    }
}
