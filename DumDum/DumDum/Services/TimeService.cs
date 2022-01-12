using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DumDum.Services
{
    public class TimeService : IHostedService
    {
        private DumDumService DumDumService { get; set; }
        private System.Timers.Timer Timer { get; set; }
        private IUnitOfWork UnitOfWork { get; set; } 

        public TimeService( DumDumService dumdumService, IUnitOfWork unitOfWork)
        {
            DumDumService = dumdumService;
            UnitOfWork = unitOfWork;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            UpdateAllKingdomsEvents();
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
            var consumptionOfTroops = 0;
            var consumptionOfBuildings = 0;
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

        public int GetFoodFromFarms(int kingdomId, int cycles)
        {
            var FoodPerFarms = 0;
            var NumberOfFarms = UnitOfWork.Buildings.GetListOfBuildingsByType(kingdomId, 2);
            foreach (var farm in NumberOfFarms)
            {
                FoodPerFarms += FoodAndGoldProduction(farm.Level, farm.BuildingTypeId);
            }

            var ProducedFood = FoodPerFarms * cycles;

            return ProducedFood;
        }

        public int GetGoldFromMines(int kingdomId, int cycles)
        {
            var GoldPerMine = 0;
            var NumberOfMines = UnitOfWork.Buildings.GetListOfBuildingsByType(kingdomId, 3);
            foreach (var mine in NumberOfMines)
            {
                GoldPerMine += FoodAndGoldProduction(mine.Level, mine.BuildingTypeId);
            }

            var ProducedGold = GoldPerMine * cycles;

            return ProducedGold;
        }

        public int FoodAndGoldProduction(int buildingLevel, int buildingTypeId)
        {
            return UnitOfWork.BuildingLevels.GetProductionByBuildingTypeAndLevel(buildingTypeId, buildingLevel);
        }
        
        public int FoodAndGoldConsumption(int buildingLevel, int buildingTypeId)
        {
            return UnitOfWork.BuildingLevels.GetConsumptionByBuildingTypeAndLevel(buildingTypeId, buildingLevel);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Timer = new System.Timers.Timer(2000);
            Timer.Elapsed += OnTimedEvent;
            Timer.AutoReset = true;
            Timer.Enabled = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
