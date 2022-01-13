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
    public class TimeService : IHostedService, ITimeService
    {
        private IDumDumService DumDumService { get; set; }
        private System.Timers.Timer IamTimeLord { get; set; }
        private IUnitOfWork UnitOfWork { get; set; } 

        public TimeService( IDumDumService dumdumService, IUnitOfWork unitOfWork)
        {
            DumDumService = dumdumService;
            UnitOfWork = unitOfWork;
        }
        public void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            UpdateAllKingdomsEvents();
        }

        public void UpdateAllKingdomsEvents()
        {
            var Kingdoms = UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();
            foreach (var kingdom in Kingdoms.Result)
            {
                GetKingdomResourcesPerCycle(kingdom.KingdomId);
            }
        }

        public async void GetKingdomResourcesPerCycle(int kingdomId)
        {
            int cycles = 1;
            var actualKingdomsGold = DumDumService.GetGoldAmountOfKingdom(kingdomId).Result;
            var actualKingdomsFood = DumDumService.GetFoodAmountOfKingdom(kingdomId).Result;
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
            var gold = UnitOfWork.Resources.GetGoldAmountOfKingdom(kingdomId).Result;
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId).Result;
            gold.Amount = newAmountOfGold;
            food.Amount = newAmountOfFood;
            UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
            UnitOfWork.Resources.UpdateFoodAmountOfKingdom(food);
            UnitOfWork.Complete();
        }

        public int GetFoodFromFarms(int kingdomId, int cycles)
        {
            var foodPerFarms = 0;
            var numberOfFarms = UnitOfWork.Buildings.GetNumberOfFarm(kingdomId);
            foreach (var farm in numberOfFarms)
            {
                foodPerFarms += HomMuchFoodOneFarmProduce(farm.Level);
            }

            var producedFood = foodPerFarms * cycles;

            return producedFood;
        }

        public int GetGoldFromMines(int kingdomId, int cycles)
        {
            var goldPerMine = 0;
            var numberOfMines = UnitOfWork.Buildings.GetNumberOfMines(kingdomId);
            foreach (var farm in numberOfMines)
            {
                goldPerMine += HomMuchFoodOneFarmProduce(farm.Level);
            }

            var producedGold = goldPerMine * cycles;

            return producedGold;
        }

        public int HomMuchFoodOneFarmProduce(int lvl)
        {
            return lvl * (1 + lvl);
        }

        public int HomMuchGoldOneMineProduce(int lvl)
        {
            return lvl * (1 + lvl);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IamTimeLord = new System.Timers.Timer(2000);
            IamTimeLord.Elapsed += OnTimedEvent;
            IamTimeLord.AutoReset = true;
            IamTimeLord.Enabled = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
