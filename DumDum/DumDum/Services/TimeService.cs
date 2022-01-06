using DumDum.Database;
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
        private ApplicationDbContext DbContext { get; set; }
        private DumDumService DumDumService { get; set; }
        private System.Timers.Timer IamTimeLord { get; set; }

        public TimeService(ApplicationDbContext dbContext, DumDumService dumdumService)
        {
            DbContext = dbContext;
            DumDumService = dumdumService;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            GetThingsDoneForAllKingdoms();
        }

        public LastChange GetPlayersTime(int PlayerId)
        {
            return DbContext.LastChanges.Where(x => x.PlayerId == PlayerId).FirstOrDefault();
        }

        public void GetRegistrationTime(string username)             //do kingdomregistration
        {
            int TimeOfPlayerRegistration = (int)(long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var time = new LastChange()
            {
                RegistrationTime = TimeOfPlayerRegistration,
                LastChangeTime = TimeOfPlayerRegistration,
                Player = DumDumService.GetPlayerByUsername(username),
                PlayerId = DumDumService.GetPlayerByUsername(username).PlayerId
            };
            DbContext.LastChanges.Add(time);
            DbContext.SaveChanges();
        }

        public long GetPlayersLastChangeTime(int PlayerId)
        {
            return DbContext.LastChanges.Where(x => x.PlayerId == PlayerId).FirstOrDefault().LastChangeTime;
        }

        //public long GetCycle(int id)   //zatim nepotrebujeme
        //{
        //    int TimeNow = (int)(long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        //    var time = GetPlayersTime(id);
        //    var TimeOfLastChange = time.LastChangeTime;

        //    var DifferenceBettweenTimes = TimeNow - TimeOfLastChange;
        //    var Cycles = DifferenceBettweenTimes / 600; //aktualni cas minus cas posledni zmeny

        //    var ModuloResult = DifferenceBettweenTimes % 600;
        //    var TimeOfLastChangeWithRestOfCycle = TimeOfLastChange - ModuloResult;
        //    time.LastChangeTime = TimeOfLastChangeWithRestOfCycle;
        //    DbContext.SaveChanges();

        //    return Cycles;
        //}

        public void GetThingsDoneForAllKingdoms()
        {
            var Kingdoms = DbContext.Kingdoms.ToList();
            foreach (var kingdom in Kingdoms)
            {
                GetThingsDone(kingdom.KingdomId);
            }
        }

        public void GetThingsDone(int kingdomId)
        {
            int cycles = 1;
            var ActualKingdomsGold = DumDumService.GetGoldAmountOfKingdom(kingdomId);
            var ActualKingdomsFood = DumDumService.Get(kingdomId);
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
            var gold = DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Gold");
            var food = DbContext.Resources.FirstOrDefault(r => r.KingdomId == kingdomId && r.ResourceType == "Food");
            gold.Amount = NewAmountOfGold;
            food.Amount = NewAmountOfGold;
            DbContext.SaveChanges();
        }

        public int GetFoodFromFarms(int kingdomId, int cycles)
        {
            var FoodPerFarms = 0;
            var NumberOfFarms = DbContext.Buildings.Where(b => b.KingdomId == kingdomId && b.BuildingType == "Farm").ToList();
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
            var NumberOfMines = DbContext.Buildings.Where(b => b.KingdomId == kingdomId && b.BuildingType == "Mine").ToList();
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
