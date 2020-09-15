//
// © Copyright 2017,2020 Kevin Pearson
//

using Top100Common;

namespace Top100UI
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
