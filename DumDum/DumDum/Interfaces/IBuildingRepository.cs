using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IBuildingRepository : IRepository<Building>
    {
        List<BuildingList> GetBuildings(int Id);
        Building AddBuilding(string building, Kingdom kingdom, BuildingType buildingType);
        double GetAllBuildingsConsumptionInKingdom(Kingdom kingdom);
    }
}
