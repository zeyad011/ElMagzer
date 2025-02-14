public class DeletePieceDto
{
    public string OrderCode { get; set; }
    public string BatchCode { get; set; }
    public List<string> PieceIds { get; set; }
}
