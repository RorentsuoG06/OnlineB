using System;
using System.Linq;
using OnlineBankingDataModel;
namespace OnlineBankingDataService
{
    public class OnlineBankingInMemoryData : IOnlineBankingDataService
    {
        public List<BankAccount> Accounts = new List<BankAccount>();

        public OnlineBankingInMemoryData()
        {
            BankAccount AccNum1 = new BankAccount { AccountNumber = 1000, Pincode = 1111, balance = 0.0, Transactions = new List<string>() };
            BankAccount AccNum2 = new BankAccount { AccountNumber = 1001, Pincode = 1111, balance = 0.0, Transactions = new List<string>() };
            BankAccount AccNum3 = new BankAccount { AccountNumber = 1002, Pincode = 2222, balance = 0.0, Transactions = new List<string>() };

            Accounts.Add(AccNum1);
            Accounts.Add(AccNum2);
            Accounts.Add(AccNum3);

        }

        public BankAccount? GetAccNum(int accountNumber)
        {
            return Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        public void Add(BankAccount account)
        {
            Accounts.Add(account);
        }
        public void Update(BankAccount account)
        {
            var existingAccount = GetAccNum(account.AccountNumber);
            if (existingAccount != null)
            {
                existingAccount.Pincode = account.Pincode;
                existingAccount.balance = account.balance;
                existingAccount.Transactions = account.Transactions;
            }
        }

        public int GenerateNewAccountNumber()
        {
            if (Accounts.Count == 0)
            {
                return 1000;
            }

            int maxAccNo = Accounts.Max(a => a.AccountNumber);
            return maxAccNo + 1;
        }
    }
}
