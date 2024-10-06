using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Utils
{
    public class ProgConstants
    {
        #region DBEntity Values
        public const string DBEntities = "A";
        public const string LogEntities = "B";
        public const string PMSEntities = "C";
        public const string SessionEntities = "D";
        public const string ReportEntities = "E";
        public const int ConnKeySize = 24;
        public const int ConnIVSize = 8;
        #endregion

        #region Header Values
        public const string RequestUUID = "RequestUUID";
        public const string ServiceRequestId = "ServiceRequestId";
        public const string ChannelId = "ChannelId";
        public const string Platform = "Platform";
        public const string OSVersion = "OSVersion";
        public const string ModelName = "ModelName";
        public const string DeviceId = "DeviceID";
        public const string ApiVersion = "ApiVersion";
        public const string AppVersion = "AppVersion";
        public const string IP = "IP";
        public const string SessionId = "SessionId";
        public const string LanguageId = "LanguageId";
        public const string JourneyId = "JourneyId";
        public const string SVersion = "SVersion";
        public const string ClientID = "clientID";
        public const string ReqRefId = "ReqRefId";
        #endregion
        #region Token Manager Values
        public const string JWTID = "JWTID";
        public const string Type = "Type";
        public const string Authorization = "Authorization";
        public const string LoginId = "LoginId";
        public const string AppDeviceId = "AppDeviceId";
        public const string RefId = "RefId";
        public const string MobileNo = "MobileNo";
        public const string DeviceToken = "DeviceToken";
        public const string IsMPINExpired = "IsMPINExpired";
        public const string IsFirstLogin = "IsFirstLogin";

        public const string RefCode = "RefCode";
        public const string Data = "Data";
        public const string SMSEnLang = "EN";
        public const string SMSHnLang = "HN";
        #endregion
        #region Admin Portal Values
        public const string SessionLoggedInUser = "LoggedInUser";
        public const string ApiToken = "ApiToken";
        public const string ErrMsg = "ErrMsg";
        public const string SuccMsg = "SuccMsg";
        public const string CurrURL = "CrrURL";
        public const string IsForgotUsername = "IsForgotUsername";
        public const string CMSUserName = "CMSUserName";
        public const string ContactNo = "ContactNo";
        public const string OTPExpiry = "OTPExpiry";
        public const string OTPATMP = "OTPATMP";
        public const string IsLoginWithGP = "IsLoginWithGP";
        public const long EmailtSentSeconds = 600;
        public const long EmailExpirationSeconds = 200;
        public const string BLKCMSUSR = "BLKCMSUSR";
        public const string ApplicationPdf = "application/pdf";
        public const string CRIFReport = "-CRIF-Report.pdf";
        public const string ApplicationHtml = "application/html";
        public const string Certificate = "Certificate_";
        public const string EmailIdRequiredMsg = "The Email ID field is Required";
        public const string SanctionLetterPostfix = "_SanctionLetter";
        public const string SanctionLetterDirectory = "SanctionLetters";
        public const string ImageBase64 = "data:image/jpg;base64,";
        public const string PopupShown = "PopupShown";
        public const string NewLoanApplications = "New Loan Applications";
        public const string SanctionLoan = "Sanctioned Loan Applications";
        public const string RejectLoan = "Rejected Loan Applications";
        public const string DisburseLoan = "Disbursed Loan Applications";
        public const string NPA = "NPA Loan Applications";
        public const string LoanClosed = "Closed Loan Applications";
        public const string PhotoNullPopupShown = "PhotoNullPopupShown";
        public const string MobileFirstSixChar = "XXXXXX";
        public const string CanVerify = "CanVerify";
        public const string Scheme = "Scheme";
        public const string OTPATMPEXD = "OTPATMPEXD";
        public const string Refresh = "Refresh";
        #endregion

        public const string ExcelFileExtension = ".xlsx";
        public const string CSVFileExtension = ".csv";
    }
}
