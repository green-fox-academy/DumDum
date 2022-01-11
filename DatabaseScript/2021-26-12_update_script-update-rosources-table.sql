use DumDum;
update  Resources set Amount = 100 where resources.kingdomId = 1;
insert into Resources(ResourceId,ResourceType,Amount,Generation,UpdatedAt, KingdomId)
Values 
(3,'Food',100,1,1,2),
(4,'Gold',100,1,1,2),
(5,'Food',100,1,1,3),
(6,'Gold',100,1,1,3),
(7,'Food',100,1,1,4),
(8,'Gold',100,1,1,4),
(9,'Food',100,1,1,5),
(10,'Gold',100,1,1,5);
