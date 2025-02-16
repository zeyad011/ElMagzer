using ElMagzer.Core.Services;
using ElMagzer.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElMagzer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlutterServiceController : ControllerBase
    {
        private readonly IFlutterServices _flutterServices;

        public FlutterServiceController(IFlutterServices flutterServices)
        {
            _flutterServices = flutterServices;
        }

        [HttpGet("GetPiece")]
        public async Task<ActionResult> GetPiece(string PieceId)
        {
            return await _flutterServices.GetPiece(PieceId);
        }
        ///[HttpGet("Login")]
        ///public async Task<ActionResult> Login(string name, string password)
        ///{
        ///    return await _flutterServices.Log(name, password);
        ///}
        ///[HttpGet("GetInventory")]
        ///public async Task<ActionResult> GetInventory(string store,DateTime? date = null)
        ///{
        ///    return await _flutterServices.Inventory(date, store);
        ///}
        ///[HttpGet("Login")]
        ///public async Task<ActionResult> Login(string name,string password)
        ///{
        ///    return await _flutterServices.Log(name, password);
        ///}
        [Authorize]
        [HttpPost("Executions")]
        public async Task<ActionResult> Executions([FromBody] NumbersDto request)
        {
            return await _flutterServices.Executions(request);
        }

        [HttpGet("Stores")]
        public async Task<ActionResult> Stores()
        {
            return await _flutterServices.GetStoresList();
        }
        [HttpGet("GetStoreDetails")]
        public async Task<ActionResult> GetStoreDetails(int storeId)
        {
            return await _flutterServices.GetStoreDetails(storeId);
        }

        [HttpGet("GetPieceTypesInStore")]
        public async Task<ActionResult> GetPieceTypesInStore(int storeId)
        {
            return await _flutterServices.GetPieceTypesInStore(storeId);
        }
        [Authorize]
        [HttpPost("ValidatePiecesInStore")]
        public async Task<ActionResult> ValidatePiecesInStore(ChecktoDto dto)
        {
            return await _flutterServices.ValidatePiecesInStore(dto.storeId, dto.pieceType, dto.pieceNumbers);
        }
        [Authorize]
        [HttpPost("TransferPieces")]
        public async Task<ActionResult> TransferPiecesAsync(TransferDto dto)
        {
            return await _flutterServices.TransferPiecesAsync(dto.sourceStoreId, dto.destinationStoreId, dto.piecesList);
        }
        [HttpGet("GetSalesOrders")]
        public async Task<ActionResult> GetSalesOrders()
        {
            return await _flutterServices.GetSalesOrders();
        }
        [HttpGet("GetRecoveryOrders")]
        public async Task<ActionResult> GetRecoveryOrders()
        {
            return await _flutterServices.GetRecoveryOrders();
        }
        [HttpGet("GetslaughteredOreder")]
        public async Task<ActionResult> GetslaughteredOreder()
        {
            return await _flutterServices.GetslaughteredOreder();
        }
        [HttpGet("GetBonelessSalesOrders")]
        public async Task<ActionResult> GetBonelessSalesOrders()
        {
            return await _flutterServices.GetBonelessSalesOrders();
        }
        [HttpGet("GetBoneInSalesOrders")]
        public async Task<ActionResult> GetBoneInSalesOrders()
        {
            return await _flutterServices.GetBoneInSalesOrders();
        }
        [Authorize]
        [HttpPost("ValidatePiecesToOrders")]
        public async Task<ActionResult> ValidatePiecesToOrders(OrdersDto ordersDto)
        {
            return await _flutterServices.ValidatePiecesToOrders(ordersDto.Ordersnumber, ordersDto.pieceNumbers);
        }
        [HttpGet("GetOrdersWithPieces")]
        public async Task<ActionResult> GetOrdersWithPieces()
        {
            return await _flutterServices.GetOrdersWithPieces();
        }
        [HttpGet("GetDetailsForClient&Supplier&TypeofCow")]
        public async Task<ActionResult> GetDetails()
        {
            return await _flutterServices.GetDetails();
        }
        [Authorize]
        [HttpPost("AddCowSeed")]
        public async Task<IActionResult> AddCowSeed([FromBody] AddCowSeedDto dto)
        {
            return await _flutterServices.AddCowSeed(dto);
        }
        [Authorize]
        [HttpPost("SealsPieces")]
        public async Task<IActionResult> SealsPieces([FromBody] NumbersDto request)
        {
            return await _flutterServices.ProcessMeatSalesOrder(request.Numbers);
        }
        [HttpGet("GetUnprocessedRecoveryPieces")]
        public async Task<IActionResult> GetUnprocessedRecoveryPieces()
        {
            return await _flutterServices.GetUnprocessedRecoveryPieces();
        }
    }
}
