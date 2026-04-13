using OnlineBankingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingDataService
{
    public interface IOnlineBankingDataService
    {
        public BankAccount? GetAccNum(int accountNumber);
        public void Add(BankAccount account);
        public int GenerateNewAccountNumber();
        public void Update(BankAccount account);
    }
}
