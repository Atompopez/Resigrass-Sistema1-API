namespace ResiGrass_API.Models
{

    public class CollectorsModel
    {

        public int id { get; set; }
        public  string nameCollector {  get; set; }
        public  string numberPhoneCollector { get; set; }
        public string profile_image { get; set; }
        public  bool status {  get; set; }
        //public  int loginCollectorId { get; set; }
        //public  int typeCollectorId { get; set; }
        public  DateTime dateCreationCollector { get; set; }

    }   
    public class CollectorsModelSelect
    {

        public int id { get; set; }
        public  string nameCollector {  get; set; }
        public  string numberPhoneCollector { get; set; }
        public string profile_image { get; set; }

        public bool status { get; set; }

        public string nextSerialNumber { get; set; }
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

    public class CollectorModelUpdate
    {
        public string nameCollector { get; set; }
        public string numberPhoneCollector { get; set; }
        public bool status { get; set; }
        public int typeCollectorId { get; set; }
        public string ProfileImageBase64 { get; set; } // Imagen en formato Base64
    }



    public class CollectorRequestModel
    {
        public CollectorModelInsert CollectorModel { get; set; }
        public string ProfileImage { get; set; }
        public loginCreationCollectorModel LoginCollectorModel { get; set; }
    }


    public class UpdateCollectionModel
    {
        public int CollectionId { get; set; } // ID de la recolección a actualizar
        public float FullPayment { get; set; } // Nuevo valor del pago
        public float NetWeight { get; set; }   // Nuevo valor del peso
    }


}
