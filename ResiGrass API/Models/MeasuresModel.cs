namespace ResiGrass_API.Models
{
    public class MeasuresModel
    {
        public int id { get; set; }
        public required string descriptionMeasures { get; set; }
        public required string abbreviation { get; set; }
        public required bool status { get; set; }
    }
}
