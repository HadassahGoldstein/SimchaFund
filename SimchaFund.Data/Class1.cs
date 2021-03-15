using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimchaFund.Data
{    
    public class SimchaFundDB
    {
        private readonly string _connectionString;
        public SimchaFundDB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Simcha> GetAllSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Simchas";
            connection.Open();
            var reader = command.ExecuteReader();
            var simchos = new List<Simcha>();
            int allContrib = GetTotalContributors();
            while (reader.Read())
            {
                int id = (int)reader["Id"];
                int totalCont = GetContributorsCountForSimcha(id);
                simchos.Add(new Simcha
                {
                    Id = id,
                    SimchaName = (string)reader["Simcha Name"],
                    Date = (DateTime)reader["Date"],
                    Total = TotalContPerSimcha(id),
                    ContCount = $"{totalCont}/{allContrib}"
                });
            }
            return simchos;
        }
        public decimal TotalContPerSimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT ISNULL(SUM(Amount),0) FROM Contributions WHERE SimchaId=@id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (decimal)command.ExecuteScalar();
        }
        public int GetTotalContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Count(*) FROM Contributors";
            connection.Open();
            return (int)command.ExecuteScalar();            
        }
        public int GetContributorsCountForSimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM Contributions c JOIN Contributors cr
                                     ON c.ContributorId=cr.Id 
                                     WHERE c.SimchaId=@id";
            connection.Open();
            command.Parameters.AddWithValue("@id", id);
            return (int)command.ExecuteScalar();
        }
        public List<Contributor> GetContributionsForSimcha(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Contributors";
            connection.Open();
            var reader = command.ExecuteReader();
            List<Contributor> contributors = new List<Contributor>();
            int counter = 0;
            while (reader.Read())
            {
                int Id = (int)reader["Id"];
                decimal amount = GetAmountDonated(Id,id);
                contributors.Add(new Contributor               
                {
                    Id = Id,
                    Name = (string)reader["Name"],
                    AlwaysInclude = (bool)reader["alwaysInclude"],
                    Balance = GetPersonBalance(Id),
                    Amount=amount,                    
                    Counter = counter,
                    Contributed=amount != 0
                });                
                counter++;
            }
            return contributors;
        }
        private decimal GetAmountDonated(int contId, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();

            command.CommandText = @"SELECT ISNULL(SUM(Amount),0) FROM Contributions WHERE SimchaId=@simchaId AND ContributorId = @contId ";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            command.Parameters.AddWithValue("@contId", contId);
            connection.Open();
            return (decimal)command.ExecuteScalar();
        }
        public decimal GetPersonBalance(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT ISNULL(SUM(Amount),0) FROM Contributions WHERE contributorId=@id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            decimal totalCont = (decimal)command.ExecuteScalar();
            decimal totalDeposits = GetTotalPersonDeposits(id);
            return (totalDeposits - totalCont);
        }
        private decimal GetTotalPersonDeposits(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT ISNULL(SUM(Amount),0) FROM Deposits WHERE contributorId=@id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (decimal)command.ExecuteScalar();
        }
        public void UpdateContributions(List<Contribution> contributions)
        {
            if (contributions[0] == null)
            {
                return;
            }
            int simchaId = contributions[0].SimchaId;
            DeleteCont(simchaId);
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contributions(SimchaId,ContributorId,Amount,Date)
                                        VALUES (@simchaId,@contributorId,@amount,GETDATE())";
            connection.Open();
            foreach (Contribution c in contributions)
            {
                if ((c.Amount != 0) && (c.Contributed))
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@simchaId", c.SimchaId);
                    command.Parameters.AddWithValue("@contributorId", c.ContributorId);
                    command.Parameters.AddWithValue("@amount", c.Amount);
                    command.ExecuteNonQuery();
                }
            }
        }
        private void DeleteCont(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM Contributions WHERE SimchaId=@id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public decimal GetTotalBalance()
        {
            decimal deposits = TotalDeposits();
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT ISNULL(SUM(Amount),0) FROM Contributions";
            connection.Open();
            decimal totalCont = (decimal)command.ExecuteScalar();
            return (deposits - totalCont);

        }
        private decimal TotalDeposits()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT ISNULL(SUM(Amount),0) FROM Deposits";
            connection.Open();
            return (decimal)command.ExecuteScalar();
        }
        public void AddSimcha(Simcha s)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Simchas ([Simcha Name],Date)
                                    VALUES (@simchaName,@date)";
            command.Parameters.AddWithValue("@simchaName", s.SimchaName);
            command.Parameters.AddWithValue("@date", s.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Contributors";
            connection.Open();
            var contributors = new List<Contributor>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader["id"];
                contributors.Add(new Contributor
                {
                    Id = id,
                    Name = (string)reader["name"],
                    AlwaysInclude = (bool)reader["alwaysInclude"],
                    Balance = GetPersonBalance(id),
                    Cell = (string)reader["cell"],
                    Date = (DateTime)reader["CreatedDate"]
                });
            }
            return contributors;
        }
        public void AddContributor(Contributor c, decimal deposit)
        {
            if (string.IsNullOrEmpty(c.Name) || c.Date == null || string.IsNullOrEmpty(c.Cell))
            {
                return;
            }
            DateTime a = new DateTime(0001,1,1);
            if (c.Date == a)
            {
                c.Date = DateTime.Now;
            }
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Contributors(name,createdDate,cell,AlwaysInclude)
                                    VALUES (@name, @date, @cell, @alwaysInclude)
                                    SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@name", c.Name);
            command.Parameters.AddWithValue("@cell", c.Cell);
            command.Parameters.AddWithValue("@date", c.Date);
            command.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            connection.Open();
            int id = (int)(decimal)command.ExecuteScalar();
            AddDeposit(new Deposit { Amount = deposit, ContributorId = id, Date = c.Date });
        }
        public void AddDeposit(Deposit d)
        {
            if (d.Amount == 0)
            {
                return;
            }
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Deposits(contributorId,amount,date)
                                    VALUES (@contributorId,@amount,@date)";
            command.Parameters.AddWithValue("@contributorId", d.ContributorId);
            command.Parameters.AddWithValue("@amount", d.Amount);
            command.Parameters.AddWithValue("@date", d.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void EditContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Contributors SET Name=@name, cell=@cell,alwaysInclude=@alwaysInclude,createdDate=@date WHERE id=@id";
            command.Parameters.AddWithValue("@id", c.Id);
            command.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            command.Parameters.AddWithValue("@name", c.Name);
            command.Parameters.AddWithValue("@cell", c.Cell);
            command.Parameters.AddWithValue("@date", c.Date);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Transaction> HistoryOfContributions(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT c.Amount,c.Date,s.[simcha name] FROM Contributions c JOIN SIMCHAS s ON c.SimchaId=s.Id WHERE c.ContributorId =@id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            List<Transaction> transactions = new List<Transaction>();
            while (reader.Read())
            {
                transactions.Add(new Transaction
                {
                    Action = $"Contribution for {(string)reader["simcha name"]}",
                    Amount = (decimal)reader["amount"],
                    Date = (DateTime)reader["date"]
                });
            }
            return transactions;
        }
        public void AddHistoryOfDeposits(List<Transaction> t, int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Deposits WHERE ContributorId=@id ";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                t.Add(new Transaction
                {
                    Action = "Deposit",
                    Date = (DateTime)reader["date"],
                    Amount = (decimal)reader["amount"]
                });
            }
        }
        public string GetNameById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT name FROM Contributors WHERE Id=@id ";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            return (string)reader["name"];
        }
        public string GetSimchaName(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT [simcha name] FROM Simchas WHERE Id=@id ";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            reader.Read();
            return (string)reader["simcha name"];
        }
    }
}
