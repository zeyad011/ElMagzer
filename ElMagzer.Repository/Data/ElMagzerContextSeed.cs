using ElMagzer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElMagzer.Repository.Data
{
    public static class ElMagzerContextSeed
    {
        //public async static Task SeedAsync(ElMagzerContext _dbContext)
        //{
        //    if (_dbContext.TypeofCows.Count() == 0)
        //    {
        //        var TypsData = File.ReadAllText("../ElMagzer.Repository/Data/DataSeed/Types.json");
        //        var Types = JsonSerializer.Deserialize<List<TypeofCows>>(TypsData);

        //        if (Types?.Count() > 0)
        //        {
        //            foreach (var Type in Types)
        //            {
        //                _dbContext.Set<TypeofCows>().Add(Type);

        //            }
        //            await _dbContext.SaveChangesAsync();
        //        }
        //    }
        //    if (_dbContext.Stores.Count() == 0)
        //    {
        //        var StoreData = File.ReadAllText("../ElMagzer.Repository/Data/DataSeed/Stores.json");
        //        var Stores = JsonSerializer.Deserialize<List<Stores>>(StoreData);

        //        if (Stores?.Count() > 0)
        //        {
        //            foreach (var store in Stores)
        //            {
        //                _dbContext.Set<Stores>().Add(store);

        //            }
        //            await _dbContext.SaveChangesAsync();
        //        }
        //    }
           
        //    if (_dbContext.cowsSeeds.Count() == 0)
        //    {
        //        var StoreData = File.ReadAllText("../ElMagzer.Repository/Data/DataSeed/CowsSeed.json");
        //        var Stores = JsonSerializer.Deserialize<List<CowsSeed>>(StoreData);

        //        if (Stores?.Count() > 0)
        //        {
        //            foreach (var store in Stores)
        //            {
        //                _dbContext.Set<CowsSeed>().Add(store);

        //            }
        //            await _dbContext.SaveChangesAsync();
        //        }
        //    }
        //    if (_dbContext.Users.Count() == 0)
        //    {
        //        var StoreData = File.ReadAllText("../ElMagzer.Repository/Data/DataSeed/Users.json");
        //        var Stores = JsonSerializer.Deserialize<List<User>>(StoreData);

        //        if (Stores?.Count() > 0)
        //        {
        //            foreach (var store in Stores)
        //            {
        //                _dbContext.Set<User>().Add(store);

        //            }
        //            await _dbContext.SaveChangesAsync();
        //        }
        //    }

        //    if (_dbContext.cuttings.Count() == 0)
        //    {
        //        var CuttingData = File.ReadAllText("../ElMagzer.Repository/Data/DataSeed/Cutts.json");
        //        var Cutts = JsonSerializer.Deserialize<List<Cutting>>(CuttingData);

        //        if (Cutts?.Count() > 0)
        //        {
        //            foreach (var cut in Cutts)
        //            {
        //                _dbContext.Set<Cutting>().Add(cut);

        //            }
        //            await _dbContext.SaveChangesAsync();
        //        }
        //    }
        //    if (_dbContext.clients.Count() == 0)
        //    {
        //        var ClientsData = File.ReadAllText("../ElMagzer.Repository/Data/DataSeed/Clients.json");
        //        var client = JsonSerializer.Deserialize<List<Clients>>(ClientsData);

        //        if (client?.Count() > 0)
        //        {
        //            foreach (var cut in client)
        //            {
        //                _dbContext.Set<Clients>().Add(cut);

        //            }
        //            await _dbContext.SaveChangesAsync();
        //        }
        //    }
        //    if (_dbContext.miscarriageTypes.Count() == 0)
        //    {
        //        var MiscarriageData = File.ReadAllText("../ElMagzer.Repository/Data/DataSeed/MiscarriageType.json");
        //        var Cutts = JsonSerializer.Deserialize<List<MiscarriageType>>(MiscarriageData);

        //        if (Cutts?.Count() > 0)
        //        {
        //            foreach (var cut in Cutts)
        //            {
        //                _dbContext.Set<MiscarriageType>().Add(cut);

        //            }
        //            await _dbContext.SaveChangesAsync();
        //        }
        //    }
        //}
    }
}
