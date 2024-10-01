using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ResiGrass_API.Models
{
    public class RecolectionModel
    {
        public int id { get; set; }
        public int collectorId { get; set; }
        public int headquarterId { get; set; }
        public int measureId { get; set; }
        public int methodPaymentId { get; set; }
        public int productId { get; set; }
        HeadQuartersModel? headQuarterModel { get; set; }
        ProductsModel? productsModel { get; set; }
        MeasuresModel? measuresModel { get; set; }
        MethodPaymentModel? methodPaymentModel { get; set; }
        CollectorsModel? collectorModel { get; set; }
        public required int receivedFull {  get; set; }
        public required int bowlEmpty { get; set; }
        public required DateTime receivedDate { get; set; }
        public required DateTime endDate { get; set; }
        public required float fullPayment {  get; set; }
        public required float priceUnit { get; set; }
        public required float netWeight { get; set; }
        public required string observations { get; set; }
        public required string collectedName { get; set; }


    }


    public class RecolectionModelInsert
    {
        public int collectorId { get; set; }
        public int headquarterId { get; set; }
        public int measureId { get; set; }
        public int methodPaymentId { get; set; }
        public int productId { get; set; }
        HeadQuartersModel? headQuarterModel { get; set; }
        ProductsModel? productsModel { get; set; }
        MeasuresModel? measuresModel { get; set; }
        MethodPaymentModel? methodPaymentModel { get; set; }
        CollectorsModel? collectorModel { get; set; }
        public required int receivedFull { get; set; }
        public required int bowlEmpty { get; set; }
        public required DateTime receivedDate { get; set; }
        public required DateTime endDate { get; set; }
        public required float fullPayment { get; set; }
        public required float priceUnit { get; set; }
        public required float netWeight { get; set; }
        public required string observations { get; set; }
        public required string collectedName { get; set; }


    }
}
