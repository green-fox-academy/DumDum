create database DumDum;
use DumDum;
Create table Players(
	PlayerId int not null auto_increment,
    Username varchar (255),
    Password varchar (255),
    KingdomId int not null,
    primary key (PlayerId)
);
insert into Players(PlayerId,Username,Password,KingdomId)
Values (1,'Nya','catcatcat',1),
(2,'Mladen','mladen',2),
(3,'Komin','dildodildo',1),
(4,'Beef69','chicken',1),
(5,'Marek','pivko',2);