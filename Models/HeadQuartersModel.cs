namespace ResiGrass_API.Models
{
    public class HeadQuartersModel
    {
        public int id { get; set; }
        public  string numberPhone { get; set; }
        public  int localityId { get; set; }
        public  int clientId { get; set; }
        public  string address { get;set; }
        public  bool status { get; set; }
        public required string nameHeadquarter { get; set; }
        public LocalitiesModel? localitiesData { get; set; }
        public ClientModel? clientData { get; set; }
        public  DateTime dateCreationHeadquarter { get; set; }
    }

    public class HeadQuartersModelGet
    {
        public int id { get; set; }
        public string numberPhone { get; set; }
        public int localityId { get; set; }
        public int clientId { get; set; }
        public string address { get; set; }
        public string? signatureImage { get; set; }
        public bool status { get; set; }
        public string? email { get; set; }
        public required string nameHeadquarter { get; set; }
        public LocalitiesModelGet? localitiesData { get; set; }                
    }

    public class HeadQuartersModelCreation
    {
        public required string numberPhone { get; set; }
        public required int localityId { get; set; }
        public required string nameHeadquarter { get; set; }
        public string? SignatureImage { get; set; }
        public required int clientId { get; set; }
        public required string address { get; set; }
        public  string email { get; set; }
        public required DateTime dateCreationHeadquarter { get; set; }
        public required bool status { get; set; }
    }
}
