using DumDum.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DumDum.Interfaces.IRepositories;
using DumDum.Interfaces.IServices;
using DumDum.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Timer = System.Timers.Timer;

namespace DumDum.Services
{
    public class TimeService : ITimeService, IHostedService

    {
        private IDumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        private Timer Timer { get; set; }
        public IServiceProvider ServiceProvider { get; }

        public TimeService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task UpdateAllKingdomsEvents()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                DumDumService = scope.ServiceProvider.GetRequiredService<IDumDumService>();

                UnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                Console.WriteLine("Nya");
                var kingdoms = new List<Kingdom>();
                try
                {
                    kingdoms = await UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("nya nya ");
                    throw;
                }

                if (kingdoms is not null)
                {
                    foreach (var kingdom in kingdoms)
                    {
                        GetKingdomResourcesPerCycle(kingdom.KingdomId);
                    }
                }
            }
        }

        public async void GetKingdomResourcesPerCycle(int kingdomId)
        {
            int cycles = 1;
            var actualKingdomsGold = await DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var actualKingdomsFood = await DumDumService.GetFoodAmountOfKingdom(kingdomId);

            var productionOfFood = await GetFoodFromFarms(kingdomId, cycles);
            var productionOfGold = await GetGoldFromMines(kingdomId, cycles);

            var consumptionOfTroops = await GetConsumptionOfTroops(kingdomId);
            var consumptionOfBuildings = await GetConsumptionOfBuildings(kingdomId);
            var allConsumption = consumptionOfBuildings + consumptionOfTroops;

            var newAmountOfFood = actualKingdomsFood + (productionOfFood - allConsumption);
            var newAmountOfGold = actualKingdomsGold + productionOfGold;

            var gold = await UnitOfWork.Resources.GetGoldAmountOfKingdom(kingdomId);
            var food = await UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
            gold.Amount = newAmountOfGold;
            food.Amount = newAmountOfFood;
            UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
            UnitOfWork.Resources.UpdateFoodAmountOfKingdom(food);
            UnitOfWork.Complete();
        }

        public async Task<int> GetConsumptionOfTroops(int kingdomId)
        {
            var listOfTroops = new List<Troop>();
            var allTroopsOfKingdom = await UnitOfWork.Troops.GetAllTroopsOfKingdom(kingdomId);

            foreach (var troop in allTroopsOfKingdom)
            {
                if (!listOfTroops.Contains(troop))
                {
                    listOfTroops.Add(troop);
                }
            }

            var result = 0;
            if (listOfTroops.Count != 0)
            {
                foreach (var troop in listOfTroops)
                {
                    result += FoodAndGoldConsumption(troop.Level, troop.TroopTypeId);
                }
            }

            return result;
        }

        public async Task<int> GetConsumptionOfBuildings(int kingdomId)
        {
            var listOfBuildings = new List<Building>();
            var listOfBuildingsInKingdom = UnitOfWork.Buildings.GetAllBuildingsOfKingdom(kingdomId);

            foreach (var building in listOfBuildingsInKingdom)
            {
                if (!listOfBuildings.Contains(building))
                {
                    listOfBuildings.Add(building);
                }
            }

            var result = 0;
            if (listOfBuildings.Count != 0)
            {
                foreach (var building in listOfBuildings)
                {
                    result += FoodAndGoldBuildingConsumption(building.Level, building.BuildingTypeId);
                }
            }

            return result;
        }

        public async Task<int> GetFoodFromFarms(int kingdomId, int cycles)
        {
            var foodPerFarms = 0;
            var numberOfFarms = UnitOfWork.Buildings.GetListOfBuildingsByType(kingdomId, 2);
            foreach (var farm in numberOfFarms)
            {
                foodPerFarms += await FoodAndGoldProduction(farm.Level, farm.BuildingTypeId);
            }

            var producedFood = foodPerFarms * cycles;

            return producedFood;
        }

        public async Task<int> GetGoldFromMines(int kingdomId, int cycles)
        {
            var goldPerMine = 0;
            var numberOfMines = UnitOfWork.Buildings.GetListOfBuildingsByType(kingdomId, 3);
            foreach (var mine in numberOfMines)
            {
                goldPerMine += await FoodAndGoldProduction(mine.Level, mine.BuildingTypeId);
            }

            var producedGold = goldPerMine * cycles;

            return producedGold;
        }

        public async Task<int> FoodAndGoldProduction(int buildingLevel, int buildingTypeId)
        {
            return UnitOfWork.BuildingLevels.GetProductionByBuildingTypeAndLevel(buildingTypeId, buildingLevel);
        }

        private int FoodAndGoldBuildingConsumption(int buildingLevel, int buildingTypeId)
        {
            return UnitOfWork.BuildingLevels.GetConsumptionByBuildingTypeAndLevel(buildingTypeId, buildingLevel);
        }

        private int FoodAndGoldConsumption(int troopLevel, int troopTypeId)
        {
            return UnitOfWork.TroopLevels.GetConsumptionByTroopTypeAndLevel(troopTypeId, troopLevel);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Timer = new Timer(300000);
            Timer.Elapsed += async (sender, e) => await UpdateAllKingdomsEvents();
            Timer.Start();
            Console.WriteLine("StartAsync");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("StopAsync");
            return Task.CompletedTask;
        }
    }
}