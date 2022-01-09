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
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            UpdateAllKingdomsEvents();
        }

        public void UpdateAllKingdomsEvents()
        {
            var Kingdoms = UnitOfWork.Kingdoms.GetAllKingdomsIncludePlayer();
            foreach (var kingdom in Kingdoms)
            {
                GetKingdomResourcesPerCycle(kingdom.KingdomId);
            }
        }

        public void GetKingdomResourcesPerCycle(int kingdomId)
        {
            int cycles = 1;
            var ActualKingdomsGold = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var ActualKingdomsFood = DumDumService.GetFoodAmountOfKingdom(kingdomId);
            //kód na produkci
            //kód na jídlo
            var ProductionOfFood = GetFoodFromFarms(kingdomId, cycles);
            //kód na zlato
            var ProductionOfGold = GetGoldFromMines(kingdomId, cycles);

            //kód na konzumaci
            var ConsumptionOfTroops = 0;
            var ConsumptionOfBuildings = 0;
            var AllConsumption = ConsumptionOfBuildings + ConsumptionOfTroops;
            //Výsledek
            var NewAmountOfFood = ActualKingdomsFood + (ProductionOfFood - AllConsumption);
            var NewAmountOfGold = ActualKingdomsGold + ProductionOfGold;


            //zapsání do db
            var gold = UnitOfWork.Resources.GetGoldAmountOfKingdom(kingdomId);
            var food = UnitOfWork.Resources.GetFoodAmountOfKingdom(kingdomId);
            gold.Amount = NewAmountOfGold;
            food.Amount = NewAmountOfFood;
            UnitOfWork.Resources.UpdateGoldAmountOfKingdom(gold);
            UnitOfWork.Resources.UpdateFoodAmountOfKingdom(food);
            UnitOfWork.Complete();
        }

        public int GetFoodFromFarms(int kingdomId, int cycles)
        {
            var FoodPerFarms = 0;
            var NumberOfFarms = UnitOfWork.Buildings.GetNumberOfFarm(kingdomId);
            foreach (var farm in NumberOfFarms)
            {
                FoodPerFarms += HomMuchFoodOneFarmProduce(farm.Level);
            }

            var ProducedFood = FoodPerFarms * cycles;

            return ProducedFood;
        }

        public int GetGoldFromMines(int kingdomId, int cycles)
        {
            var GoldPerMine = 0;
            var NumberOfMines = UnitOfWork.Buildings.GetNumberOfMines(kingdomId);
            foreach (var farm in NumberOfMines)
            {
                GoldPerMine += HomMuchFoodOneFarmProduce(farm.Level);
            }

            var ProducedGold = GoldPerMine * cycles;

            return ProducedGold;
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
