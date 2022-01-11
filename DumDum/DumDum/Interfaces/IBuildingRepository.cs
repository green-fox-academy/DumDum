using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IBuildingRepository : IRepository<Building>
    {
        List<BuildingList> GetBuildings(int Id);
        Building AddBuilding(string building, Kingdom kingdom, BuildingType buildingType);
        List<Building> GetNumberOfFarm(int kingdomId);
        List<Building> GetNumberOfMines(int kingdomId);
        double GetAllBuildingsConsumptionInKingdom(Kingdom kingdom);
    }
}
