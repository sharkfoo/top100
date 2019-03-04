//
// © Copyright 2017 Kevin Pearson
//

using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading;

namespace Top100Common
{
    public class Store : IStore
    {
        private readonly IMongoDatabase db;
        private IMongoCollection<SongDocument> songCollection => db.GetCollection<SongDocument>("Top100");

        public Store(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "mongodb://127.0.0.1:27019/top100";
            }
            var client = new MongoClient(connectionString);
            db = client.GetDatabase("top100");
        }
        public async Task<string> CreateAsync(Song song, CancellationToken cancelToken)
        {
            try
            {
                if (await Find(song, cancelToken) == null)
                {
                    var document = new SongDocument(song);
                    await songCollection.InsertOneAsync(document, null, cancelToken);
                    return document._id.ToString();
                }
                    throw new Top100Exception(ReasonType.Conflict);
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
                throw new Top100Exception(ReasonType.Unknown);
            }
        }

        public async Task<string> CreateOrUpdateAsync(Song song, CancellationToken cancelToken)
        {
            try
            {
                var songDb = await Find(song, cancelToken);
                if (songDb == null)
                {
                    var document = new SongDocument(song);
                    await songCollection.InsertOneAsync(document, null, cancelToken);
                    return document._id.ToString();
                }
                else
                {
                    if (!songDb.Equals(song))
                    {
                        var id = await UpdateAsync(song, cancelToken);
                        if (id != songDb._id.ToString())
                        {
                            throw new Top100Exception(ReasonType.Conflict);
                        }
                    }
                    return songDb._id.ToString();
                }
            }
            catch (MongoException e)
            {
                Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
                throw new Top100Exception(ReasonType.Unknown);
            }
        }

        public async Task<Song> ReadAsync(int year, int number, CancellationToken cancelToken)
        {
            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;

            var yearFilter = builder.Eq(x => x.Song.Year, year);
            filter = builder.And(yearFilter, filter);

            var numberFilter = builder.Eq(x => x.Song.Number, number);
            filter = builder.And(numberFilter, filter);

            try
            {
                var songDb = await songCollection.Find(filter).FirstAsync(cancelToken);
                return songDb.Song;
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
        public async Task<Song> DeleteAsync(int year, int number, CancellationToken cancelToken)
        {
            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;

            var yearFilter = builder.Eq(x => x.Song.Year, year);
            filter = builder.And(yearFilter, filter);

            var numberFilter = builder.Eq(x => x.Song.Number, number);
            filter = builder.And(numberFilter, filter);

            try
            {
                var document = await songCollection.Find(filter).FirstAsync(cancelToken);
                var result = songCollection.DeleteOne(x => x._id == document._id, cancelToken);
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

        public async Task<string> UpdateAsync(Song song, CancellationToken cancelToken)
        {
            var dbSong = await Find(song, cancelToken);
            if (dbSong != null)
            {
                if (!dbSong.Equals(song))
                {
                    try
                    {
                        var result = await songCollection.ReplaceOneAsync(x => x._id == dbSong._id, new SongDocument(dbSong._id, song), new UpdateOptions {IsUpsert = false}, cancelToken);
                        if (result.ModifiedCount == 1)
                        {
                            return dbSong._id.ToString();
                        }

                        Console.WriteLine($"ERROR:  Invalid result modified={result.ModifiedCount}, matched={result.MatchedCount}");
                        throw new Top100Exception(ReasonType.Unknown);
                    }
                    catch (MongoException e)
                    {
                        Console.WriteLine($"Mongo Exception in Insert.  ex={e}");
                        throw new Top100Exception(ReasonType.Unknown);
                    }
                }
                else
                {
                    return dbSong._id.ToString();
                }
            }
            Console.WriteLine($"Warning Song not found to update title={song.Title}, artist={song.Artist}, year={song.Year}, number={song.Number}");
            throw new Top100Exception(ReasonType.NotFound);
        }

        public async Task<IList<Song>> SearchAsync(string titleFilterString, string artistFilterString, string yearFilterString, string numberFilterString, string ownFilterString, CancellationToken cancelToken)
        {
            var builder = Builders<SongDocument>.Filter;
            var filter = builder.Empty;
            var retList = new List<Song>();

            if (!string.IsNullOrWhiteSpace(titleFilterString))
            {
                var titleFilter = builder.Regex(x => x.Song.Title, new BsonRegularExpression(titleFilterString, "i"));
                filter = builder.And(titleFilter, filter);
            }
            if (!string.IsNullOrWhiteSpace(artistFilterString))
            {
                var artistFilter = builder.Regex(x => x.Song.Artist, new BsonRegularExpression(artistFilterString, "i"));
                filter = builder.And(artistFilter, filter);
            }
            if (!string.IsNullOrWhiteSpace(yearFilterString))
            {
                if (int.TryParse(yearFilterString, out var year))
                {
                    var yearFilter = builder.Eq(x => x.Song.Year, year);
                    filter = builder.And(yearFilter, filter);
                }
            }
            if (!string.IsNullOrWhiteSpace(numberFilterString))
            {
                if (int.TryParse(numberFilterString, out var number))
                {
                    var numberFilter = builder.Eq(x => x.Song.Number, number);
                    filter = builder.And(numberFilter, filter);
                }
            }
            if (!string.IsNullOrWhiteSpace(ownFilterString))
            {
                if (bool.TryParse(ownFilterString, out var own))
                {
                    var ownFilter = builder.Eq(x => x.Song.Own, own);
                    filter = builder.And(ownFilter, filter);
                }
            }
            try
            {
                await songCollection.Find(filter).SortBy(x=>x.Song.Year).ThenBy(x=>x.Song.Number).ForEachAsync(x => retList.Add(x.Song), cancelToken);
                Console.WriteLine($"FindAsync found count={retList.Count} documents");
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"FindAsync ex={ex}");
            }

            return retList;
        }

        private async Task<SongDocument> Find(Song song, CancellationToken cancelToken)
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
                if (await cursor.CountDocumentsAsync(cancelToken) == 1)
                    result = await cursor.FirstAsync(cancelToken);
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"ERROR:  error in find.  ex={ex}");
            }

            return result;
        }
    }
}
