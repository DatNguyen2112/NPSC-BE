using System;
using System.Collections.Generic;
using NSPC.Common;

namespace NSPC.Common
{
    public class NSPCConstants
    {
        public static class CommandTypes
        {
            public const string Push = "PUSH_CMD";
            public const string Pull = "PULL_CMD";
        }

        public static class ClaimConstants
        {
            public static string USER_ID = "x-userId";
            public static string APP_ID = "x-appId";
            public static string USER_NAME = "x-userName";
            public static string FULL_NAME = "x-fullName";
            public static string ROLES = "x-roles";
            public static string RIGHTS = "x-rights";
            public static string LEVEL = "x-level";
            public static string ISSUED_AT = "x-iat";
            public static string EXPIRES_AT = "x-exp";
            public static string CHANNEL = "x-channel";
            public static string PHONE = "x-phone";
            public static string LANGUAGE = "x-language";
            public static string CURRENCY = "x-currency";
            public static string PARTNER_ID = "x-partner-id";
        }

        public static class CurrencyConstants
        {
            public const string Default = "USD";
        }

        public static class AppChanel
        {
            public static int WebAdmin = 1;
            public static int Mobile = 0;
        }
        public static Dictionary<string, int> RoleLevelDict = new Dictionary<string, int>()
        {
            { RoleConstants.AdminRoleCode, RoleConstants.AdministratorLevel},
            { RoleConstants.SuperAdminRoleCode, RoleConstants.SuperAdministratorLevel }
            // { RoleConstants.PermissionFullControlRoleCode, RoleConstants.PermissionFullControlLevel},
            // { RoleConstants.PostFullControlRoleCode, RoleConstants.PostFullControlLevel},
            // { RoleConstants.LeaveFullControlRoleCode, RoleConstants.LeaveFullControlLevel},
            // { RoleConstants.WorkDayFullControlRoleCode, RoleConstants.WorkDayFullControlLevel},
            // { RoleConstants.EmployeeFullControlRoleCode, RoleConstants.EmployeeFullControlLevel},
            // { RoleConstants.OrderSideRoleCode, RoleConstants.EmployeeFullControlLevel},
            // { RoleConstants.FarmerSideRoleCode, RoleConstants.EmployeeFullControlLevel},
            //
            // { RoleConstants.CSKHRoleCode, RoleConstants.EmployeeFullControlLevel},
            //  { RoleConstants.KTRoleCode, RoleConstants.EmployeeFullControlLevel},
            //
            // { RoleConstants.NPApproverCode, RoleConstants.NPApproverLevel},
            // { RoleConstants.NPComposerCode, RoleConstants.NPComposerLevel},
            // { RoleConstants.NPPublisherCode, RoleConstants.NPComposerLevel},
            // { RoleConstants.NPAdminCode, RoleConstants.NPAdminLevel}
        };


        public static class LanguageConstants
        {
            public const string Default = "vn";
            public const string English = "en";
            public const string Vietnamese = "vn";
            public const string Japanese = "jp";
            public const string Chinese = "zh";

            public static string[] AvailableLanguages = { English, Vietnamese, Japanese, Chinese };
        }

        public static class PaymentRequestConstants
        {
            public const string Status_Reject = "REJECT"; // Hủy
            public const string Status_WaitConfirm = "WAIT_CONFIRM"; // Chờ duyệt
            public const string Status_BankTransfer = "BANK_TRANSFER"; // Chờ duyệt
            public const string Status_Completed = "COMPLETED";
        }

        public static class ProfileInteractionConstants
        {
            public const string View = "VIEW";
            public const string Share = "DOWNLOAD";
            public const string Download = "DOWNLOAD";
            public const string Bookmark = "BOOKMARK";
            public const string Connect = "CONTACT";
        }

        public static class DocTypeConstants
        {
            public const string LogoAndBrand = "PRF-B";
            public const string FacilityStore = "PRF-F";
            public const string Product = "PRF-P";
            public const string Document = "PRF-D";
            public const string Verification = "PRF-V";
        }

        public static class ApplicationStatusConstants
        {
            public const string Draft = "DRAFT";
            public const string Actived = "ACTIVED";
            public const string Locked = "LOCKED";
        }
    }
}