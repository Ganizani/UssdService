using MBDInternal.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USSD.DAL
{
    public class Repository<T> : IRepository<T> where T:class
    {
        DataAccess da= Shared.da;

        public Repository(DataAccess dataAccess)
        {
            this.da = dataAccess;
        }

        public void Delete(T model)
        {
            da.Delete<T>(model);
        }

        public List<T> List()
        {
            return da.List<T>();
        }

        public List<T> ListFiltered(T model)
        {
            return da.List<T>(model);
        }

        public T Fetch(T model)
        {
            return da.Fetch<T>(model);
        }

        public void Insert(T model)
        {
            da.Insert<T>(model);
        }

        public void Update(T model)
        {
            da.Update<T>(model);
        }
    }
}
