namespace ResiGrass_API.Models
{
    public class HeadQuartersModel
    {
        public int id { get; set; }
        public ClientModel? clientData { get; set; }
        public LocalitiesModel? localitiesData { get; set; }
        public required string numberPhone { get; set; }
        public required int localityId { get; set; }
        public required string nameHeadquarter { get; set; }
        public required int clientId { get; set; }
        public required string address { get;set; }
        public required DateTime dateCreationHeadquarter { get; set; }
        public required bool status { get; set; }
    }

    public class HeadQuartersModelCreation
    {
        public required string numberPhone { get; set; }
        public required int localityId { get; set; }
        public required string nameHeadquarter { get; set; }
        public required int clientId { get; set; }
        public required string address { get; set; }
        public required DateTime dateCreationHeadquarter { get; set; }
        public required bool status { get; set; }
    }
}
