using OnlineBankingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineBankingDataService
{
    public class OnlineBankingJson : IOnlineBankingDataService
    {
        private List<BankAccount> accounts = new List<BankAccount>();

        private string _jsonFileName;

        public OnlineBankingJson()
        {
            _jsonFileName = $"{AppDomain.CurrentDomain.BaseDirectory}/BankAccount.json";

            PopulateJsonFile();
        }

        private void PopulateJsonFile()
        {
            RetrieveDataFromJsonFile();

            if (accounts.Count <= 0)
            {
                BankAccount AccNum1 = new BankAccount { AccountNumber = 1000, Pincode = 1111, balance = 0.0, Transactions = new List<string>() };
                BankAccount AccNum2 = new BankAccount { AccountNumber = 1001, Pincode = 1111, balance = 0.0, Transactions = new List<string>() };
                BankAccount AccNum3 = new BankAccount { AccountNumber = 1002, Pincode = 2222, balance = 0.0, Transactions = new List<string>() };

                accounts.Add(AccNum1);
                accounts.Add(AccNum2);
                accounts.Add(AccNum3);

                SaveDataToJsonFile();
            }
        }

        private void SaveDataToJsonFile()
        {
            using (var outputStream = File.OpenWrite(_jsonFileName))
            {
                JsonSerializer.Serialize<List<BankAccount>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    { SkipValidation = true, Indented = true })
                    , accounts);
            }
        }

        private void RetrieveDataFromJsonFile()
        {
            using (var jsonFileReader = File.OpenText(this._jsonFileName))
            {
                this.accounts = JsonSerializer.Deserialize<List<BankAccount>>
                    (jsonFileReader.ReadToEnd(), new JsonSerializerOptions
                    { PropertyNameCaseInsensitive = true })
                    .ToList();
            }
        }

        public BankAccount? GetAccNum(int accountNumber)
        {
            return accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        public void Add(BankAccount account)
        {
            accounts.Add(account);
            SaveDataToJsonFile();
        }
        public void Update(BankAccount account)
        {
            var existingAccount = GetAccNum(account.AccountNumber);
            if (existingAccount != null)
            {
                existingAccount.Pincode = account.Pincode;
                existingAccount.balance = account.balance;
                existingAccount.Transactions = account.Transactions;
                SaveDataToJsonFile();
            }
        }

        public int GenerateNewAccountNumber()
        {
            RetrieveDataFromJsonFile();
            if (accounts == null || accounts.Count == 0)
                return 1000;

                int MaxAccNo = accounts.Max(a => a.AccountNumber);
                return MaxAccNo + 1;
        }
    }
}