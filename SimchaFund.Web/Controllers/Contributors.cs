using Microsoft.AspNetCore.Mvc;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFund.Web.Controllers
{
    public class Contributors : Controller
    {
        private string _connectionString =
        @"Data Source=.\sqlexpress; Initial Catalog=SimchaFund;Integrated Security=true;";
        public IActionResult Index()
        {
            SimchaFundDB db = new SimchaFundDB(_connectionString);
            ContributorsViewModel vm = new()
            {
                Contributors = db.GetContributors(),
                Total = db.GetTotalBalance()
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult New(Contributor c, decimal initialDeposit)
        {
            SimchaFundDB db = new(_connectionString);
            if (c.Id != 0)
            {
                db.EditContributor(c);
            }
            else
            {
                db.AddContributor(c, initialDeposit);
            }
            return Redirect("/Contributors/");
        }
        [HttpPost]
        public IActionResult Deposit(Deposit d)
        {
            SimchaFundDB db = new(_connectionString);
            db.AddDeposit(d);
            return Redirect("/Contributors");
        }
        public IActionResult ShowHistory(int id)
        {
            SimchaFundDB db = new(_connectionString);            
            List<Transaction> t = db.HistoryOfContributions(id);
            db.AddHistoryOfDeposits(t, id);
            t = t.OrderBy(t => t.Date).ToList();
            ShowHistoryViewModel vm = new()
            {
                Balance = db.GetPersonBalance(id),
                Transactions = t,
                Name = db.GetNameById(id)
            };
            return View(vm);
        }        
    }
}
