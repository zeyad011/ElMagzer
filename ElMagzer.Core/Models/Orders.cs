namespace ElMagzer.Core.Models
{
    public class Orders:BaseModel
    {
        public string? OrderCode {  get; set; }
        public string? OrederType { get; set; }
        public int? numberofCows { get; set; }
        public int? numberofbatches { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ?EndDate {  get; set; }
        public DateTime ?Date {  get; set; }
        public string? status { get; set; }
        public double? Performance { get; set; }
        public int? ClientsId { get; set; }
        public Clients? Clients { get; set; }
        public string Approve { get; set; } = "pending";
        public ICollection<Batch> Batches { get; set; } = new HashSet<Batch>();
    }
}
