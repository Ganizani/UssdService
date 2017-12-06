using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USSD.DAL;
using USSD.Entities;
using MBDInternal.Data;

namespace USSD.BLL
{
    public class GenericTypeLogicLayer<T> where T : class
    {
        private IRepository<T> m_repo = null;
        public GenericTypeLogicLayer(DataAccess da) : this(new Repository<T>(da)) { }

        public GenericTypeLogicLayer(IRepository<T> repository)
            : base()
        {
            m_repo = repository;
        }

        
        public Type RepoType
        {
            get
            {
                return m_repo.GetType();
            }
        }
        public virtual List<T> List()
        {
            return m_repo.List();
        }
        public virtual List<T> ListFiltered(T model)
        {
            return m_repo.ListFiltered(model);
        }
        
        public virtual T Fetch(T model)
        {
            return m_repo.Fetch(model);
        }
        public virtual void Insert(T modelItem)
        {
             m_repo.Insert(modelItem);
            
        }

        public virtual void Delete(T modelItem)
        {
             m_repo.Delete(modelItem);
            
        }

        public virtual void Update(T modelItem)
        {
             m_repo.Update(modelItem);
            
        }

       
        public virtual IEnumerable<RuleViolation> GetRuleViolations(T model)
        {

            //all the business rules should be enforced here
            yield break;
        }

    }
}
