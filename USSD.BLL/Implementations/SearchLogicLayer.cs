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
    public class SearchLogicLayer : GenericTypeLogicLayer<Search>
    {
        public SearchLogicLayer(DataAccess da)
            : base(da)
        {

        }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        #region Method that Return Scalar value

        public List<Search> GetRegions()
        {
            Search searchInstruction = new Search();
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("sp_SpotsSearch_Region", searchInstruction);

            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }

        public List<Search> GetSuburbs(int companyId)
        {
            Search searchInstruction = new Search { FilterTracerId = companyId };
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("sp_SpotsSearch_Suburb", searchInstruction);

            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetTowns(int companyId)
        {
            Search searchInstruction = new Search { FilterTracerId = companyId };
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("sp_SpotsSearch_Town", searchInstruction);

            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetTownsForReport(Search model)
        {
            
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("rpt_SpotsSearch_Town", model);

            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetTownsForReportDetails(Search model)
        {

            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("rpt_SpotsSearch_ListWithoutReference", model);

            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetEmployers(int companyId)
        {
            Search searchInstruction = new Search { FilterTracerId = companyId };
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("sp_SpotsSearch_Employer", searchInstruction);

            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<CompanyInfo> GetCompanyInfo(int companyId)
        {
            CompanyInfo searchInstruction = new CompanyInfo { wID= companyId };
            List<CompanyInfo> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<CompanyInfo>("sp_SpotsCompanyInfo_LocationInfo", searchInstruction);

            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetAvailableRegions(int companyId)
        {
            Search searchInstruction = new Search {FilterCompanyId= companyId };
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("sp_SpotsSearch_AvailableRegions", searchInstruction);
            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetSelectedRegions(int companyId)
        {
            Search searchInstruction = new Search {FilterCompanyId= companyId };
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("sp_SpotsSearch_SelectedRegions", searchInstruction);
            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetLocationLookUps(Search model)
        {
            
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("sp_SpotsSearch_LocationLookUp", model);
            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public List<Search> GetExpiringReport(Search model)
        {
            
            List<Search> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<Search>("rpt_SpotsSearch_Expiring", model);
            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        
        public List<BankDetail> GetAccountTypes()
        {

            List<BankDetail> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<BankDetail>("sp_SpotsAccountType", null);
            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }

        public List<BankDetail> GetFeedBackBankDetails(string referenceNumber)
        {
            BankDetail searchModel = new BankDetail { FilterReferenceNumber = referenceNumber.Trim() };
            List<BankDetail> result = new List<BankDetail>();
            try
            {
                result = Shared.da.ExecuteQuery<BankDetail>("sp_SpotsAccountType", searchModel);
            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }
            return result;
        }
        public int GetGlobalSearchCount(Search model)
        {
        
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsSearch_Count", model);
            }
            catch (SqlException sqlexp)
            {
                if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
            }
            catch (Exception exp)
            {
                if (log.IsErrorEnabled) log.Error(exp.ToString());
            }

            return result;
        }

        #endregion
    }
}
