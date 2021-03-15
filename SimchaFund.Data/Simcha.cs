using System;
using System.Collections.Generic;
using System.Text;

namespace SimchaFund.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string SimchaName { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string ContCount { get; set; }
    }
    public class Contribution
    {
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public int SimchaId { get; set; }
        public bool Contributed { get; set; }
    }
    public class Contributor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cell { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int Counter { get; set; }
        public bool Contributed { get; set; }
    }
    public class Deposit
    {
        public int ContributorId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
    public class Transaction
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Action { get; set; }
    }
}
