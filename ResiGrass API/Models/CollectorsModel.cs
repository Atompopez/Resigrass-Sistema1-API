namespace ResiGrass_API.Models
{
    public class CollectorsModel
    {
        public int id { get; set; }
        TypeCollectorsModel? typeCollectorsModelId { get; set; }
        LoginCollectorModel? loginCollectorModelId { get; set; }
        public required string nameCollector {  get; set; }
        public required int numberPhoneCollector { get; set; }
        public required bool status {  get; set; }

    }
}
