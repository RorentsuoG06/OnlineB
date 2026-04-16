using OnlineBankingAppService;
using OnlineBankingDataModel;
using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace OnlineBanking_Act1
{
    public class Program
    {
        static OnlineBankAppService appService = new OnlineBankAppService();
        static void Main(string[] args)
        {
            Console.Write("WELCOME TO ONLINE BANKING \n");
            MainMenu();
        }

        static void MainMenu()
        {
            while (true)
            {

                Console.Write("-------------------------\n");
                Console.Write("1. CREATE ACCOUNT \n" +
                              "2. LOGIN (BALANCE, DEPOSIT, WITHDRAW) \n" +
                              "3. EXIT \n");
                Console.Write("-------------------------\n");
                Console.Write("PLEASE SELECT AN OPTION: ");
                int MenuInput;

                if (!int.TryParse(Console.ReadLine(), out MenuInput))
                {
                    Console.WriteLine("INVALID OPTION. ENTER A NUMBER.");
                    continue;
                }

                switch (MenuInput)
                {
                    case 1:
                        Register();
                        break;
                    case 2:
                        LOGIN();
                        break;
                    case 3:
                        Console.WriteLine("THANK YOU FOR USING ONLINE BANKING!");
                        return;
                    default:
                        Console.WriteLine("INVALID OPTION. PLEASE TRY AGAIN.");
                        continue;
                }
            }
        }

        static void Register()
        {
            /* 
             * Register Requirements
             * User must be over 18, Enter 4 digit pin, Confirm 4 digit pin
             */

            bool success = false;

            do
            {
                Console.Write("-------------------------\n");
                Console.Write("ACCOUNT REGISTRATION\n" +
                              "AGE VALIDATION: \n" +
                              "ENTER YOUR AGE: ");
                int age;

                // Age Validation
                if (!int.TryParse(Console.ReadLine(), out age))
                {
                    Console.WriteLine("INVALID AGE INPUT.");
                    continue;
                }

                if (age < 18)
                {
                    Console.WriteLine("SORRY, YOU MUST BE AT LEAST 18 YEARS OLD TO CREATE AN ACCOUNT.");
                    Console.WriteLine("-------------------------");
                    Console.WriteLine("REGISTRATION FAILED");
                    Console.Write("DO YOU WANT TO REGISTER AGAIN? [Y|N]: ");
                    string retry = Console.ReadLine().ToUpper();

                    if (retry != "Y")
                    {
                        success = true;
                        Console.Write("-------------------------\n");
                        Console.WriteLine("RETURNING TO THE MAIN MENU...");
                        MainMenu();
                        return;
                    }
                    else if (retry == "Y")
                    {
                        continue;
                    }
                }

                // Enter 4 Digit Pin
                Console.Write("ENTER A 4-DIGIT PIN: ");
                string pinCode = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(pinCode) || pinCode.Length != 4 || !int.TryParse(pinCode, out int pinInt))
                {
                    Console.WriteLine("INVALID PIN. PLEASE ENTER A 4-DIGIT PIN.");
                    continue;
                }

                // Confirm 4 Digit Pin
                Console.Write("CONFIRM YOUR 4-DIGIT PIN: ");
                string securityCode = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(securityCode) || securityCode.Length != 4 || !int.TryParse(securityCode, out int ConfirmPin))
                {
                    Console.WriteLine("INVALID CONFIRMATION PIN. PLEASE TRY AGAIN");
                    continue;
                }

                if (pinInt != ConfirmPin)
                {
                    Console.WriteLine("SECURITY CODE DOES NOT MATCH THE PIN.");
                    continue;
                }

                   BankAccount newAccount = appService.CreateAccount(age, pinInt, ConfirmPin);

                //Display new account Information
                if (newAccount != null)
                {
                    success = true;

                    Console.WriteLine("-------------------------------------");
                    Console.WriteLine("YOUR ACCOUNT HAS BEEN REGISTERED SUCCESSFULLY!");
                    Console.WriteLine($"YOUR ACCOUNT NUMBER IS: {newAccount.AccountNumber}");
                    Console.WriteLine($"INITIAL BALANCE: PHP {newAccount.balance}");
                    Console.WriteLine("PLEASE KEEP YOUR PIN SECURE.");

                    Console.WriteLine("-------------------------");
                    Console.WriteLine("YOU MAY NOW LOG IN TO START THE TRANSACTIONS.");
                    LOGIN();
                    return;
                }

            } while (!success);

        }

        static void LOGIN()
        {
            bool isContinue = true;
            do
            {
                Console.WriteLine("-------------------------");
                Console.Write("ENTER ACCOUNT NUMBER: ");
                int UserAccountNum;

                //Input Validation
                if(!int.TryParse(Console.ReadLine(), out UserAccountNum))
                {
                    Console.WriteLine("YOU MAY HAVE TYPED YOUR ACCOUNT NUMBER WRONG. PLEASE TRY AGAIN.");
                    continue;
                }

                //Account Validation
                var acc = appService.GetAccNum(UserAccountNum);

                if(acc == null)
                {
                    Console.WriteLine("THE ACCOUNT NUMBER YOU HAVE ENTERED DOES NOT EXIST IN OUR SYSTEM.");
                    continue;
                }

                bool authenticated = false;

                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine("-------------------------");
                    Console.Write("ENTER 4-DIGIT CODE: ");
                    int UserPin;

                    if (!int.TryParse(Console.ReadLine(), out UserPin))
                    {
                        Console.WriteLine("YOU MAY HAVE TYPED YOUR PIN NUMBER WRONG. PLEASE TRY AGAIN.");
                        continue;
                    }

                    authenticated = appService.Authenticate(UserAccountNum, UserPin);

                    if (authenticated)
                    {
                        Console.WriteLine("\nLogin Successful!");
                        Choices(UserAccountNum); return;
                    }
                    else
                    {
                        Console.WriteLine("You only have " + (2 - i) + " tries left. Incorrect MPIN entered.");
                    }
                }

                Console.WriteLine("-------------------------");
                Console.Write("Do you want to continue? [Y/N]: ");
                string continueInput = Console.ReadLine();

                if (continueInput.ToUpper() == "Y")
                {
                    isContinue = true;
                }
                else if (continueInput.ToUpper() == "N")
                {
                    Console.WriteLine("RETURNING TO MAIN MENU...");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Invalid input. System will exit.");
                    Environment.Exit(0);
                }
            } while (isContinue);
        }
        static void Choices(int accountNumber)
        {
            bool stayLoogedIn = true;

            while (stayLoogedIn)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("Welcome! What do you want to do today? \n" +
                              "1. BALANCE \n" +
                              "2. DEPOSIT \n" +
                              "3. WITHDRAW \n" +
                              "4. EXIT \n" +
                              "OTHER OPTIONS ON THE WAY!");

                Console.Write("PLEASE SELECT AN OPTION: ");
                int MenuInput;
                if (!int.TryParse(Console.ReadLine(), out MenuInput))
                {
                    Console.WriteLine("PLEASE ENTER A NUMBER.");
                    continue;
                }

                switch (MenuInput)
                {
                    case 1: //RETRIEVE
                        Console.WriteLine("-------------------------");
                        Console.WriteLine("Your Balance is: PHP " + appService.GetBalance(accountNumber));
                        break;
                    case 2: // CASH-IN
                        Console.Write("-------------------------");
                        Console.Write("\n DEPOSIT CHOICES: \n" +
                                      "1. BANK CASH-IN [BCI]\n" +
                                      "2. OVER-THE-COUNTER CASH-IN [OTC]\n" +
                                      "3. PARTNER OUTLET OPTIONS [PO]\n" +
                                      "PLEASE SELECT AN OPTION [BCI|OTC][PO]: ");
                        string SectionInput = Console.ReadLine().ToUpper();

                        string BankInput = "";

                        switch (SectionInput)
                        {
                            case "BCI":
                                Console.WriteLine("-------------------------");
                                Console.Write("\nBANK CASH-IN OPTIONS: \n" +
                                                  "1. BPI \n" +
                                                  "2. BDO \n" +
                                                  "3. LANDBANK \n" +
                                                  "ENTER BANK CASH-IN [BCI] BANK: ");
                                BankInput = Console.ReadLine().ToUpper();
                                break;
                            case "OTC":
                                Console.WriteLine("-------------------------");
                                Console.Write("\nBANK OVER-THE-COUNTER OPTIONS: \n" +
                                                  "1. ROBINSONS \n" +
                                                  "2. HANDYMAN \n" +
                                                  "3. 7-ELEVEN \n" +
                                                  "ENTER OVER-THE-COUNTER CASH-IN [OTC] BANK: ");
                                BankInput = Console.ReadLine().ToUpper();
                                break;
                            case "PO":
                                Console.WriteLine("-------------------------");
                                Console.Write("\nPARTNER OUTLET OPTIONS: \n" +
                                                  "1. 7-ELEVEN \n" +
                                                  "2. SM \n" +
                                                  "3. PUREGOLD \n" +
                                                  "ENTER PARTNER OUTLET CASH-OUT: ");
                                BankInput = Console.ReadLine().ToUpper();
                                break;

                            default:
                                Console.WriteLine("Invalid deposit option.");
                                return;
                        }

                        Console.WriteLine("-------------------------");
                        Console.Write("ENTER THE AMOUNT TO DEPOSIT: PHP ");
                        double amount;
                        if (!double.TryParse(Console.ReadLine(), out amount))
                        {
                            Console.WriteLine("Invalid amount.");
                            break;
                        }

                        var result = appService.Deposit(accountNumber, SectionInput, BankInput, amount);

                        if (result.success)
                        {
                            Console.WriteLine($"Fee: {result.fee}");
                            Console.WriteLine($"Balance: {result.newBalance}");
                        }
                        else
                        {
                            Console.WriteLine("Deposit failed. Please check inputs.");
                        }

                        break;

                    case 3: // CASH-OUT
                        Console.WriteLine("-------------------------");
                        Console.Write("\n WITHDRAW / TRANSFER CHOICES: \n" +
                                      "1. SEND MONEY (INTERNAL TRANSFER) [SM] \n" +
                                      "2. BANK TRANSFER [BT]\n" +
                                      "3. OVER-THE-COUNTER CASH-OUT [OTC]\n" +
                                      "4. PARTNER OUTLET CASH-OUT [PO]\n" +
                                      "PLEASE SELECT AN OPTION [SM|BT|OTC|PO]: ");
                        string sectionInput2 = Console.ReadLine().ToUpper();

                        string bankInput2 = "";

                        switch (sectionInput2)
                        {
                            case "SM":
                                Console.WriteLine("-------------------------");
                                Console.Write("\nSEND MONEY (INTERNAL TRANSFER) \n");
                                Console.Write("ENTER RECEIVER ACCOUNT NUMBER [EX. 1000]: ");
                                bankInput2 = Console.ReadLine();
                                break;

                            case "BT":
                                Console.WriteLine("-------------------------");
                                Console.Write("\nBANK TRANSFER OPTIONS: \n" +
                                              "1. BPI \n" +
                                              "2. BDO \n" +
                                              "3. LANDBANK \n" +
                                              "ENTER BANK TRANSFER [BPI|BDO|LANDBANK]: ");
                                bankInput2 = Console.ReadLine().ToUpper();
                                break;

                            case "OTC":
                                Console.WriteLine("-------------------------");
                                Console.Write("\nOVER-THE-COUNTER CASH-OUT OPTIONS: \n" +
                                              "1. PALAWAN \n" +
                                              "2. CEBUANA \n" +
                                              "3. VILLARICA \n" +
                                              "ENTER OVER-THE-COUNTER CASH-OUT: ");
                                bankInput2 = Console.ReadLine().ToUpper();
                                break;

                            case "PO":
                                Console.WriteLine("-------------------------");
                                Console.Write("\nPARTNER OUTLET CASH-OUT OPTIONS: \n" +
                                              "1. 7-ELEVEN \n" +
                                              "2. SM \n" +
                                              "3. PUREGOLD \n" +
                                              "ENTER PARTNER OUTLET CASH-OUT: ");
                                bankInput2 = Console.ReadLine().ToUpper();
                                break;

                            default:
                                Console.WriteLine("Invalid withdraw option.");
                                return;
                        }

                        Console.WriteLine("-------------------------");
                        Console.Write("ENTER THE AMOUNT TO WITHDRAW: PHP ");

                        if (!double.TryParse(Console.ReadLine(), out double wAmount))
                        {
                            Console.WriteLine("Invalid amount.");
                            return;
                        }

                        if (sectionInput2 == "SM")
                        {
                            var wresult = appService.SendMoney(accountNumber, bankInput2, wAmount);

                            if (wresult.success)
                            {
                                Console.WriteLine("Transfer successful.");
                                Console.WriteLine($"New balance: PHP {wresult.newBalance}");
                            }
                            else
                            {
                                Console.WriteLine("Transfer failed.");
                            }
                        }
                        else
                        {
                            var wresult = appService.Withdraw(accountNumber, sectionInput2, bankInput2, wAmount);

                            if (wresult.success)
                            {
                                Console.WriteLine("Withdrawal successful.");
                                Console.WriteLine($"Fee: PHP {wresult.fee}");
                                Console.WriteLine($"New balance: PHP {wresult.newBalance}");
                            }
                            else
                            {
                                Console.WriteLine("Withdrawal failed.");
                            }
                        }
                        break;

                    case 4: //EXIT
                        var receipt = appService.PrintReceipt(accountNumber);

                        Console.WriteLine("\n-----------DIGITAL RECEIPT------------");
                        Console.WriteLine($"ACCOUNT: {receipt.accountNumber}");
                        Console.WriteLine("TRANSACTIONS:");

                        foreach (var t in receipt.transactions)
                        {
                            Console.WriteLine($"  - {t}");
                        }

                        Console.WriteLine($"CURRENT BALANCE: PHP {receipt.balance}");
                        Console.WriteLine($"DATE: {receipt.date:dd-MMM-yyyy}");
                        Console.WriteLine("-------------------------------------");
                        stayLoogedIn = false;
                        break;

                    default:
                        Console.WriteLine("INVALID OPTION. PLEASE TRY AGAIN.");
                        break;

                }
            }
        }
    }
}

