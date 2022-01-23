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
        Task<decimal> GetAllBuildingsConsumptionInKingdom(Kingdom kingdom);
        public List<Building> GetListOfBuildingsByType(int kingdomId, int buildingTypeId);
        public List<Building> GetAllBuildingsOfKingdom(int kingdomId);
    }
}
