﻿using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using System.Collections.Generic;

namespace DumDum.Interfaces
{
    public interface IKingdomRepository : IRepository<Kingdom>
    {
        Kingdom GetKingdomByName(string kingdomName);
        Kingdom GetKingdomById(int kingdomId);
        KingdomsListResponse GetAllKingdoms();
        Kingdom FindPlayerByKingdomId(int kingdomId);
        List<Kingdom> GetAllKingdomsIncludePlayer();
        List<BuildingPoints> GetListBuildingPoints();

    }
}