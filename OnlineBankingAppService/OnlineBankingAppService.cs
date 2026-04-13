using OnlineBankingDataModel;
using OnlineBankingDataService;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace OnlineBankingAppService
{
    public class OnlineBankAppService
    {
        BankingDataService dataService = new BankingDataService(new OnlineBankingInMemoryData());

        public BankAccount GetAccNum(int accountNumber)
        {
            return dataService.GetAccNum(accountNumber);
        }

        public bool Authenticate(int accountNumber, int pincode)
        {
            var account = dataService.GetAccNum(accountNumber);
            return account != null && account.Pincode == pincode;

        }

        public BankAccount CreateAccount(int age, int pin, int securityCode)
        {

            int newAccNo = dataService.GenerateNewAccountNumber();
            var newAccount = new BankAccount
            {
                AccountNumber = newAccNo,
                Pincode = pin,
                balance = 0,
                Transactions = new List<string>()
            };

            dataService.Add(newAccount);
            return newAccount;
        }

        public double GetBalance(int accountNumber)
        {
            var account = dataService.GetAccNum(accountNumber);
            return account != null ? account.balance : 0.0;
        }

        public (bool success, double fee, double newBalance) Deposit(int accountNumber, string SectionInput, string BankInput, double amount) // CASH-IN
        {
            var account = dataService.GetAccNum(accountNumber);
            if (account == null || amount <= 0)
            {
                return (false, 0, 0);
            }

            double Fee = 0.0;

            switch (SectionInput)
            {
                case "BCI":
                    switch (BankInput)
                    {
                        case "BPI":
                        case "BDO":
                        case "LANDBANK":
                            Fee = 15.00;
                            break;
                        default:
                            return (false, 0, 0);
                    }
                    break;
                case "OTC":
                    switch (BankInput)
                    {
                        case "ROBINSONS":
                        case "HANDYMAN":
                            Fee = 15.00;
                            break;
                        case "7-ELEVEN":
                            Fee = 0.02;
                            break;
                        default:
                            return (false, 0, 0);
                    }
                    break;
                case "PO":
                    switch (BankInput)
                    {
                        case "7-ELEVEN":
                            Fee = 0.02;
                            break;
                        case "SM":
                        case "PUREGOLD":
                            Fee = 10.00;
                            break;
                        default:
                            return (false, 0, 0);
                    }
                    break;
                default:
                    return (false, 0, 0);
            }

            account.balance += amount - Fee;
            account.Transactions.Add($"DEPOSIT PHP {amount} via {BankInput}");
            dataService.Update(account);
            
            return (true, Fee, account.balance);
        }

        public (bool success, double newBalance) SendMoney(int senderAccNo, string receiverAccInput, double amount)
        {
            if (!int.TryParse(receiverAccInput, out int receiverAccNo))
                return (false, 0);

            var sender = dataService.GetAccNum(senderAccNo);
            var receiver = dataService.GetAccNum(receiverAccNo);

            if (sender == null || receiver == null)
                return (false, 0);

            if (amount <= 0)
                return (false, 0);

            if (sender.balance < amount)
                return (false, 0);

            sender.balance -= amount;
            receiver.balance += amount;

            sender.Transactions.Add($"SEND MONEY PHP {amount} TO ACCOUNT {receiverAccNo}");
            receiver.Transactions.Add($"RECEIVED PHP {amount} FROM ACCOUNT {senderAccNo}");

            dataService.Update(sender);
            dataService.Update(receiver);

            return (true, sender.balance);
        }

        public (bool success, double fee, double newBalance) Withdraw(int accountNumber, string sectionInput, string bankInput, double amount)
        {
            var account = dataService.GetAccNum(accountNumber);
            if (account == null)
                return (false, 0, 0);

            if (amount <= 0)
                return (false, 0, 0);

            double fee = 0.0;

            switch (sectionInput)
            {
                case "SM":
                    {
                        var result = SendMoney(accountNumber, bankInput, amount);
                        return (result.success, 0, result.newBalance);
                    }

                case "BT":
                    switch (bankInput)
                    {
                        case "BPI":
                        case "BDO":
                        case "LANDBANK":
                            fee = 20.00;
                            break;
                        default:
                            return (false, 0, 0);
                    }
                    break;

                case "OTC":
                    switch (bankInput)
                    {
                        case "PALAWAN":
                        case "CEBUANA":
                        case "VILLARICA":
                            fee = 15.00;
                            break;
                        default:
                            return (false, 0, 0);
                    }
                    break;

                case "PO":
                    switch (bankInput)
                    {
                        case "7-ELEVEN":
                            fee = 0.02;
                            break;
                        case "SM":
                        case "PUREGOLD":
                            fee = 10.00;
                            break;
                        default:
                            return (false, 0, 0);
                    }
                    break;

                default:
                    return (false, 0, 0);
            }

            if (account.balance < amount + fee)
                return (false, 0, 0);

            account.balance -= amount + fee;
            account.Transactions.Add($"WITHDRAW PHP {amount} via {bankInput}");

            dataService.Update(account);

            return (true, fee, account.balance);
        }

        public (int accountNumber, List<string> transactions, double balance, DateTime date) PrintReceipt(int accountNumber)
        {
            var account = dataService.GetAccNum(accountNumber);

            if (account == null)
                return (0, new List<string>(), 0, DateTime.Now);

            return (account.AccountNumber, account.Transactions, account.balance, DateTime.Now);
        }
    }
}
