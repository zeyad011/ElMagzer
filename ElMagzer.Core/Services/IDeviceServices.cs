using ElMagzer.Core.Models;
using ElMagzer.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ElMagzer.Core.Services
{
    public interface IDeviceServices
    {
        
        public Task<ActionResult> ScanDevice1 (string CowsId,string TechId,string DocId,int MachId);
        public Task<ActionResult> ScanDevice2 (double weight,int TypeOfCow,string TechId, int MachId,int storeId);
        //public Task<ActionResult> ModifyCowPieceType(int option);
        public Task<ActionResult> ScanDevice3(string pieceId, double weight, string TechId, int status, int MachId);
        public Task<ActionResult> ScanDevice4(string typeofPiece,double weight,int status,string TechId,int MachId, int storeId);
        public Task<ActionResult> ScanDevice5(double weight, string CowsId, string TechId, int MachId, string MiscarriageTypeId);
        public Task<ActionResult> GetLastCowsId();
        public Task<IActionResult> GetLastPiece();
        
        public Task<bool> SendCowPieceDataToExternalApi(CowsPieces cowPiece, string orderNumber, string store);

        public Task<IActionResult> SendTodayCowPieces([FromQuery] string orderNumber);
        public Task<ActionResult> UpdateLastCowPieceAsync(double newWeight, int newStoreId);
        public Task<bool> SendDevice4DataToExternalApi(Cow_Pieces_2 cowPiece, int storeId);
    }
}
