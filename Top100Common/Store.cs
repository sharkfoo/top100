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
        public async Task<string> CreateAsync(Song song)
        {
            try
            {
                if (Find(song) == null)
                {
                    var document = new SongDocument(song);
                    await songCollection.InsertOneAsync(document);
                    return document._id.ToString();
                }
                else
                {
                    throw new Top100Exception(ReasonType.Conflict);
                }
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
                throw new Top100Exception(ReasonType.Unknown);
            }
        }

        public async Task<Song> ReadAsync(int year, int number)
        {
            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;

            var yearFilter = builder.Eq(x => x.Song.Year, year);
            filter = builder.And(yearFilter, filter);

            var numberFilter = builder.Eq(x => x.Song.Number, number);
            filter = builder.And(numberFilter, filter);

            try
            {
                var document = await songCollection.Find(filter).FirstAsync();
                return document.Song;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"ArgumentNullException.  ex={e}");
                throw new Top100Exception(ReasonType.NotFound);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"InvalidOperationException.  ex={e}");
                throw new Top100Exception(ReasonType.NotFound);
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
                throw new Top100Exception(ReasonType.Unknown);
            }
        }
        public async Task<Song> DeleteAsync(int year, int number)
        {
            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;

            var yearFilter = builder.Eq(x => x.Song.Year, year);
            filter = builder.And(yearFilter, filter);

            var numberFilter = builder.Eq(x => x.Song.Number, number);
            filter = builder.And(numberFilter, filter);

            try
            {
                var document = await songCollection.Find(filter).FirstAsync();
                var result = songCollection.DeleteOne(x => x._id == document._id);
                if (result.DeletedCount == 1)
                {
                    return document.Song;
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine($"ArgumentNullException.  ex={e}");
                throw new Top100Exception(ReasonType.NotFound);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"InvalidOperationException.  ex={e}");
                throw new Top100Exception(ReasonType.NotFound);
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
                throw new Top100Exception(ReasonType.Unknown);
            }
            return null;
        }

        public async Task<string> UpdateAsync(Song song)
        {
            var dbSong = Find(song);
            if (dbSong != null)
            {
                try
                {
                    var result = await songCollection.ReplaceOneAsync(x => x._id == dbSong._id, new SongDocument(dbSong._id, song), new UpdateOptions { IsUpsert = false });
                    if (result.ModifiedCount == 1)
                    {
                        return dbSong._id.ToString();
                    }
                    else
                    {
                        Console.WriteLine($"ERROR:  Invalid result count={result.MatchedCount}");
                        throw new Top100Exception(ReasonType.Unknown);
                    }
                }
                catch (MongoException e)
                {
                    Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
                    throw new Top100Exception(ReasonType.Unknown);
                }
            }
            else
            {
                Console.WriteLine($"Warning Song not found to update title={song.Title}, artist={song.Artist}, year={song.Year}, number={song.Number}");
                throw new Top100Exception(ReasonType.NotFound);
            }
        }

        public async Task<IList<Song>> SearchAsync(string titleFilterString, string artistFilterString, string yearFilterString, string numberFilterString, string ownFilterString)
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

        private SongDocument Find(Song song)
        {
            SongDocument result = null;

            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;

            //var titleFilter = builder.Eq(x => x.Song.Title, song.Title);
            //filter = builder.And(titleFilter, filter);

            //var artistFilter = builder.Eq(x => x.Song.Artist, song.Artist);
            //filter = builder.And(artistFilter, filter);

            var numberFilter = builder.Eq(x => x.Song.Number, song.Number);
            filter = builder.And(numberFilter, filter);

            var yearFilter = builder.Eq(x => x.Song.Year, song.Year);
            filter = builder.And(yearFilter, filter);

            //var ownFilter = builder.Eq(x => x.Song.Own, song.Own);
            //filter = builder.And(ownFilter, filter);

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
