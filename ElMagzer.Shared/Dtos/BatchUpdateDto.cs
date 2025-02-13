namespace ElMagzer.Shared.Dtos
{
    public class BatchUpdateDto
    {
        public string BatchCode { get; set; }
        public string cowType { get; set; }
        public int Quntity { get; set; }
        public List<CowDto> Cows { get; set; }
    }
}
