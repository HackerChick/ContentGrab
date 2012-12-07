using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ContentGrab
{
    public static class StringExtensions
    {
        public static string GetTextBetween(this string source, string endText, params string[] startText)
        {
            int startIndex = GetIndexAfter(source, startText);
            if (startIndex < 0) return null;

            int endIndex = source.IndexOf(endText, startIndex);
            if (endIndex < 0) throw new Exception(endText + " not found in string: " + source);

            string text = source.Substring(startIndex, endIndex - startIndex);

            // trim
            text = new Regex("&nbsp;").Replace(text, " ");
            text = text.Trim();

            return text;
        }

        public static int GetIndexAfter(this string source, params string[] textToMovePast)
        {
            int startIndex = 0;

            for (int i = 0; i < textToMovePast.Length; i++)
            {
                string text = textToMovePast[i];
                startIndex = source.IndexOf(text, startIndex);
                if (startIndex < 0) break;
                startIndex += text.Length;
            }

            return startIndex;
        }

        public static string CleanFileName( this string source )
        {
            // convert : to - and then strip out all potentially unsafe chars
            // Will leave: alpha, period, @ symbol, and hyphen
            string fileName = Regex.Replace(source, @": ", "-");
            fileName = Regex.Replace(fileName, @":", "-");
            fileName = Regex.Replace(fileName, @"'", "");
            return Regex.Replace(fileName, @"[^\w\.@-]", "");
        }

        private static void WriteToFile(this string content, string fileName)
        {
            var sw = new StreamWriter(fileName);
            sw.Write(content);
            sw.Close();
        }
    }
}
