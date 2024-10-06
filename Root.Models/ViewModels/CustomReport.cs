using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class CustomReport
    {
        [DisplayName("State Name")]
        public string StateName { get; set; }

        [DisplayName("Application No")]
        public string ApplicationNo { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Stage 1 Status")]
        public string Stage1Status { get; set; }

        [DisplayName("District Name")]
        public string DistrictName { get; set; }

        [DisplayName("Block Name")]
        public string BlockName { get; set; }

        [DisplayName("GramPanchayat Name")]
        public string GramPanchayatName { get; set; }

        [DisplayName("ULBName")]
        public string ULBName { get; set; }

        [DisplayName("Trade Name")]
        public string TradeName { get; set; }

        [DisplayName("Gender")]
        public string Gender { get; set; }

        [DisplayName("Category")]
        public string Category { get; set; }

        [DisplayName("Divyangjan YES/NO")]
        public string Divyangjan { get; set; }

        [DisplayName("Divyangjan Type")]
        public string DivyangjanType { get; set; }

        [DisplayName("Minority YES/NO")]
        public string Minority { get; set; }

        [DisplayName("Minority Type")]
        public string MinorityType { get; set; }

        [DisplayName("Loan Required YES/NO")]
        public string LoanRequired { get; set; }

        [DisplayName("Marital Status")]
        public string MaritalStatus { get; set; }

        [DisplayName("Stage 1 Action Date")]
        public string Stage1ActionDate { get; set; }

        [DisplayName("Stage 2 Status")]
        public string Stage2Status { get; set; }

        [DisplayName("Stage 2 Action Date")]
        public string Stage2ActionDate { get; set; }

        [DisplayName("Stage 3 Status")]
        public string Stage3Status { get; set; }

        [DisplayName("Stage 3 Action Date")]
        public string Stage3ActionDate { get; set; }

        [DisplayName("DOB")]
        public DateTime? DOB { get; set; }

        [DisplayName("Saving Acount Verification status(At Branch)")]
        public string AVStage1Status { get; set; }

        [DisplayName("MobileNo")]
        public string MobileNo { get; set; }

        [DisplayName("Branch Action date")]
        public string AVStage1ActionDate { get; set; }

        [DisplayName("Saving Acount Verification status(At SLBC)")]
        public string AVStage2Status { get; set; }

        [DisplayName("SLBC Action date")]
        public string AVStage2ActionDate { get; set; }

        [DisplayName("Skill Verification YES/NO")]
        public string SkillVerification { get; set; }

        [DisplayName("Basic Training YES/NO")]
        public string BasicTraining { get; set; }

        [DisplayName("UAP Certificate YES/NO")]
        public string UAPCertificate { get; set; }

        [DisplayName("CRIFF YES/NO")]
        public string Criffreport { get; set; }

        [DisplayName("E-Voucher YES/NO")]
        public string EVoucherIssued { get; set; }

        [DisplayName("PMV Certificate YES/NO")]
        public string PMVCertificateprinted { get; set; }

        [DisplayName("Total Enrollments")]
        public long? TotalEnrollments { get; set; }

        [DisplayName("Stage 1 Verification")]
        public long? Stage1Verification { get; set; }

        [DisplayName("Stage 2 Verification")]
        public long? Stage2Verification { get; set; }

        [DisplayName("Stage 3 Verification")]
        public long? Stage3Verification { get; set; }

        [DisplayName("Branch Verification")]
        public long? BranchVerification { get; set; }

        [DisplayName("SLBC Verification")]
        public long? SLBCVerification { get; set; }

        [DisplayName("Stage 1 Approved")]
        public long? Stage1Approved { get; set; }

        [DisplayName("Stage 1 Rejected")]
        public long? Stage1Rejected { get; set; }

        [DisplayName("Stage 1 Pending")]
        public long? Stage1Pending { get; set; }

        [DisplayName("Stage 2 Approved")]
        public long? Stage2Approved { get; set; }

        [DisplayName("Stage 2 Rejected")]
        public long? Stage2Rejected { get; set; }

        [DisplayName("Stage 2 Pending")]
        public long? Stage2Pending { get; set; }

        [DisplayName("Stage 3 Approved")]
        public long? Stage3Approved { get; set; }

        [DisplayName("Stage 3 Rejected")]
        public long? Stage3Rejected { get; set; }

        [DisplayName("Stage 3 Pending")]
        public long? Stage3Pending { get; set; }

        [DisplayName("Branch Approved")]
        public long? BranchApproved { get; set; }

        [DisplayName("Branch Rejected")]
        public long? BranchRejected { get; set; }

        [DisplayName("Branch Pending")]
        public long? BranchPending { get; set; }

        [DisplayName("SLBC Approved")]
        public long? SLBCApproved { get; set; }

        [DisplayName("SLBC Rejected")]
        public long? SLBCRejected { get; set; }

        [DisplayName("SLBC Pending")]
        public long? SLBCPending { get; set; }

        [DisplayName("Loan Required")]
        public long? LoanRequiredCount { get; set; }

        [DisplayName("UAP Certificate")]
        public long? UAPCertificateCount { get; set; }

        [DisplayName("Criff report")]
        public long? CriffreportCount { get; set; }

        [DisplayName("EVoucherIssued")]
        public long? EVoucherIssuedCount { get; set; }

        [DisplayName("PMV Certificate printed")]
        public long? PMVCertificateprintedCount { get; set; }


        [DisplayName("Loan Application No")]
        public string LoanApplicationNo { get; set; }

        [DisplayName("Loan Applicant Name")]
        public string LoanApplicantName { get; set; }

        [DisplayName("Bank Name")]
        public string BankName { get; set; }

        [DisplayName("Bank Officer Name")]
        public string BankOfficerName { get; set; }

        [DisplayName("Bank Officer Email")]
        public string BankOfficerEmail { get; set; }

        [DisplayName("Bank Officer Mobile")]
        public string BankOfficerMobile { get; set; }

        [DisplayName("Sanctioned Date")]
        public string SanctionedDate { get; set; }

        [DisplayName("Sanctioned Amount")]
        public string SanctionedAmount { get; set; }

        [DisplayName("Disbursed Date")]
        public string DisbursedDate { get; set; }

        [DisplayName("Disbursed Amount")]
        public string DisbursedAmount { get; set; }

        [DisplayName("Loan Status")]
        public string LoanStatus { get; set; }

        [DisplayName("Rejected Date")]
        public string RejectedDate { get; set; }

        [DisplayName("Rejection Reason")]
        public string RejectionReason { get; set; }

        [DisplayName("Rejection Remarks")]
        public string RejectionRemarks { get; set; }

        [DisplayName("Branch Name")]
        public string BranchName { get; set; }

        [DisplayName("Total Applications Forwarded To Bank")]
        public long? TotalApplicationsForwardedToBank { get; set; }

        [DisplayName("Total Applications Sanctioned")]
        public long? TotalApplicationsSanctioned { get; set; }

        [DisplayName("Total Sanctioned Amount")]
        public decimal? TotalSanctionedAmount { get; set; }

        [DisplayName("Total Applications Disbursed")]
        public long? TotalApplicationsDisbursed { get; set; }

        [DisplayName("Total Disbursed Amount")]
        public decimal? TotalDisbursedAmount { get; set; }

        [DisplayName("Total Rejected Applications")]
        public long? TotalRejectedApplications { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

    }
}
