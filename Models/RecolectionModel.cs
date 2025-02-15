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
        public HeadQuartersModel? headQuarterModel { get; set; }
        ProductsModel? productsModel { get; set; }
        MeasuresModel? measuresModel { get; set; }
        MethodPaymentModel? methodPaymentModel { get; set; }
        CollectorsModel? collectorModel { get; set; }
        public required float receivedFull { get; set; }
        public required int bowlEmpty { get; set; }
        public required DateTime receivedDate { get; set; }
        public required DateTime endDate { get; set; }
        public required float fullPayment { get; set; }
        public required float priceUnit { get; set; }
        public required float netWeight { get; set; }
        public required string observations { get; set; }

        public string email { get; set; }

        public string collectedName { get; set; } // Nombre del recolector
        public string businessType { get; set; }
        public string address { get; set; }
        public string numberPhone { get; set; }// Nombre del tipo de negocio
        public int businessTypeId { get; set; } // ID del tipo de negocio

        public string nameHeadquarter { get; set; } // Nombre de la sede
        public string nitCc { get; set; }
        public string nameClient { get; set; }
        public string serial_number { get; set; }// NIT del cliente
        public string? nameLocality { get; set; }
        public byte[]? signature_image { get; set; }


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
        public float receivedFull { get; set; }
        public int bowlEmpty { get; set; }
        public DateTime receivedDate { get; set; }
        public DateTime endDate { get; set; }
        public float fullPayment { get; set; }
        public float priceUnit { get; set; }
        public float netWeight { get; set; }
        public string observations { get; set; }
        public string number_collection { get; set; }
    }

    public class InfoOil
    {
        public List<WeeklyOilData> WeeklyOilData { get; set; }
        public List<ClientOil> Clients { get; set; }
        public double Total { get; set; }
    }

    public class ClientOil
    {
        public string Name { get; set; }
        public double Oil { get; set; }
    }

    public class WeeklyOilData
    {
        public string WeekStart { get; set; }
        public string WeekEnd { get; set; }
        public double TotalOilAllWeeks { get; set; }
    }


    public class RecolectionModelStat
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string nameClient { get; set; }
        public string City { get; set; }
        public string NIT { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int BusinessType { get; set; }
        public int HeadquarterId { get; set; }
        public string NameHeadquarter { get; set; }
        public DateTime Date { get; set; }
        public float Amount { get; set; }
        public int MeasureId { get; set; }
        public string NameMeasure { get; set; }
        public int CollectorId { get; set; }
        public string NameCollector { get; set; }
        public float fullPayment { get; set; }
        public string Observations { get; set; }
        public string Serial { get; set; }
        public bool IsSent { get; set; }
        public string BusinessTypeName { get; set; } // Nuevo campo
        public string SignatureImage { get; set; } // Nuevo campo
    }


    public class WeeklyOilStatistic
    {
        public DateTime WeekStart { get; set; } 
        public float TotalOil { get; set; } 
    }

    public class DateRangeRequest
    {
        public DateTime StartDate { get; set; } // Fecha de inicio.
        public DateTime EndDate { get; set; }   // Fecha de fin.
    }




}
