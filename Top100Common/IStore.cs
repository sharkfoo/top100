//
// © Copyright 2017 Kevin Pearson
//

using System.Threading.Tasks;

namespace Top100Common
{
    public interface IStore
    {
        Task<string> Insert(Song song);
    }
}
