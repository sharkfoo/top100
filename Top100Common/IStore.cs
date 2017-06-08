//
// © Copyright 2017 Kevin Pearson
//

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Top100Common
{
    public interface IStore
    {
        Task<string> InsertAsync(Song song);
        Task<Song> GetAsync(int year, int number);
        Task<IList<Song>> FindAsync(string titleFilterString, string artistFilterString, string yearFilterString, string numberFilterString, string ownFilterString);
    }
}
