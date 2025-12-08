using NSPC.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NSPC.Business
{
    public class AdminDashboardViewModel
    {
        public decimal TotalIncomeAmount { get; set; }
        public decimal TotalCommissionAmount { get; set; }
        public int TotalOffer { get; set; }
        public int TotalProposal { get; set; }
        public int TotalUser { get; set; }
        public int TotalEmailVerifiedUser { get; set; }
        public int TotalActiveUser { get; set; }
        public int TotalProfile { get; set; }
        public int TotalPurchasedProfile { get; set; }
    }

}