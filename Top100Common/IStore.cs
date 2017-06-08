//
// © Copyright 2017 Kevin Pearson
//

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Top100Common
{
    public interface IStore
    {
        Task<string> CreateAsync(Song song);
        Task<Song> ReadAsync(int year, int number);
        Task<Song> DeleteAsync(int year, int number);
        Task<string> UpdateAsync(Song song);

        Task<IList<Song>> SearchAsync(string titleFilterString, string artistFilterString, string yearFilterString, string numberFilterString, string ownFilterString);
    }
}
