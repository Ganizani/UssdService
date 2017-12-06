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
    public class InstructionLogicLayer : GenericTypeLogicLayer<Instruction>
    {
        public InstructionLogicLayer(DataAccess da)
            : base(da)
        {

        }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Method that Return Scalar value

        public int GetIncompleteFeedBack(int searchParam)
        {
            Instruction searchInstruction = new Instruction { FilterFeedBack = searchParam };
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_IncompleteFeedBack", searchInstruction);

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

        public int GetExpiringSoon(int searchParam)
        {
            Instruction searchInstruction = new Instruction { FilterFeedBack = searchParam };
            int result = 0;
            try
            {

                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_ExpiringSoon", searchInstruction);

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
        public int GetAllocated(int searchParam)
        {
            Instruction searchInstruction = new Instruction { FilterFeedBack = searchParam };
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_Allocated", searchInstruction);
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
        public int GetUnAllocated()
        {
            Instruction searchInstruction = new Instruction();
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_UnAllocated", searchInstruction);
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

        public int GetReserved(int searchParam)
        {
            Instruction searchInstruction = new Instruction { FilterFeedBack = searchParam };
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_Reserved", searchInstruction);
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
        
        public int GetAllInstructionsCount()
        {
            Instruction searchInstruction = new Instruction ();
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_TotalCount", searchInstruction);
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
        
        public List<Search> GetLocation(int tracerId)
        {
            Search searchInstruction = new Search();
            if (tracerId > 0)
                searchInstruction.FilterTracerId = tracerId;

            List<Search> result = null;
            try
            {
                var workItem = new DataAccessWorkItem
                {
                    CommandText = "sp_SpotsSearch_GetLocation",
                    CommandTimeOut = 300,
                    CommandType = System.Data.CommandType.StoredProcedure,
                    ReadResults = true


                };
                result = Shared.da.ExecuteUniversal<Search>(searchInstruction, workItem);
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
        public int GetFeedback(int searchParam)
        {
            Instruction searchInstruction = new Instruction { FilterFeedBack = searchParam };
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_Feedback", searchInstruction);
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
        public int GetCurrentTracepack(int searchParam)
        {
            Instruction searchInstruction = new Instruction { FilterFeedBack = searchParam };
            int result = 0;
            try
            {
                result = (int)Shared.da.ExecuteScalarQuery("sp_SpotsInstructions_CurrentTracepack", searchInstruction);
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
        public int ProcessInstructionAllDetails(string referenceNumber)
        {
            Instruction searchInstruction = new Instruction { FilterReferenceNumber= referenceNumber};
            int result = 0;
            try
            {
                result = Shared.da.ExecuteNonQuery("sp_Badger_InstructionAllDetails", searchInstruction);
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
        public List<InstructionDetail> ShowDetails(string referenceNumber)
        {
            InstructionDetail searchInstruction = new InstructionDetail { FilterReferenceNumber = referenceNumber };
            List<InstructionDetail> result = null;
            try
            {
                result = Shared.da.ExecuteQuery<InstructionDetail>("sp_SPOTS_Instructions_ShowDetails", searchInstruction);
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
        public List<FeedBack> GetFeedBackOptions()
        {
            FeedBack model = new FeedBack();
            List<FeedBack> result = new List<FeedBack>();
            try
            {
               result = Shared.da.ExecuteQuery<FeedBack>("sp_SpotsFeedBack_Option", model);
              
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

        public List<InstructionDetail> GetPreTracers(string referenceNumber)
        {
            InstructionDetail model = new InstructionDetail { FilterReferenceNumber = referenceNumber};
            List<InstructionDetail> result = new List<InstructionDetail>();
            try
            {
                result = Shared.da.ExecuteQuery<InstructionDetail>("sp_SpotsInstructions_PreTracers", model);
              
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
        public Instruction GetCurrentStatusFeedBack(string referenceNumber)
        {
            Instruction model = new Instruction{ FilterReferenceNumber = referenceNumber};
            Instruction result = new Instruction ();
            try
            {
                result = Shared.da.Fetch<Instruction>("sp_SpotsInstructions_CurrentFeedBack", model);
              
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

        public Instruction GetCurrentStatus(string referenceNumber)
        {
            Instruction model = new Instruction{ FilterReferenceNumber = referenceNumber};
            Instruction result = new Instruction ();
            try
            {
                result = Shared.da.Fetch<Instruction>("sp_SpotsInstructions_CurrentStatus", model);
              
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

        public List<Instruction> GetInstrunctionHistories(string referenceNumber)
        {
            Instruction model = new Instruction { FilterReferenceNumber = referenceNumber };
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("sp_SpotsInstructions_History", model);

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

        public List<Instruction> GetInstrunctionNoYetValidated()
        {
            Instruction model = new Instruction ();
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("rpt_SpotsInstruction_NoYetValidated", model);

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
        public List<Instruction> GetValidatedUserByDate(DateTime fromDate, DateTime toDate, string user)
        {
            Instruction model = new Instruction { FromDate = fromDate, ToDate = toDate, User = user};
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("sp_SPOTS_ShowValidatedByUser", model);

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
        public List<FeedBack> GetFeedBackAnalysisDetailedReport(DateTime fromDate, DateTime toDate, int tracerId, int transactionCode)
        {
            Instruction model = new Instruction { FromDate = fromDate, ToDate = toDate, wTracerID = tracerId ,wTransactionCode = transactionCode};
            List<FeedBack> result = new List<FeedBack>();
            try
            {
                result = Shared.da.ExecuteQuery<FeedBack>("rpt_SpotsInstructions_FeedBackAnalisisDetails", model);

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
        
        public List<Instruction> GetValidatedUserByDateDetails(DateTime fromDate, DateTime toDate, string user, int id)
        {
            Instruction model = new Instruction { FromDate = fromDate, ToDate = toDate, User = user , Transaction = id};
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("sp_SPOTS_ShowValidatedByUserReference", model);

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
        public List<Instruction> GetInstructionsStatusBreakDown(int? tracerId)
        {
            Instruction model = new Instruction { wTracerID = tracerId??-1};
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("rpt_SpotsInstructions_StatusBreakDown", model);

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
        public List<Instruction> GetInstructionsStatusBreakDownByDate(int? tracerId, DateTime fromDate, DateTime toDate)
        {
            Instruction model = new Instruction { wTracerID = tracerId ?? -1 , FromDate = fromDate, ToDate = toDate};
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("rpt_SpotsInstructions_StatusBreakDownByDate", model);

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
        public List<Instruction> GetInstructionsStatusBreakDownByDateAndTracer(int tracerId, DateTime fromDate, DateTime toDate)
        {
            Instruction model = new Instruction { wTracerID = tracerId , FromDate = fromDate, ToDate = toDate};
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("rpt_SpotsInstructions_StatusBreakDownByDateAndTracer", model);

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
        
        public List<Instruction> GetInstructionsStatusBreakDownByTracer(int tracerId)
        {
            Instruction model = new Instruction {wTracerID  = tracerId};
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("rpt_SpotsInstructions_StatusBreakDownByTracer", model);

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
        public List<Instruction> GetFeedbackAnalusisByTracer(Instruction model)
        {
            
            List<Instruction> result = new List<Instruction>();
            try
            {
                result = Shared.da.ExecuteQuery<Instruction>("rpt_SpotsFeedBack_Analysis", model);

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
