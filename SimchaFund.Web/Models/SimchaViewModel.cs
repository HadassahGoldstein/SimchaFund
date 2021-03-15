using SimchaFund.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFund.Web.Models
{
    public class SimchaViewModel
    {
        public List<Simcha> Simchas { get; set; }
        public decimal TotalBalance { get; set; }
    }
    public class ContributionsViewModel
    {
        public string SimchaName { get; set; }
        public List<Contributor> Contributors { get; set; }
        public int SimchaId { get; set; }
        public int Counter { get; set; }
    }
    public class ContributorsViewModel
    {
        public List<Contributor> Contributors { get; set; }
        public decimal Total { get; set; }
    }
    public class ShowHistoryViewModel
    {
        public List<Transaction> Transactions { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}
