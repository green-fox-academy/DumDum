using System;
using System.Collections.Generic;

namespace DumDum.Models.Entities
{
    public class Building
    {
        public int BuildingId { get; set; }
        public string BuildingType { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public Kingdom Kingdom { get; set; }
        public int KingdomId { get; set; }
        public List<BuildingType> BuildingTypes { get; set; }
        public int BuildingTypeId { get; set; }

        public List<Building> GetActiveBuildings(List<Building> allBuildings)
        {
            List<Building> activeBuildings = new List<Building>();
            for (int i = 0; i < allBuildings.Count; i++)
            {
                if (allBuildings[i].FinishedAt < (int) DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    activeBuildings.Add(allBuildings[i]);
                }
            }

            return activeBuildings;
        }
        public bool IsActive(Building building)
        {
            return building.FinishedAt < (int) DateTimeOffset.Now.ToUnixTimeSeconds();
        }
    }
}
