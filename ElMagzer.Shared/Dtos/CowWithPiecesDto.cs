namespace ElMagzer.Shared.Dtos
{
    public class CowWithPiecesDto
    {
        public string CowsId { get; set; }
        public double? Cow_Weight { get; set; }
        public string CowType { get; set; }
        public string Tech {  get; set; }
        public string Doctor { get; set; }
        public string batch { get; set; }
        public string order { get; set; }

        public string Create_At_Divece1 { get; set; }
        public List<CowPieceDto> Pieces { get; set; } = new List<CowPieceDto>();
    }
}
