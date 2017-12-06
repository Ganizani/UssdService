using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBD.Capitaldata.Entities;
using MBDInternal.Data;
using NLog;
using MBD.Capitaldata.DAL;
using System.Data.SqlClient;

namespace MBD.Capitaldata.BLL
{
    public class UserLogicLayer : GenericTypeLogicLayer<User>
    {
        public UserLogicLayer(DataAccess da)
            : base(da)
        {

        }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Business rules
        public List<User> GetAllRegisteredUsers()
        {
            User model = new User();
            List<User> models = new List<User>();
           try
           {
               models = Shared.da.ExecuteQuery<User>("sp_SpotsUser_AllRegistered", model);

           }
           catch (SqlException sqlexp)
           {
               if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
           }
           catch (Exception exp)
           {
               if (log.IsErrorEnabled) log.Error(exp.ToString());
           }

           return models;
            
        }

        public override IEnumerable<RuleViolation> GetRuleViolations(User model)
        {
            User searchModel = new User();
            if (!string.IsNullOrEmpty(model.szUserName))
            {
                searchModel.FilterUserName = model.szUserName;
            }
            var searchedModel = Logic.Instance.Users.ListFiltered(searchModel).FirstOrDefault();;
            searchModel = new User();
            if (searchedModel != null)
            {
                if (model.szUserName == searchedModel.szUserName)
                {
                    yield return new RuleViolation("The Username already exist.", "szUserName");
                }
            }
            searchModel = new User();
            if (!string.IsNullOrEmpty(model.szEMail))
            {
                searchModel.FilterEmail = model.szEMail;
            }
            searchedModel = Logic.Instance.Users.ListFiltered(searchModel).FirstOrDefault(); 
            if (searchedModel != null)
            {
                if (model.szEMail == searchedModel.szEMail)
                {
                    yield return new RuleViolation("The Email Address already exist.", "szEMail");
                }
           
            }
            searchModel = new User();
            if (!string.IsNullOrEmpty(model.szIDNumber))
            {
                searchModel.FilterIDNumber = model.szIDNumber;
            }
            searchedModel = Logic.Instance.Users.ListFiltered(searchModel).FirstOrDefault();
            if (searchedModel != null)
            {
                
                if (model.szIDNumber == searchedModel.szIDNumber)
                {
                    yield return new RuleViolation("The Identity Number already exist.", "szIDNumber");
                }
            }
            searchModel = new User();
            if (!string.IsNullOrEmpty(model.szCellNo))
            {
                searchModel.FilterCellNo = model.szCellNo;
            }
            searchedModel = Logic.Instance.Users.ListFiltered(searchModel).FirstOrDefault(); 
            if (searchedModel != null)
            {
                if (model.szCellNo == searchedModel.szCellNo)
                {
                    yield return new RuleViolation("The Cellphone Number already exist.", "szCellNo");
                }
                
            }
            CompanyInfo company = Logic.Instance.CompanyInfos.ListFiltered(new CompanyInfo { FilterCompanyName = model.szCompanyName }).FirstOrDefault();
            
            if(company == null)
            {
                yield return new RuleViolation("The Comapny Name doest not exist.", "szCompanyName");
            }
            else
            {
                model.wCompanyID = company.wID;
            }
            yield break;
        }
        #endregion
    }
}
