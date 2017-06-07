//
// © Copyright 2017 Kevin Pearson
//

using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Top100Common
{
    public class Store : IStore
    {
        private IMongoDatabase db;
        private IMongoCollection<SongDocument> songCollection => db.GetCollection<SongDocument>("Top100");

        public Store(string connectionString)
        {
            var client = new MongoClient(connectionString);
            db = client.GetDatabase("top100");
        }
        public async Task<string> InsertAsync(Song song)
        {
            try
            {
                if (Find(song) == null)
                {
                    var document = new SongDocument(song);
                    await songCollection.InsertOneAsync(document);
                    return document._id.ToString();
                }
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
            }
            return null;
        }

        public async Task<Song> GetAsync(string id)
        {
            try
            {
                var filter = Builders<SongDocument>.Filter.Eq(x=>x._id, ObjectId.Parse(id));
                var document = await songCollection.Find(filter).FirstOrDefaultAsync();
                return document.Song;
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
            }
            return null;
        }
        public async Task<IList<Song>> FindAsync(string titleFilterString, string artistFilterString, string yearFilterString, string numberFilterString, string ownFilterString)
        {
            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;
            var retList = new List<Song>();

            if (!String.IsNullOrWhiteSpace(titleFilterString))
            {
                var titleFilter = builder.Regex(x => x.Song.Title, new BsonRegularExpression(titleFilterString, "i"));
                filter = builder.And(titleFilter, filter);
            }
            if (!String.IsNullOrWhiteSpace(artistFilterString))
            {
                var artistFilter = builder.Regex(x => x.Song.Artist, new BsonRegularExpression(artistFilterString, "i"));
                filter = builder.And(artistFilter, filter);
            }
            if (!String.IsNullOrWhiteSpace(yearFilterString))
            {
                int year;
                if (Int32.TryParse(yearFilterString, out year))
                {
                    var yearFilter = builder.Eq(x => x.Song.Year, year);
                    filter = builder.And(yearFilter, filter);
                }
            }
            if (!String.IsNullOrWhiteSpace(numberFilterString))
            {
                int number;
                if (Int32.TryParse(numberFilterString, out number))
                {
                    var numberFilter = builder.Eq(x => x.Song.Number, number);
                    filter = builder.And(numberFilter, filter);
                }
            }
            if (!String.IsNullOrWhiteSpace(ownFilterString))
            {
                bool own = false;
                if (Boolean.TryParse(ownFilterString, out own))
                {
                    var ownFilter = builder.Eq(x => x.Song.Own, own);
                    filter = builder.And(ownFilter, filter);
                }
            }
            try
            {
                await songCollection.Find(filter).SortBy(x=>x.Song.Year).ThenBy(x=>x.Song.Number).ForEachAsync(x => retList.Add(x.Song));
                Console.WriteLine($"FindAsync found count={retList.Count} documents");
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"FindAsync ex={ex}");
            }

            return retList;
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
