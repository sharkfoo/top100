using System;
using System.Collections.Generic;
using iTunesExport.Parser;

namespace Top40Modify
{
	public class MainClass
	{
        static Dictionary<string, Action<string>> paramList = new Dictionary<string, Action<string>>();

        static private void ParseArguments(IEnumerable<string> args)
        {
            char[] splitChars = { '=' };
            foreach (string arg in args)
            {
                string[] keyValue = arg.Split(splitChars, 2, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length == 2)
                {
                    if (paramList.ContainsKey(keyValue[0]))
                    {
                        paramList[keyValue[0]].Invoke(keyValue[1]);
                    }
                }
            }
        }

        public static void Main(string[] args)
		{
            bool fix_featuring = false;
            Int16 year = 0;
            Func<Song, bool> compare;

            paramList.Add("debug", a => Top40Settings.Debug = Boolean.Parse(a));
            paramList.Add("preview", a => Top40Settings.Preview = Boolean.Parse(a));
            paramList.Add("year", a => year = Int16.Parse(a));
            paramList.Add("fix_featuring", a => fix_featuring = Boolean.Parse(a));

            ParseArguments(args);

            var timer = Top40Timer.Start("Parsing iTunes library");
            List<Song> iTunesSongList = new List<Song>();
            LibraryParser library = new LibraryParser(LibraryParser.GetDefaultLibraryLocation());
            foreach (Playlist p in library.Playlists)
            {
                if (p.Name == "Music")
                {
                    foreach (Song s in p.Songs)
                    {
                        if (!s.AppleMusic)
                        {
                            iTunesSongList.Add(s);
                        }
                        else
                        {
                            Top40Util.Debug("Skipping AppleMusic file: " + s);
                        }
                    }
                }
            }
            timer.End();

            if (year > 0)
            {
                compare = (x) => x.Year == year;
            }
            else
            {
                compare = (x) => true;
            }

            using (var db = new Top40DB())
            {
                if (fix_featuring)
                {
                    db.ModifyFeaturing(compare);
                }
                db.UpdateDbOwnership(iTunesSongList, compare);
                db.FindMissingOwnership(iTunesSongList, compare);
                db.FindMissingTagsAndComments(iTunesSongList, compare);
            }
		}
	}
}
