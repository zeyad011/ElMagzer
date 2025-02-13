namespace ElMagzer.Shared.Dtos
{
    public class cowDetailsDto
    {
        public string CowId { get; set; }
        public string OrderId { get; set; }
        public string BatchCode { get; set; }
        public string TypeOfCow { get; set; }
        public string ProductionDate { get; set; }
        public double Weight { get; set; }
        public string Doctor { get; set; }
        public string Worker { get; set; }
    }
}
