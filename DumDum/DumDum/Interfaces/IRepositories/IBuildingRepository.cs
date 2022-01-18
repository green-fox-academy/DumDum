using System.Collections.Generic;
using System.Threading.Tasks;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;

namespace DumDum.Interfaces.IRepositories
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
