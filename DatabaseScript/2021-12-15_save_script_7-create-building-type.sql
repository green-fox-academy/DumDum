use DumDum;
Create table BuildingTypes(
    BuildingTypeId int not null auto_increment,
    BuildingTypeName varchar (255),
    BuildingLevelId int not null,
    primary key (BuildingTypeId) 
);
insert into BuildingTypes(BuildingTypeId, BuildingTypeName, BuildingLevelId)
Values (1,'Townhall',1),
(2,'Farm',2),
(3,'Mine',3),
(4,'Barracks',4),
(5,'Academy',5),
(6, 'Wall', 6);
