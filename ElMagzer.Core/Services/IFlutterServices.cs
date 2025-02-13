using ElMagzer.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
namespace ElMagzer.Core.Services
{
    public interface IFlutterServices
    {
        public Task<ActionResult> GetPiece(string PieceId);

        //public Task<ActionResult> Inventory(DateTime? date,string store);
        //public Task<ActionResult> Log(string name, string password);
        public Task<ActionResult> Executions([FromBody] NumbersDto request);
        public Task<ActionResult> GetStoresList();
        public Task<ActionResult> GetStoreDetails(int storeId);
        public Task<ActionResult> GetPieceTypesInStore(int storeId);
        public Task<ActionResult> ValidatePiecesInStore(int storeId, string pieceType, List<string> pieceNumbers);
        public Task<ActionResult> TransferPiecesAsync(int sourceStoreId, int destinationStoreId, List<string> piecesList);
        public Task<ActionResult> GetSalesOrders();
        public Task<ActionResult> GetRecoveryOrders();
        public Task<ActionResult> ValidatePiecesToOrders(string orderId, List<string> pieceIds);
        public Task<ActionResult> GetslaughteredOreder();
        public Task<ActionResult> GetOrdersWithPieces();
        public Task<ActionResult> GetBoneInSalesOrders();
        public Task<ActionResult> GetBonelessSalesOrders();
        public Task<ActionResult> GetDetails();
        public Task<ActionResult> AddCowSeed(AddCowSeedDto dto);
        public Task<ActionResult> ProcessMeatSalesOrder(List<string> pieceIds);

    }
}
