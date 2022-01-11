use dumdum;
alter table dumdum.players add Email varchar(250), add IsVerified bool;
UPDATE players
SET Email = "nya@nya.nya", IsVerified = 1  WHERE PlayerId =1;
UPDATE players
SET Email = "mladen@nya.nya", IsVerified = 1  WHERE PlayerId =2;
UPDATE players
SET Email = "komin@nya.nya", IsVerified = 1  WHERE PlayerId =3;
UPDATE players
SET Email = "beef69@nya.nya", IsVerified = 1  WHERE PlayerId =4;
UPDATE players
SET Email = "pivko@nya.nya", IsVerified = 1  WHERE PlayerId =5;
