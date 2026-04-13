using OnlineBankingDataModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace OnlineBankingDataService
{
    public class OnlineBankingDBData : IOnlineBankingDataService
    {
        private string connectionString
            = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=OnlineBanking;Integrated Security=True;TrustServerCertificate=True;";

        private SqlConnection sqlConnection;

        public OnlineBankingDBData()
        {
            sqlConnection = new SqlConnection(connectionString);
            AddSeeds();
        }

        private void AddSeeds()
        {
            var existing = GetAccounts();

            if (existing.Count == 0)
            {
                BankAccount acc1 = new BankAccount { AccountNumber = 0000, Pincode = 1111, balance = 0 };
                BankAccount acc2 = new BankAccount { AccountNumber = 1001, Pincode = 1111, balance = 0 };
                BankAccount acc3 = new BankAccount { AccountNumber = 1002, Pincode = 2222, balance = 0 };

                Add(acc1);
                Add(acc2);
                Add(acc3);
            }
        }


        public void Add(BankAccount account)
        {
            string insertStatement = @"INSERT INTO BankAccounts (AccountNumber, Pincode, Balance, Transactions)
                                       VALUES (@AccountNumber, @Pincode, @Balance, @Transactions)";

            SqlCommand insertCommand = new SqlCommand(insertStatement, sqlConnection);

            insertCommand.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);
            insertCommand.Parameters.AddWithValue("@Pincode", account.Pincode);
            insertCommand.Parameters.AddWithValue("@Balance", account.balance);
            insertCommand.Parameters.AddWithValue("@Transactions", string.Join(";", account.Transactions));

            sqlConnection.Open();
            insertCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public List<BankAccount> GetAccounts()
        {
            string selectStatement = "SELECT AccountNumber, Pincode, Balance, Transactions FROM BankAccounts";
            SqlCommand selectCommand = new SqlCommand(selectStatement, sqlConnection);

            sqlConnection.Open();
            SqlDataReader reader = selectCommand.ExecuteReader();

            var accounts = new List<BankAccount>();

            while (reader.Read())
            {
                BankAccount acc = new BankAccount
                {
                    AccountNumber = int.Parse(reader["AccountNumber"].ToString()),
                    Pincode = int.Parse(reader["Pincode"].ToString()),
                    balance = double.Parse(reader["Balance"].ToString()),
                    Transactions = reader["Transactions"] == DBNull.Value 
                    ? new List<string>()
                    : reader["Transactions"].ToString().Split(';').ToList()
                };

                accounts.Add(acc);
            }

            sqlConnection.Close();
            return accounts;
        }

        public BankAccount? GetAccNum(int accountNumber)
        {
            string query = "SELECT AccountNumber, Pincode, Balance, Transactions FROM BankAccounts WHERE AccountNumber = @AccountNumber";
            SqlCommand cmd = new SqlCommand(query, sqlConnection);
            cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);

            sqlConnection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            BankAccount account = null;

            if (reader.Read())
            {
                account = new BankAccount
                {
                    AccountNumber = int.Parse(reader["AccountNumber"].ToString()),
                    Pincode = int.Parse(reader["Pincode"].ToString()),
                    balance = double.Parse(reader["Balance"].ToString()),
                    Transactions = string.IsNullOrEmpty(reader["Transactions"].ToString())
                    ? new List<string>() : reader["Transactions"].ToString().Split(';').ToList()
                };
            }

            sqlConnection.Close();
            return account;
        }

        public int GenerateNewAccountNumber()
        {
            string query = "SELECT ISNULL(MAX(AccountNumber), 999) FROM BankAccounts";
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                sqlConnection.Open();
                object result = command.ExecuteScalar();
                sqlConnection.Close();

                int maxAccNo = (result == DBNull.Value) ? 999 : Convert.ToInt32(result);
                return maxAccNo + 1; 
            }
        }

        public void Update(BankAccount account)
        {
            string updateStmt = @"UPDATE BankAccounts 
                                SET Pincode = @Pincode,
                                Balance = @Balance,
                                Transactions = @Transactions
                                WHERE AccountNumber = @AccountNumber";

            SqlCommand cmd = new SqlCommand(updateStmt, sqlConnection);

            cmd.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);
            cmd.Parameters.AddWithValue("@Pincode", account.Pincode);
            cmd.Parameters.AddWithValue("@Balance", account.balance);
            cmd.Parameters.AddWithValue("@Transactions", string.Join(";", account.Transactions));

            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}