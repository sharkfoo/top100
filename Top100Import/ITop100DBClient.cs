//
// © Copyright 2017 Kevin Pearson
//

using System.Threading.Tasks;

namespace Top100Import
{
    public interface ITop100DBClient
    {
        Task<bool> Insert(Song song);
    }
}
