using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimchaFund.Data;
using SimchaFund.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimchaFund.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
        @"Data Source=.\sqlexpress; Initial Catalog=SimchaFund;Integrated Security=true;";
        public IActionResult Index()
        {
            SimchaFundDB db = new(_connectionString);

            SimchaViewModel vm = new()
            {
                Simchas = db.GetAllSimchas(),                               
            };
            return View(vm);
        }
        public IActionResult NewSimcha(Simcha s)
        {
            SimchaFundDB db = new(_connectionString);
            db.AddSimcha(s);
            return Redirect("/");
        }
        public IActionResult ContributionsPerSimcha(int id)
        {
            SimchaFundDB db = new(_connectionString);
            ContributionsViewModel vm = new()
            {
                SimchaName = db.GetSimchaName(id),
                SimchaId=id,
                Contributors=db.GetContributionsForSimcha(id)
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult ContributionsPerSimcha(List<Contribution> contributions)
        {
            SimchaFundDB db = new(_connectionString);
            db.UpdateContributions(contributions);
            return Redirect("/");
        }

      
    }
}
