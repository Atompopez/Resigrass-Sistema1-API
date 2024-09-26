namespace ResiGrass_API.Models
{
    public class LoginCollectorModel
    {
        public int id { get; set; }
        public required string user { get; set; }
        public required string password { get; set; }
        public required bool status { get; set; }

    }
}
