//
// © Copyright 2017,2020 Kevin Pearson
//

using Top100Common;

namespace Top100UI
{
    public class SongResult
    {
        public SongResult(Song song)
        {
            Song = song;
        }

        public Song Song { get; set; }
    }
}
