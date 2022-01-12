using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IBuildingRepository : IRepository<Building>
    {
        List<BuildingList> GetBuildings(int Id);
        Building AddBuilding(string building, Kingdom kingdom, BuildingType buildingType);
        List<Building> GetListOfBuildingsByType(int kingdomId, int buildingTypeId);
        double GetAllBuildingsConsumptionInKingdom(Kingdom kingdom);
        List<Building> GetAllBuildingsOfKingdom(int kingdomId);
    }
}