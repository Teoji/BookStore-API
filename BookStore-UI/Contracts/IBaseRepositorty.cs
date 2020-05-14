using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Contracts
{
    public interface IBaseRepositorty<T>  where T : class
    {
        Task<T> Get(string URL, int id);
        Task<IList<T>> Get(string URL);

        Task<bool> Create(string URL, T obj);

        Task<bool> Update(string URL, T obj);

        Task<bool> Delete(string URL, int id);
    }
}
