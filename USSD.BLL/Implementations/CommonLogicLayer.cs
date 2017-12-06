using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USSD.Entities;
using MBDInternal.Data;
using NLog;
using USSD.DAL;
using System.Data.SqlClient;
namespace USSD.BLL
{
    public class CommonLogicLayer
    {
        public CommonLogicLayer(DataAccess da)
        {

        }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Method that Return Scalar value
        public class TempModel
        {
            [DataField]
            public int Days { get; set; }
            [DataField]
            public string WindowPeriod { get; set; }
            [DataField]
            public string FullString { get; set; }

        }

        public DateTime? GetWorkingDay(int days)
        {
            var model = new TempModel { Days = days };
            DateTime? result = default(DateTime);
            try
            {
                result = (DateTime)Shared.da.ExecuteScalarQuery("sp_GetWorkingDay", model);

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

        public DateTime? DetermineStrikeDate(string fullString, string windowPeriod)
        {
            var model = new TempModel { FullString = fullString, WindowPeriod = windowPeriod };
            DateTime? result = default(DateTime);
            try
            {
                var tempResult = Shared.da.ExecuteScalarQuery("spc_DetermineStrikeDate", model);
                if (tempResult != null)
                {
                    result = DateTime.Parse(tempResult.ToString());
                }

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

        //public bool ValidateBankAccount(string branchCode, string accountNumber, int accountType)
        //{
        //    var model = new BankDetailsValidation { BranchCode = branchCode, AccountNo = accountNumber, AccountType = accountType };
        //    bool result = false;
        //    try
        //    {
        //        var tempResult = Shared.da.ExecuteScalarQuery("sp_SPOTS_ValidateBankAccount", model);
        //        if (tempResult != null)
        //        {
        //            result = bool.Parse(tempResult.ToString());
        //        }

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }
        //    return result;
        //}

        //public List<string> GetAllEmployerNames()
        //{
        //    Employer searchModel = new Employer();
        //    List<string> result = new List<string>();
        //    try
        //    {
        //        var model = Shared.da.ExecuteQuery<Employer>("sp_Employer_Names_List", searchModel);
        //        if (model != null && model.Count > 0)
        //        {
        //            result = model.Select(n => n.szEmployerName).ToList();
        //        }

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }
        //    return result;
        //}
        //public void SetRateTracePackFeedBackCorrect(string referenceNumber)
        //{
        //    Instruction searchModel = new Instruction { szReferenceNumber = referenceNumber };

        //    try
        //    {
        //        Shared.da.ExecuteNonQuery("SetRateTracePackFeedBackCorrect", searchModel);


        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //}
        //public List<User> GetSLAReceivedUsers()
        //{
        //    Employer searchModel = new Employer();
        //    List<User> result = new List<User>();
        //    try
        //    {
        //        result = Shared.da.ExecuteQuery<User>("sp_SpotsUser_SLAReceived", searchModel);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }
        //    return result;

        //}
        //public void UpdateSLAReceivedUsers(int userId, string sendEmail, string sendPRN, string sendUserIdPswd, string sendSLAReceived, string sendSMS)
        //{
        //    SendEMailSMS searchModel = new SendEMailSMS
        //    {
        //        EMail = sendEmail,
        //        SendPRN = sendPRN,
        //        SendUserIDPswd = sendUserIdPswd,
        //        SLAReceived = sendSLAReceived,
        //        SMS = sendSMS,
        //        wUserID = userId
        //    };

        //    try
        //    {
        //        Shared.da.ExecuteNonQuery("sp_SPOTS_SendEMailSMS", searchModel);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }


        //}
        //public List<InstructionDetail> GetFeedBackNoTraces()
        //{
        //    var searchModel = new InstructionDetail();
        //    List<InstructionDetail> models = new List<InstructionDetail>();
        //    try
        //    {
        //        models = Shared.da.List<InstructionDetail>(searchModel);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}

        public List<AgeAnalysise> GetAgeAnalysis()
        {
            var searchModel = new AgeAnalysise();
            List<AgeAnalysise> models = new List<AgeAnalysise>();
            try
            {
                models = Shared.da.ExecuteQuery<AgeAnalysise>("sp_AgeAnalysis", searchModel);

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
        public List<AgeAnalysisDetail> GetAgeAnalysisDetails(AgeAnalysisDetail model)
        {

            List<AgeAnalysisDetail> models = new List<AgeAnalysisDetail>();
            try
            {
                models = Shared.da.ExecuteQuery<AgeAnalysisDetail>("sp_SpotsInstructionsTransactions_Count", model);

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
        //public List<RegionCode> GetRegionCodeByTracerId(int tracerId)
        //{
        //    RegionCode model = new RegionCode { ID = tracerId };
        //    List<RegionCode> models = new List<RegionCode>();
        //    try
        //    {
        //        models = Shared.da.ExecuteQuery<RegionCode>("sp_SPOTS_GetCompanyLocations", model);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}
        //public List<CompanyInfo> GetCompanyLoadBalancing(int? tracerId)
        //{
        //    CompanyInfo model = new CompanyInfo { FilterId = tracerId ?? -1 };
        //    List<CompanyInfo> models = new List<CompanyInfo>();
        //    try
        //    {
        //        models = Shared.da.ExecuteQuery<CompanyInfo>("rpt_SpotsCompanyInfo_LoadBalancing", model);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}
        //public List<GridReport> GetGridReports()
        //{
        //    GridReport model = new GridReport { };
        //    List<GridReport> models = new List<GridReport>();
        //    try
        //    {
        //        models = Shared.da.ExecuteQuery<GridReport>("sp_SPOTS_GridReport_EDB", model);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}
        //public List<GridReport> GetGridReportForTown(int regionCode)
        //{
        //    GridReport model = new GridReport { SelectedRegionCode = regionCode };
        //    List<GridReport> models = new List<GridReport>();
        //    try
        //    {
        //        models = Shared.da.ExecuteQuery<GridReport>("sp_SPOTS_GridReport_EDB_Towns", model);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}
        //public List<RegionCode> GetAllRegionNames(int regionCode)
        //{
        //    RegionCode model = new RegionCode { FilterID = regionCode };
        //    List<RegionCode> models = new List<RegionCode>();
        //    try
        //    {
        //        models = Shared.da.ExecuteQuery<RegionCode>("sp_Spots_RegionCodesLookUp", model);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}
        //public List<Search> GetGridReportByRegionAndType(int regionCode, int gridTypeId)
        //{
        //    Search model = new Search { RegionCode = regionCode, GridType = gridTypeId };
        //    List<Search> models = new List<Search>();
        //    try
        //    {
        //        models = Shared.da.ExecuteQuery<Search>("sp_SPOTS_GridReport_EDB_Details_PerType_PerRegion", model);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}
        //public List<Search> GetCurrentTracerPacksReport(int tracerId, int transactionCode, string referenceNumber)
        //{
        //    Search model = new Search { FilterTracerId = tracerId, FilterTransactionCode = transactionCode};
        //    if (!string.IsNullOrEmpty(referenceNumber))
        //    {
        //        model.FilterReferenceNumber = referenceNumber;
        //    }
            
        //    List<Search> models = new List<Search>();
        //    try
        //    {
        //        models = Shared.da.ExecuteQuery<Search>("rpt_SpotsInstructions_CurrentTracepack", model);

        //    }
        //    catch (SqlException sqlexp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(sqlexp.ToString());
        //    }
        //    catch (Exception exp)
        //    {
        //        if (log.IsErrorEnabled) log.Error(exp.ToString());
        //    }

        //    return models;
        //}
        public List<AgeAnalysisForTracer> GetAgeAnalysisReportForTracer(AgeAnalysisForTracer model)
        {

            List<AgeAnalysisForTracer> models = new List<AgeAnalysisForTracer>();
            try
            {
                models = Shared.da.ExecuteQuery<AgeAnalysisForTracer>("rpt_SpotsTracerAgeAnalysis", model);

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
        
        private class BankDetailsValidation
        {
            [DataField]
            public string BranchCode { get; set; }
            [DataField]
            public string AccountNo { get; set; }
            [DataField]
            public int AccountType { get; set; }

        }
        private class SendEMailSMS
        {
            [DataField]
            public string EMail { get; set; }
            [DataField]
            public string SMS { get; set; }
            [DataField]
            public int wUserID { get; set; }
            [DataField]
            public string SendPRN { get; set; }
            [DataField]
            public string SendUserIDPswd { get; set; }
            [DataField]
            public string SLAReceived { get; set; }

        }

        #region Model for reporting purpose only
        public class AgeAnalysise
        {
            [DataField]
            public string szTracerName { get; set; }
            [DataField]
            public int wTracerID { get; set; }
            [DataField]
            public int Age0_30 { get; set; }
            [DataField]
            public int Age31_60 { get; set; }
            [DataField]
            public int Age61_90 { get; set; }
            [DataField]
            public int Age91P { get; set; }

        }

        public class AgeAnalysisDetail
        {
            [DataField]
            public string szTransactionDescription { get; set; }
            [DataField]
            public int wCount { get; set; }
            [DataField]
            public int FromDay { get; set; }
            [DataField]
            public int ToDay { get; set; }
            [DataField]
            public int FilterTracerID { get; set; }
            [DataField]
            public int FilterToDay { get; set; }
            [DataField]
            public int FilterFromDay { get; set; }


        }

        public class AgeAnalysisForTracer
        {
            [DataField]
            public string szReferenceNumber { get; set; }
            [DataField]
            public int wRegionCode { get; set; }
            [DataField]
            public string szIDNumber { get; set; }
            [DataField]
            public int wTracerID { get; set; }
            [DataField]
            public DateTime dtTransactionDate { get; set; }
            [DataField]
            public bool bAllocated { get; set; }
            [DataField]
            public int wTransactionCode { get; set; }
            [DataField]
            public string szRegionName { get; set; }
            [DataField]
            public DateTime dtHandOverDate { get; set; }
            [DataField]
            public string szCompanyName { get; set; }
             [DataField]
            public DateTime wNumDaysSinceHandOver { get; set; }
            [DataField]
             public DateTime wNumDaysSincelastTran { get; set; }
             [DataField]
            public string szLastTransaction { get; set; }

            [DataField]
            public int FilterTracerId { get; set; }
            [DataField]
            public int FilterTransactionCode { get; set; }
            
                
    }
        

        
        #endregion

        #endregion
    }
}
