using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ResiGrass_API.Models
{
    public class RecolectionModel
    {
        public int id { get; set; }
        HeadQuartersModel? headQuarterId { get; set; }
        ProductsModel? productsId { get; set; }
        MeasuresModel? measuresId { get; set; }
        MethodPaymentModel? methodPayment { get; set; }
        CollectorsModel? collectorId { get; set; }
        public required int receivedFull {  get; set; }
        public required int bowlEmpty { get; set; }
        public required DateTime receivedDate { get; set; }
        public required DateTime endDate { get; set; }
        public required float fullPayment {  get; set; }
        public required float priceUnit { get; set; }
        public required float netWeight { get; set; }
        public required string observations { get; set; }
        public required string collectedName { get; set; }
        public required bool status { get; set; }







    }
}
