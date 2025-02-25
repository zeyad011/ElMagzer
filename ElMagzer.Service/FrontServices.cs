using AutoMapper;
using ElMagzer.Core.Models;
using ElMagzer.Core.Repositories;
using ElMagzer.Core.Services;
using ElMagzer.Core.Specifications.Spec;
using ElMagzer.Repository.Data;
using ElMagzer.Shared.Dtos;
using ElMagzer.Shared.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ElMagzer.Service
{
    public class FrontServices : IFrontServices
    {
        private readonly ElMagzerContext _context;
        private readonly HttpClient _httpClient;
        private readonly IGenericRepository<Cows> _genericRepository;
        private readonly IMapper _mapper;

        public FrontServices(ElMagzerContext context, HttpClient httpClient, IGenericRepository<Cows> genericRepository, IMapper mapper)
        {
            _context = context;
            _httpClient = httpClient;
            _genericRepository = genericRepository;
            _mapper = mapper;
        }


        public async Task<ActionResult> GetHorizonDetails(DateTime? date = null, DateTime? graphtime = null)
        {
            DateTime targetDate = date ?? DateTime.Today;
            DateTime nextDay = targetDate.AddDays(1);

            DateTime graphTime = graphtime ?? targetDate;
            int cowRequest = await _context.Orders
                      .Where(o => o.CreatedDate >= targetDate && o.CreatedDate < nextDay)
                      .SumAsync(o => o.numberofCows ?? 0);

            double totalMiscarage = await _context.Cows
                     .Where(c => c.Miscarage.HasValue && c.Create_At_Divece5 >= targetDate && c.Create_At_Divece5 < nextDay)
                     .SumAsync(c => c.Miscarage!.Value);

            int killedCowCount = await _context.Cows
                .CountAsync(c => c.Create_At_Divece1 >= targetDate && c.Create_At_Divece1 < nextDay);

            int reminders = cowRequest - killedCowCount;

            double totalWeightOfKilledCows = await _context.Cows
                .Where(c => c.Create_At_Divece1 >= targetDate && c.Create_At_Divece1 < nextDay)
                .SumAsync(c => c.Cow_Weight ?? 0); 

            double totalWaste = await _context.Cows
                .Where(c => c.Create_At_Divece1 >= targetDate && c.Create_At_Divece1 < nextDay)
                .SumAsync(c => c.Waste ?? 0);

            //var startOfDay = targetDate.Date.AddHours(9);//تعديل
            //var endOfDay = targetDate.Date.AddHours(17);

            var hours = Enumerable.Range(5, 15).Select(h => new { Hour = h, Time = targetDate.AddHours(h) }).ToList();


            var hourlyData = (await _context.CowsPieces
                    .Where(cp => cp.Create_At_Divece2 >= graphTime && cp.Create_At_Divece2 < graphTime.AddHours(24))
                    .ToListAsync())
                    .GroupBy(cp => cp.Create_At_Divece2.Hour)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(cp => cp.pieceWeight_In)
                    );

            var graph = hours.Select(h => new
            {
                Time = h.Time.ToString("h tt"),
                Value = hourlyData.ContainsKey(h.Hour) ? Math.Round(hourlyData[h.Hour], 2) : (double?)null
            }).ToList();


            var cowsDeviceTimes = await _context.Cows
        .Select(c => new { Time = c.Create_At_Divece1, Device = "Device1" })
        .ToListAsync();

            var cowPiecesDeviceTimes = await _context.Cow_Pieces_2
                .Select(cp => new { Time = cp.Create_At_Divece4, Device = "Device4" })
                .ToListAsync();

            var deviceTimes = cowsDeviceTimes
                .Concat(cowPiecesDeviceTimes)
                .OrderBy(dt => dt.Time)
                .ToList();

            int uptimeCounter = 0;
            for (int i = 1; i < deviceTimes.Count; i++)
            {
                var previousTime = deviceTimes[i - 1].Time;
                var currentTime = deviceTimes[i].Time;

                if ((currentTime - previousTime).TotalMinutes > 10)
                {
                    uptimeCounter += 5;
                }
            }

            var orders = await _context.Orders
                .Where(order => order.CreatedDate.Date == targetDate.Date
                                && order.OrederType == "ذبح"
                                && order.status == "Success")
                .SelectMany(order => order.Batches.Select(batch => new
                {
                    OrderId = order.OrderCode,
                    Batch = batch.BatchCode,
                    NumberOfCowOrPieces = batch.Cows.Count(),
                    StartDate = order.CreatedDate.ToString("dd/MM/yyyy"),
                    EndDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    OrderType = order.OrederType,
                    Customer = order.Clients != null ? order.Clients.Name : "N/A",
                    TotalWeight = Math.Round(batch.Cows.Sum(c => c.Cow_Weight ?? 0), 2),
                    TypeOfCows = batch.CowOrPiecesType,
                    Waste = Math.Round(batch.Cows.Sum(c => c.Waste ?? 0), 2),
                    Miscarage = Math.Round(batch.Cows.Sum(c => c.Miscarage ?? 0), 2)
                }))
                .ToListAsync();


            double uptimePercentage = ((double)uptimeCounter / 480) * 100;

            int standardRate = 8;
            DateTime startOfWorkDay = targetDate.Date.AddHours(9);

            double elapsedHours = (DateTime.Now - startOfWorkDay).TotalHours;
            elapsedHours = elapsedHours > 0 ? elapsedHours : 1;

            double performance = (killedCowCount / (standardRate * elapsedHours)) * 100;

            string upTimeFormatted = uptimePercentage > 0 ? $"{(100 - uptimePercentage):F2}%" : "100.00%";
            var response = new
            {
                CowRequest = cowRequest,
                KilledCow = killedCowCount,
                Reminders = reminders,
                WeightOfKilledCows = Math.Round(totalWeightOfKilledCows, 2),
                TotalWaste = Math.Round(totalWaste, 2),
                TotalMiscarage = Math.Round(totalMiscarage, 2),
                Performance = $"{performance:F2}",
                UpTime = upTimeFormatted,
                Graph = graph,
                Table = orders
            };

            return new OkObjectResult(response);
        }

        //public async Task<ActionResult> GetTable(DateTime? date)
        //{
        //    DateTime selectedDate = date ?? DateTime.Today;

        //    var orders = await _context.Orders
        //        .Where(order => order.CreatedDate.Date == selectedDate.Date
        //                        && order.OrederType == "ذبح"
        //                        && order.status == "Success")
        //        .SelectMany(order => order.Batches.Select(batch => new
        //        {
        //            OrderId = order.OrderCode,
        //            Batch = batch.BatchCode,
        //            NumberOfCowOrPieces = batch.Cows.Count(),
        //            StartDate = order.CreatedDate.ToString("dd/MM/yyyy"),
        //            EndDate = DateTime.Now.ToString("dd/MM/yyyy"),
        //            OrderType = order.OrederType,
        //            Customer = order.ClientName,
        //            TotalWeight = batch.Cows.Sum(c => c.Cow_Weight ?? 0),
        //            TypeOfCows = batch.CowOrPiecesType,
        //            Waste = batch.Cows.Sum(c => c.Waste ?? 0),
        //            Miscarage = batch.Cows.Sum(c => c.Miscarage ?? 0),
        //        }))
        //        .ToListAsync();

        //    return new OkObjectResult(orders);
        //}
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (dto.Code == null) return new NotFoundObjectResult(new ApiResponse(404));
            var orders = await _context.Orders
                        .FirstOrDefaultAsync(o => o.OrderCode == dto.Code);

            if (orders is not null) return new BadRequestObjectResult(new ApiResponse(400, "This Order Already Exist"));
            int? clientId = null;

            if (!string.IsNullOrWhiteSpace(dto.ClientName))
            {
                var client = await _context.clients
                    .FirstOrDefaultAsync(c => c.Name == dto.ClientName);

                if (client is null)
                {
                    return new NotFoundObjectResult(new ApiResponse(404, "Client not found."));
                }

                clientId = client.Id;
            }
            var order = new Orders
            {
                OrderCode = dto.Code,
                OrederType = dto.OrderType,
                numberofCows = dto.NoOfCows,
                numberofbatches = dto.NoOfBatches,
                ClientsId = clientId,
                EndDate = dto.date,
                status = "Pending"
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var batchList = new List<Batch>();
            var random = new Random();
            for (int i = 1; i <= dto.NoOfBatches; i++)
            {
                var batchCode = random.Next(100000, 999999).ToString();
                var batch = new Batch
                {
                    BatchCode = batchCode,
                    BatchType = dto.OrderType,
                    OrderId = order.Id
                };

                batchList.Add(batch);
            }

            await _context.Batches.AddRangeAsync(batchList);
            await _context.SaveChangesAsync();

            var response = new
            {
                OrderCode = order.OrderCode,
                NumberOfCows = order.numberofCows,
                NumberOfBatches = order.numberofbatches,
                Client = clientId.HasValue ? dto.ClientName : null,
                Batches = batchList.Select(b => new { b.BatchCode }).ToList()
            };

            return new OkObjectResult(response);
        }
        public async Task<ActionResult> GetTypesWithCows()
        {
            var typesWithCows = await _context.TypeofCows
                     .Select(type => new
                     {
                         TypeId = type.Id,
                         TypeName = type.TypeName,
                         Cows = _context.cowsSeeds
                             .Where(cow => cow.TypeofCowsId == type.Id && cow.BatchId == null)
                             .Select(cow => new
                             {
                                 CowId = cow.CowsId
                             })
                             .ToList()
                     })
                     .ToListAsync();

            return new OkObjectResult(typesWithCows);
        }

        public async Task<ActionResult> AssignBatchesToCows(AssignBatchesDto dto)
        {
            var order = await _context.Orders
                    .Include(o => o.Batches)
                    .FirstOrDefaultAsync(o => o.OrderCode == dto.OrderCode);
            if (order == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Order not found."));

            var batchCodes = dto.Batches.Select(b => b.BatchCode).ToList();
            var existingBatches = order.Batches
                .Where(b => batchCodes.Contains(b.BatchCode))
                .ToList();

            if (existingBatches.Count != dto.Batches.Count)
                return new BadRequestObjectResult(new ApiResponse(400, "One or more BatchCodes not found for the given Order."));

            foreach (var batchDto in dto.Batches)
            {
                var batch = existingBatches.FirstOrDefault(b => b.BatchCode == batchDto.BatchCode);
                if (batch == null) continue;

                var cowIds = batchDto.Cows.Select(c => c.CowId).ToList();
                var existingCows = await _context.cowsSeeds
                    .Where(c => cowIds.Contains(c.CowsId))
                    .ToListAsync();
                batch.numberOfCowOrPieces = batchDto.Quntity;
                batch.CowOrPiecesType = batchDto.cowType;
                foreach (var cow in existingCows)
                {
                    cow.BatchId = batch.Id;
                }
            }
            await _context.SaveChangesAsync();

            return new OkObjectResult("OK");
        }

        public async Task<ActionResult> GetCowsPiecesIds()
        {
            var piecesByType = await _context.CowsPieces
                        .Where(p => _context.Cows.Any(c => c.Id == p.CowId && c.BatchId == p.BatchId))
                        .GroupBy(p => p.PieceTybe)
                        .Select(group => new
                        {
                            TypeName = group.Key,
                            PiecesIds = group.Select(p => p.pieceId).ToList()
                        })
                        .ToListAsync();

            if (piecesByType == null || !piecesByType.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No pieces found without a BatchId."));

            return new OkObjectResult(piecesByType);
        }

        public async Task<ActionResult> GetCowPieces2Numbers()
        {
            var piecesByType = await _context.Cow_Pieces_2
                         .Where(p => p.BatchId == null)
                         .GroupBy(p => p.Cutting.CutName)
                         .Select(group => new
                         {
                             TypeName = group.Key,
                             PieceNumbers = group.Select(p => p.PieceNumber).ToList()
                         })
                         .ToListAsync();

            if (piecesByType == null || !piecesByType.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No pieces found without a BatchId."));

            return new OkObjectResult(piecesByType);
        }

        public async Task<IActionResult> GetCowsIdsByBatch(string batchCode)
        {
            var cowSpec = new CowsByBatchSpecification(batchCode);

            var cows = await _genericRepository.GetAllAysncWithspec(cowSpec);

            if (cows == null || !cows.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "Batch not found or no cows in this batch."));


            var response = new { Cows = cows.Select(c => new { c.CowsId }) };

            return new OkObjectResult(response);
        }

        public async Task<ActionResult> GetCowDetails(string cowId)
        {
            var cowSpec = new CowDetailsSpecification(cowId);
            var cow = await _genericRepository.GetByIDAysncWithspec(cowSpec);

            if (cow == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Cow not found."));

            ///var response = new
            ///{
            ///    CowId = cow.CowsId ?? "N/A",
            ///    OrderId = cow.Batch?.Order?.OrderCode ?? "N/A",
            ///    BatchCode = cow.Batch?.BatchCode ?? "N/A",
            ///    TypeOfCow = cow.TypeofCows?.TypeName ?? "N/A",
            ///    ProductionDate = cow.Batch?.Order?.CreatedDate != null
            ///             ? cow.Batch.Order.CreatedDate.ToString("dd/MM/yyyy")
            ///             : "N/A",
            ///    Weight = cow.CowsSeed?.weight ?? 0, 
            ///    Doctor = cow.Doctor_Id ?? "N/A",
            ///    Worker = cow.techOfDevice1 ?? "N/A"
            ///};
            var response = _mapper.Map<Cows, cowDetailsDto>(cow);

            return new OkObjectResult(response);
        }

        public async Task<ActionResult> AssignBatchesToPieces(AssignBatchesToPiecesDto dto)
        {
            var order = await _context.Orders
            .Include(o => o.Batches)
            .FirstOrDefaultAsync(o => o.OrderCode == dto.OrderCode);
            if (order == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Order not found."));

            var batchCodes = dto.Batches.Select(b => b.BatchCode).ToList();
            var existingBatches = order.Batches
                .Where(b => batchCodes.Contains(b.BatchCode))
                .ToList();

            if (existingBatches.Count != dto.Batches.Count)
                return new BadRequestObjectResult(new ApiResponse(400, "One or more BatchCodes not found for the given Order."));

            foreach (var batchDto in dto.Batches)
            {
                var batch = existingBatches.FirstOrDefault(b => b.BatchCode == batchDto.BatchCode);
                if (batch == null) continue;

                var pieceIds = batchDto.Pieces.Select(p => p.PieceId).ToList();

                var existingCowsPieces = await _context.CowsPieces
                    .Where(p => pieceIds.Contains(p.pieceId))
                    .ToListAsync();
                batch.numberOfCowOrPieces = batchDto.Quntity;
                batch.CowOrPiecesType = batchDto.cowType;
                foreach (var piece in existingCowsPieces)
                {
                    piece.BatchId = batch.Id;
                }
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult("OK");
        }
        public async Task<ActionResult> AssignBatchesToPieces2(AssignBatchesToPiecesDto dto)
        {
            var order = await _context.Orders
            .Include(o => o.Batches)
            .FirstOrDefaultAsync(o => o.OrderCode == dto.OrderCode);
            if (order == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Order not found."));

            var batchCodes = dto.Batches.Select(b => b.BatchCode).ToList();
            var existingBatches = order.Batches
                .Where(b => batchCodes.Contains(b.BatchCode))
                .ToList();

            if (existingBatches.Count != dto.Batches.Count)
                return new BadRequestObjectResult(new ApiResponse(400, "One or more BatchCodes not found for the given Order."));

            foreach (var batchDto in dto.Batches)
            {
                var batch = existingBatches.FirstOrDefault(b => b.BatchCode == batchDto.BatchCode);
                if (batch == null) continue;

                var pieceIds = batchDto.Pieces.Select(p => p.PieceId).ToList();

                var existingCowPieces2 = await _context.Cow_Pieces_2
                    .Where(p => pieceIds.Contains(p.PieceNumber))
                    .ToListAsync();
                batch.numberOfCowOrPieces = batchDto.Quntity;
                batch.CowOrPiecesType = batchDto.cowType;
                foreach (var piece in existingCowPieces2)
                {
                    piece.BatchId = batch.Id;
                }
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult("OK");
        }

        public async Task<ActionResult> GetStorePieces()
        {
            var storePieces = await _context.Stores
        .Select(store => new
        {
            StoreId = store.Id,
            StoreName = store.storeName,
            Pieces = store.CowsPieces
                .Select(cp => cp.pieceId)
                .Union(store.CowPieces2.Select(cp2 => cp2.PieceNumber))
                .ToList()
        })
        .ToListAsync();

            if (!storePieces.Any())
            {
                return new NotFoundObjectResult(new ApiResponse(404, "No stores or pieces found."));
            }

            return new OkObjectResult(storePieces);
        }

        public async Task<ActionResult> GetStores(DateTime? date = null, string? sort = null)
        {
            DateTime startOfDay = date?.Date ?? DateTime.MinValue;
            DateTime endOfDay = date.HasValue ? startOfDay.AddDays(1) : DateTime.MaxValue;

            var stores = await _context.Stores
                .Include(store => store.CowsPieces)
                    .ThenInclude(cp => cp.Batch)
                    .ThenInclude(b => b.Order)
                .Include(store => store.CowPieces2)
                    .ThenInclude(cp2 => cp2.Batch)
                    .ThenInclude(b => b.Order)
                .Include(store => store.CowPieces2)
                    .ThenInclude(cp2 => cp2.Cutting)
                .ToListAsync();

            var storesData = stores.Select(store => new
            {
                StoreName = store.storeName,
                TotalPieces = store.CowsPieces
                                   .Count(cp => cp.Create_At_Divece2 >= startOfDay && cp.Create_At_Divece2 < endOfDay && string.IsNullOrEmpty(cp.Status))
                               + store.CowPieces2
                                   .Count(cp2 => cp2.Create_At_Divece4 >= startOfDay && cp2.Create_At_Divece4 < endOfDay && cp2.status != "Out"),
                TotalWeight = Math.Round(
            store.CowsPieces
                .Where(cp => cp.Create_At_Divece2 >= startOfDay && cp.Create_At_Divece2 < endOfDay && string.IsNullOrEmpty(cp.Status))
                .Sum(cp => cp.pieceWeight_In)
            + store.CowPieces2
                .Where(cp2 => cp2.Create_At_Divece4 >= startOfDay && cp2.Create_At_Divece4 < endOfDay && cp2.status != "Out")
                .Sum(cp2 => cp2.Weight), 2),
                HeightCapacity = store.HeightCapacity,
                StoreFilledPercentage = Math.Round(
                ((store.CowsPieces
                               .Count(cp => cp.Create_At_Divece2 >= startOfDay && cp.Create_At_Divece2 < endOfDay && string.IsNullOrEmpty(cp.Status))
                           + store.CowPieces2
                               .Count(cp2 => cp2.Create_At_Divece4 >= startOfDay && cp2.Create_At_Divece4 < endOfDay && cp2.status != "Out"))
                 / (double)store.HeightCapacity) * 100, 2),

                Pieces = store.CowsPieces
                    .Where(cp => cp.Create_At_Divece2 >= startOfDay && cp.Create_At_Divece2 < endOfDay && string.IsNullOrEmpty(cp.Status))
                    .Select(cp => new StorePieceDto
                    {
                        PieceNumber = cp.pieceId,
                        Weight = Math.Round(cp.pieceWeight_In, 2),
                        PieceType = cp.PieceTybe ?? "N/A",
                        TechDevice = cp.techOfDevice2,
                        BatchNumber = cp.Batch?.BatchCode ?? "N/A",
                        OrderNumber = cp.Batch?.Order?.OrderCode ?? "N/A"
                    })
                    .Concat(
                        store.CowPieces2
                            .Where(cp2 => cp2.Create_At_Divece4 >= startOfDay && cp2.Create_At_Divece4 < endOfDay && cp2.status != "Out")
                            .Select(cp2 => new StorePieceDto
                            {
                                PieceNumber = cp2.PieceNumber,
                                Weight = Math.Round(cp2.Weight, 2),
                                PieceType = cp2.Cutting.CutName,
                                TechDevice = cp2.techofDevice4,
                                BatchNumber = cp2.Batch?.BatchCode ?? "N/A",
                                OrderNumber = cp2.Batch?.Order?.OrderCode ?? "N/A"
                            })
                    )
                    .OrderBy(p => p.PieceNumber)
                    .ToList()
            })
            .ToList();

            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.ToLower() == "desc")
                {
                    storesData = storesData.OrderByDescending(store => store.TotalWeight).ToList();
                }
                else if (sort.ToLower() == "asc")
                {
                    storesData = storesData.OrderBy(store => store.TotalWeight).ToList();
                }
            }

            return new OkObjectResult(storesData);
        }


        public async Task<ActionResult> AddNewPiece(string Name, string Code, string Type)
        {
            if (Type == "لحم مشفي")
            {
                var isCodeExists = await _context.cuttings.FirstOrDefaultAsync(c => c.Code == Code);
                if (isCodeExists is not null)
                    return new BadRequestObjectResult(new ApiResponse(400, "This Code already exists in the Cuttings table."));

                var cutting = new Cutting
                {
                    Code = Code,
                    CutName = Name,
                };
                _context.cuttings.Add(cutting);
            }
            else if (Type == "سقط")
            {
                var isCodeExists = await _context.miscarriageTypes.FirstOrDefaultAsync(m => m.Code == Code);
                if (isCodeExists is not null)
                    return new BadRequestObjectResult(new ApiResponse(400, "This Code already exists in the MiscarriageType table."));

                var miscarriageType = new MiscarriageType
                {
                    Code = Code,
                    Name = Name,
                };
                _context.miscarriageTypes.Add(miscarriageType);
            }
            else
            {
                return new BadRequestObjectResult(new ApiResponse(400, "Invalid Type. Must be 'لحم مشفي' or 'سقط'."));
            }
            await _context.SaveChangesAsync();

            return new OkObjectResult("OK");
        }

        public async Task<ActionResult> GetAllOrdersWithApprove(string? orderCode = null)
        {
            var query = _context.Orders
                .Where(o => o.Approve == "approve")
        .Include(o => o.Clients)
        .Include(o => o.Batches)
            .ThenInclude(b => b.Cows)
            .ThenInclude(c => c.TypeofCows)
        .Include(o => o.Batches)
            .ThenInclude(b => b.CowsPieces)
        .Include(o => o.Batches)
            .ThenInclude(b => b.CowPieces2)
            .ThenInclude(cp2 => cp2.Cutting)
        .AsQueryable();

            if (!string.IsNullOrEmpty(orderCode))
            {
                query = query.Where(o => o.OrderCode == orderCode);
            }

            var orders = await query.ToListAsync();

            var today = DateTime.Today;
            var cowsSeedData = await _context.cowsSeeds
                .Include(t => t.TypeofCows)
            .AsNoTracking()
            .ToListAsync();
            var orderList = orders.Select(order =>
            {

                //if (order.CreatedDate.Date != today && order.status != "Success")
                //    order.status = "Failed";

                return new
                {
                    OrderNumber = order.OrderCode,
                    TotalCount = order.OrederType == "ذبح"
                        ? order.numberofCows
                        : order.Batches.Sum(b => b.numberOfCowOrPieces),
                    Customer = order.OrederType != "ذبح" ? order.Clients?.Name : null,
                    OrderType = order.OrederType,
                    CreateDate = order.CreatedDate.ToString("MM/dd/yyyy"),
                    DeliverDate = order.EndDate?.ToString("MM/dd/yyyy"),
                    StartDate = order.Date?.ToString("MM/dd/yyyy"),
                    Status = order.status ?? "Pending",
                    Approve = order.Approve,
                    Batches = order.Batches.Select(b => new
                    {
                        BatchNumber = b.BatchCode,
                        Count = b.numberOfCowOrPieces,
                        BatchType = b.CowOrPiecesType,
                        StartDate = order.CreatedDate.ToString("MM/dd/yyyy"),
                        EndDate = order.EndDate?.ToString("MM/dd/yyyy"),
                        Numbers = order.OrederType == "ذبح"
                    ? cowsSeedData
                    .Where(cs => cs.BatchId == b.Id)
                    .Select(cs => new
                    {
                        Number = cs.CowsId,
                        Weights = cs.weight,
                        Type = cs.TypeofCows?.TypeName ?? "Unknown",
                        DoctorId = b.Cows.Select(c => c.Doctor_Id).FirstOrDefault() ?? "N/A",
                        Technician = b.Cows.Select(c => c.techOfDevice1).FirstOrDefault() ?? "N/A",
                    }).ToList()
                    : b.CowsPieces.Any()
                        ? b.CowsPieces.Select(cp => new
                        {
                            Number = cp.pieceId,
                            Weights = cp.pieceWeight_In,
                            Type = cp.PieceTybe ?? "Unknown",
                            DoctorId = "N/A",
                            Technician = "Mohamed"
                        }).ToList()
                        : b.CowPieces2.Select(cp2 => new
                        {
                            Number = cp2.PieceNumber,
                            Weights = cp2.Weight,
                            Type = cp2.Cutting.CutName ?? "Unknown",
                            DoctorId = "N/A",
                            Technician = "Mohamed"
                        }).ToList()
                    }).ToList()
                };
            }).ToList();

            await _context.SaveChangesAsync();

            return new OkObjectResult(orderList);
        }

        public async Task<ActionResult> GetAllOrders(string? orderCode = null)
        {
            var query = _context.Orders
        .Include(o => o.Clients)
        .Include(o => o.Batches)
            .ThenInclude(b => b.Cows)
            .ThenInclude(c => c.TypeofCows)
        .Include(o => o.Batches)
            .ThenInclude(b => b.CowsPieces)
        .Include(o => o.Batches)
            .ThenInclude(b => b.CowPieces2)
            .ThenInclude(cp2 => cp2.Cutting)
        .AsQueryable();

            if (!string.IsNullOrEmpty(orderCode))
            {
                query = query.Where(o => o.OrderCode == orderCode);
            }

            var orders = await query.ToListAsync();

            var today = DateTime.Today;
            var cowsSeedData = await _context.cowsSeeds
                .Include(t => t.TypeofCows)
            .AsNoTracking()
            .ToListAsync();
            var orderList = orders.Select(order =>
            {

                //if (order.CreatedDate.Date != today && order.status != "Success")
                //    order.status = "Failed";

                return new
                {
                    OrderNumber = order.OrderCode,
                    TotalCount = order.OrederType == "ذبح"
                        ? order.numberofCows
                        : order.Batches.Sum(b => b.numberOfCowOrPieces),
                    Customer = order.OrederType != "ذبح" ? order.Clients?.Name : null,
                    OrderType = order.OrederType,
                    CreateDate = order.CreatedDate.ToString("MM/dd/yyyy"),
                    DeliverDate = order.EndDate?.ToString("MM/dd/yyyy"),
                    StartDate = order.Date?.ToString("MM/dd/yyyy"),
                    Status = order.status ?? "Pending",
                    Approve = order.Approve,
                    Batches = order.Batches.Select(b => new
                    {
                        BatchNumber = b.BatchCode,
                        Count = b.numberOfCowOrPieces,
                        BatchType = b.CowOrPiecesType,
                        StartDate = order.CreatedDate.ToString("MM/dd/yyyy"),
                        EndDate = order.EndDate?.ToString("MM/dd/yyyy"),
                        Numbers = order.OrederType == "ذبح"
                    ? cowsSeedData
                    .Where(cs => cs.BatchId == b.Id)
                    .Select(cs => new
                    {
                        Number = cs.CowsId,
                        Weights = cs.weight,
                        Type = cs.TypeofCows?.TypeName ?? "Unknown",
                        DoctorId = b.Cows.Select(c => c.Doctor_Id).FirstOrDefault() ?? "N/A",
                        Technician = b.Cows.Select(c => c.techOfDevice1).FirstOrDefault() ?? "N/A",
                    }).ToList()
                    : b.CowsPieces.Any()
                        ? b.CowsPieces.Select(cp => new
                        {
                            Number = cp.pieceId,
                            Weights = cp.pieceWeight_In,
                            Type = cp.PieceTybe ?? "Unknown",
                            DoctorId = "N/A",
                            Technician = "Mohamed"
                        }).ToList()
                        : b.CowPieces2.Select(cp2 => new
                        {
                            Number = cp2.PieceNumber,
                            Weights = cp2.Weight,
                            Type = cp2.Cutting.CutName ?? "Unknown",
                            DoctorId = "N/A",
                            Technician = "Mohamed"
                        }).ToList()
                    }).ToList()
                };
            }).ToList();

            await _context.SaveChangesAsync();

            return new OkObjectResult(orderList);
        }

        public async Task<ActionResult> AddNewStore(string Name, int HeightCapacity, string SiteId)
        {
            if (string.IsNullOrEmpty(Name) || HeightCapacity <= 0)
            {
                return new BadRequestObjectResult(new ApiResponse(400, "Invalid data. Name cannot be empty and HeightCapacity must be greater than 0."));
            }

            var existingStore = await _context.Stores.FirstOrDefaultAsync(s => s.storeName == Name);
            if (existingStore != null)
                return new BadRequestObjectResult(new ApiResponse(400, "A store with the same name already exists."));

            var newStore = new Stores
            {
                storeName = Name,
                HeightCapacity = HeightCapacity,
                Code = SiteId,
                quantity = 0
            };
            _context.Stores.Add(newStore);
            await _context.SaveChangesAsync();

            return new OkObjectResult("OK");
        }

        public async Task<ActionResult> AddNewClient(string Name, string Code)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Code))
            {
                return new BadRequestObjectResult(new ApiResponse(400, "Invalid data. Name or Code cannot be empty."));
            }
            var existingclient = await _context.clients.FirstOrDefaultAsync(s => s.Code == Code || s.Name == Name);
            if (existingclient != null)
                return new BadRequestObjectResult(new ApiResponse(400, "A Client with the same name or Code already exists."));
            var newClient = new Clients
            {
                Name = Name,
                Code = Code,
            };
            _context.clients.Add(newClient);
            await _context.SaveChangesAsync();

            return new OkObjectResult("OK");
        }

        public async Task<ActionResult> GetClientOrders(string? search = null)
        {
            var query = _context.clients.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(client => client.Name.Contains(search) || client.Code.Contains(search));
            }


            query = query
                .Include(client => client.Orders)
                    .ThenInclude(order => order.Batches)
                        .ThenInclude(batch => batch.CowsPieces)
                .Include(client => client.Orders)
                    .ThenInclude(order => order.Batches)
                        .ThenInclude(batch => batch.CowPieces2)
                .Include(client => client.Orders)
                    .ThenInclude(order => order.Batches)
                       .ThenInclude(batch => batch.Cows);


            var clientsData = await query.ToListAsync();


            var result = clientsData.Select(client => new
            {
                ClientName = client.Name,
                Code = client.Code,
                LastOrder = client.Orders
                    .Where(order => order.status == "Success")
                    .OrderByDescending(order => order.CreatedDate)
                    .Select(order => new
                    {
                        Quantity = Math.Round(GetOrderQuantity(order), 2),
                        Date = order.CreatedDate.ToString("MM/dd/yyyy"),
                        DeliveryDate = order.EndDate != null
                            ? order.EndDate.Value.ToString("MM/dd/yyyy")
                            : "N/A",
                    })
                    .FirstOrDefault(),
                ActiveOrder = client.Orders
                    .Where(order => order.status == "Running")
                    .Select(order => new
                    {
                        Quantity = Math.Round(GetOrderQuantity(order), 2),
                        Date = order.CreatedDate.ToString("MM/dd/yyyy"),
                        DeliveryDate = order.EndDate != null
                            ? order.EndDate.Value.ToString("MM/dd/yyyy")
                            : "N/A",
                    })
                    .FirstOrDefault(),
                TotalQuantity = Math.Round(GetTotalQuantity(client.Orders), 2)
            }).ToList();

            return new OkObjectResult(result);
        }
        private static double GetOrderQuantity(Orders order)
        {
            return order.Batches.Sum(batch =>
                batch.CowsPieces.Sum(cp => cp.pieceWeight_In) +
                batch.CowPieces2.Sum(cp2 => cp2.Weight) +
                batch.Cows.Sum(cow => cow.Cow_Weight??0)
            );
        }
        private static double GetTotalQuantity(IEnumerable<Orders> orders)
        {
            return orders
                .SelectMany(order => order.Batches)
                .Sum(batch =>
                    batch.CowsPieces.Sum(cp => cp.pieceWeight_In) +
                    batch.CowPieces2.Sum(cp2 => cp2.Weight) +
                    batch.Cows.Sum(cow => cow.Cow_Weight??0)
                );
        }

        public async Task<ActionResult> GetHorizonDetailsForSeals(DateTime? date, DateTime? graphtime)
        {
            DateTime targetDate = date ?? DateTime.Today;
            DateTime nextDay = targetDate.AddDays(1);


            DateTime graphTime = graphtime ?? targetDate;
            var orders = await _context.Orders
                 .Where(o => o.EndDate >= targetDate && o.EndDate < nextDay && o.OrederType == "بيع لحم مشفي" || o.OrederType == "بيع لحم بعضم")
                 .Include(o => o.Batches)
                 .ToListAsync();

            int piecesRequest = orders.Sum(o => o.Batches.Sum(b => b.numberOfCowOrPieces ?? 0));

            int soldPiecesCount = await _context.CowsPieces
                .Where(cp => cp.Create_At_Divece3 >= targetDate && cp.Create_At_Divece3 < nextDay && cp.Status == "Out")
                .CountAsync();

            int reminders = piecesRequest - soldPiecesCount;

            double totalWeightOfSoldPieces = await _context.CowsPieces
                .Where(cp => cp.Create_At_Divece3 >= targetDate && cp.Create_At_Divece3 < nextDay && cp.Status == "Out")
                .SumAsync(cp => cp.pieceWeight_Out ?? 0);

            var hours = Enumerable.Range(5, 15).Select(h => new { Hour = h, Time = targetDate.AddHours(h) }).ToList();

            var hourlyData = (await _context.CowsPieces
                    .Where(cp => cp.Create_At_Divece3 >= graphTime && cp.Create_At_Divece3 < graphTime.AddHours(24) && cp.Status == "Out")
                    .ToListAsync())
                    .GroupBy(cp => cp.Create_At_Divece3?.Hour ?? 0)
                    .ToDictionary(
                        g => g.Key,
                        g => Math.Round(g.Sum(cp => cp.pieceWeight_Out ?? 0), 2)
                    );

            var graph = hours.Select(h => new
            {
                Time = h.Time.ToString("h tt"),
                Value = hourlyData.ContainsKey(h.Hour) ? hourlyData[h.Hour] : (double?)null
            }).ToList();

            var cowPiecesDevice3Times = await _context.CowsPieces
                        .Select(cp => new { Time = cp.Create_At_Divece3, Device = "Device3" })
                        .OrderBy(cp => cp.Time)
                        .ToListAsync();

            int uptimeCounterDevice3 = 0;

            for (int i = 1; i < cowPiecesDevice3Times.Count; i++)
            {
                var previousTime = cowPiecesDevice3Times[i - 1].Time;
                var currentTime = cowPiecesDevice3Times[i].Time;

                if (previousTime.HasValue && currentTime.HasValue)
                {
                    var timeDifference = currentTime.Value - previousTime.Value;

                    if (timeDifference.TotalMinutes > 10)

                        uptimeCounterDevice3 += 5;

                }
            }


            var res = await _context.Orders
                .Where(order => order.CreatedDate.Date == targetDate.Date
                                && (order.OrederType == "بيع لحم مشفي" || order.OrederType == "بيع لحم بعضم")
                                && order.status == "Success")
                .SelectMany(order => order.Batches.Select(batch => new
                {
                    OrderId = order.OrderCode,
                    Batch = batch.BatchCode,
                    NumberOfCowOrPieces = batch.numberOfCowOrPieces ?? 0,
                    StartDate = order.EndDate.HasValue ? order.EndDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    EndDate = order.Date.HasValue ? order.Date.Value.ToString("dd/MM/yyyy") : "N/A",
                    OrderType = order.OrederType,
                    Customer = order.Clients != null ?
                  order.Clients.Name : "N/A",
                    TotalWeight = Math.Round(batch.CowsPieces.Sum(cp => cp.pieceWeight_Out ?? 0), 2),
                }))
                .ToListAsync();
            double uptimePercentage = ((double)uptimeCounterDevice3 / 480) * 100;
            string upTimeFormatted = uptimePercentage > 0 ? $"{(100 - uptimePercentage):F2}%" : "100.00%";
            int standardRate = 100;
            double elapsedHours = (DateTime.Now - targetDate).TotalHours;
            elapsedHours = elapsedHours > 0 ? elapsedHours : 1;
            double performance = (soldPiecesCount / (standardRate * elapsedHours)) * 100;
            var response = new
            {
                PiecesRequest = piecesRequest,
                SoldPieces = soldPiecesCount,
                Reminders = reminders,
                WeightOfSoldPieces = Math.Round(totalWeightOfSoldPieces, 2),
                Performance = $"{performance:F2}",
                UpTime = upTimeFormatted,
                Graph = graph,
                Table = res
            };

            return new OkObjectResult(response);
        }

        public async Task<ActionResult> GetHorizonDetailsForRecvory(DateTime? date, DateTime? graphtime)
        {
            DateTime targetDate = date ?? DateTime.Today;
            DateTime nextDay = targetDate.AddDays(1);

            DateTime graphTime = graphtime ?? targetDate;
            var orders = await _context.Orders
                 .Where(o => o.EndDate >= targetDate && o.EndDate < nextDay && o.OrederType == "تشافي")
                 .Include(o => o.Batches)
                 .ToListAsync();

            int piecesRequest = orders.Sum(o => o.Batches.Sum(b => b.numberOfCowOrPieces ?? 0));

            int soldPiecesCount = await _context.CowsPieces
                .Where(cp => cp.Create_At_Divece3 >= targetDate && cp.Create_At_Divece3 < nextDay && cp.Status == "Cutting")
                .CountAsync();

            int reminders = piecesRequest - soldPiecesCount;

            double totalWeightOfSoldPieces = await _context.CowsPieces
                .Where(cp => cp.Create_At_Divece3 >= targetDate && cp.Create_At_Divece3 < nextDay && cp.Status == "Cutting")
                .SumAsync(cp => cp.pieceWeight_Out ?? 0);

            var hours = Enumerable.Range(5, 15).Select(h => new { Hour = h, Time = targetDate.AddHours(h) }).ToList();

            var hourlyData = (await _context.CowsPieces
                    .Where(cp => cp.Create_At_Divece3 >= graphTime && cp.Create_At_Divece3 < graphTime.AddHours(24) && cp.Status == "Cutting")
                    .ToListAsync())
                    .GroupBy(cp => cp.Create_At_Divece3?.Hour ?? 0)
                    .ToDictionary(
                        g => g.Key,
                        g => Math.Round(g.Sum(cp => cp.pieceWeight_Out ?? 0), 2)
                    );

            var graph = hours.Select(h => new
            {
                Time = h.Time.ToString("h tt"),
                Value = hourlyData.ContainsKey(h.Hour) ? hourlyData[h.Hour] : (double?)null
            }).ToList();
            var cowPiecesDevice3Times = await _context.CowsPieces
                       .Select(cp => new { Time = cp.Create_At_Divece3, Device = "Device3" })
                       .OrderBy(cp => cp.Time)
                       .ToListAsync();

            var cuttingPieces = await _context.CowsPieces
                 .Where(cp => cp.Status == "Cutting" && cp.machien_Id_Device3 != null)
                 .Select(cp => new { cp.pieceId, cp.pieceWeight_Out ,cp.pieceWeight_In,cp.Batch.BatchCode,cp.Batch.Order.OrderCode,cp.machien_Id_Device3,cp.PieceTybe})
                 .ToListAsync();
            int uptimeCounterDevice3 = 0;

            for (int i = 1; i < cowPiecesDevice3Times.Count; i++)
            {
                var previousTime = cowPiecesDevice3Times[i - 1].Time;
                var currentTime = cowPiecesDevice3Times[i].Time;

                if (previousTime.HasValue && currentTime.HasValue)
                {
                    var timeDifference = currentTime.Value - previousTime.Value;

                    if (timeDifference.TotalMinutes > 10)

                        uptimeCounterDevice3 += 5;

                }
            }

            var res = await _context.Orders
                .Where(order => order.CreatedDate.Date == targetDate.Date
                                && order.OrederType == "تشافي"
                                && order.status == "Success")
                .SelectMany(order => order.Batches.Select(batch => new
                {
                    OrderId = order.OrderCode,
                    Batch = batch.BatchCode,
                    NumberOfCowOrPieces = batch.numberOfCowOrPieces ?? 0,
                    StartDate = order.EndDate.HasValue ? order.EndDate.Value.ToString("dd/MM/yyyy") : "N/A",
                    EndDate = order.Date.HasValue ? order.Date.Value.ToString("dd/MM/yyyy") : "N/A",
                    OrderType = order.OrederType,
                    Customer = order.Clients != null ?
                  order.Clients.Name : "N/A",
                    TotalWeight = Math.Round(batch.CowsPieces.Sum(cp => cp.pieceWeight_Out ?? 0), 2),
                }))
                .ToListAsync();
            double uptimePercentage = ((double)uptimeCounterDevice3 / 480) * 100;
            string upTimeFormatted = uptimePercentage > 0 ? $"{(100 - uptimePercentage):F2}%" : "100.00%";
            int standardRate = 100;
            double elapsedHours = (DateTime.Now - targetDate).TotalHours;
            elapsedHours = elapsedHours > 0 ? elapsedHours : 1;
            double performance = (soldPiecesCount / (standardRate * elapsedHours)) * 100;
            var response = new
            {
                PiecesRequest = piecesRequest,
                SoldPieces = soldPiecesCount,
                Reminders = reminders,
                WeightOfSoldPieces = Math.Round(totalWeightOfSoldPieces, 2),
                Performance = $"{performance:F2}",
                UpTime = upTimeFormatted,
                Graph = graph,
                CuttingPieces = cuttingPieces,
                Table = res
            };

            return new OkObjectResult(response);
        }

        public async Task<string> GetAuthToken()
        {
            var url = "https://login.microsoftonline.com/be88f713-a964-488f-89ef-00a04bc0f789/oauth2/v2.0/token";

            var requestData = new List<KeyValuePair<string, string>>
        {
            new("client_id", "af9c6191-37aa-4bb4-a623-5e7f2c364c17"),
            new("client_secret", "GTq8Q~zIvI3XbcewOdV-4OEAgZYCFQiC4EOwYdbK"),
            new("grant_type", "client_credentials"),
            new("scope", "https://shatat-uat.sandbox.operations.dynamics.com/.default")
        };

            var requestContent = new FormUrlEncodedContent(requestData);
            var response = await _httpClient.PostAsync(url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                using var jsonDoc = JsonDocument.Parse(responseContent);
                if (jsonDoc.RootElement.TryGetProperty("access_token", out var tokenElement))
                {
                    var token = tokenElement.GetString();
                    if (token != null)
                    {
                        return token;
                    }
                    throw new Exception("Access token is null.");
                }

                throw new Exception("Access token not found in the response.");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve token. Status code: {response.StatusCode}. Error: {errorContent}");
        }

        //public async Task<string> GetWorkOrderDataAsync(string companyId, string transDate)
        //{
        //    var url = "https://shatat-uat.sandbox.operations.dynamics.com/api/Services/Sha_IntegrationServiceGroup/Sha_IntegrationService/GetWorkOrderData";

        //    var token = await GetAuthToken();

        //    var requestData = new
        //    {
        //        CompanyId = companyId,
        //        TransDate = transDate
        //    };

        //    var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

        //    var request = new HttpRequestMessage(HttpMethod.Post, url)
        //    {
        //        Content = requestContent
        //    };
        //    request.Headers.Add("Authorization", $"Bearer {token}");

        //    var response = await _httpClient.SendAsync(request);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        return responseContent; 
        //    }

        //    var errorContent = await response.Content.ReadAsStringAsync();
        //    throw new Exception($"Failed to retrieve WorkOrderData. Status code: {response.StatusCode}. Error: {errorContent}");
        //}

        public async Task<WorkOrderResponse> GetWorkOrderDataAsync2(string companyId, string transDate, string token)
        {

            //var token = await GetAuthToken();
            //if (string.IsNullOrWhiteSpace(token))
            //    throw new Exception("Failed to retrieve authentication token.");

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://shatat-uat.sandbox.operations.dynamics.com/api/Services/Sha_IntegrationServiceGroup/Sha_IntegrationService/GetWorkOrderData");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var content = new StringContent($"{{\"CompanyId\": \"{companyId}\", \"TransDate\": \"{transDate}\"}}", Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WorkOrderResponse>(responseBody);
        }

        public async Task<ActionResult> ProcessWorkOrderAndCreateOrder(string companyId, string transDate)
        {
            var token = await GetAuthToken();
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Failed to retrieve authentication token.");

            var workOrderData = await GetWorkOrderDataAsync2(companyId, transDate, token);
            if (workOrderData == null || !workOrderData.Status)
                return new BadRequestObjectResult(new ApiResponse(400, workOrderData?.ErrorMessage ?? "Failed to fetch work order data."));

            // Step 2: Extract Data for CreateOrder
            foreach (var workOrder in workOrderData.Sha_headerResponse)
            {
                var client = await _context.clients.FirstOrDefaultAsync(c => c.Code == workOrder.CustmerAccount);
                if (client == null)
                {
                    client = new Clients
                    {
                        Name = workOrder.CustmerName,
                        Code = workOrder.CustmerAccount
                    };
                    _context.clients.Add(client);
                    await _context.SaveChangesAsync();
                }
                var cowType = await _context.TypeofCows.FirstOrDefaultAsync(t => t.TypeName == workOrder.linesResponse.First().ItemName);
                if (cowType == null)
                {
                    cowType = new TypeofCows
                    {
                        TypeName = workOrder.linesResponse.First().ItemName
                    };
                    _context.TypeofCows.Add(cowType);
                    await _context.SaveChangesAsync();
                }

                var dto = new CreateOrderDto
                {
                    Code = workOrder.WorkOrder,
                    OrderType = "ذبح",
                    NoOfCows = workOrder.linesResponse.Count,
                    NoOfBatches = 1,
                    ClientName = workOrder.CustmerName,
                    date = DateTime.Parse(workOrder.TransDate)
                };

                // Step 3: Call CreateOrder
                var createOrderResult = await CreateOrder(dto) as OkObjectResult;
                if (createOrderResult == null || createOrderResult.Value == null)
                    return new BadRequestObjectResult(new ApiResponse(400, "Failed to create order."));

                var createdOrder = (dynamic)createOrderResult.Value;
                var batchCode = createdOrder.Batches[0].BatchCode;
                foreach (var line in workOrder.linesResponse)
                {
                    var cow = new CowsSeed
                    {
                        CowsId = line.SerialId.Substring(0, 5),
                        weight = line.Weight,
                        TypeofCowsId = cowType.Id,
                        clientId = client.Id,
                        suppliersId = 1,
                    };

                    _context.cowsSeeds.Add(cow);
                }
                await _context.SaveChangesAsync();
                // Step 4: Assign BatchCode to Cows
                var assignDto = new AssignBatchesDto
                {
                    OrderCode = workOrder.WorkOrder,
                    Batches = new List<BatchUpdateDto>
                     { 
                         new BatchUpdateDto
                         {
                             BatchCode = batchCode,
                             cowType = workOrder.linesResponse.First().ItemName,
                             Quntity = workOrder.linesResponse.Count,
                             Cows = workOrder.linesResponse.Select(l => new CowDto
                             {
                                 CowId = l.SerialId.Substring(0, 5)
                             }).ToList()
                         }
                     }
                };

                await AssignBatchesToCows(assignDto);
            }
            return new OkObjectResult(new { StatusCode = 200, Message = "OK" });
        }

        public async Task<ActionResult> UpdateOrderApproval(OrderApprovalDto dto)
        {
            if (string.IsNullOrEmpty(dto.OrderCode) || string.IsNullOrEmpty(dto.ApproveStatus))
                return new BadRequestObjectResult(new ApiResponse(400, "No Data"));

            var order = await _context.Orders
                .Include(o => o.Batches)
                .ThenInclude(b => b.CowsPieces)
                .ThenInclude(c => c.Cow)
                .FirstOrDefaultAsync(o => o.OrderCode == dto.OrderCode);

            if (order == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Order not Found"));

            if (dto.ApproveStatus.ToLower() != "approve" && dto.ApproveStatus.ToLower() != "rejected")
                return new BadRequestObjectResult(new ApiResponse(400, "approve أو rejected"));

            order.Approve = dto.ApproveStatus.ToLower();

            if (dto.ApproveStatus.ToLower() == "rejected")
            {
                var cowsIds = order.Batches
                    .SelectMany(b => b.CowsPieces)
                    .Select(cp => cp.CowId)
                    .ToList();

                var piecesToUpdate = await _context.CowsPieces
                    .Where(p => cowsIds.Contains(p.CowId))
                    .Include(p => p.Cow)  
                    .ToListAsync();

                foreach (var piece in piecesToUpdate)
                {
                    if (piece.Cow != null) 
                    {
                        piece.BatchId = piece.Cow.BatchId;
                    }
                }

                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
            return new OkObjectResult(new ApiResponse(200, "Success"));
        }

        public async Task FetchAndStoreClientsAsync()
        {
            var requestUrl = "https://shatat-uat.sandbox.operations.dynamics.com/data/CustomersV2?$filter=dataAreaId eq '003'&cross-company=true&$select=OrganizationName,CustomerAccount";

            var token = await GetAuthToken();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Request failed: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var jsonObject = JObject.Parse(jsonResponse);
            var customers = jsonObject["value"]?.ToObject<List<CustomerDto>>();

            if (customers == null || !customers.Any()) return;

            foreach (var customer in customers)
            {
                if (!_context.clients.Any(c => c.Code == customer.CustomerAccount))
                {
                    var client = new Clients
                    {
                        Name = customer.OrganizationName,
                        Code = customer.CustomerAccount,
                        NameENG = null
                    };

                    _context.clients.Add(client);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task FetchAndStoreSuppliersAsync()
        {
            var apiUrl = "https://shatat-uat.sandbox.operations.dynamics.com/data/Vendors?$filter=dataAreaId eq '003' &cross-company=true&$select=VendorAccountNumber,VendorName";

            var token = await GetAuthToken();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch suppliers");
            }
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var jsonObject = JObject.Parse(jsonResponse);
            var suppliers = jsonObject["value"]?.ToObject<List<SupplierDto>>();
            if (suppliers == null || !suppliers.Any()) return;
            foreach (var supplier in suppliers)
            {
                if (!_context.suppliers.Any(c => c.Code == supplier.VendorAccountNumber))
                {
                    var suplier = new Suppliers
                    {
                        SupplierName = supplier.VendorName,
                        Code = supplier.VendorAccountNumber,
                        NameENG = null
                    };

                    _context.suppliers.Add(suplier);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ActionResult> DeletePiecesFromOrder(DeletePieceDto dto)
        {
            if (string.IsNullOrEmpty(dto.OrderCode) || string.IsNullOrEmpty(dto.BatchCode) || dto.PieceIds == null || !dto.PieceIds.Any())
                return new BadRequestObjectResult(new ApiResponse(400, "Invalid data"));

            var order = await _context.Orders
                .Include(o => o.Batches)
                    .ThenInclude(b => b.CowsPieces)
                        .ThenInclude(cp => cp.Cow)
                .FirstOrDefaultAsync(o => o.OrderCode == dto.OrderCode);

            if (order == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Order not found"));

            var batch = order.Batches.FirstOrDefault(b => b.BatchCode == dto.BatchCode);
            if (batch == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Batch not found in the given order"));

            var pieces = batch.CowsPieces.Where(p => dto.PieceIds.Contains(p.pieceId)).ToList();

            if (!pieces.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No pieces found in the batch"));

            var nonReversiblePieces = new List<string>();
            var convertedPieces = new List<string>();

            foreach (var piece in pieces)
            {
                if (piece.pieceWeight_Out != null)
                {
                    nonReversiblePieces.Add(piece.pieceId);
                }
                else
                {
                    if (piece.Cow != null)
                    {
                        piece.BatchId = piece.Cow.BatchId;
                    }
                    convertedPieces.Add(piece.pieceId);
                }
            }

            if (!convertedPieces.Any())
            {
                return new BadRequestObjectResult(new
                {
                    Status = 400,
                    Message = "Operation cannot be reversed as all selected pieces already output .",
                    NonReversiblePieces = nonReversiblePieces
                });
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                Status = 200,
                Message = $"{convertedPieces.Count} pieces removed and returned to original batches.",
                ConvertedPieces = convertedPieces,
                NonReversiblePieces = nonReversiblePieces.Any()
                    ? new { Count = nonReversiblePieces.Count, Pieces = nonReversiblePieces }
                    : null
            });
        }

        public async Task<ActionResult<List<CowWithPiecesDto>>> GetCowsWithPiecesByDate(DateTime date)
        {
            var cows = await _context.Cows
                        .Where(c => c.Create_At_Divece1.Date == date.Date)
                        .Include(c => c.CowMiscarriages)
                        .Include(c => c.Batch)
                        .ThenInclude(c=>c.Order)
                        .Include(c => c.CowsSeed)
                        .Include(c => c.TypeofCows)
                        .ToListAsync();

            if (cows == null || cows.Count == 0)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "No cows found for the given date"));
            }

            var cowsWithPieces = cows.Select(cow => new CowWithPiecesDto
            {
                CowsId = cow.CowsId,
                Cow_Weight = cow.Cow_Weight,
                batch = cow.Batch.BatchCode,
                order = cow.Batch.Order.OrderCode,
                CowType = cow.TypeofCows.TypeName,
                Doctor = cow.Doctor_Id,
                Tech = cow.techOfDevice1,
                Create_At_Divece1 = cow.Create_At_Divece1,
                Pieces = _context.CowsPieces
            .Where(p => p.CowId == cow.Id)
            .Select(p => new CowPieceDto
            {
                PieceId = p.pieceId,
                PieceWeight_In = p.pieceWeight_In,
                PieceWeight_Out = p.pieceWeight_Out,
                PieceType = p.PieceTybe,
                Status = p.Status ?? p.Status_From_Device_2
            }).ToList()
            }).ToList();

            return new OkObjectResult(cowsWithPieces);
        }
        public async Task<ActionResult<List<CowPieceDto>>> GetPiecesByCowId(string cowId)
        {
            var cow = await _context.Cows
               .FirstOrDefaultAsync(c => c.CowsId == cowId);
            var pieces = await _context.CowsPieces
             .Where(p => p.CowId == cow.Id) 
             .Select(p => new CowPieceDto
             {
                 PieceId = p.pieceId,
                 PieceWeight_In = p.pieceWeight_In,
                 PieceWeight_Out = p.pieceWeight_Out,
                 PieceType = p.PieceTybe,
                 Status = p.Status
             })
             .ToListAsync();

                 if (pieces == null || pieces.Count == 0)
                 {
                     return new NotFoundObjectResult(new ApiResponse(404, "No pieces found for the given cowId"));
                 }

                 return new OkObjectResult(pieces);
        }

        public async Task<int> DeleteOldCowsPieces()
        {
            var today = DateTime.Now.Date;

            var oldPieces = await _context.CowsPieces
                .Where(p => p.Create_At_Divece2.Date != today)
                .ToListAsync();

            if (!oldPieces.Any())
                return 0;

            _context.CowsPieces.RemoveRange(oldPieces);
            await _context.SaveChangesAsync();

            return oldPieces.Count;
        }

        public async Task<int> DeleteOldCows()
        {
            var today = DateTime.Now.Date;

            var oldCows = await _context.Cows
                .Where(c => c.Create_At_Divece1.Date != today)
                .ToListAsync();

            if (!oldCows.Any())
                return 0; 

            _context.Cows.RemoveRange(oldCows);
            await _context.SaveChangesAsync();

            return oldCows.Count;
        }
    }
}
