using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Root.Models.Utils;
using Root.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Data.UnitOfWork
{
    public class ProcUOM : IDisposable
    {
        #region Private member variables

        private readonly IConfiguration _configuration = null;

        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public ProcUOM(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Public Procedure Methods

        private SqlConnection GetDbConnection<TDbContext>(bool ReadOnly = false) where TDbContext : DbContext
        {
            string value = (string)typeof(ProgConstants).GetField(typeof(TDbContext).Name).GetValue(null);
            ConnHelper connHelper = new ConnHelper();
            string connectionString = connHelper.GetDBConn(_configuration, value);
            if (ReadOnly)
                connectionString += ";ApplicationIntent=ReadOnly;";
            return new SqlConnection(connectionString);
        }

        public async Task<T> ExecuteScalarAsync<TDbContext, T>(string SpName, List<Microsoft.Data.SqlClient.SqlParameter> parameters = null, bool ReadOnly = false, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            T obj = default(T);
            System.Data.Common.DbConnection conn = GetDbConnection<TDbContext>(ReadOnly);
            try
            {
                using (System.Data.Common.DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SpName;
                    cmd.CommandTimeout = QueryTimeOut;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Open();
                    }

                    object t = await cmd.ExecuteScalarAsync();


                    if (t != DBNull.Value)
                    {
                        obj = (T)t;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                //conn.Close();
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return obj;
        }

        public async Task<int> ExecuteNonQueryAsync<TDbContext>(string SpName, List<Microsoft.Data.SqlClient.SqlParameter> parameters = null, bool ReadOnly = false, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            int i = 0;
            System.Data.Common.DbConnection conn = GetDbConnection<TDbContext>(ReadOnly);
            try
            {
                using (System.Data.Common.DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SpName;
                    cmd.CommandTimeout = QueryTimeOut;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Open();
                    }
                    i = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            return i;
        }

        private async Task<DataTable> ExecuteSPDataTableAsync<TDbContext>(string SpName, List<Microsoft.Data.SqlClient.SqlParameter> parameters = null, bool ReadOnly = false, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            DataTable dt = new DataTable();
            System.Data.Common.DbConnection conn = GetDbConnection<TDbContext>(ReadOnly);
            try
            {
                using (System.Data.Common.DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SpName;
                    cmd.CommandTimeout = QueryTimeOut;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Close();
                        conn.Open();
                    }

                    using (System.Data.Common.DbDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dt.Load(reader);
                    }
                    conn.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public async Task<List<T>> ExecuteSPAsync<TDbContext, T>(string SpName, List<Microsoft.Data.SqlClient.SqlParameter> parameters = null, bool ReadOnly = false, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            List<T> result = new List<T>();
            try
            {
                DataTable dt = await ExecuteSPDataTableAsync<TDbContext>(SpName, parameters, ReadOnly, QueryTimeOut);
                if (dt != null)
                {
                    result = CommonLib.ConvertJsonToObject<List<T>>(CommonLib.ConvertObjectToJson(dt));
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> ExecuteSPtoCSVAsync<TDbContext, T>(string SpName, string FileName, List<Microsoft.Data.SqlClient.SqlParameter> parameters = null, bool appendHeader = true, bool ReadOnly = false, string selectedColumList = null, int QueryTimeOut = 60) where TDbContext : DbContext
        {
            bool Success = false;
            try
            {
                List<T> result = await ExecuteSPAsync<TDbContext, T>(SpName, parameters, ReadOnly, QueryTimeOut);
                await ToCSV(result, FileName, appendHeader, selectedColumList);
                Success = true;
                return Success;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task ToCSV<T>(List<T> result, string strFilePath, bool AppendHeader = false, string selectedColumList = null)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = AppendHeader,
                Delimiter = ",",
                Encoding = Encoding.UTF8,
            };

            using (var writer = new StreamWriter(strFilePath, AppendHeader ? false : true))
            using (var csvWriter = new CsvWriter(writer, csvConfig))
            {
                if (!string.IsNullOrEmpty(selectedColumList))
                {
                    csvWriter.Context.RegisterClassMap(new CustomReportMap(selectedColumList?.Split(',').ToList()));
                }
                await csvWriter.WriteRecordsAsync(result);
            }
        }
        #endregion
    }


    public class CustomReportMap : ClassMap<CustomReport>
    {
        public CustomReportMap(List<string> customOrder)
        {
            foreach (var column in customOrder)
            {
                switch (column)
                {
                    case "State Name":
                        Map(m => m.StateName).Name("State Name");
                        break;
                    case "Application No":
                        Map(m => m.ApplicationNo).Name("Application No");
                        break;
                    case "Name":
                        Map(m => m.Name).Name("Name");
                        break;
                    case "Stage 1 Status":
                        Map(m => m.Stage1Status).Name("Stage 1 Status");
                        break;
                    case "District Name":
                        Map(m => m.DistrictName).Name("District Name");
                        break;
                    case "Block Name":
                        Map(m => m.BlockName).Name("Block Name");
                        break;
                    case "GramPanchayat Name":
                        Map(m => m.GramPanchayatName).Name("GramPanchayat Name");
                        break;
                    case "ULBName":
                        Map(m => m.ULBName).Name("ULBName");
                        break;
                    case "Trade Name":
                        Map(m => m.TradeName).Name("Trade Name");
                        break;
                    case "Gender":
                        Map(m => m.Gender).Name("Gender");
                        break;
                    case "Category":
                        Map(m => m.Category).Name("Category");
                        break;
                    case "Divyangjan YES/NO":
                        Map(m => m.Divyangjan).Name("Divyangjan YES/NO");
                        break;
                    case "Divyangjan Type":
                        Map(m => m.DivyangjanType).Name("Divyangjan Type");
                        break;
                    case "Minority YES/NO":
                        Map(m => m.Minority).Name("Minority YES/NO");
                        break;
                    case "Minority Type":
                        Map(m => m.MinorityType).Name("Minority Type");
                        break;
                    case "Loan Required YES/NO":
                        Map(m => m.LoanRequired).Name("Loan Required YES/NO");
                        break;
                    case "Marital Status":
                        Map(m => m.MaritalStatus).Name("Marital Status");
                        break;
                    case "Stage 1 Action Date":
                        Map(m => m.Stage1ActionDate).Name("Stage 1 Action Date");
                        break;
                    case "Stage 2 Status":
                        Map(m => m.Stage2Status).Name("Stage 2 Status");
                        break;
                    case "Stage 2 Action Date":
                        Map(m => m.Stage2ActionDate).Name("Stage 2 Action Date");
                        break;
                    case "Stage 3 Status":
                        Map(m => m.Stage3Status).Name("Stage 3 Status");
                        break;
                    case "Stage 3 Action Date":
                        Map(m => m.Stage3ActionDate).Name("Stage 3 Action Date");
                        break;
                    case "DOB":
                        Map(m => m.DOB).Name("DOB");
                        break;
                    case "Saving Acount Verification status(At Branch)":
                        Map(m => m.AVStage1Status).Name("Saving Acount Verification status(At Branch)");
                        break;
                    case "MobileNo":
                        Map(m => m.MobileNo).Name("MobileNo");
                        break;
                    case "Branch Action date":
                        Map(m => m.AVStage1ActionDate).Name("Branch Action date");
                        break;
                    case "Saving Acount Verification status(At SLBC)":
                        Map(m => m.AVStage2Status).Name("Saving Acount Verification status(At SLBC)");
                        break;
                    case "SLBC Action date":
                        Map(m => m.AVStage2ActionDate).Name("SLBC Action date");
                        break;
                    case "Skill Verification YES/NO":
                        Map(m => m.SkillVerification).Name("Skill Verification YES/NO");
                        break;
                    case "Basic Training YES/NO":
                        Map(m => m.BasicTraining).Name("Basic Training YES/NO");
                        break;
                    case "UAP Certificate YES/NO":
                        Map(m => m.UAPCertificate).Name("UAP Certificate YES/NO");
                        break;
                    case "CRIFF YES/NO":
                        Map(m => m.Criffreport).Name("CRIFF YES/NO");
                        break;
                    case "E-Voucher YES/NO":
                        Map(m => m.EVoucherIssued).Name("E-Voucher YES/NO");
                        break;
                    case "PMV Certificate YES/NO":
                        Map(m => m.PMVCertificateprinted).Name("PMV Certificate YES/NO");
                        break;
                    case "Total Enrollments":
                        Map(m => m.TotalEnrollments).Name("Total Enrollments");
                        break;
                    case "Stage 1 Verification":
                        Map(m => m.Stage1Verification).Name("Stage 1 Verification");
                        break;
                    case "Stage 2 Verification":
                        Map(m => m.Stage2Verification).Name("Stage 2 Verification");
                        break;
                    case "Stage 3 Verification":
                        Map(m => m.Stage3Verification).Name("Stage 3 Verification");
                        break;
                    case "Branch Verification":
                        Map(m => m.BranchVerification).Name("Branch Verification");
                        break;
                    case "SLBC Verification":
                        Map(m => m.SLBCVerification).Name("SLBC Verification");
                        break;
                    case "Stage 1 Approved":
                        Map(m => m.Stage1Approved).Name("Stage 1 Approved");
                        break;
                    case "Stage 1 Rejected":
                        Map(m => m.Stage1Rejected).Name("Stage 1 Rejected");
                        break;
                    case "Stage 1 Pending":
                        Map(m => m.Stage1Pending).Name("Stage 1 Pending");
                        break;
                    case "Stage 2 Approved":
                        Map(m => m.Stage2Approved).Name("Stage 2 Approved");
                        break;
                    case "Stage 2 Rejected":
                        Map(m => m.Stage2Rejected).Name("Stage 2 Rejected");
                        break;
                    case "Stage 2 Pending":
                        Map(m => m.Stage2Pending).Name("Stage 2 Pending");
                        break;
                    case "Stage 3 Approved":
                        Map(m => m.Stage3Approved).Name("Stage 3 Approved");
                        break;
                    case "Stage 3 Rejected":
                        Map(m => m.Stage3Rejected).Name("Stage 3 Rejected");
                        break;
                    case "Stage 3 Pending":
                        Map(m => m.Stage3Pending).Name("Stage 3 Pending");
                        break;
                    case "Branch Approved":
                        Map(m => m.BranchApproved).Name("Branch Approved");
                        break;
                    case "Branch Rejected":
                        Map(m => m.BranchRejected).Name("Branch Rejected");
                        break;
                    case "Branch Pending":
                        Map(m => m.BranchPending).Name("Branch Pending");
                        break;
                    case "SLBC Approved":
                        Map(m => m.SLBCApproved).Name("SLBC Approved");
                        break;
                    case "SLBC Rejected":
                        Map(m => m.SLBCRejected).Name("SLBC Rejected");
                        break;
                    case "SLBC Pending":
                        Map(m => m.SLBCPending).Name("SLBC Pending");
                        break;
                    case "Loan Required":
                        Map(m => m.LoanRequiredCount).Name("Loan Required");
                        break;
                    case "UAP Certificate":
                        Map(m => m.UAPCertificateCount).Name("UAP Certificate");
                        break;
                    case "Criff report":
                        Map(m => m.CriffreportCount).Name("Criff report");
                        break;
                    case "EVoucherIssued":
                        Map(m => m.EVoucherIssuedCount).Name("EVoucherIssued");
                        break;
                    case "PMV Certificate printed":
                        Map(m => m.PMVCertificateprintedCount).Name("PMV Certificate printed");
                        break;
                    case "Loan Application No":
                        Map(m => m.LoanApplicationNo).Name("ApplicationNo");
                        break;
                    case "Loan Applicant Name":
                        Map(m => m.LoanApplicantName).Name("Name");
                        break;
                    case "Bank Name":
                        Map(m => m.BankName).Name("Bank Name");
                        break;
                    case "Bank Officer Name":
                        Map(m => m.BankOfficerName).Name("Bank Officer Name");
                        break;
                    case "Bank Officer Email":
                        Map(m => m.BankOfficerEmail).Name("Bank Officer Email");
                        break;
                    case "Bank Officer Mobile":
                        Map(m => m.BankOfficerMobile).Name("Bank Officer Mobile");
                        break;
                    case "Sanctioned Date":
                        Map(m => m.SanctionedDate).Name("Sanctioned Date");
                        break;
                    case "Sanctioned Amount":
                        Map(m => m.SanctionedAmount).Name("Sanctioned Amount");
                        break;
                    case "Disbursed Date":
                        Map(m => m.DisbursedDate).Name("Disbursed Date");
                        break;
                    case "Disbursed Amount":
                        Map(m => m.DisbursedAmount).Name("Disbursed Amount");
                        break;
                    case "Loan Status":
                        Map(m => m.LoanStatus).Name("Loan Status");
                        break;
                    case "Rejected Date":
                        Map(m => m.RejectedDate).Name("Rejected Date");
                        break;
                    case "Rejection Reason":
                        Map(m => m.RejectionReason).Name("Rejection Reason");
                        break;
                    case "Rejection Remarks":
                        Map(m => m.RejectionRemarks).Name("Rejection Remarks");
                        break;
                    case "Branch Name":
                        Map(m => m.BranchName).Name("Branch Name");
                        break;
                    case "Total Applications Forwarded To Bank":
                        Map(m => m.TotalApplicationsForwardedToBank).Name("Total Applications Forwarded To Bank");
                        break;
                    case "Total Applications Sanctioned":
                        Map(m => m.TotalApplicationsSanctioned).Name("Total Applications Sanctioned");
                        break;
                    case "Total Sanctioned Amount":
                        Map(m => m.TotalSanctionedAmount).Name("Total Sanctioned Amount");
                        break;
                    case "Total Applications Disbursed":
                        Map(m => m.TotalApplicationsDisbursed).Name("Total Applications Disbursed");
                        break;
                    case "Total Disbursed Amount":
                        Map(m => m.TotalDisbursedAmount).Name("Total Disbursed Amount");
                        break;
                    case "Total Rejected Applications":
                        Map(m => m.TotalRejectedApplications).Name("Total Rejected Applications");
                        break;
                    case "Address":
                        Map(m => m.Address).Name("Address");
                        break;

                }
            }

        }
    }
}
