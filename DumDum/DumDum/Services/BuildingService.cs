﻿using DumDum.Database;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities;
using DumDum.Models.JsonEntities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Services
{
    public class BuildingService
    {
        private ApplicationDbContext DbContext { get; set; }
        private AuthenticateService AuthenticateService { get; set; }
        private DumDumService DumDumService { get; set; }
        public BuildingService(ApplicationDbContext dbContex, AuthenticateService authService, DumDumService dumService)
        {
            DbContext = dbContex;
            AuthenticateService = authService;
            DumDumService = dumService;
        }

        public List<BuildingList> GetBuildings(int Id)
        {
            return DbContext.Buildings.Where(b => b.KingdomId == Id).
                Select(b => new BuildingList()
                {
                    BuildingId = b.BuildingId,
                    BuildingType = b.BuildingType,
                    Level = b.Level,
                    StartedAt = b.StartedAt,
                    FinishedAt = b.FinishedAt
                }).ToList();
        }

        public KingdomResponse GetKingdom(int id)
        {
            var kingdom = DumDumService.GetKingdomById(id);
            var player = DumDumService.GetPlayerById(kingdom.PlayerId);
            var locations = DumDumService.AddLocations(kingdom);
            return new KingdomResponse()
            {
                KingdomId = kingdom.KingdomId,
                KingdomName = kingdom.KingdomName,
                Ruler = player.Username,
                Population = 0,
                Location = locations,
            };
        }

        public BuildingResponse ListBuildings(string authorization, int kingdomId, out int statusCode)
        {
            var response = new BuildingResponse();
            if (authorization != null && kingdomId != null)
            {
                AuthRequest request = new AuthRequest();
                request.Token = authorization;
                var player = AuthenticateService.GetUserInfo(request);
                if (player != null && player.KingdomId == kingdomId)
                {
                    response.Kingdom = GetKingdom(kingdomId);
                    response.Buildings = GetBuildings(kingdomId);
                    statusCode = 200;
                    return response;
                }
            }
            statusCode = 401;
            return response;
        }

        public Kingdom FindPlayerByKingdomId(int Id)
        {
            var kingdom = DbContext.Kingdoms.Include(p => p.Player).FirstOrDefault(k => k.KingdomId == Id);
            return kingdom;
        }

        public void AddBuilding(string building, int id, string authorization, out int statusCode)
        {
            string buildingLover = building.ToLower();
            var buildingList = GetBuildings(id);
            AuthRequest authRequest = new AuthRequest(){Token = authorization};
            
            var player = AuthenticateService.GetUserInfo(authRequest);
            if (player == null)
            {
                statusCode = 401;
            }

            if (building != "townhall" || building != "barracks" || building != "academy" || building != "walls" || building != "farm" || building != "mine")
            {
                statusCode = 400;
            }
            
            
            var kingdom = FindPlayerByKingdomId(id);
            DbContext.Buildings.Add(new Building(){BuildingType = building, KingdomId = kingdom.KingdomId, Kingdom = kingdom, });
        }
    }
}