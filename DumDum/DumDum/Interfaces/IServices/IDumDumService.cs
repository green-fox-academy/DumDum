﻿using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Kingdom;
using DumDum.Models.JsonEntities.Player;

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
        void TakeFood(int kingdomId, int amount);
        Player GetPlayerById(int id);
        Location AddLocations(Kingdom kingdom);
        int GetFoodAmountOfKingdom(int kingdomId);
        void GiveGold(int kingdomId, int amount);
        void GiveFood(int kingdomId, int amount);
        Kingdom RegisterKingdomToDB(int coordinateX, int coordinateY, int kingdomId);
        bool IsKingdomIdValid(int kingdomId);
        bool AreCredentialsValid(string username, string password);
        bool AreCoordinatesValid(int coordinateX, int coordinateY);
        bool DoCoordinatesExist(int coordinateX, int coordinateY);
        Kingdom CreateKingdom(string kingdomname, string username);
        Player Register(string username, string password, string kingdomName);
        Kingdom GetKingdomByName(string kingdomName);
    }
}