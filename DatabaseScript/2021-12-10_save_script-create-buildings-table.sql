use dumdum;
create table Buildings(
	BuildingId int not null auto_increment,
    BuildingType varchar (255),
    Level int,
    StartedAt bigint,
    FinishedAt bigint,
    KingdomId int not null,
    primary key (BuildingId) 
    );
    
    insert into Buildings(BuildingId,BuildingType,Level,StartedAt,FinishedAt,KingdomId)
    Values ("1","Townhall","1","1","1","1"),
    ("2","Farm","1","1","1","1"),
    ("3","Mine","1","1","1","1"),
    ("4","Townhall","1","1","1","2"),
    ("5","Farm","1","1","1","2"),
    ("6","Mine","1","1","1","2"),
    ("7","Townhall","1","1","1","3"),
    ("8","Farm","1","1","1","3"),
    ("9","Mine","1","1","1","3"),
    ("10","Townhall","1","1","1","4"),
    ("11","Farm","1","1","1","4"),
    ("12","Mine","1","1","1","4"),
    ("13","Townhall","1","1","1","5"),
    ("14","Farm","1","1","1","5"),
    ("15","Mine","1","1","1","5")
    ;
    
    
    
    