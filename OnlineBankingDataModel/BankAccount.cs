namespace OnlineBankingDataModel
{
    public class BankAccount
    {
        public int AccountNumber { get; set; }
        public int Pincode { get; set; }
        public double balance { get; set; }
        public List<string> Transactions { get; set; } = new List<string>();
    }
}
