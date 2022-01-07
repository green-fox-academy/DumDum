using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IBuildingRepository : IRepository<Building>
    {
        Task<List<BuildingList>> GetBuildings(int Id);
        Task<Building> AddBuilding(string building, Kingdom kingdom, BuildingType buildingType);
        Task<double> GetAllBuildingsConsumptionInKingdom(Kingdom kingdom);
    }
}
