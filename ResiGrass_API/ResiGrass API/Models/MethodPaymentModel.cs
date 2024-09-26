namespace ResiGrass_API.Models
{
    public class MethodPaymentModel
    {
        public int id { get; set; }
        public required string descriptionPayment { get; set; }
        public required bool status { get; set; }

    }
}
