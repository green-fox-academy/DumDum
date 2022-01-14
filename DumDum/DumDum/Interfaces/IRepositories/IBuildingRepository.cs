using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IBuildingRepository : IRepository<Building>
    {
        Task<List<BuildingList>> GetBuildings(int Id);
        Task<Building> AddBuilding(string building, Kingdom kingdom, BuildingType buildingType);
        List<Building> GetNumberOfFarm(int kingdomId);
        List<Building> GetNumberOfMines(int kingdomId);
        Task<double> GetAllBuildingsConsumptionInKingdom(Kingdom kingdom);
    }
}
