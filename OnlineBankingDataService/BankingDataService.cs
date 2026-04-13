using OnlineBankingDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingDataService
{
    public class BankingDataService
    {
        IOnlineBankingDataService _dataService;
        public BankingDataService(IOnlineBankingDataService dataService)
        {
            _dataService = dataService;
        }

        public BankAccount? GetAccNum(int accountNumber)
        {
            return _dataService.GetAccNum(accountNumber);
        }

        public void Add(BankAccount account)
        {
            _dataService.Add(account);
        }

        public void Update(BankAccount account)
        {
            _dataService.Update(account);
        }

        public int GenerateNewAccountNumber()
        {
            return _dataService.GenerateNewAccountNumber();
        }
    }
}
