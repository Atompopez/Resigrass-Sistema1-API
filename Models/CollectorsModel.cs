namespace ResiGrass_API.Models
{

    public class CollectorsModel
    {

        public int id { get; set; }
        public TypeCollectorsModel? typeCollectorsModelId { get; set; }
        public UserAdminModel? loginCollectorModelId { get; set; }
        public  string nameCollector {  get; set; }
        public  string numberPhoneCollector { get; set; }
        public  bool status {  get; set; }
        public  int loginCollectorId { get; set; }
        public  int typeCollectorId { get; set; }
        public  DateTime dateCreationCollector { get; set; }

    }   
    public class CollectorsModelSelect
    {

        public int id { get; set; }
        public  string nameCollector {  get; set; }
        public  string numberPhoneCollector { get; set; }
        public TypeCollectorsModelSelect? typeCollectorsModelId { get; set; }        
        

    }
    public class CollectorModelInsert
    {

        public required string nameCollector { get; set; }
        public required string numberPhoneCollector { get; set; }
        public required bool status { get; set; }
        public  int loginCollectorId { get; set; }
        public required int typeCollectorId { get; set; }
        public required DateTime dateCreationCollector { get; set; }


    }



    public class CollectorRequestModel
    {
        public CollectorModelInsert CollectorModel { get; set; }
        public loginCreationCollectorModel LoginCollectorModel { get; set; }
    }

}
