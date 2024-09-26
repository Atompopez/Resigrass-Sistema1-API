namespace ResiGrass_API.Models
{
    public class LocalitiesModel
    {
        public int id { get; set; }
        public int municipalityId { get; set; }
        public required string nameLocality { get; set; }
        public bool status { get; set; }
        public MunicipalityModel MunicipalityData { get; set; }
    }
}
