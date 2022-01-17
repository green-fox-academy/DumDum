using DumDum.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DumDum.Models.Entities;
using Timer = System.Timers.Timer;

namespace DumDum.Services
{
    public class TimeService : ITimeService

    {
        private IDumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        public static Timer timer = new System.Timers.Timer(6000);
        
        public TimeService(IDumDumService dumdumService, IUnitOfWork unitOfWork)
        {
            DumDumService = dumdumService;
            UnitOfWork = unitOfWork;
            timer.Elapsed += async ( sender, e ) =>  await UpdateAllKingdomsEvents();
            timer.Start();
        }
        
        private   Timer ExecuteAsync()
        {
            Timer timer = new Timer(5000);
            timer.Elapsed += async ( sender, e ) =>  await UpdateAllKingdomsEvents();
            timer.Start();
            return timer;
        }

        public async Task UpdateAllKingdomsEvents()
        {
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
            return;
        }

        public async void GetKingdomResourcesPerCycle(int kingdomId)
        {
            int cycles = 1;
            var actualKingdomsGold = await DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var actualKingdomsFood = await DumDumService.GetFoodAmountOfKingdom(kingdomId);
            //kód na produkci
            //kód na jídlo
            var productionOfFood = await GetFoodFromFarms(kingdomId, cycles);
            //kód na zlato
            var productionOfGold = await GetGoldFromMines(kingdomId, cycles);

            //kód na konzumaci
            var consumptionOfTroops = await GetConsumptionOfTroops(kingdomId);
            var consumptionOfBuildings = await GetConsumptionOfBuildings(kingdomId);
            var allConsumption = consumptionOfBuildings + consumptionOfTroops;
            //Výsledek
            var newAmountOfFood = actualKingdomsFood + (productionOfFood - allConsumption);
            var newAmountOfGold = actualKingdomsGold + productionOfGold;


            //zapsání do db
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
    }
}