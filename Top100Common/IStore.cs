﻿//
// © Copyright 2017,2020 Kevin Pearson
//

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Top100Common
{
    public interface IStore
    {
        Task<string> CreateAsync(Song song, CancellationToken cancelToken);
        Task<string> CreateOrUpdateAsync(Song song, CancellationToken cancelToken);
        Task<Song> GetAsync(int year, int number, CancellationToken cancelToken);
        Task<Song> DeleteAsync(int year, int number, CancellationToken cancelToken);
        Task<string> UpdateAsync(Song song, CancellationToken cancelToken);
        Task<IList<Song>> FindAsync(string titleFilterString, string artistFilterString, string yearFilterString, string numberFilterString, string ownFilterString, CancellationToken cancelToken);
        Task<IList<Song>> FindAllAsync(CancellationToken cancelToken);
    }
}
