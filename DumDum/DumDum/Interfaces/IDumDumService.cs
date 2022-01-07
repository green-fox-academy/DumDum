using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Authorization;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IDumDumService
    {
        PlayerResponse RegisterPlayerLogic(PlayerRequest playerRequest, out int statusCode);
        string RegisterKingdom(string authorization, KingdomRegistrationRequest kingdomRequest, out int statusCode);
        KingdomsListResponse GetAllKingdoms();
        Player GetPlayerByUsername(string username);
        Kingdom GetKingdomById(int kingdomId);
        int GetGoldAmountOfKingdom(int kingdomId);
        void TakeGold(int kingdomId, int amount);
        Player GetPlayerById(int id);
        Location AddLocations(Kingdom kingdom);
    }
}
