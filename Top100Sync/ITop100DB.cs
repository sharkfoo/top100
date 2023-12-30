using System;
using System.Collections.Generic;

namespace Top100Sync
{
    public interface ITop100DB
    {
        void ModifyFeaturing(Func<Song, bool> compare);
        void UpdateDbOwnership(List<Song> iTunesSongList, Func<Song, bool> compare);
        void FindMissingOwnership(List<Song> iTunesSongList, Func<Song, bool> compare);
        void FindMissingTagsAndComments(List<Song> iTunesSongList, Func<Song, bool> compare);
    }
}

