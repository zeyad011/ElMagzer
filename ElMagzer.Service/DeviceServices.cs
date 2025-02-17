using ElMagzer.Core.Models;
using ElMagzer.Core.Services;
using ElMagzer.Repository.Data;
using ElMagzer.Shared.Errors;
using ElMagzer.Shared.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace ElMagzer.Service
{
    public class DeviceServices : IDeviceServices
    {
        private readonly ElMagzerContext _context;
        private readonly IHubContext<CowHub> _hubContext;
        private readonly IFrontServices _frontServices;

        public DeviceServices(ElMagzerContext context, IHubContext<CowHub> hubContext,IFrontServices frontServices)
        {
            _context = context;
            _hubContext = hubContext;
            _frontServices = frontServices;
        }
        private string mapType(int cowType)
        {
            return cowType switch
            {
                1 => "زند شمال",
                2 => "فخده شمال",
                3 => "زند يمين",
                4 => "فخده يمين",
                _ => throw new ArgumentException("Invalid cowType.")
            };
        }
        private string mapTypee3(int cowType)
        {
            return cowType switch
            {
                1 => "left Sholder",
                2 => "left Thigh",
                3 => "Right Sholder",
                4 => "Right Thigh",
                _ => throw new ArgumentException("Invalid cowType.")
            };
        }
        private string mapType2(int? cowType)
        {
            return cowType switch
            {
                1 => "10-2001",
                2 => "10-2002",
                3 => "10-2003",
                4 => "10-2004",
                _ => throw new ArgumentException("Invalid cowType.")
            };
        }
        private string statusType(int status)
        {
            return status switch
            {
                1 => "Out",
                2 => "Cutting",
                _ => throw new ArgumentException("Invalid status.")
            };
        }
        private string statusType2(int status)
        {
            return status switch
            {
                1 => "In",
                2 => "Out",
                _ => throw new ArgumentException("Invalid status.")
            };
        }
        public async Task<ActionResult> ScanDevice1(string CowsId, string TechId, string DocId, int MachId)
        {
            if (string.IsNullOrWhiteSpace(CowsId) || string.IsNullOrWhiteSpace(TechId) || string.IsNullOrWhiteSpace(DocId) || MachId <= 0)
                return new BadRequestObjectResult(new ApiResponse(400, "Invalid input data."));

            var cowSeed = await _context.cowsSeeds
                     .FirstOrDefaultAsync(c => c.CowsId == CowsId);
            var cow = await _context.Cows
                     .FirstOrDefaultAsync(c => c.CowsId == CowsId);
            if (cow != null)
                return new BadRequestObjectResult(new ApiResponse(400, "Already Exist"));
            if (cowSeed == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Cow seed not found. Verify CowsId."));
            if (cowSeed.BatchId == null || cowSeed.BatchId <= 0)
                return new NotFoundObjectResult(new ApiResponse(404, "Not found"));
            var order = await _context.Orders
                    .Include(o => o.Batches)
                    .ThenInclude(b => b.Cows)
                    .FirstOrDefaultAsync(o => o.Batches.Any(b => b.Id == cowSeed.BatchId));
            if (order == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Order not found"));

            if (order.Approve?.ToLower() != "approve") 
                return new BadRequestObjectResult(new ApiResponse(400, "Order not approved"));

            var cows = new Cows
            {
                CowsId = CowsId,
                techOfDevice1 = TechId,
                machien_Id_Device1 = MachId,
                Doctor_Id = DocId,
                BatchId = cowSeed.BatchId ?? 0,
                TypeofCowsId = cowSeed.TypeofCowsId,
                Cow_Weight = cowSeed.weight,
                CowsSeedId = cowSeed.Id,
            };
            _context.Cows.Add(cows);
            await _context.SaveChangesAsync();

           
                
            if (order.OrederType == "ذبح")
            {              
                    var totalCowsNeeded = order.numberofCows;
                    var totalCowsScanned = order.Batches.SelectMany(b => b.Cows).Count();

                    order.status = totalCowsScanned == totalCowsNeeded ? "Success" : "Running";
                    order.Date = DateTime.Today;
                
            }
            await _context.SaveChangesAsync();
            var response = await _frontServices.GetHorizonDetails(null,null);
            await _hubContext.Clients.All.SendAsync("CowScanned", response);
            return new OkObjectResult("OK");
        }

        public async Task<ActionResult> ScanDevice2(double weight,int TypeofCow,string TechId, int MachId, int storeId)
        {

            var cow = await _context.Cows
                         .Include(c => c.TypeofCows)
                         .Include(c => c.Batch)
                         .ThenInclude(b => b.Order)
                         .ThenInclude(o => o.Clients)
                         .Include(c=>c.CowsSeed)
                         .ThenInclude(cs=>cs.suppliers)
                         .OrderBy(c => c.Create_At_Divece1)
                         .FirstOrDefaultAsync(c => !_context.CowsPieces
                           .Where(cp => cp.CowId == c.Id)
                           .GroupBy(cp => cp.CowId)
                           .Any(g => g.Count() >= 4));

            if (cow == null)
                return new NotFoundObjectResult(new ApiResponse(404));
            int existingPiecesCount = await _context.CowsPieces
        .CountAsync(cp => cp.CowId == cow.Id);

            int Type = existingPiecesCount + 1;
            string pieceType = mapType(Type);


            //var existingPiece = await _context.CowsPieces
            //                .Include(cp => cp.Batch) 
            //                .ThenInclude(b => b.Order)
            //                .Include(cp => cp.Store)
            //               .FirstOrDefaultAsync(cp => cp.CowId == cow.Id && cp.PieceTybe == pieceType);

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == storeId);
            if (store == null)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "Store not found"));
            }

            if (store.quantity >= store.HeightCapacity)
            {
                return new BadRequestObjectResult(new ApiResponse(400, "Store capacity exceeded"));
            }
            string pieceId;
            Random random = new Random();
            do
            {
                int randomSixDigits = random.Next(100, 999);
                pieceId = $"{cow.CowsId}{randomSixDigits}";
            }
            while (await _context.CowsPieces.AnyAsync(cp => cp.pieceId == pieceId));

            var cowPiece = new CowsPieces
            {
                CowId = cow.Id,
                pieceId = pieceId,
                PieceTybe = pieceType,
                Tybe = Type,
                machien_Id_Device2 = MachId,
                pieceWeight_In = weight,
                techOfDevice2 = TechId,
                StoreId = storeId,
                BatchId = cow.BatchId
            };
            _context.CowsPieces.Add(cowPiece);

            store.quantity = (store.quantity ?? 0) + 1;

            #region for shatat server
            bool isSent = await SendCowPieceDataToExternalApi(cowPiece, cow.Batch.Order.OrderCode, storeId.ToString()); 
            if (!isSent)
                {
                    return new BadRequestObjectResult(new ApiResponse(400, "Failed to send data to external API"));
                }
            #endregion
            await _context.SaveChangesAsync();

            var cowPieces = await _context.CowsPieces
                    .Where(cp => cp.CowId == cow.Id)
                    .ToListAsync();
            if (cowPieces is null) return new NotFoundObjectResult(new ApiResponse(404, "There is no cowPiece"));

            #region NullMistake
            //if (cowPieces.Count == 4 && cowPieces.Any(cp => cp.PieceTybe == null))
            //{
            //    var existingTypes = cowPieces.Where(cp => cp.PieceTybe != null).Select(cp => cp.PieceTybe).ToHashSet();

            //    var existingNumericTypes = cowPieces.Where(cp => cp.Tybe.HasValue).Select(cp => cp.Tybe.Value).ToHashSet();

            //    var allTypes = new HashSet<string> { "كتف يمين", "فخده شمال", "فخده يمين", "كتف شمال" };
            //    var allNumericTypes = new HashSet<int> { 1, 2, 3, 4 };

            //    var missingType = allTypes.Except(existingTypes).FirstOrDefault();
            //    var missingNumericType = allNumericTypes.Except(existingNumericTypes).FirstOrDefault();

            //    if (missingType != null || missingNumericType != 0)
            //    {
            //        var pieceToFill = cowPieces.FirstOrDefault(cp => cp.PieceTybe == null || !cp.Tybe.HasValue);
            //        if (pieceToFill != null)
            //        {
            //            pieceToFill.PieceTybe = missingType;
            //            pieceToFill.Tybe = missingNumericType;
            //            await _context.SaveChangesAsync();
            //        }
            //    }
            //} 
            #endregion
            if (cowPieces.Count == 4)
            {
                double? totalPieceWeight = cowPieces.Sum(cp => cp.pieceWeight_In);
                cow.Waste = cow.Cow_Weight - totalPieceWeight;
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
            #region ForOK2
            //if (existingPiece != null)
            //{

            //    //return new OkObjectResult(new
            //    //{
            //    //    statusCode = 200,
            //    //    message = $"Ok2 Z{cowPiece.pieceId}L",
            //    //    supplierName = "ElRawda",
            //    //    cowId = cow.CowsId,
            //    //    typeOfCow = cow.TypeofCows.TypeName,
            //    //    clientName = cowPiece.Batch.Order.Clients,
            //    //    orderNumber = cowPiece.Batch.Order.OrderCode,
            //    //    pieceType = cowPiece.PieceTybe,
            //    //    weight = weight,
            //    //    createDate = cowPiece.Create_At_Divece2.ToString("dd/MM/yy"),
            //    //    batchNumber = cowPiece.BatchId
            //    //});
            //    return new OkObjectResult(new { statusCode = 200, message = $"Ok2 Z{cowPiece.pieceId}L" });
            //}
            //var response = new
            //{
            //    statusCode = 200,
            //    message = $"OK1 Z{cowPiece.pieceId}L",
            //    supplierName = "ElRawda",
            //        cowId = cow.CowsId,
            //        typeOfCow = cow.TypeofCows.TypeName,
            //        clientName = cowPiece.Batch.Order.Clients,
            //        orderNumber = cowPiece.Batch.Order.OrderCode,
            //        pieceType = cowPiece.PieceTybe,
            //        weight = weight,
            //        createDate = cowPiece.Create_At_Divece2.ToString("dd/MM/yy"),
            //        batchNumber = cowPiece.BatchId
            //}; 

            #endregion
            //var Result = $"OK1 Z{cowPiece.pieceId},ElRawda,{cowPiece.Batch.Order.Clients},{cowPiece.Batch.BatchCode},{cowPiece.Batch.Order.OrderCode},Baaldy,left Shoulder,{weight},{cowPiece.Create_At_Divece2.ToString("dd/MM/yy")}L";

            var messageParts = new[]
                     {
                     $"OK1 Z{cowPiece?.pieceId ?? "null"}",
                     $"{cowPiece ?.Cow ?.CowsSeed.suppliers.NameENG ?? "null"}",
                     $"{cowPiece ?.Batch ?.Order ?.Clients ?.NameENG ?? "null"}",
                     $"{(cowPiece?.BatchId.HasValue == true ? cowPiece.BatchId.ToString() : "null")}",
                     $"{cowPiece?.Batch?.Order?.OrderCode ?? "null"}",
                     $"{cowPiece?.Cow.TypeofCows.TypeNameENG ??"null"}",
                     $"{mapTypee3(Type)} {Type}",
                     $"{weight}",
                     $"{cowPiece?.Create_At_Divece2.ToString("dd/MM/yy") ?? "null"}"
                     };
            var Result = $"{string.Join(",", messageParts)}L";

            var response = new
            {
                statusCode = 200,
                message = Result
            };
            var signal = await _frontServices.GetHorizonDetails(null, null);
            await _hubContext.Clients.All.SendAsync("CowScanned", signal);
            return new OkObjectResult(response);
        }

        #region ModifyPiece
        //public async Task<ActionResult> ModifyCowPieceType(int option)
        //{

        //    var allCowPieces = await _context.CowsPieces.ToListAsync();
        //    var today = DateTime.Today;


        //    var todayCowPieces = allCowPieces
        //        .Where(cp => cp.Create_At_Divece2.Date == today)
        //        .ToList();


        //    var duplicatePieces = todayCowPieces
        //                          .GroupBy(cp => new { cp.CowId, cp.PieceTybe, cp.Tybe })
        //                          .Where(g => g.Count() > 1)
        //                          .SelectMany(g => g.OrderBy(cp => cp.Id))
        //                          .ToList();


        //    if (duplicatePieces.Any())
        //    {
        //        var cowPieceToModify = option == 1 ? duplicatePieces.Skip(1).FirstOrDefault() : duplicatePieces.FirstOrDefault();
        //        if (cowPieceToModify != null)
        //        {
        //            cowPieceToModify.PieceTybe = null;
        //            cowPieceToModify.Tybe = null;
        //        }
        //        await _context.SaveChangesAsync();
        //    }


        //    var cowGroups = todayCowPieces.GroupBy(cp => cp.CowId);
        //    foreach (var cowGroup in cowGroups)
        //    {
        //        if (cowGroup.Count() == 4)
        //        {
        //            var existingTypes = cowGroup.Where(cp => cp.PieceTybe != null).Select(cp => cp.PieceTybe).ToHashSet();
        //            var existingNumericTypes = cowGroup.Where(cp => cp.Tybe.HasValue).Select(cp => cp.Tybe.Value).ToHashSet();
        //            var allTypes = new HashSet<string> { "كتف يمين", "فخده شمال", "فخده يمين", "كتف شمال" };
        //            var allNumericTypes = new HashSet<int> { 1, 2, 3, 4 };

        //            var missingType = allTypes.Except(existingTypes).FirstOrDefault();
        //            var missingNumericType = allNumericTypes.Except(existingNumericTypes).FirstOrDefault();
        //            if (missingType != null)
        //            {
        //                var pieceToFill = cowGroup.FirstOrDefault(cp => cp.PieceTybe == null || !cp.Tybe.HasValue);
        //                if (pieceToFill != null)
        //                {
        //                    pieceToFill.PieceTybe = missingType;
        //                    pieceToFill.Tybe = missingNumericType;
        //                }
        //            }
        //        }
        //    }
        //    var response = new
        //    {
        //        statusCode = 200,
        //        message = "OK"
        //    };
        //    await _context.SaveChangesAsync();
        //    return new OkObjectResult(response);
        //} 
        #endregion

        public async Task<ActionResult> ScanDevice3(string pieceId, double weight, string TechId, int status, int MachId)
        {

            var cowPiece = await _context.CowsPieces
                             .Include(cp => cp.Batch)
                             .ThenInclude(b => b.Order)
                             .Include(cp => cp.Store)
                             .FirstOrDefaultAsync(cp => cp.pieceId == pieceId);

            if (cowPiece == null)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "NO3"));
            }
            if (cowPiece.Batch?.Order?.Approve?.ToLower() != "approve")
                return new BadRequestObjectResult(new ApiResponse(400, "Order not approved"));

            if (status == 1) 
            {
                if (cowPiece.Batch == null || cowPiece.Batch.Order == null || cowPiece.Batch.Order.OrederType != "بيع لحم بعضم")
                {
                    return new BadRequestObjectResult(new ApiResponse(400, "NO1"));
                }
            }
            else 
            {
                if (cowPiece.Batch == null || cowPiece.Batch.Order == null || cowPiece.Batch.Order.OrederType != "تشافي")
                {
                    return new BadRequestObjectResult(new ApiResponse(400, "NO2"));
                }
            }

            cowPiece.pieceWeight_Out = weight;
            cowPiece.techOfDevice3 = TechId;
            cowPiece.machien_Id_Device3 = MachId;
            cowPiece.Status = statusType(status);
            cowPiece.Create_At_Divece3 = DateTime.Now;
            await _context.SaveChangesAsync();
            if (cowPiece.Store != null && cowPiece.Store.quantity > 0)
            {
                cowPiece.Store.quantity -= 1; 
            }
            else
            {
                return new BadRequestObjectResult(new ApiResponse(400, "Insufficient stock in store"));
            }
            var order = await _context.Orders
                    .Include(o => o.Batches) 
                    .ThenInclude(b => b.CowsPieces) 
                    .FirstOrDefaultAsync(o => o.Id == cowPiece.Batch.OrderId);
            if (order != null)
            {

                    var totalPiecesNeeded = order.Batches.Sum(b => b.numberOfCowOrPieces);
                    var totalPiecesProcessed = order.Batches
                    .Sum(b => b.CowsPieces.Count(cp => cp.pieceWeight_Out.HasValue && cp.BatchId == b.Id));


                order.status = totalPiecesNeeded==totalPiecesProcessed ? "Success" : "Running";
                order.Date = DateTime.Now;
              
            }
            var response = new
            {
                pieceWeight_InZ = cowPiece.pieceWeight_In,
            };

            await _context.SaveChangesAsync();
            if (status == 1)
            {
                var signal = await _frontServices.GetHorizonDetailsForSeals(null, null);
                await _hubContext.Clients.All.SendAsync("Seals", signal);
            }
            else
            {
                var signal = await _frontServices.GetHorizonDetailsForRecvory(null, null);
                await _hubContext.Clients.All.SendAsync("Recvory", signal);
            }
            return new OkObjectResult(response);
        }

        public async Task<ActionResult> ScanDevice4(string typeofPiece, double weight, int status, string TechId, int MachId,int storeId)
        {
            var cutting = await _context.cuttings.FirstOrDefaultAsync(c => c.Code == typeofPiece);
            if (cutting == null)
                return new BadRequestObjectResult(new ApiResponse(400, "Invalid TypePiece Code"));
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == storeId);
            if (store == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Store not found"));

            if (store.quantity >= store.HeightCapacity)
                return new BadRequestObjectResult(new ApiResponse(400, "Store capacity exceeded"));

            Random random = new Random();
            string pieceNumber = $"P{random.Next(100000, 999999)}";
            var cowPiece = new Cow_Pieces_2
            {
                PieceNumber = pieceNumber,
                //PieceType = pieceType,
                Weight = weight,
                status = statusType2(status),
                techofDevice4 = TechId,
                machien_Id_Device4 = MachId,
                StoreId = storeId,
                CuttingId = cutting.Id
            };
            _context.Cow_Pieces_2.Add(cowPiece);
            store.quantity = (store.quantity ?? 0) + 1;
            await _context.SaveChangesAsync();
            var response = new
            {
                statusCode = 200,
                message = $"OK1 Z{cowPiece.PieceNumber}L"
            };
            return new OkObjectResult(response);

        }

        public async Task<ActionResult> ScanDevice5(double weight, string CowsId, string TechId, int MachId, string MiscarriageTypeId)
        {
            var cow = await _context.Cows
       .FirstOrDefaultAsync(c => c.CowsId == CowsId);

            if (cow == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Cow not found."));

            var miscarriageType = await _context.miscarriageTypes
                .FirstOrDefaultAsync(mt => mt.Code == MiscarriageTypeId);

            if (miscarriageType == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Miscarriage type not found."));


            cow.Miscarage = (cow.Miscarage ?? 0) + weight; 
            cow.techfDevice5 = TechId;
            cow.machien_Id_Device5 = MachId;
            cow.Create_At_Divece5 = DateTime.Now;


            var cowMiscarriage = await _context.cowMiscarriages
                .FirstOrDefaultAsync(cm => cm.CowsId == cow.Id && cm.MiscarriageTypeId == miscarriageType.Id);
            string barCode;
            Random random = new Random();
            int randomSixDigits = random.Next(100, 999);
            if (cowMiscarriage != null)
                return new BadRequestObjectResult(new ApiResponse(400, "5od"));

                                                            
            barCode = $"{cow.CowsId}{randomSixDigits}";     
            _context.cowMiscarriages.Add(new CowMiscarriage 
            {                                               
                BarCode = $"{cow.CowsId}{randomSixDigits}", 
                CowsId = cow.Id,                            
                MiscarriageTypeId = miscarriageType.Id,     
                Weight = weight,                            
            });


            await _context.SaveChangesAsync();
            var signal = await _frontServices.GetHorizonDetails(null, null);
            await _hubContext.Clients.All.SendAsync("CowScanned", signal);
            return new OkObjectResult($"OK1 Z{barCode}L");
        }

        public async Task<ActionResult> GetLastCowsId()
        {
            var today = DateTime.Today; 

            var lastCow = await _context.cowsSeeds
                .Where(c => !c.IsPrinted && c.CreatedAt.Date == today) 
                .FirstOrDefaultAsync();

            if (lastCow == null)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "NO1"));
            }
            lastCow.IsPrinted = true;

            await _context.SaveChangesAsync();

            return new OkObjectResult(new { LastCowsId = $"OK1 Z{lastCow.CowsId}L" });
        }

        public async Task<bool> SendCowPieceDataToExternalApi(CowsPieces cowPiece, string orderNumber, string store)
        {
            var token = await _frontServices.GetAuthToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Failed to retrieve authentication token.");

            var requestData = new
            {
                TypeId = 1, 
                CompanyID = "003", 
                OrderNumber = orderNumber,
                Itemid = mapType2(cowPiece.Tybe),
                Weight = cowPiece.pieceWeight_In,
                BarCode = cowPiece.pieceId, 
                TransDate = DateTime.Now.ToString("yyyy-MM-dd"), 
                Store = store, 
                MainItemid = "" 
            };
            var jsonContent = new StringContent(
              JsonSerializer.Serialize(requestData),
              Encoding.UTF8,
              "application/json"
              );

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://shatat-uat.sandbox.operations.dynamics.com/api/Services/Sha_IntegrationServiceGroup/Sha_IntegrationService/AddBarCodeWeightByType")
            {
                Content = jsonContent
            };
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                
                return true;
            }

            
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error sending data: {response.StatusCode} - {errorContent}");
            return false;
        }

        public async Task<IActionResult> GetLastPiece()
        {
            var lastPiece = await _context.CowsPieces
                .Include(cp => cp.Cow)
                    .ThenInclude(c => c.CowsSeed)
                        .ThenInclude(cs => cs.suppliers)
                .Include(cp => cp.Batch)
                    .ThenInclude(b => b.Order)
                        .ThenInclude(o => o.Clients)
                .OrderByDescending(cp => cp.Id)
                .FirstOrDefaultAsync();

            if (lastPiece == null)
                return new NotFoundObjectResult(new ApiResponse(404, "NO1"));

            var messageParts = new[]
            {
                $"OK1 Z{lastPiece?.pieceId ?? "null"}",
                $"{lastPiece?.Cow?.CowsSeed?.suppliers?.NameENG ?? "null"}",
                $"{lastPiece?.Batch?.Order?.Clients?.NameENG ?? "null"}",
                $"{(lastPiece?.BatchId.HasValue == true ? lastPiece.BatchId.ToString() : "null")}",
                $"{lastPiece?.Batch?.Order?.OrderCode ?? "null"}",
                $"{lastPiece?.Cow?.TypeofCows?.TypeNameENG ?? "null"}",
                $"{mapTypee3(lastPiece.Tybe ?? 0)} {lastPiece.Tybe ?? 0}",
                $"{lastPiece.pieceWeight_In}",
                $"{lastPiece?.Create_At_Divece2.ToString("dd/MM/yy") ?? "null"}"
             };
            var result = $"{string.Join(",", messageParts)}L";

            var response = new
            {
                statusCode = 200,
                message = result
            };

            return new OkObjectResult(response);
        }

        public async Task<IActionResult> SendTodayCowPieces([FromQuery] string orderNumber)
        {
            var today = DateTime.UtcNow.Date;
            var cowPieces = await _context.CowsPieces
                .Where(p => p.Create_At_Divece2.Date == today)
                .Include(cp=>cp.Batch)
                .Include(cp=>cp.Cow)
                .Include(cp=>cp.Store)
                .ToListAsync();

            if (!cowPieces.Any())
            {
                return new NotFoundObjectResult(new { Message = "No cow pieces found for today." });
            }

            foreach (var piece in cowPieces)
            {
                bool isSent = await SendCowPieceDataToExternalApi(piece, orderNumber,piece.Store.Id.ToString());

                if (!isSent)
                {
                    return new BadRequestObjectResult(new { Message = $"Failed to send cow piece {piece.pieceId}." });
                }
            }

            return new OkObjectResult(new { Message = "All cow pieces for today have been sent successfully!" });
        }
    }
}
