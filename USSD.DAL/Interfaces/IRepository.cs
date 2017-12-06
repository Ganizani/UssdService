using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.DAL
{
    public interface IRepository<T> where T : class
    {
        void Delete(T Entity);
        List<T> List();
        List<T> ListFiltered(T Entity);
        T Fetch(T Entity);
        void Insert(T Entity);
        void Update(T Entity);

    }
}
