use DumDum;
Create table Kingdoms(
    KingdomId int not null auto_increment,
    KingdomName varchar (255),
    CoordinateX int,
    CoordinateY int,
    PlayerId int not null,
    primary key (KingdomId) 
);
insert into Kingdoms(KingdomId,KingdomName,CoordinateX,CoordinateY,PlayerId)
Values (1,'Nya Nya Land',10,10,1),
(2,'Jaja Land',11,11,2),
(3,'Nevim',12,12,3),
(4,'Wano',13,13,4),
(5,'Pivko',14,14,5);
