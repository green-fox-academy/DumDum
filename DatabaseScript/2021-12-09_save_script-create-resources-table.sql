use DumDum;
Create table Resources(
    ResourceId int not null auto_increment,
    ResourceType varchar (255),
    Amount int,
    Generation int not null,
    UpdatedAt bigint,
    KingdomId int,
    primary key (ResourceId) 
);
insert into Resources(ResourceId,ResourceType,Amount,Generation,UpdatedAt, KingdomId)
Values (1,'Food',1,1,1,1),
(2,'Gold',1,1,1,1);
