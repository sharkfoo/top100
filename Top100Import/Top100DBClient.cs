//
// © Copyright 2017 Kevin Pearson
//

using System;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Top100Import
{
    public class Top100DBClient : ITop100DBClient
    {
        private IMongoDatabase db;
        private IMongoCollection<SongDocument> songCollection => db.GetCollection<SongDocument>("Top100");

        public Top100DBClient(string connectionString)
        {
            var client = new MongoClient(connectionString);
            db = client.GetDatabase("top100");
        }
        public async Task<bool> Insert(Song song)
        {
            try
            {
                if (Find(song) == null)
                {
                    await songCollection.InsertOneAsync(new SongDocument(song));
                    return true;
                }
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
            }
            return false;
        }
        public SongDocument Find(Song song)
        {
            SongDocument result = null;

            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;

            var titleFilter = builder.Eq(x => x.Song.Title, song.Title);
            filter = builder.And(titleFilter, filter);

            var artistFilter = builder.Eq(x => x.Song.Artist, song.Artist);
            filter = builder.And(artistFilter, filter);

            var numberFilter = builder.Eq(x => x.Song.Number, song.Number);
            filter = builder.And(numberFilter, filter);

            var yearFilter = builder.Eq(x => x.Song.Year, song.Year);
            filter = builder.And(yearFilter, filter);

            var ownFilter = builder.Eq(x => x.Song.Own, song.Own);
            filter = builder.And(ownFilter, filter);

            try
            {
                var cursor = songCollection.Find(filter);
                if (cursor?.Count() > 0)
                {
                    result = cursor.First();
                }
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"ERROR:  error in find.  ex={ex}");
            }

            return result;

        }
    }
}
