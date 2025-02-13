

namespace ElMagzer.Core.Models
{
    public class Cows: BaseModel
    {
        public string CowsId { get; set; }
        public double ?Cow_Weight { get; set; }
        public DateTime Create_At_Divece1 { get; set; } = DateTime.Now;
        public DateTime? Create_At_Divece5 { get; set; }
        public double? Waste { get; set; }
        public double? Miscarage { get; set; }
        public int machien_Id_Device1 { get; set; }
        public int? machien_Id_Device5 { get; set; }
        public string Doctor_Id { get; set; }
        public string techOfDevice1 { get; set; }
        public string? techfDevice5 { get; set; }
        public int BatchId { get; set; }
        public Batch Batch { get; set; }
        public int TypeofCowsId { get; set; }
        public TypeofCows TypeofCows { get; set; }
        public int CowsSeedId { get; set; }
        public CowsSeed CowsSeed { get; set; }
        public ICollection<CowMiscarriage> CowMiscarriages { get; set; } = new HashSet<CowMiscarriage>();
    }
}
