using DumDum.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DumDum.Models.Entities;
using Microsoft.Extensions.Hosting;

namespace DumDum.Services
{
    public class TimeService :   ITimeService

    {
        private IDumDumService DumDumService { get; set; }
        private IUnitOfWork UnitOfWork { get; set; }
        private IHostedService HostedService { get; set; }

        public TimeService(IDumDumService dumdumService, IUnitOfWork unitOfWork, IHostedService hostedService)
        {
            DumDumService = dumdumService;
            UnitOfWork = unitOfWork;
            HostedService = hostedService;
            HostedService.StartAsync(new CancellationToken());
        }

        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            
        }

        public void UpdateAllKingdomsEvents()
        {
            var kingdoms = UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();
            foreach (var kingdom in kingdoms)
            {
                GetKingdomResourcesPerCycle(kingdom.KingdomId);
            }
        }

        public void GetKingdomResourcesPerCycle(int kingdomId)
        {
            int cycles = 1;
            var actualKingdomsGold = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var actualKingdomsFood = DumDumService.GetFoodAmountOfKingdom(kingdomId);
            //kód na produkci
            //kód na jídlo
            var productionOfFood = GetFoodFromFarms(kingdomId, cycles);
            //kód na zlato
            var productionOfGold = GetGoldFromMines(kingdomId, cycles);

            //kód na konzumaci
            var consumptionOfTroops = GetConsumptionOfTroops(kingdomId);
            var consumptionOfBuildings = GetConsumptionOfBuildings(kingdomId);
            var allConsumption = consumptionOfBuildings + consumptionOfTroops;
            //Výsledek
            var newAmountOfFood = actualKingdomsFood + (productionOfFood - allConsumption);
            var newAmountOfGold = actualKingdomsGold + productionOfGold;


            //zapsání do db
            var gold = UnitOfWork.Resources.GetGoldAmountOfKingdom(kingdomId);
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
            gold.Amount = newAmountOfGold;
            food.Amount = newAmountOfFood;
            UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
            UnitOfWork.Resources.UpdateFoodAmountOfKingdom(food);
            UnitOfWork.Complete();
        }

        private int GetConsumptionOfTroops(int kingdomId)
        {
            var listOfTroops = new List<Troop>();
            var allTroopsOfKingdom = UnitOfWork.Troops.GetAllTroopsOfKingdom(kingdomId);

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

        private int GetConsumptionOfBuildings(int kingdomId)
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

        public int GetFoodFromFarms(int kingdomId, int cycles)
        {
            var foodPerFarms = 0;
            var numberOfFarms = UnitOfWork.Buildings.GetListOfBuildingsByType(kingdomId, 2);
            foreach (var farm in numberOfFarms)
            {
                foodPerFarms += FoodAndGoldProduction(farm.Level, farm.BuildingTypeId);
            }

            var producedFood = foodPerFarms * cycles;

            return producedFood;
        }

        public int GetGoldFromMines(int kingdomId, int cycles)
        {
            var goldPerMine = 0;
            var numberOfMines = UnitOfWork.Buildings.GetListOfBuildingsByType(kingdomId, 3);
            foreach (var mine in numberOfMines)
            {
                goldPerMine += FoodAndGoldProduction(mine.Level, mine.BuildingTypeId);
            }

            var producedGold = goldPerMine * cycles;

            return producedGold;
        }

        public int FoodAndGoldProduction(int buildingLevel, int buildingTypeId)
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