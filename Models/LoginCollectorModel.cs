namespace ResiGrass_API.Models
{
    public class loginCollectorModel
    {
        public int id { get; set; }
        public required string user { get; set; }
        public required string password { get; set; }
        public required bool status { get; set; }

    }

    public class loginCreationCollectorModel
    {
        public required string user { get; set; }
        public required string password { get; set; }
        public required bool status { get; set; }

    }

    public class loginCreationCollectorModelValidate
    {
        public required string user { get; set; }
        public required string password { get; set; }

    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<CollectorsModel> Collectors { get; set; }

        public LoginResponse()
        {
            Collectors = new List<CollectorsModel>();
        }
    }

}
