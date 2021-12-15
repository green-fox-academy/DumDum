using DumDum.Database;
using DumDum.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Services
{
    public class TimeService
    {
        private ApplicationDbContext DbContext { get; set; }
        private DumDumService DumDumService { get; set; }

        public TimeService(ApplicationDbContext dbContext, DumDumService dumdumService)
        {
            DbContext = dbContext;
            DumDumService = dumdumService;
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

        public long GetCycle(int id)
        {
            int TimeNow = (int)(long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var time = GetPlayersTime(id);
            var TimeOfLastChange = time.LastChangeTime;

            var DifferenceBettweenTimes = TimeNow - TimeOfLastChange;
            var Cycles = DifferenceBettweenTimes / 600; //aktualni cas minus cas posledni zmeny

            var ModuloResult = DifferenceBettweenTimes % 600;
            var TimeOfLastChangeWithRestOfCycle = TimeOfLastChange-ModuloResult;
            time.LastChangeTime = TimeOfLastChangeWithRestOfCycle;
            DbContext.SaveChanges();

            return Cycles;
        }


    }
}
