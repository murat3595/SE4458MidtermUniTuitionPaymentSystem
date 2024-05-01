namespace TuitionApi.Models.Data
{
    public class PaymentModel
    {
        public PaymentModel(string paymentId, int userId, double amount, bool processed, bool success)
        {
            PaymentId = paymentId;
            UserId = userId;
            Amount = amount;
            Processed = processed;
            Success = success;
        }

        public string PaymentId { get; set; }
        public int UserId { get; set; }
        public double Amount { get; set; }
        public bool Processed { get; set; }
        public bool Success { get; set; }

    }
}
