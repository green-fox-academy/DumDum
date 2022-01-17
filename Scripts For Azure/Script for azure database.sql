
Create table Players(
	PlayerId int not null,
    Username varchar (255) not null,
    Password varchar (255),
    KingdomId int not null,
    primary key (PlayerId)
);

Create table Kingdoms(
    KingdomId int not null,
    KingdomName varchar (255),
    CoordinateX int,
    CoordinateY int,
    PlayerId int not null,
    primary key (KingdomId) 
);

Create table Resources(
    ResourceId int not null,
    ResourceType varchar (255),
    Amount int,
    Generation int not null,
    UpdatedAt bigint,
    KingdomId int,
    primary key (ResourceId) 
);

Create table Troops(
    TroopId int not null,
    TroopTypeId int,
    Level int,
    StartedAt int,
    FinishedAt int,
    KingdomId int not null,
    primary key (TroopId) 
);

create table Buildings(
	BuildingId int not null,
    BuildingType varchar (255),
    Level int,
    StartedAt bigint,
    FinishedAt bigint,
    KingdomId int not null,
    primary key (BuildingId) 
    );
    
CREATE TABLE trooplevel (
  TroopLevelId int NOT NULL,
  Level int DEFAULT NULL,
  Cost int DEFAULT NULL,
  Attack decimal DEFAULT NULL,
  Defence decimal DEFAULT NULL,
  CarryCap int DEFAULT NULL,
  Consumption decimal DEFAULT NULL,
  Speed decimal DEFAULT NULL,
  SpecialSkills int DEFAULT NULL,
  HP int DEFAULT NULL,
  TroopTypeId int DEFAULT NULL,
  PRIMARY KEY (TroopLevelId)
);

Create table TroopTypes(
    TroopTypeId int not null,
    TroopType varchar (255),
    primary key (TroopTypeId) 
);

Create table BuildingTypes(
    BuildingTypeId int not null,
    BuildingTypeName varchar (255),
    BuildingLevelId int not null,
    primary key (BuildingTypeId) 
);

Create table BuildingLevels(
    BuildingLevelId int,
    LevelNumber int,
    Cost int,
    ConstTime bigint,
    ResearchTime bigint,
    MaxStorage int,
    Defense int,
    Production int,
    Consumption int,
    DefBoost decimal,
    CreatedTimeAxemen bigint,
    CreatedTimePhalanx bigint,
    CreatedTimeKnights bigint,
    CreatedTimeCatapult bigint,
    CreatedTimeTheSpy bigint,
    CreatedTimeSenator bigint
);

Create table Battles(
    BattleId int not null,
    AttackerId int,
    DefenderId int,
    BattleType varchar (255),
    FoodStolen int,
    GoldStolen int,
    ResolutionTime bigint,
    TimeToStartTheBattle bigint,
    WinnerPlayerId int,
    primary key (BattleId) 
);

Create table TroopsLost(
    TroopLostId int not null,
    Type int,
    Quantity int,
    PlayerId int,
    BattleId int,
    primary key (TroopLostId) 
);

insert into Players(PlayerId,Username,Password,KingdomId)
Values (1,'Nya','catcatcat',1),
(2,'Mladen','mladen',2),
(3,'Komin','dildodildo',3),
(4,'Beef69','chicken',4),
(5,'Marek','pivko',5);

insert into Kingdoms(KingdomId,KingdomName,CoordinateX,CoordinateY,PlayerId)
Values (1,'Nya Nya Land',10,10,1),
(2,'Jaja Land',11,11,2),
(3,'Nevim',12,12,3),
(4,'Wano',13,13,4),
(5,'Pivko',14,14,5);

insert into Resources(ResourceId,ResourceType,Amount,Generation,UpdatedAt, KingdomId)
Values (1,'Food',1,1,1,1),
(2,'Gold',1,1,1,1);

insert into Troops(TroopId,TroopTypeId,Level,StartedAt,FinishedAt,KingdomId)
Values (1,1,1,888,999,1),
(2,2,1,888,999,1),
(3,3,1,888,999,1),
(4,4,1,888,999,1),
(5,5,1,888,999,1);

insert into Buildings(BuildingId,BuildingType,Level,StartedAt,FinishedAt,KingdomId)
    Values (1,'Townhall',1,1,1,1),
    (2,'Farm',1,1,1,1),
    (3,'Mine',1,1,1,1),
    (4,'Townhall',1,1,1,2),
    (5,'Farm',1,1,1,2),
    (6,'Mine',1,1,1,2),
    (7,'Townhall',1,1,1,3),
    (8,'Farm',1,1,1,3),
    (9,'Mine',1,1,1,3),
    (10,'Townhall',1,1,1,4),
    (11,'Farm',1,1,1,4),
    (12,'Mine',1,1,1,4),
    (13,'Townhall',1,1,1,5),
    (14,'Farm',1,1,1,5),
    (15,'Mine',1,1,1,5)
    ;
    
Alter table Buildings add Hp int ;
update Buildings set Hp = '1';
alter table dumdum.buildings add BuildingTypeId int;

UPDATE players
SET Password = 'APewWIgW7uSCM45/No4EA0FVbATb+iz0ojPFkosjqCzurpr0gfMWMYiqssU0kfiomQ=='  WHERE PlayerId =1;
UPDATE players
SET Password = 'ABcZ6W+H0YSxg0OnBnF0G3Fd1XMOtvFp8mR+G9943HmqKsRqP+fCCKhClrlYw1Yc8g=='  WHERE PlayerId =2;
UPDATE players
SET Password = 'AL7yR+PWiLQ9QLMRaaV/hPRFS4U9zh2cpZeM5t4aUOzXzxflQlo44zOthHxEJR18pQ=='  WHERE PlayerId =3;
UPDATE players
SET Password = 'APtZgIIV/jwVBTxPEkx1rruwcCc+V2xuXl6LaiBwqncFHzDcml9pcNM4G7J4qUgYdw=='  WHERE PlayerId =4;
UPDATE players
SET Password = 'AKHErNCkQP79+5JUAfcGLtSodHMsKXVebp2NkWfGAlI7H2GxSdm+dnUqdwbeS9g/IA=='  WHERE PlayerId =5;

INSERT INTO trooplevel VALUES (1,1,77,8.4,5.3,30,1,1,0,1,1),
(2,2,154,8.8,5.5,35,1.2,1.2,0,2,1),
(3,3,231,9.2,5.8,40,1.4,1.4,0,3,1),
(4,4,308,9.6,6,45,1.6,1.6,0,4,1),
(5,5,385,10,6.3,50,1.8,1.8,0,5,1),
(6,1,77,5.3,8.4,30,1,0.8,0,1,2),
(7,2,154,5.5,8.8,35,1.2,1,0,2,2),
(8,3,231,5.8,9.2,40,1.4,1.2,0,3,2),
(9,4,308,6,9.6,45,1.6,1.4,0,4,2),
(10,5,385,6.3,10,50,1.8,1.6,0,5,2),
(11,1,121,15.8,10.5,0,2,1.6,0,3,3),
(12,2,242,16.5,11,0,2.2,1.8,0,4,3),
(13,3,363,17.3,11.5,0,2.4,2,0,5,3),
(14,4,484,18,12,0,2.6,2.2,0,6,3),
(15,5,605,18.8,12.5,0,2.8,2.4,0,7,3),
(16,1,121,4.2,3.2,0,2,2.5,0,3,4),
(17,2,242,4.4,3.3,0,2.2,2.7,0,4,4),
(18,3,363,4.6,3.4,0,2.4,2.9,0,5,4),
(19,4,484,4.8,3.6,0,2.6,3.1,0,6,4),
(20,5,605,5,3.8,0,2.8,3.3,0,7,4),
(21,1,1800,0,0,0,5,0.5,25,1,5);

insert into BuildingTypes(BuildingTypeId, BuildingTypeName, BuildingLevelId)
Values (1,'Townhall',1),
(2,'Farm',2),
(3,'Mine',3),
(4,'Barracks',4),
(5,'Academy',5),
(6, 'Wall', 6);

insert into BuildingLevels(BuildingLevelId, LevelNumber, Cost, ConstTime, ResearchTime, MaxStorage, Defense, Production, Consumption, DefBoost, CreatedTimeAxemen, CreatedTimePhalanx, CreatedTimeKnights, CreatedTimeCatapult, CreatedTimeTheSpy, CreatedTimeSenator)
Values (1, 1, 120, 3600, 0, 1200, 10, 0, 1, 0, 0, 0, 0, 0, 0, 0),
(1, 2, 135, 4500, 0, 2400, 12, 0, 2, 0, 0, 0, 0, 0, 0, 0),
(1, 3, 271, 5400, 0, 3600, 15, 0, 3, 0, 0, 0, 0, 0, 0, 0),
(1, 4, 406, 6300, 0, 4800, 17, 0, 4, 0, 0, 0, 0, 0, 0, 0),
(1, 5, 542, 7200, 0, 6000, 20, 0, 5, 0, 0, 0, 0, 0, 0, 0),
(2, 1, 60, 480, 0, 0, 0, 6, 1, 0, 0, 0, 0, 0, 0, 0),
(2, 2, 67, 600, 0, 0, 0, 13, 2, 0, 0, 0, 0, 0, 0, 0),
(2, 3, 135, 720, 0, 0, 0, 20, 3, 0, 0, 0, 0, 0, 0, 0),
(2, 4, 203, 840, 0, 0, 0, 27, 4, 0, 0, 0, 0, 0, 0, 0),
(2, 5, 271, 960, 0, 0, 0, 34, 5, 0, 0, 0, 0, 0, 0, 0),
(3, 1, 80, 600, 0, 0, 0, 5, 1, 0, 0, 0, 0, 0, 0, 0),
(3, 2, 90, 750, 0, 0, 0, 11, 2, 0, 0, 0, 0, 0, 0, 0),
(3, 3, 180, 900, 0, 0, 0, 17, 3, 0, 0, 0, 0, 0, 0, 0),
(3, 4, 271, 1050, 0, 0, 0, 23, 4, 0, 0, 0, 0, 0, 0, 0),
(3, 5, 361, 1200, 0, 0, 0, 28, 5, 0, 0, 0, 0, 0, 0, 0),
(4, 1, 130, 3000, 0, 0, 0, 0, 1, 0, 590,  590, 1180, 1770, 885, 3555),
(4, 2, 146, 3780, 0, 0, 0, 0, 2, 0, 580, 580, 1160, 1740, 870, 3510),
(4, 3, 293, 4500, 0, 0, 0, 0, 3, 0, 570, 570, 1140, 1710, 855, 3465),
(4, 4, 440, 5220, 0, 0, 0, 0, 4, 0, 560, 560, 1120, 1680, 840, 3420),
(4, 5, 587, 6000, 0, 0, 0, 0, 5, 0, 550, 550, 1100, 1650, 825, 3375),
(5, 1, 140, 4200, 2880, 0, 20, 0, 1, 0, 0, 0, 0, 0, 0, 0),
(5, 2, 158, 5250, 3360, 0, 20, 0, 2, 0, 0, 0, 0, 0, 0, 0),
(5, 3, 316, 6300, 3840, 0, 20, 0, 3, 0, 0, 0, 0, 0, 0, 0),
(5, 4, 474, 7290, 4320, 0, 20, 0, 4, 0, 0, 0, 0, 0, 0, 0),
(5, 5, 632, 8400, 4800, 0, 20, 0, 5, 0, 0, 0, 0, 0, 0, 0),
(6, 1, 100, 1800, 0, 0, 20, 0, 1, 2, 0, 0, 0, 0, 0, 0),
(6, 2, 112, 2250, 0, 0, 25, 0, 2, 4, 0, 0, 0, 0, 0, 0),
(6, 3, 225, 2700, 0, 0, 30, 0, 3, 6, 0, 0, 0, 0, 0, 0),
(6, 4, 338, 3150, 0, 0, 35, 0, 4, 8, 0, 0, 0, 0, 0, 0),
(6, 5, 451, 3600, 0, 0, 40, 0, 5, 10, 0, 0, 0, 0, 0, 0);

UPDATE Buildings SET BuildingTypeId = '1' WHERE (BuildingId = '1');
UPDATE Buildings SET BuildingTypeId = '2' WHERE (BuildingId = '2');
UPDATE Buildings SET BuildingTypeId = '3' WHERE (BuildingId = '3');
UPDATE Buildings SET BuildingTypeId = '1' WHERE (BuildingId = '4');
UPDATE Buildings SET BuildingTypeId = '2' WHERE (BuildingId = '5');
UPDATE Buildings SET BuildingTypeId = '3' WHERE (BuildingId = '6');
UPDATE Buildings SET BuildingTypeId = '1' WHERE (BuildingId = '7');
UPDATE Buildings SET BuildingTypeId = '2' WHERE (BuildingId = '8');
UPDATE Buildings SET BuildingTypeId = '3' WHERE (BuildingId = '9');
UPDATE Buildings SET BuildingTypeId = '1' WHERE (BuildingId = '10');
UPDATE Buildings SET BuildingTypeId = '2' WHERE (BuildingId = '11');
UPDATE Buildings SET BuildingTypeId = '3' WHERE (BuildingId = '12');
UPDATE Buildings SET BuildingTypeId = '1' WHERE (BuildingId = '13');
UPDATE Buildings SET BuildingTypeId = '2' WHERE (BuildingId = '14');
UPDATE Buildings SET BuildingTypeId = '3' WHERE (BuildingId = '15');

Alter table TroopLevel add ConstTime int ;
UPDATE TroopLevel SET ConstTime = 700 WHERE TroopLevelId =1;
UPDATE TroopLevel SET ConstTime = 600 WHERE TroopLevelId =2;
UPDATE TroopLevel SET ConstTime = 500 WHERE TroopLevelId =3;
UPDATE TroopLevel SET ConstTime = 400 WHERE TroopLevelId =4;
UPDATE TroopLevel SET ConstTime = 300 WHERE TroopLevelId =5;
UPDATE TroopLevel SET ConstTime = 700 WHERE TroopLevelId =6;
UPDATE TroopLevel SET ConstTime = 600 WHERE TroopLevelId =7;
UPDATE TroopLevel SET ConstTime = 500 WHERE TroopLevelId =8;
UPDATE TroopLevel SET ConstTime = 400 WHERE TroopLevelId =9;
UPDATE TroopLevel SET ConstTime = 300 WHERE TroopLevelId =10;
UPDATE TroopLevel SET ConstTime = 700 WHERE TroopLevelId =11;
UPDATE TroopLevel SET ConstTime = 600 WHERE TroopLevelId =12;
UPDATE TroopLevel SET ConstTime = 500 WHERE TroopLevelId =13;
UPDATE TroopLevel SET ConstTime = 400 WHERE TroopLevelId =14;
UPDATE TroopLevel SET ConstTime = 300 WHERE TroopLevelId =15;
UPDATE TroopLevel SET ConstTime = 700 WHERE TroopLevelId =16;
UPDATE TroopLevel SET ConstTime = 600 WHERE TroopLevelId =17;
UPDATE TroopLevel SET ConstTime = 500 WHERE TroopLevelId =18;
UPDATE TroopLevel SET ConstTime = 400 WHERE TroopLevelId =19;
UPDATE TroopLevel SET ConstTime = 300 WHERE TroopLevelId =20;
UPDATE TroopLevel SET ConstTime = 700 WHERE TroopLevelId =21;

alter table Buildinglevels drop CreatedTimeAxemen;
alter table Buildinglevels drop CreatedTimePhalanx;
alter table Buildinglevels drop CreatedTimeKnights;
alter table Buildinglevels drop CreatedTimeCatapult;
alter table Buildinglevels drop CreatedTimeTheSpy;
alter table Buildinglevels drop CreatedTimeSenator;

update  Resources set Amount = 100 where resources.kingdomId = 1;
insert into Resources(ResourceId,ResourceType,Amount,Generation,UpdatedAt, KingdomId)
Values 
(3,'Food',100,1,1,2),
(4,'Gold',100,1,1,2),
(5,'Food',100,1,1,3),
(6,'Gold',100,1,1,3),
(7,'Food',100,1,1,4),
(8,'Gold',100,1,1,4),
(9,'Food',100,1,1,5),
(10,'Gold',100,1,1,5);

alter table Players add Email varchar(250);
alter table Players add IsVerified bool;

UPDATE players
SET Email = 'nya@nya.nya', IsVerified = 1  WHERE PlayerId =1;
UPDATE players
SET Email = 'mladen@nya.nya', IsVerified = 1  WHERE PlayerId =2;
UPDATE players
SET Email = 'komin@nya.nya', IsVerified = 1  WHERE PlayerId =3;
UPDATE players
SET Email = 'beef69@nya.nya', IsVerified = 1  WHERE PlayerId =4;
UPDATE players
SET Email = 'pivko@nya.nya', IsVerified = 1  WHERE PlayerId =5;