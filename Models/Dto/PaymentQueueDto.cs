namespace TuitionApi.Models.Dto
{
    public class PaymentQueueDto
    {
        public string PaymentID { get; set; }
        public bool Success { get; set; }
        public bool Processed { get; set; }
    }
}
