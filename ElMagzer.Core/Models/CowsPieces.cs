namespace ElMagzer.Core.Models
{
    public class CowsPieces:BaseModel
    {
        public string pieceId { get; set; }
        public double pieceWeight_In { get; set; }
        public double? pieceWeight_Out { get; set; }
        public string? PieceTybe { get; set; }
        public int? Tybe { get; set; }
        public DateTime Create_At_Divece2 { get; set; } = DateTime.Now;
        public DateTime dateOfExpiere { get; set; } = DateTime.Now.AddDays(8);
        public DateTime? Create_At_Divece3 { get; set; }
        public string techOfDevice2 { get; set; }
        public string? techOfDevice3 { get; set; }
        public int machien_Id_Device2 { get; set; }
        public int? machien_Id_Device3 { get; set; }
        public string? Status { get; set; }
        public string? Status_From_Device_2 { get; set; }
        public bool isExecutions { get; set; } = false;
        public int? BatchId { get; set; } 
        public Batch Batch { get; set; }
        public int StoreId { get; set; } 
        public Stores Store { get; set; }
        public int CowId { get; set; } 
        public Cows Cow { get; set; }
    }
}
