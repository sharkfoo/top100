//
// © Copyright 2017,2020 Kevin Pearson
//
using MongoDB.Bson;

namespace Top100Common
{
    public class SongDocument
    {
        public SongDocument(Song song)
        {
            _id = ObjectId.GenerateNewId();
            Song = song;
        }

        public SongDocument(ObjectId id, Song song)
        {
            _id = id;
            Song = song;
        }

        public ObjectId _id { get; set; }
        public Song Song{ get; set; }
        public bool Equals(Song s)
        {
            return Song.Equals(s);
        }
    }
}
