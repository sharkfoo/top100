using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using iTunesExport.Parser;
using System.Text.RegularExpressions;
using MonoDevelop.MacInterop;

namespace Top100Modify
{
    public class Top100DB : IDisposable
    {
        private MySqlConnection dbConnection;
        private Songs dbSongsList;

        public Top100DB()
        {
            string connectionString = "Server=localhost;Database=Top100;User ID=kevin;Password=admin;Pooling=false";
            dbConnection = new MySqlConnection(connectionString);
            try
            {
                dbConnection.Open();
                dbSongsList = new Songs(dbConnection);
            }
            catch (MySqlException)
            {
                Top100Util.Error("Could not connect to database.");
                throw;
            }

        }
            
        public void ModifyFeaturing(Func<Song, bool> compare)
        {
            var timer = Top100Timer.Start("ModifyFeaturing");
            foreach (var song in dbSongsList.List.FindAll(x => compare(x)))
            {
                using (var updateCmd = dbConnection.CreateCommand())
                {
                    if (song.Artist.Contains("Featuring", StringComparison.OrdinalIgnoreCase) ||
                        song.Artist.Contains("Feat.", StringComparison.OrdinalIgnoreCase))
                    {
                        string newTitle = MySqlHelper.EscapeString(song.Title + " " + getFeaturing(song.Artist));
                        string newArtist = MySqlHelper.EscapeString(getArtist(song.Artist));
                        updateCmd.CommandText = String.Format("UPDATE songs set title='{0}', artist='{1}' WHERE id={2}", newTitle, newArtist, song.DbId);
                        Top100Util.Debug(String.Format("Db Featuring: {0}", song));
                        if (!Top100Settings.Preview)
                        {
                            song.Artist = newArtist;
                            song.Title = newTitle;
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            timer.End();
        }

        public void UpdateDbOwnership(List<Song> iTunesSongList, Func<Song, bool> compare)
        {
            var timer = Top100Timer.Start("UpdateDbOwnership");
            foreach(Song t in dbSongsList.List.FindAll(x=> x.Own.Equals(false) && compare(x)))
            {
                var s = iTunesSongList.Find(x => x.IsMatch(t));
                if (s != null)
                {
                    using (var updateCmd = dbConnection.CreateCommand())
                    {
                        updateCmd.CommandText = "UPDATE songs set title=@title, artist=@artist, own='1' WHERE id=@id";
                        updateCmd.Parameters.AddWithValue("@title", s.Title);
                        updateCmd.Parameters.AddWithValue("@artist", s.Artist);
                        updateCmd.Parameters.AddWithValue("@id", t.DbId);
                        if (!Top100Settings.Preview)
                        {
                            t.Title = s.Title;
                            t.Artist = s.Artist;
                            t.Own = true;
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    Top100Util.Debug(String.Format("Db Ownership: {0} => {1}", t, s));
                }
            }
            timer.End();
        }

        public void FindMissingOwnership(List<Song> iTunesSongList, Func<Song, bool> compare)
        {
            var timer = Top100Timer.Start("FindMissingOwnership");
            foreach(Song dbSong in dbSongsList.List.FindAll(x => x.Own.Equals(true) && compare(x)))
            {
                var list = iTunesSongList.FindAll(x => x.IsMatch(dbSong));
                if ((list == null) || (list.Count < 1))
                {
                    Top100Util.Debug(String.Format("iTunes Missing: {0}", dbSong));
                }
            }
            timer.End();
        }

        public void FindMissingTagsAndComments(List<Song> iTunesSongList, Func<Song, bool> compare)
        {
            string top100 = "Top 100";
            Regex comment = new Regex("(?<year>^[0-9][0-9][0-9][0-9]), #(?<number>[01][0-9][0-9]).*");
            Regex badComment = new Regex("^[0-9][0-9][0-9][0-9], #[01][0-9][0-9] [0-9][0-9][0-9][0-9], #[0-9]?[1-9].*");

            var timer = Top100Timer.Start("FindMissingTagsAndComments");
            foreach(Song dbSong in dbSongsList.List.FindAll(x => x.Own.Equals(true) && compare(x)))
            {
                var list = iTunesSongList.FindAll(x => x.IsMatch(dbSong));
                if ((list != null) && (list.Count >= 1))
                {
                    foreach (Song s in list)
                    {
                        bool updateSong = false;
                        string appleScript = "tell application \"iTunes\"\n" +
                                             "  activate\n" +
                               String.Format("  set results to (every file track of playlist \"Library\" whose name contains (\"{0}\") and artist contains (\"{1}\"))\n", scrubString(s.Title), scrubString(s.Artist)) +
                                             "  repeat with t in results\n";
                        if (!s.Grouping.Contains(top100))
                        {
                            appleScript += String.Format("    set t's grouping to \"{0}\" as text\n", addTag(s.Grouping, top100));
                            Top100Util.Debug(String.Format("Missing Grouping: {0}=>{1}", s, addTag(s.Grouping, top100)));
                            updateSong = true;
                        }

                        if (badComment.IsMatch(s.Comments))
                        {
                            appleScript += String.Format("    set t's comment to \"{0}\" as text\n", prependComment(s.Comments, dbSong.Year, dbSong.Number));
                            Top100Util.Debug(String.Format("Bad Comment: {0}=>{1}", s, prependComment(s.Comments, dbSong.Year, dbSong.Number)));
                            updateSong = true;
                        }
                        else if (!comment.IsMatch(s.Comments))
                        {
                            appleScript += String.Format("    set t's comment to \"{0}\" as text\n", prependComment(s.Comments, dbSong.Year, dbSong.Number));
                            Top100Util.Debug(String.Format("Missing Comment: {0}=>{1}", s, prependComment(s.Comments, dbSong.Year, dbSong.Number)));
                            updateSong = true;
                        }
                        else if (comment.IsMatch(s.Comments))
                        {
                            if (dbSong.Number < s.Number)
                            {
                                appleScript += String.Format("    set t's comment to \"{0}\" as text\n", prependComment(s.Comments, dbSong.Year, dbSong.Number));
                                Top100Util.Debug(String.Format("Updating Comment: {0}=>{1}", s, prependComment(s.Comments, dbSong.Year, dbSong.Number)));
                                updateSong = true;
                            }
                        }
                        if (updateSong)
                        {
                            appleScript += "  end repeat\n" + "end tell\n";
                            try
                            {
                                if (!Top100Settings.Preview)
                                {
                                    AppleScript.Run(appleScript);
                                }
                            }
                            catch (Exception e)
                            {
                                Top100Util.Error(String.Format("Cannot update song: {0}\n\tException: {1}\n\tUsing: {2}", s, e, appleScript));
                            }
                        }
                    }
                }
                else
                {
                    Top100Util.Error("Cannot find owned song in Library. " + dbSong);
                }
            }
            timer.End();
        }
            
        private string scrubString(string src)
        {
            string retString = src.Replace("\"", "\\\"");
            return retString;
        }
            
        private string prependComment(string currentComment, int year, int number)
        {
            string retString = "";
            Regex badComment = new Regex("^[0-9][0-9][0-9][0-9], #[0-9]?[1-9].*");
            Regex badComment1 = new Regex("^[0-9][0-9][0-9][0-9], #[01][0-9][0-9] [0-9][0-9][0-9][0-9], #[0-9]?[1-9].*");
            string yearString = String.Format("{0}, #{1:D3}", year, number);

            if (badComment.IsMatch(currentComment))
            {
                retString = Regex.Replace(currentComment, badComment.ToString(), yearString);
            }
            else if (badComment1.IsMatch(currentComment))
            {
                retString = Regex.Replace(currentComment, badComment1.ToString(), yearString);
            }
            else if (!String.IsNullOrEmpty(currentComment))
            {
                retString = yearString + " " + scrubString(currentComment);
            }
            else
            {
                retString = yearString;
            }
            return retString;
        }

        private string addTag(string currentTag, string newTag)
        {
            if (String.IsNullOrEmpty(currentTag))
            {
                currentTag = newTag;
            }
            else
            {
                currentTag =  currentTag + "," + newTag;
            }
            return tagSort(currentTag);
        }

        private string tagSort(string tagList)
        {
            string[] oldTagArray = tagList.Split(',');
            string[] newTagArray = new string[oldTagArray.Length];
            int index = 0;

            foreach (string tag in oldTagArray)
            {
                newTagArray[index] = oldTagArray[index].Trim();
                index++;
            }
            Array.Sort(newTagArray);
            string retTags = "";
            for (int i = 0; i < newTagArray.Length; i++)
            {
                retTags += newTagArray[i];
                if (i < newTagArray.Length - 1)
                {
                    retTags += ",";
                }
            }
            return retTags;
        }

        private static string getFeaturing(string artist)
        {
            string retString = "";
            if (artist.Contains("Featuring", StringComparison.OrdinalIgnoreCase))
            {
                int index = artist.LastIndexOf("Featuring", StringComparison.OrdinalIgnoreCase);

                retString = String.Format("(feat. {0})", artist.Substring(index + 10));
            }
            else if (artist.Contains("(feat.", StringComparison.OrdinalIgnoreCase))
            {
                int index = artist.LastIndexOf("(feat.", StringComparison.OrdinalIgnoreCase);

                retString = String.Format("(feat. {0}", artist.Substring(index + 7));
            }
            else if (artist.Contains(" feat.", StringComparison.OrdinalIgnoreCase))
            {
                int index = artist.LastIndexOf(" feat.", StringComparison.OrdinalIgnoreCase);

                retString = String.Format("(feat. {0})", artist.Substring(index + 7));
            }
            retString = retString.Replace(" and ", " & ");
            return retString;
        }

        private static string getArtist(string artist)
        {
            int index = -1;        
            string retString = "";

            if (artist.Contains("Featuring", StringComparison.OrdinalIgnoreCase))
            {
                index = artist.LastIndexOf("Featuring", StringComparison.OrdinalIgnoreCase);
            }
            else if (artist.Contains("(feat.", StringComparison.OrdinalIgnoreCase))
            {
                index = artist.LastIndexOf("(feat.", StringComparison.OrdinalIgnoreCase);
            }
            else if (artist.Contains(" feat.", StringComparison.OrdinalIgnoreCase))
            {
                index = artist.LastIndexOf(" feat.", StringComparison.OrdinalIgnoreCase);
            }
            if (index > 0)
            {
                retString = artist.Remove(index - 1);
            }
            return retString;
        }
            
        public void Dispose()
        {
            if (dbConnection != null)
            {
                dbConnection.Dispose();
                dbConnection = null;
            }
        }
    }
}

