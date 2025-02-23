using ElMagzer.Core.Models;
using ElMagzer.Core.Services;
using ElMagzer.Repository.Data;
using ElMagzer.Shared.Dtos;
using ElMagzer.Shared.Errors;
using ElMagzer.Shared.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ElMagzer.Service
{
    public class FlutterService : IFlutterServices
    {
        private readonly ElMagzerContext _context;
        private readonly IHubContext<CowHub> _hubContext;

        public FlutterService(ElMagzerContext context, IHubContext<CowHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ActionResult> Executions([FromBody] NumbersDto request)
        {
            if (request.Numbers == null || !request.Numbers.Any())
            {
                return new BadRequestObjectResult(new ApiResponse(400, "The numbers list is empty."));
            }

            var piecesFromCowPieces = await _context.CowsPieces
                .Where(p => request.Numbers.Contains(p.pieceId) && !p.isExecutions)
                .ToListAsync();

            var piecesFromCowPieces2 = await _context.Cow_Pieces_2
                .Where(p => request.Numbers.Contains(p.PieceNumber) && !p.isExecutions)
                .ToListAsync();

            if (!piecesFromCowPieces.Any() && !piecesFromCowPieces2.Any())
            {
                return new NotFoundObjectResult(new ApiResponse(404, "No matching pieces found."));
            }

            foreach (var piece in piecesFromCowPieces)
            {
                piece.isExecutions = true;
            }

            foreach (var piece in piecesFromCowPieces2)
            {
                piece.isExecutions = true;
            }
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { statusCode = 200, Message = "Pieces updated" });
        }

        public async Task<ActionResult> GetPiece(string PieceId)
        {
            var piece = await _context.CowsPieces
                .Where(p => p.pieceId == PieceId)
                .Select(p => new
                {
                    pieceId = PieceId,
                    WorkerId = p.techOfDevice2 ?? "N/A",
                    NumberOfCow = p.Cow.CowsId ?? "Unknown",
                    WeightOfPiece = $"{p.pieceWeight_In} Kg",
                    Client = p.Batch != null && p.Batch.Order != null && p.Batch.Order.Clients != null
                                    ? p.Batch.Order.Clients.Name
                                    : "Unknown",
                    Supplier = "El_Rawda",
                    ProductionDate = p.Create_At_Divece2.ToString("dd/MM/yyyy"),
                    ExpireDate = p.dateOfExpiere.ToString("dd/MM/yyyy"),
                    TypeOfPiece = p.PieceTybe ?? "Beef",
                    Pitch = p.Batch != null ? p.Batch.BatchCode : "Unknown",
                    JobOrder = p.Batch != null && p.Batch.Order != null
                                 ? p.Batch.Order.OrderCode
                                 : null,
                    Store = p.StoreId
                })
                .FirstOrDefaultAsync();


            if (piece == null)
            {
                piece = await _context.Cow_Pieces_2
                    .Include(cp => cp.Cutting)
                    .Where(p => p.PieceNumber == PieceId)
                    .Select(p => new
                    {
                        pieceId = PieceId,
                        WorkerId = p.techofDevice4 ?? "N/A",
                        NumberOfCow = "N/A",
                        WeightOfPiece = $"{p.Weight} Kg",
                        Client = p.Batch != null && p.Batch.Order != null && p.Batch.Order.Clients != null
                                    ? p.Batch.Order.Clients.Name
                                    : "Unknown",
                        Supplier = "El_Rawda",
                        ProductionDate = p.Create_At_Divece4.ToString("dd/MM/yyyy"),
                        ExpireDate = p.dateOfExpiere.ToString("dd/MM/yyyy"),
                        TypeOfPiece = p.Cutting.CutName ?? "Beef",
                        Pitch = p.Batch != null ? p.Batch.BatchCode : "Unknown",
                        JobOrder = p.Batch != null && p.Batch.Order != null
                                 ? p.Batch.Order.OrderCode
                                 : null,
                        Store = p.StoreId
                    })
                    .FirstOrDefaultAsync();
            }
            if (piece == null) {
                 var miscarriage = await _context.cowMiscarriages
                     .Include(cp => cp.MiscarriageType)
                     .Include(cp => cp.Cow)
                           .ThenInclude(cp => cp.Batch)
                           .ThenInclude(cp => cp.Order)
                     .Where(p => p.BarCode == PieceId)
                     .FirstOrDefaultAsync();

                if (miscarriage != null)
                {
                    piece = new
                    {
                        pieceId = PieceId,
                        WorkerId = miscarriage.Cow.techfDevice5 ?? "N/A",
                        NumberOfCow = miscarriage.Cow?.CowsId ?? "Unknown",
                        WeightOfPiece = $"{miscarriage.Weight} Kg",
                        Client = "Ziad",
                        Supplier = "El_Rawda",
                        ProductionDate = miscarriage.Cow?.Create_At_Divece5.HasValue == true
                              ? miscarriage.Cow.Create_At_Divece5.Value.ToString("dd/MM/yyyy")
                              : "N/A",
                        ExpireDate = miscarriage.Cow?.Create_At_Divece5 != null
                                 ? miscarriage.Cow.Create_At_Divece5.Value.ToString("dd/MM/yyyy")
                                 : "N/A",
                        TypeOfPiece = miscarriage.MiscarriageType?.Name ?? "Beef",
                        Pitch = miscarriage.Cow?.Batch?.BatchCode ?? "Unknown",
                        JobOrder = miscarriage.Cow?.Batch?.Order?.OrderCode,
                        Store = 1
                    };
                }
            }

            if (piece == null)
            {
                return new NotFoundObjectResult(new ApiResponse(404, $"Piece with ID {PieceId} not found."));
            }

            return new OkObjectResult(piece);
        }


        //public async Task<ActionResult> Inventory(DateTime? date,string store) // تعديل 
        //{
        //    var inventory = await _context.CowsPieces
        //             .Where(p => !date.HasValue || p.Create_At_Divece2.Date == date.Value.Date)
        //             .GroupBy(p => p.PieceTybe)
        //             .Select(group => new
        //             {
        //                 PieceType = group.Key ?? "Unknown",
        //                 Quantity = group.Count(),
        //                 TotalWeight = group.Sum(p => p.pieceWeight_In) 
        //             })
        //             .ToListAsync();

        //    if (!inventory.Any())
        //        return new NotFoundObjectResult(new ApiResponse(404, "No inventory data found for the specified date."));

        //    return new OkObjectResult(inventory);

        //}
        //public async Task<ActionResult> Log(string name, string password) // لو في وقت هعمل identity
        //{
           
        //    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
        //    {
        //        return new BadRequestObjectResult(new ApiResponse(400, "Name and password must not be empty"));
        //    }

            
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == name && u.Password == password);

        //    if (user == null)
        //    {
        //        return new NotFoundObjectResult(new ApiResponse(404, "Invalid username or password"));
        //    }

        //    return new OkObjectResult("تم تسجيل الدخول بنجاح");
        //}
        public async Task<ActionResult> GetStoresList()
        {
            var stores = await _context.Stores.Select(store => new
            {
                StoreId = store.Id,
                StoreName = store.storeName
            }).ToListAsync();

            return new OkObjectResult(stores);
        }
        public async Task<ActionResult> GetStoreDetails(int storeId)
        {
            var store = await _context.Stores
                .Include(s => s.CowsPieces)
                .Include(s => s.CowPieces2)
                .ThenInclude(cp => cp.Cutting)
                .FirstOrDefaultAsync(s => s.Id == storeId);

            if (store == null)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "Store not found"));
            }

            var pieces = store.CowsPieces
                .Select(p => new
                {
                    PieceType = p.PieceTybe,
                    PieceId = p.pieceId,
                    WeightIn = p.pieceWeight_In,
                    WeightOut = p.pieceWeight_Out,
                    CreatedAt = p.Create_At_Divece2.ToString("dd/MM/yyyy"),
                    DateOfExpire = p.dateOfExpiere.ToString("dd/MM/yyyy")
                })
                .Concat(store.CowPieces2
                .Select(p => new
                {
                    PieceType = (string?)p.Cutting.CutName,
                    PieceId = p.PieceNumber,
                    WeightIn = p.Weight,
                    WeightOut = (double?)null,
                    CreatedAt = p.Create_At_Divece4.ToString("dd/MM/yyyy"),
                    DateOfExpire = p.dateOfExpiere.ToString("dd/MM/yyyy")
                }))
                .GroupBy(p => p.PieceType)
                .Select(g => new
                {
                    PieceType = g.Key,
                    Count = g.Count(),
                    Details = g.ToList()
                })
                .ToList();

            var response = new
            {
                StoreName = store.storeName,
                TotalPieces = store.quantity,
                Pieces = pieces
            };

            return new OkObjectResult(response);
        }

        public async Task<ActionResult> GetPieceTypesInStore(int storeId)
        {
            var store = await _context.Stores
         .Include(s => s.CowsPieces)
         .Include(s => s.CowPieces2)
         .ThenInclude(cp => cp.Cutting)
         .FirstOrDefaultAsync(s => s.Id == storeId);

            if (store == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Store not found"));


            var cowPiecesTypes = store.CowsPieces
                .Where(cp => !string.IsNullOrEmpty(cp.PieceTybe))
                .Select(cp => cp.PieceTybe);


            var cowPieces2Types = store.CowPieces2
                .Where(cp2 => !string.IsNullOrEmpty(cp2.Cutting.CutName))
                .Select(cp2 => cp2.Cutting.CutName);


            var combinedTypes = cowPiecesTypes
                .Union(cowPieces2Types)
                .Distinct()
                .ToList();


            var response = new
            {
                types = combinedTypes
            };

            return new OkObjectResult(response);
        }
        public async Task<ActionResult> ValidatePiecesInStore(int storeId, string pieceType, List<string> pieceNumbers)
        {
            var store = await _context.Stores
                .Include(s => s.CowsPieces)
                .Include(s => s.CowPieces2)
                .ThenInclude(cp => cp.Cutting)
                .FirstOrDefaultAsync(s => s.Id == storeId);

            if (store == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Store not found"));

            var registeredPieces = store.CowsPieces
                .Where(cp => cp.PieceTybe == pieceType)
                .Select(cp => cp.pieceId)
                .Union(
                    store.CowPieces2
                    .Where(cp2 => cp2.Cutting.CutName == pieceType)
                    .Select(cp2 => cp2.PieceNumber)
                )
                .ToHashSet();

            var missingPieces = registeredPieces.Except(pieceNumbers).ToList(); 
            var extraPieces = pieceNumbers.Except(registeredPieces).ToList(); 


            if (missingPieces.Any() && extraPieces.Any())
            {
                return new OkObjectResult(new
                {
                    Status = "عدم التطابق",
                    Message = "هناك قطع مفقودة وإضافية.",
                    MissingPieces = missingPieces,
                    ExtraPieces = extraPieces
                });
            }
            else if (missingPieces.Any())
            {
                return new OkObjectResult(new
                {
                    Status = "مفقود",
                    Message = "هناك قطع مفقودة.",
                    MissingPieces = missingPieces
                });
            }
            else if (extraPieces.Any())
            {
                return new OkObjectResult(new
                {
                    Status = "زياده",
                    Message = "هناك قطع إضافية.",
                    ExtraPieces = extraPieces
                });
            }
            else
            {
                return new OkObjectResult(new
                {
                    Status = "مطابق",
                    Message = "تم الجرد بنجاح."
                });
            }
        }

        public async Task<ActionResult> TransferPiecesAsync(int sourceStoreId, int destinationStoreId, List<string> piecesList)
        {
            if (sourceStoreId == destinationStoreId)
                return new BadRequestObjectResult(new ApiResponse(400, "Source and destination stores must be different."));
            

            if (piecesList == null || !piecesList.Any())
                return new BadRequestObjectResult(new ApiResponse(400, "The pieces list is empty."));

            var piecesFromCowsPieces = await _context.CowsPieces
                     .Where(p => piecesList.Contains(p.pieceId) && p.StoreId == sourceStoreId)
                     .ToListAsync();

            var piecesFromCowPieces2 = await _context.Cow_Pieces_2
                .Include(cp=>cp.Cutting)
                .Where(p => piecesList.Contains(p.PieceNumber) && p.StoreId == sourceStoreId)
                .ToListAsync();
            var foundPieces = piecesFromCowsPieces.Select(p => p.pieceId)
                     .Concat(piecesFromCowPieces2.Select(p => p.PieceNumber))
                     .ToList();

            var missingPieces = piecesList.Except(foundPieces).ToList();

            if (missingPieces.Any())
            {
                var errorResponse = new
                {
                    StatusCode = 400,
                    Message = "Some pieces were not found in the source store.",
                    MissingPieces = missingPieces
                };

                return new BadRequestObjectResult(errorResponse);
            }
            if (piecesFromCowsPieces == null || piecesFromCowPieces2 == null)
                return new NotFoundObjectResult (new ApiResponse(404));

            foreach (var piece in piecesFromCowsPieces)
                piece.StoreId = destinationStoreId;
            

            foreach (var piece in piecesFromCowPieces2)
                piece.StoreId = destinationStoreId;

            var sourceStore = await _context.Stores.FirstOrDefaultAsync(s => s.Id == sourceStoreId);
            var destinationStore = await _context.Stores.FirstOrDefaultAsync(s => s.Id == destinationStoreId);

            if (sourceStore == null || destinationStore == null)
                return new NotFoundObjectResult(new ApiResponse(404, "One or both stores are not found."));


            sourceStore.quantity = (sourceStore.quantity ?? 0) - piecesList.Count;
            destinationStore.quantity = (destinationStore.quantity ?? 0) + piecesList.Count;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { statusCode = 200, Message = "OK" });
        } //تعديل

        public async Task<ActionResult> GetSalesOrders()
        {
            var salesOrders = await _context.Orders
                            .AsNoTracking()
                            .Where(o => o.OrederType == "بيع لحم بعضم" || o.OrederType == "بيع لحم مشفي" && o.status != "Success")
                            .Select(o => new
                            {
                                o.OrderCode,
                                ClientName = o.Clients != null ? o.Clients.Name : null,
                                Pieces = o.Batches
                                    .SelectMany(b => b.CowsPieces.Select(p => new
                                    {
                                        PieceId = p.pieceId,
                                        OrderType = "بيع لحم بعضم",
                                        StoreNumber=p.StoreId
                                    }))
                                    .Union(
                                        o.Batches.SelectMany(b => b.CowPieces2.Select(p2 => new
                                        {
                                            PieceId = p2.PieceNumber,
                                            OrderType = "بيع لحم مشفي",
                                            StoreNumber = p2.StoreId
                                        }))
                                    )
                                    .ToList()
                            })
                            .ToListAsync();

            if (!salesOrders.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No sales orders found."));

            return new OkObjectResult(salesOrders);
        }

        public async Task<ActionResult> GetRecoveryOrders()
        {
            var recoveryOrders = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.OrederType == "تشافي" && o.status != "Success")
                    .Select(o => new
                    {
                        o.OrderCode,
                        ClientName = o.Clients != null ? o.Clients.Name : null,
                        Total = o.Batches
                                 .SelectMany(b => b.CowsPieces)
                                 .Count(),
                        Pieces = o.Batches
                            .SelectMany(b => b.CowsPieces)
                            .GroupBy(p => p.PieceTybe)
                            .Select(g => new
                            {
                                PieceType = g.Key,
                                Items = g.Select(p => new
                                {
                                    PieceId = p.pieceId,
                                    StoreNumber = p.StoreId,
                                    Status = p.Status
                                }).ToList()
                            }).ToList()
                    })
                    .ToListAsync();

            if (!recoveryOrders.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No recovery orders found."));

            return new OkObjectResult(recoveryOrders);
        }
        public async Task<ActionResult> GetslaughteredOreder()
        {
            var slaughteredOrders = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.OrederType == "ذبح" && o.status != "Success")
                    .Select(o => new
                    {
                        o.OrderCode,
                        Total = o.Batches
                                 .SelectMany(b => _context.cowsSeeds.Where(cs => cs.BatchId == b.Id))
                                 .Count(),
                        Pieces = o.Batches
                            .SelectMany(b => _context.cowsSeeds.Where(cs => cs.BatchId == b.Id))
                            .GroupBy(cs => cs.TypeofCows.TypeName)
                            .Select(g => new
                            {
                                PieceType = g.Key,
                                Items = g.Select(cs => new
                                {
                                    PieceId = cs.CowsId,
                                    Store = "مخزن العجول"
                                }).ToList()
                            }).ToList()
                    })
                    .ToListAsync();

            if (!slaughteredOrders.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No slaughtered orders found."));

            return new OkObjectResult(slaughteredOrders);
        }
        public async Task<ActionResult> ValidatePiecesToOrders(string orderId, List<string> pieceIds)
        {
            if (string.IsNullOrWhiteSpace(orderId) || pieceIds == null || !pieceIds.Any())
                return new BadRequestObjectResult(new ApiResponse(400, "Invalid input data."));

            var order = await _context.Orders
                 .Include(o => o.Batches)
                 .ThenInclude(b => b.CowsPieces)
                 .Include(o => o.Batches)
                 .ThenInclude(b => b.CowPieces2)
                 .ThenInclude(cp => cp.Cutting)
                 .FirstOrDefaultAsync(o => o.OrderCode == orderId);

            if (order == null)
                return new NotFoundObjectResult(new ApiResponse(404, "Order not found."));

            var orderPieces = order.Batches
                 .SelectMany(b => b.CowsPieces.Select(cp => cp.pieceId)
                 .Concat(b.CowPieces2.Select(cp2 => cp2.PieceNumber)))
                 .ToHashSet();

            var invalidPieces = pieceIds.Where(id => !orderPieces.Contains(id)).ToList();

            if (invalidPieces.Any())
                return new BadRequestObjectResult(new ApiResponse(400, $"Invalid pieces: {string.Join(", ", invalidPieces)}"));

            foreach (var batch in order.Batches)
            {
                foreach (var piece in batch.CowsPieces.Where(p => pieceIds.Contains(p.pieceId)))
                {
                    piece.Status = "Out";
                }

                foreach (var piece2 in batch.CowPieces2.Where(p2 => pieceIds.Contains(p2.PieceNumber)))
                {
                    piece2.status = "Out";
                }
            }
            if (order.Date != null)
                order.status = "Running";
            else
            {
                var totalPieces = orderPieces.Count;
                var processedPieces = pieceIds.Count;
                order.status = totalPieces == processedPieces ? "Success" : "Running";
            }
            await _context.SaveChangesAsync();
            return new OkObjectResult(new ApiResponse(200, "Order and pieces updated successfully."));
        }

        public async Task<ActionResult> GetOrdersWithPieces()
        {
            var orderTypes = new[] { "ذبح", "تشافي", "بيع لحم بعضم", "بيع لحم مشفي" };

            var orders = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Batches)
                .Include(o => o.Batches)
                    .ThenInclude(b => b.CowsPieces)
                .Include(o => o.Batches)
                    .ThenInclude(b => b.CowPieces2)
                .ToListAsync();

            var cowsSeedData = await _context.cowsSeeds
            .AsNoTracking()
            .ToListAsync();
            var ordersWithPieces = orders
                .Where(o => orderTypes.Contains(o.OrederType) || o.OrederType == null)
                .Select(o => new
                {
                    OrederType = o.OrederType,
                    Pieces = o.OrederType switch
                    {
                        "ذبح" => o.Batches
                             .SelectMany(b => cowsSeedData
                                         .Where(cs => cs.BatchId == b.Id)
                                         .Select(cs => cs.CowsId))
                                         .ToList(),
                        "بيع لحم بعضم" or "تشافي" => o.Batches
                            .SelectMany(b => b.CowsPieces.Select(p => p.pieceId)).ToList(),
                        "بيع لحم مشفي" => o.Batches
                            .SelectMany(b => b.CowPieces2.Select(p2 => p2.PieceNumber)).ToList(),
                        _ => new List<string>()
                    }
                })
                .GroupBy(o => o.OrederType)
                .Select(g => new
                {
                    OrderType = g.Key,
                    Pieces = g.SelectMany(o => o.Pieces).ToList()
                })
                .ToList();

            var result = orderTypes.Select(orderType => new
            {
                OrderType = orderType,
                Pieces = ordersWithPieces
                    .FirstOrDefault(o => o.OrderType == orderType)?.Pieces
            }).ToList();

            return new OkObjectResult(result);
        }

        public async Task<ActionResult> GetDetails()
        {
            var clients = await _context.clients
       .Select(client => new
       {
           Id = client.Id,
           Name = client.Name
       })
       .ToListAsync();

            var suppliers = await _context.suppliers
                .Select(supplier => new
                {
                    Id = supplier.Id,
                    Name = supplier.SupplierName
                })
                .ToListAsync();

            var cowTypes = await _context.TypeofCows
                .Select(cowType => new
                {
                    Id = cowType.Id,
                    Name = cowType.TypeName
                })
                .ToListAsync();

            var result = new
            {
                Clients = clients,
                Suppliers = suppliers,
                CowTypes = cowTypes
            };

            return new OkObjectResult(result);
        }

        public async Task<ActionResult> AddCowSeeds(AddCowSeedRequestDto request)
        {
            if (request?.Cows == null || !request.Cows.Any())
            {
                return new BadRequestObjectResult(new ApiResponse(400, "No cows provided"));
            }

            var dtos = request.Cows; 

            var typeOfCow = await _context.TypeofCows.FirstOrDefaultAsync(t => t.Id == dtos.First().TypeofCowsId);
            if (typeOfCow == null)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "TypeofCowsId not found"));
            }

            var clientExists = await _context.clients.AnyAsync(c => c.Id == dtos.First().ClientId);
            if (!clientExists)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "ClientId not found"));
            }

            var supplierExists = await _context.suppliers.AnyAsync(s => s.Id == dtos.First().SuppliersId);
            if (!supplierExists)
            {
                return new NotFoundObjectResult(new ApiResponse(404, "SuppliersId not found"));
            }

            var existingCowIds = await _context.cowsSeeds
                                              .Where(c => dtos.Select(d => d.numberOfCow).Contains(c.CowsId))
                                              .Select(c => c.CowsId)
                                              .ToListAsync();

            var newCows = dtos.Where(d => !existingCowIds.Contains(d.numberOfCow)).ToList();

            if (!newCows.Any())
            {
                return new ConflictObjectResult(new
                {
                    Message = "All cows already exist.",
                    ExistingCows = existingCowIds
                });
            }

            var random = new Random();
            var orderCode = "ORD-" + random.Next(100000, 999999).ToString();

            var order = new Orders
            {
                OrderCode = orderCode,
                OrederType = "ذبح",
                numberofCows = newCows.Count,
                numberofbatches = 1,
                ClientsId = newCows.First().ClientId,
                EndDate = DateTime.UtcNow.AddDays(2),
                status = "Pending"
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var batchCode = random.Next(100000, 999999).ToString();

            var batch = new Batch
            {
                BatchCode = batchCode,
                BatchType = "ذبح",
                OrderId = order.Id,
                numberOfCowOrPieces = newCows.Count,
                CowOrPiecesType = typeOfCow.TypeName,
            };

            await _context.Batches.AddAsync(batch);
            await _context.SaveChangesAsync();

            var cowsSeeds = new List<CowsSeed>();

            foreach (var dto in newCows)
            {
                var cowSeed = new CowsSeed
                {
                    CowsId = dto.numberOfCow,
                    weight = dto.Weight,
                    TypeofCowsId = dto.TypeofCowsId,
                    clientId = dto.ClientId,
                    suppliersId = dto.SuppliersId,
                    BatchId = batch.Id
                };

                cowsSeeds.Add(cowSeed);
            }

            await _context.cowsSeeds.AddRangeAsync(cowsSeeds);
            await _context.SaveChangesAsync();


            foreach (var cow in cowsSeeds)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveCowId", cow.CowsId);
            }

            return new OkObjectResult(new
            {
                Message = "Cow seeds added successfully.",
                OrderCode = orderCode,
                BatchCode = batchCode,
                TotalNewCows = newCows.Count,
                ExistingCows = existingCowIds
            });
        }



        public async Task<ActionResult> GetBoneInSalesOrders()
        {
            var boneInSalesOrders = await _context.Orders
                 .AsNoTracking()
                 .Where(o => o.OrederType == "بيع لحم بعضم")
                 .Select(o => new
                 {
                     o.OrderCode,
                     ClientName = o.Clients != null ? o.Clients.Name : null,
                     Total = o.Batches
                                 .SelectMany(b => b.CowsPieces)
                                 .Count(),
                     Pieces = o.Batches
                         .SelectMany(b => b.CowsPieces)
                         .GroupBy(p => p.PieceTybe)
                         .Select(g => new
                         {
                             PieceType = g.Key,
                             Items = g.Select(p => new
                             {
                                 PieceId = p.pieceId,
                                 StoreNumber = p.StoreId
                             }).ToList()
                         }).ToList()
                 })
                 .ToListAsync();

            if (!boneInSalesOrders.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No bone-in sales orders found."));

            return new OkObjectResult(boneInSalesOrders);
        }

        public async Task<ActionResult> GetBonelessSalesOrders()
        {
            var bonelessSalesOrders = await _context.Orders
                      .AsNoTracking()
                      .Where(o => o.OrederType == "بيع لحم مشفي")
                      .Select(o => new
                      {
                          o.OrderCode,
                          ClientName = o.Clients != null ? o.Clients.Name : null,
                          Total = o.Batches
                                     .SelectMany(b => b.CowPieces2)
                                     .Count(),
                          Pieces = o.Batches
                              .SelectMany(b => b.CowPieces2)
                              .GroupBy(p => p.Cutting.CutName)
                              .Select(g => new
                              {
                                  PieceType = g.Key,
                                  Items = g.Select(p => new
                                  {
                                      PieceId = p.PieceNumber,
                                      StoreNumber = p.StoreId
                                  }).ToList()
                              }).ToList()
                      })
                      .ToListAsync();

            if (!bonelessSalesOrders.Any())
                return new NotFoundObjectResult(new ApiResponse(404, "No boneless sales orders found."));

            return new OkObjectResult(bonelessSalesOrders);
        }

        public async Task<ActionResult> ProcessMeatSalesOrder(List<string> pieceIds)
        {
            var existingPieces = await _context.CowsPieces
                     .Include(p=>p.Batch)
                     .ThenInclude(b=>b.Order)
                     .Where(p => pieceIds.Contains(p.pieceId))
                     .ToListAsync();

            var missingPieces = pieceIds.Except(existingPieces.Select(p => p.pieceId)).ToList();
            if (missingPieces.Any())
            {
                return new BadRequestObjectResult(new { Message = "Some pieces were not found", MissingPieces = missingPieces });
            }
            var piecesWithOrders = existingPieces.Where(p => p.Batch != null && p.Batch.BatchType == "بيع لحم بعضم").ToList();
            if (piecesWithOrders.Any())
            {
                return new BadRequestObjectResult(new
                {
                    Message = "Some pieces are already assigned to a meat sale order",
                    PiecesInOrder = piecesWithOrders.Select(p => p.pieceId).ToList()
                });
            }
            var orderCode = "ORD-" + new Random().Next(100000, 999999).ToString();
            var order = new Orders
            {
                OrderCode = orderCode,
                OrederType = "بيع لحم بعضم",
                numberofbatches = 1,
                ClientsId = 1,
                EndDate = DateTime.UtcNow.AddDays(2),
                status = "Pending"
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            var batchCode = new Random().Next(100000, 999999).ToString();
            var batch = new Batch
            {
                BatchCode = batchCode,
                BatchType = "بيع لحم بعضم",
                OrderId = order.Id,
                numberOfCowOrPieces = existingPieces.Count,
                CowOrPiecesType = "ذند شمال"
            };
            await _context.Batches.AddAsync(batch);
            await _context.SaveChangesAsync();
            foreach (var piece in existingPieces)
            {
                piece.BatchId = batch.Id;
            }
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                Message = "Meat sale order created successfully",
                OrderCode = orderCode,
                BatchCode = batchCode
            });
        }

        public async Task<IActionResult> GetUnprocessedRecoveryPieces()
        {
            var today = DateTime.Now.Date;

            var pieces = await _context.CowsPieces
                .Include(cp => cp.Batch)
                .ThenInclude(b => b.Order)
                .Where(cp => cp.Batch != null &&
                             cp.Batch.Order != null &&
                             cp.Batch.Order.OrederType == "تشافي" &&
                             cp.pieceWeight_Out == null &&
                             cp.Batch.Order.Date.HasValue &&
                             cp.Batch.Order.Date.Value.Date == today)
                .ToListAsync();

            var groupedResult = pieces
                .GroupBy(cp => cp.PieceTybe)
                .Select(group => new
                {
                    PieceType = group.Key,
                    Pieces = group.Select(cp => new
                    {
                        PieceId = cp.pieceId,
                        piece_Weight_In = cp.pieceWeight_In,
                        Batch = cp.BatchId,
                        Order = cp.Batch.OrderId,
                        OrderType = cp.Batch.Order.OrederType,
                        Store = cp.StoreId,
                        OrderDate = cp.Batch.Order.Date 
                    }).ToList()
                })
                .ToList();

            return new OkObjectResult(groupedResult);
        }



    }
}
