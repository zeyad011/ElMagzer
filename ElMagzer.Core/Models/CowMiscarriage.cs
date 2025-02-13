namespace ElMagzer.Core.Models
{
    public class CowMiscarriage
    {
        public string BarCode { get; set; }
        public double Weight { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int CowsId { get; set; }
        public Cows Cow { get; set; }
        public int MiscarriageTypeId { get; set; }
        public MiscarriageType MiscarriageType { get; set; }
    }
}
