using ElMagzer.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ElMagzer.Core.Services
{
    public interface IFrontServices
    {
        public Task<ActionResult> GetHorizonDetails(DateTime? date, DateTime? graphtime);
        public Task<ActionResult> GetHorizonDetailsForSeals(DateTime? date, DateTime? graphtime);
        public Task<ActionResult> GetHorizonDetailsForRecvory(DateTime? date, DateTime? graphtime);

        //public Task<ActionResult> GetTable(DateTime? date);
        public Task<ActionResult> CreateOrder(CreateOrderDto dto);
        public Task<ActionResult> GetTypesWithCows();
        public Task<ActionResult> AssignBatchesToCows(AssignBatchesDto dto);
        public Task<ActionResult> AssignBatchesToPieces(AssignBatchesToPiecesDto dto);
        public Task<ActionResult> AssignBatchesToPieces2(AssignBatchesToPiecesDto dto);
        public Task<ActionResult> GetCowsPiecesIds();
        public Task<ActionResult> GetCowPieces2Numbers();
        public Task<IActionResult> GetCowsIdsByBatch(string batchCode);
        public Task<ActionResult> GetCowDetails(string cowId);
        public Task<ActionResult> GetStorePieces();
        public Task<ActionResult> GetStores(DateTime? date = null, string? sort = null);
        public Task<ActionResult> AddNewPiece(string Name,string Code, string Type);
        public Task<ActionResult> GetAllOrders(string? orderCode = null);
        public Task<ActionResult> GetAllOrdersWithApprove(string? orderCode = null);
        public Task<ActionResult> AddNewStore(string Name,int HeightCapacity,string SiteId);
        public Task<ActionResult> AddNewClient (string Name,string Code);
        public Task<ActionResult> GetClientOrders(string? search = null);
        public Task<string> GetAuthToken();
       // public Task<string> GetWorkOrderDataAsync(string companyId, string transDate);
        public Task<WorkOrderResponse> GetWorkOrderDataAsync2(string companyId, string transDate, string token);
        public Task<ActionResult> ProcessWorkOrderAndCreateOrder(string companyId, string transDate);
        public Task<ActionResult> UpdateOrderApproval(OrderApprovalDto dto);
        Task FetchAndStoreClientsAsync();
        Task FetchAndStoreSuppliersAsync();
        public Task<ActionResult> DeletePiecesFromOrder(DeletePieceDto dto);
        Task <ActionResult<List<CowWithPiecesDto>>> GetCowsWithPiecesByDate(DateTime date);
        public  Task<ActionResult<List<CowPieceDto>>> GetPiecesByCowId(string cowId);
    }
}
