namespace ElMagzer.Core.Models
{
    public class Batch:BaseModel
    {
        public string BatchCode { get; set; }
        public string BatchType { get; set; }
        public int? numberOfCowOrPieces { get; set; }
        public string? CowOrPiecesType { get; set; }
        public int OrderId { get; set; } 
        public Orders Order { get; set; }

        public ICollection<CowsPieces> CowsPieces { get; set; } = new HashSet<CowsPieces>();
        public ICollection<Cow_Pieces_2> CowPieces2 { get; set; } = new HashSet<Cow_Pieces_2>();
        public ICollection<Cows> Cows { get; set; } = new HashSet<Cows>();
        public ICollection<CowsSeed> CowsSeed { get; set; } = new HashSet<CowsSeed>();
    }
}
