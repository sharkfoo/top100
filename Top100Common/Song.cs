//
// © Copyright 2017 Kevin Pearson
//

namespace Top100Common
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public int Year { get; set; }
        public int Number { get; set; }
        public bool Own { get; set; }

        public bool Equals(Song s)
        {
            if (this.Title != s.Title) return false;
            if (this.Artist != s.Artist) return false;
            if (this.Year != s.Year) return false;
            if (this.Number != s.Number) return false;
            if (this.Own != s.Own) return false;
            return true;
        }
    }
}
