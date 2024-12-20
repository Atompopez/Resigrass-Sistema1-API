namespace ResiGrass_API.Models
{
    public class ClientModel
    {
        public int id { get; set; }
        public TypeBusinessModel businessModelData { get; set; }
        public required string nitCc { get; set; }
        public required string nameClient { get; set; }
        public int typeBusinessId { get; set; }

        public required DateTime dateCreationClient { get; set; }
        public required bool status {  get; set; }

    
    }

    public class ClientModelInsert
    {
    
        public  string nitCc { get; set; }
        public  string nameClient { get; set; }
        public int typeBusinessId { get; set; }
        public  DateTime dateCreationClient { get; set; }
        public  bool status { get; set; }


    }
}
