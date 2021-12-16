use DumDum;
Create table TroopTypes(
    TroopTypeId int not null auto_increment,
    TroopType varchar (255),
    primary key (TroopTypeId) 
);
insert into TroopTypes(TroopTypeId,TroopType)
Values (1,'Axemen'),
(2,'Phalanx'),
(3,'Knight'),
(4,'Spy'),
(5,'Senator');
