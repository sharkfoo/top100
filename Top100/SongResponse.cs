//
// © Copyright 2017 Kevin Pearson
//

using Top100Common;

namespace Top100
{
    public class SongResponse
    {
        public SongResponse(Song song)
        {
            Song = song;
        }

        public Song Song { get; set; }
    }
}
