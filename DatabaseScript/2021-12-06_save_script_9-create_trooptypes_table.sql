use DumDum;
Create table TroopTypes(
    TroopTypeId int not null auto_increment,
    TroopType varchar (255),
    TroopLevelId int,
    primary key (TroopTypeId) 
);
insert into TroopTypes(TroopTypeId,TroopType,TroopLevelId)
Values (1,'Axemen',1),
(2,'Phalanx',1),
(3,'Knight',1),
(4,'Spy',1),
(5,'Senator',1);
