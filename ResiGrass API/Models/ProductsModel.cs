namespace ResiGrass_API.Models
{
    public class ProductsModel
    {
        public int id { get; set; }
        public required string descriptionProduct { get; set; }
        public required string abbreviation { get; set; }
        public required bool status { get; set; }

    }
}
