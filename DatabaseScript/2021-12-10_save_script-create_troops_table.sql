use DumDum;
Create table Troops(
    TroopId int not null auto_increment,
    TroopType varchar (255),
    `Level` int,
    HP int,
    Attack int,
    Defence int,
    Speed double,
    StartedAt int,
    FinishedAt int,
    KingdomId int not null,
    primary key (TroopId) 
);
insert into Troops(TroopId,TroopType,`Level`,HP,Attack,Defence,Speed,StartedAt,FinishedAt,KingdomId)
Values (1,"Pikemen",1,10,5,8,0.8,888,999,1),
(2,'Axemen',1,10,8,5,1,888,999,1),
(3,'Knight',1,10,15,10,1.6,888,999,1),
(4,'Spies',1,10,4,3,2.5,888,999,1),
(5,'Senators',1,10,0,0,0.5,888,999,1)
