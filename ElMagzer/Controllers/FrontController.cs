using ElMagzer.Core.Models;
using ElMagzer.Core.Services;
using ElMagzer.Shared.Dtos;
using ElMagzer.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ElMagzer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrontController : ControllerBase
    {
        private readonly IFrontServices _frontServices;

        public FrontController(IFrontServices frontServices)
        {
            _frontServices = frontServices;
        }
        [HttpGet("GetCowStatistics")]
        public async Task<ActionResult> GetCowStatistics(DateTime? date = null, DateTime? graphtime = null)
        {
            return await _frontServices.GetHorizonDetails(date, graphtime);
        }
        [HttpGet("GetHorizonDetailsForSeals")]
        public async Task<ActionResult> GetHorizonDetailsForSeals(DateTime? date = null, DateTime? graphtime = null)
        {
            return await _frontServices.GetHorizonDetailsForSeals(date, graphtime);
        }
        [HttpGet("GetHorizonDetailsForRecvory")]
        public async Task<ActionResult> GetHorizonDetailsForRecvory(DateTime? date = null, DateTime? graphtime = null)
        {
            return await _frontServices.GetHorizonDetailsForRecvory(date, graphtime);
        }
        //[HttpGet("GetTable")]
        //public async Task<ActionResult> GetTable(DateTime? date = null)
        //{
        //    return await _frontServices.GetTable(date);
        //}
        [HttpGet("GetCowsIdsByBatch")]
        public async Task<IActionResult> GetCowsIdsByBatch(string batchCode)
        {
            return await _frontServices.GetCowsIdsByBatch(batchCode);
        }
        [HttpGet("GetCowDetails")]
        public async Task<ActionResult> GetCowDetails(string cowId)
        {
            return await _frontServices.GetCowDetails(cowId);
        }
        [HttpGet("GetTypesWithCows")]
        public async Task<IActionResult> GetTypesWithCows()
        {
            return await _frontServices.GetTypesWithCows();
        }
        [HttpPost("create-order")]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            return await _frontServices.CreateOrder(dto);
        }

        [HttpPost("AssignBatchesToCows")]
        public async Task<ActionResult> AssignBatchesToCows([FromBody] AssignBatchesDto dto)
        {
            return await _frontServices.AssignBatchesToCows(dto);
        }
        [HttpGet("GetCowsPiecesIds")]
        public async Task<ActionResult<IEnumerable<string>>> GetCowsPiecesIds()
        {
            return await _frontServices.GetCowsPiecesIds();
        }
        [HttpGet("GetCowPieces2Numbers")]
        public async Task<ActionResult<IEnumerable<string>>> GetCowPieces2Numbers()
        {
            return await _frontServices.GetCowPieces2Numbers();
        }

        [HttpPost("AssignBatchesToPieces")]
        public async Task<ActionResult> AssignBatchesToPieces([FromBody] AssignBatchesToPiecesDto dto)
        {
            return await _frontServices.AssignBatchesToPieces(dto);
        }
        [HttpPost("AssignBatchesToPieces2")]
        public async Task<ActionResult> AssignBatchesToPieces2([FromBody] AssignBatchesToPiecesDto dto)
        {
            return await _frontServices.AssignBatchesToPieces2(dto);
        }
        [HttpGet("GetStorePieces")]
        public async Task<ActionResult> GetStorePieces()
        {
            return await _frontServices.GetStorePieces();
        }
        [HttpGet("GetStores")]
        public async Task<IActionResult> GetStores(DateTime? date = null, string? sort = null)
        {
            return await _frontServices.GetStores(date, sort);
        }
        [HttpPost("AddCutting")]
        public async Task<IActionResult> AddCuttingWithDate([FromBody] CuttingDto cuttingDto)
        {
            return await _frontServices.AddNewPiece(cuttingDto.CutName, cuttingDto.Code,cuttingDto.Type);
        }
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult> GetAllOrders(string? orderCode = null)
        {
            return await _frontServices.GetAllOrders(orderCode);
        }
        [HttpGet("GetAllOrdersWithApprove")]
        public async Task<ActionResult> GetAllOrdersWithApprove(string? orderCode = null)
        {
            return await _frontServices.GetAllOrdersWithApprove(orderCode);
        }
        [HttpPost("add-store")]
        public async Task<ActionResult> AddNewStore(string name, int heightCapacity, string SiteId)
        {
            return await _frontServices.AddNewStore(name, heightCapacity,SiteId);
        }
        [HttpPost("add-Client")]
        public async Task<ActionResult> AddnewClient(string name, string code)
        {
            return await _frontServices.AddNewClient(name, code);
        }
        [HttpGet("get-client-orders")]
        public async Task<ActionResult> GetClientOrders(string? search = null)
        {
            return await  _frontServices.GetClientOrders(search);
        }
        [Authorize]
        [HttpGet("Test")]
        public IActionResult Test()
        {
            var response = new
            {
                Feed1 = 192.25,
                Feed2 = 111,
                RealMaterial1 = 555,
                RealMaterial2 = 250,
                LiquidStarch1 = 450,
                LiquidStarch2 = 155,
                Oil1 = 320,
                Oil2 = 555,
                OEE = 80,
                UpTime = 70,
                MonthlyData = new List<object>
        {
                          new { Month = "January", Feed1 = 150 },
                          new { Month = "February", Feed1 = 160 },
                          new { Month = "March", Feed1 = 170 },
                          new { Month = "April", Feed1 = 180 },
                          new { Month = "May", Feed1 = 190 },
                          new { Month = "June", Feed1 = 200 },
                          new { Month = "July", Feed1 = 210 },
                          new { Month = "August", Feed1 = 220 },
                          new { Month = "September", Feed1 = 230 },
                          new { Month = "October", Feed1 = 240 },
                          new { Month = "November", Feed1 = 250 },
                          new { Month = "December", Feed1 = 260 }
        }
            };
            return Ok(response);
        }
        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = new List<TestDto>
        {
            new TestDto { Name = "Order1", Status = "Active" },
            new TestDto { Name = "Order2", Status = "Closed" },
            new TestDto { Name = "Order3", Status = "Active" },
            new TestDto { Name = "Order4", Status = "Closed" },
            new TestDto { Name = "Order5", Status = "Closed" },
            new TestDto { Name = "Order6", Status = "Active" }
        };

            return Ok(orders);
        }

        [HttpGet("GetToken")]
        public async Task<ActionResult> GetToken()
        {
            var token = await _frontServices.GetAuthToken();
            return Ok(new { AccessToken = token });
        }
        //[HttpPost("GetWorkOrderData")]
        //public async Task<IActionResult> GetWorkOrderData([FromBody] WorkOrderRequestDto request)
        //{
        //    try
        //    {
        //        var result = await _frontServices.GetWorkOrderDataAsync2(request.CompanyId, request.TransDate);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Error = ex.Message });
        //    }
        //}
        [HttpPost("ProcessWorkOrderAndCreateOrder")]
        public async Task<IActionResult> ProcessWorkOrderAndCreateOrder([FromBody] WorkOrderRequestDto request)
        {
            try
            {
                var result = await _frontServices.ProcessWorkOrderAndCreateOrder(request.CompanyId, request.TransDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        [HttpPost("ApproveOrder")]
        public async Task<ActionResult> UpdateOrderApproval([FromBody] OrderApprovalDto dto)
        {
           return await _frontServices.UpdateOrderApproval(dto);
        }
        [HttpDelete("EditeOrder")]
        public async Task<ActionResult> DeletePiecesFromOrder([FromBody] DeletePieceDto dto)
        {
            return await _frontServices.DeletePiecesFromOrder(dto);
        }
        [HttpPost("fetch-clients")]
        public async Task<IActionResult> FetchClients()
        {
            await _frontServices.FetchAndStoreClientsAsync();
            return Ok("Clients fetched and stored successfully.");
        }
        [HttpPost("fetch-supplier")]
        public async Task<IActionResult> FetchSupplier()
        {
            await _frontServices.FetchAndStoreSuppliersAsync();
            return Ok("Suppliers fetched and stored successfully.");
        }
    }
}
