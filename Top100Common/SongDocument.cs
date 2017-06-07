//
// © Copyright 2017 Kevin Pearson
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

        public ObjectId _id { get; set; }
        public Song Song{ get; set; }
    }
}
