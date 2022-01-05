use DumDum;
Create table Battles(
    BattleId int not null auto_increment,
    AttackerName varchar (255),
    Target varchar (255),
    BattleType varchar (255),
    ResolutionTime bigint,
    TimeToStartTheBattle bigint,
    Winner varchar (255),
    primary key (BattleId) 
);
Create table Attackers(
    AttackerId int not null auto_increment,
    AttackerName varchar (255),
    BattleId int not null,
    primary key (AttackerId) 
);
Create table Defenders(
    DefenderId int not null auto_increment,
    DefenderName varchar (255),
    BattleId int not null,
    primary key (DefenderId) 
);

