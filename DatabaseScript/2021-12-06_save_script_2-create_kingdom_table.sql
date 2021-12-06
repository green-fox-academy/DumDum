use DumDum;
Create table Kingdoms(
    KingdomId int not null auto_increment,
    CoordinateX int,
    CoordinateY int,
    PlayerId int not null,
    primary key (KingdomId) 
);
insert into Kingdoms(KingdomId,CoordinateX,CoordinateY,PlayerId)
Values (1,10,10,1),
(2,11,11,2),
(3,12,12,3),
(4,13,13,4),
(5,14,14,5);
