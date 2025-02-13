
namespace ElMagzer.Core.Models
{
    public class Cow_Pieces_2:BaseModel
    {
        public string PieceNumber { get; set; }
        public double Weight { get; set; }
        public string status { get; set; }
        public DateTime Create_At_Divece4 { get; set; } = DateTime.Now;
        public DateTime dateOfExpiere { get; set; } = DateTime.Now.AddDays(8);
        public string techofDevice4 { get; set; }
        public int machien_Id_Device4 { get; set; }
        public bool isExecutions { get; set; } = false;
        public int StoreId { get; set; }
        public Stores Store { get; set; }
        public int? BatchId { get; set; } 
        public Batch Batch { get; set; }
        public int CuttingId { get; set; }
        public Cutting Cutting { get; set; }

    }
}
