using ElMagzer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElMagzer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceServices _deviceServices;

        public DevicesController(IDeviceServices deviceServices)
        {
            _deviceServices = deviceServices;
        }
        [HttpPost("ScanForDevice1")]
        public async Task<ActionResult> ScanCows(string CowsId, string TechId, string DocId, int MachId)
        {
            return await _deviceServices.ScanDevice1(CowsId,TechId,DocId,MachId);
        }
        [HttpPost("ScanForDevice2")]
        public async Task<ActionResult> ScanCows2(double weight, int TypeOfCow, string TechId, int MachId,int storeId)
        {
            return await _deviceServices.ScanDevice2(weight, TypeOfCow, TechId, MachId, storeId);
        }
        //[HttpPost("ModifyCowPieceType")]
        //public async Task<ActionResult> ModifyCowPieceType(int option)
        //{
        //    return await _deviceServices.ModifyCowPieceType(option);
        //}
        [HttpPost("ScanForDevice3")]
        public async Task<ActionResult> 
            ScanCows3(string pieceId, double weight, string TechId, int status, int MachId)
        {
            return await _deviceServices.ScanDevice3(pieceId, weight, TechId, status,MachId);
        }
        [HttpPost("ScanForDevice4")]
        public async Task<ActionResult> ScanCows4(string typeofPiece, double weight, int status, string TechId, int MachId, int storeId)
        {
            return await _deviceServices.ScanDevice4(typeofPiece, weight, status, TechId, MachId, storeId);
        }
        [HttpPost("ScanForDevice5")]
        public async Task<ActionResult> ScanCows5(double weight, string CowsId, string TechId, int MachId,string type)
        {
            return await _deviceServices.ScanDevice5(weight, CowsId, TechId, MachId, type);
        }
        [HttpGet("GetLastCowsId")]
        public async Task <ActionResult> GetLastCowsId()
        {

            return await _deviceServices.GetLastCowsId();
        }
        [HttpGet("TEST")]
        public IActionResult GetRandomNumber(int inputNumber)
        {

            var random = new Random();
            int randomNumber = random.Next();
           

            return Ok(new { RandomNumberZ = randomNumber });
        }
        [HttpGet("GetLastPiece")]
        public async Task<IActionResult> GetLastPiece()
        {
            return await _deviceServices.GetLastPiece();
        }
        [HttpGet("PushToApi")]
        public async Task<IActionResult> SendTodayCowPieces([FromQuery] string orderNumber)
        {
            return await _deviceServices.SendTodayCowPieces(orderNumber);
        }
    }
}
