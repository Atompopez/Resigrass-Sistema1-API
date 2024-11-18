namespace ResiGrass_API.Models
{
    public class userAdminModel
    {
        public int id { get; set; }
        public required string name { get; set; }
        public required string user { get; set; }
        public required string password { get; set; }
        public string phoneNumber { get; set; }
        public int profileId { get; set; }

    }
    
    public class userAdminLoginModel
    {        
        public required string password { get; set; }
        public required string user { get; set; }        
    }
}

