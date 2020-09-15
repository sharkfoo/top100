//
// © Copyright 2020 Kevin Pearson
//
using System;
using System.Text.RegularExpressions;

namespace Top100Sync
{
    public class Song
    {
        public int TrackId { get; set; }
        public int DbId { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public Int16 Year { get; set; }
        public Int16 Number { get; set; }
        public bool Own { get; set; }
        public string Grouping { get; set; }
        public string Comments { get; set; }
        public bool AppleMusic { get; set; }

        public Song()
        {
            TrackId = -1;
            DbId = -1;
            Artist = "";
            Title = "";
            Year = -1;
            Number = -1;
            Own = false;
            Grouping = "";
            Comments = "";
            AppleMusic = false;
        }

        public Song(int trackid, string title, string artist, string grouping, string comments, short year = -1, short number = -1, bool appleMusic = false)
        {
            TrackId = trackid;
            Title = title;
            Artist = artist;
            Grouping = grouping;
            Comments = comments;
            Year = year;
            Number = number;
            AppleMusic = appleMusic;
        }

        public bool IsMatch(Song s)
        {
            if ( (Artist.Equals(s.Artist)) && (Title.Equals(s.Title)) )
            {
                //Console.WriteLine(String.Format("Direct Match: {0} => {1}", this, s));
                return true;
            }
            else
            {
                char[] removeCharacters = { ';', ',', '"', ' ', '.', '\t', '\n'};
                string Artist1 = RemoveEmbeddedParensAndQuotes(Artist.ToLower()).Trim(removeCharacters);
                string Artist2 = RemoveEmbeddedParensAndQuotes(s.Artist.ToLower()).Trim(removeCharacters);

                string Title1 = RemoveEmbeddedParensAndQuotes(Title.ToLower()).Trim(removeCharacters);
                string Title2 = RemoveEmbeddedParensAndQuotes(s.Title.ToLower()).Trim(removeCharacters);
                if (Artist1.Equals(Artist2) && Title1.Equals(Title2))
                {
                    //Console.WriteLine(String.Format("Direct Match: {0} => {1}", this, s));
                    return true;
                }
            }

            return false;
        }

        private string RemoveEmbeddedParensAndQuotes(string s)
        {
            const string paren = "\\([^)]*\\)";
            const string brace = "\\[[^]]*\\]";
            const string ampersand = "&";

            string ret = s;

            if (Regex.IsMatch(ret, paren))
            {
                ret = Regex.Replace(ret, paren, "");
                //Console.WriteLine(String.Format("Replace: {0} => {1}", s, ret));
            }
            if (Regex.IsMatch(ret, brace))
            {
                ret = Regex.Replace(ret, brace, "");
                //Console.WriteLine(String.Format("Replace: {0} => {1}", s, ret));
            }
            if (Regex.IsMatch(ret, ampersand))
            {
                ret = Regex.Replace(ret, ampersand, "and");
                //Console.WriteLine(String.Format("Replace: {0} => {1}", s, ret));
            }
            //Remove embedded quotes
            ret = ret.Replace("\"", "");

            return ret;
        }

        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}|{3}", Title, Artist, Grouping, Comments);
        }

    }
}

