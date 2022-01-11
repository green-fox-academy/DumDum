use DumDum;
DROP TABLE IF EXISTS `battles`;
DROP TABLE IF EXISTS `troopslost`;
Create table Battles(
    BattleId int not null auto_increment,
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
    TroopLostId int not null auto_increment,
    Type int,
    Quantity int,
    PlayerId int,
    BattleId int,
    primary key (TroopLostId) 
);
