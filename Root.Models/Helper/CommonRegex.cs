using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Helper
{
    public static class CommonRegex
    {
        public const string PositiveAmountFormat = @"(([1-9]([0-9]+)?(\.[0-9][0-9]?)?)|([0]\.([1-9][0-9]|[0-9][1-9])))";
        public const string PositiveAmountFormatErr = @"Enter valid positive amount up to 2 decimals.";

        public const string PositiveAmountTwoDigitFormat = @"(([1-9]([0-9]+)?(\.[0-9]{1,2})?$)?)";
        public const string PositiveAmountTwoDigitFormatErr = @"Enter valid positive amount up to 2 decimals.";

        public const string PercentFormat = @"^(?:\d{1,2}(?:\.\d{1,3})?|100(?:\.0?0)?)$";
        public const string PercentFormatErr = @"Enter valid percentage between 0 to 100 up to 2 decimals.";

        public const string LatitudeFormat = @"^(\+|-)?(?:90(?:(?:\.0{1,10})?)|(?:[0-9]|[1-8][0-9])(?:(?:\.[0-9]{1,10})?))$";
        public const string LatitudeFormatErr = @"Enter valid Latitude";

        public const string LongitudeFormat = @"^(\+|-)?(?:180(?:(?:\.0{1,10})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\.[0-9]{1,10})?))$";
        public const string LongitudeFormatErr = @"Enter valid Longitude";

        public const string AccountNoFormat = @"^(?=.*[1-9a-zA-Z])[\da-zA-Z]*$";
        public const string AccountNoFormatErr = @"{0} must be alphanumeric string.";

        public const string MobileNoFormat = @"^((?!(0))[0-9]{10})$";
        public const string MobileNoFormatErr = @"The {0} field should be 10 digits only.";


        public const string SpecialFormat = @"^(?!.*(script|alert|[><,'\"":;\\]\\[}{)(./])).*$";
        public const string SpecialFormatErr = @"Input is invalid. Please ensure it does not contain any of the following characters or words: >, <, ',', "", :, ;, ], [, }, {, ), (, ., /, 'script', 'alert'.";


        public const string AddressFormat = @"^[^\\:*$@=^&)\(]+$";
        public const string AddressFormatErr = @"The {0} field should not contain '@', '&','*','=','^','$','\\' and ':' characters.";

        public const string BusinessNameFormat = @"^[^\\:*$@=^]+$";
        public const string BusinessNameFormatErr = @"The {0} field should not contain '@', '&','*','=','^','$','\\' and ':' characters.";

        public const string EmailIDFormat = @"^\s*(([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([;.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*\s*$";
        public const string EmailIDFormatErr = @"The {0} field must be valid.";

        public const string GSTNoFormate = @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$";
        public const string GSTNoFormateErr = @"The {0} field must be valid.";

        public const string AppVersionFormat = @"^([1-9](\d)*)(\.\d+)?(\.\d+)?(\.\d+)?$";
        public const string AppVersionFormatErr = @"The {0} field should be in valid format(i.e. X.X.X.X).";

        public const string CodeFormat = @"^[A-Za-z]*$";
        public const string CodeFormatErr = @"The {0} field should contain only Characters.";
        public const string NameFormat = @"[\u00C0-\u017Fa-zA-Z']+([- ]+[\u00C0-\u017Fa-zA-Z']+)*";
        public const string NameFormatErr = @"The {0} field should contain only characters and Hyphen(-) and apostrophe(').";

        public const string FeedbackNameFormat = @"^[\u00C0-\u017Fa-zA-Z\s]+$";
        public const string FeedbackNameFormatErr = @"The {0} field should contain only characters.";


        public const string FontColorFormat = @"^#(?:[0-9a-fA-F]{3}){1,2}$";
        public const string FontColorFormatErr = @"The {0} field is not in valid format(i.e. '#000000' or '#000').";

        public const string FontSizeFormat = @"^[1-9]([0-9]{0,1})px$";
        public const string FontSizeFormatErr = @"The {0} field is not in valid format(i.e. '12px').";

        public const string MenuFormat = @"^[^\\:*$@=^&)\(]+$";
        public const string MenuFormatErr = @"The {0} field should not contain '@', '&','*','=','^','$','\\' and ':' characters.";

        public const string PositiveNoFormat = @"([1-9]([0-9]+)?|[0])(\.[0-9][0-9]?)?";
        public const string PositiveNoFormatErr = @"Enter valid positive number.";

        public const string WebPasswordFormat = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,16}";
        public const string WebPasswordFormatErr = @"The {0} field must be Minimum 8 and Maximum 16 characters at least 1 Uppercase Alphabet, 1 Lowercase Alphabet, 1 Number and 1 Special Character($, @, !, %, *, ?, &).";

        public const string PositiveDecimalFormat = @"([1-9]([0-9]+)?|[0])(\.[0-9][0-9]?)?";
        public const string PositiveDecimalFormatErr = @"Enter valid positive number.";

        public const string PositiveIntegerFormat = @"^[1-9]([0-9]+)?";
        public const string PositiveIntegerFormatErr = @"Enter valid positive number.";

        public const string BankAccountMaxLengthFormat = @"([1-9][0-9]*)";
        public const string BankAccountMaxLengthFormatErr = @"Max Length must be a natural number";
        public const string VPARegularExpression = @"^[a-zA-Z0-9.-]{3,256}@[a-zA-Z]{3,64}$";
        public const string VPARegularExpressionErr = @"Please enter valid value for UPI ID/VPA.";
        public const string VPARegularErr = @"Please enter valid value for VPA ID.";

        public const string PANCardFormat = @"[A-Z]{3}[ABCFGHLJPTF]{1}[A-Z]{1}[0-9]{4}[A-Z]{1}?";
        public const string PANCardFormatErr = @"Enter valid PAN Card Details.";

        public const string RationCardFormat = @"^(?!0)[0-9]{10,15}$";
        public const string RationCardFormatErr = @"Enter valid Ration Card Details.";

        public const string IFSCCodeFormat = @"^[A-Z]{4}0[A-Z0-9]{6}$";
        public const string IFSCCodeFormatErr = @"Enter valid IFSC Code Details.";


        public const string Password = "Regex.Replace(Guid.NewGuid().ToString(), \"[^0-9a-zA-Z]+\", \"\").Substring(0, 10)";
        public const string searchPattern = "<td[^>]*>\\s*<h6[^>]*>\\s*</h6>\\s*</td>";
        public const string customPattern = @"<td\s+style=""width:\s*54%;\s*vertical-align:\s*bottom;"">\s*<h4\s+style=""font-size:\s*13px;\s*font-weight:\s*500;\s*border-bottom:\s*1px\s*solid\s*#A58950;\s*margin:\s*0px;\s*height:\s*24px;"">\s*</h4>\s*</td>";
        public const string patternId = "<td style=\"vertical-align: bottom; padding-left: 5px;\">\\s*<h6\\s*style=\"margin: 0px; text-align: left; font-size: 10px; font-weight: 400; border-bottom: 1px solid #000;\">\\s*</h6>\\s*</td>";
        public const string divPattern = @"<div\s+class=""id-pic-box""[^>]*>[\s\S]*?</div>";
        #region Code For QR Code
        public const string QrPattern = @"<div class=""qr-img"" style=""float: left;""[^>]*>\s*([\s\S]*?)\s*</div>";
        #endregion
    }
}
