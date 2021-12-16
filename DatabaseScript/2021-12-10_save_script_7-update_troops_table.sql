use DumDum;
DROP TABLE IF EXISTS `troops`;
Create table Troops(
    TroopId int not null auto_increment,
    TroopTypeId int,
    `Level` int,
    StartedAt int,
    FinishedAt int,
    KingdomId int not null,
    primary key (TroopId) 
);
insert into Troops(TroopId,TroopTypeId,`Level`,StartedAt,FinishedAt,KingdomId)
Values (1,1,1,888,999,1),
(2,2,1,888,999,1),
(3,3,1,888,999,1),
(4,4,1,888,999,1),
(5,5,1,888,999,1);
