use DumDum;
DROP TABLE IF EXISTS `trooplevel`;
CREATE TABLE `trooplevel` (
  `TroopLevelId` int NOT NULL AUTO_INCREMENT,
  `Level` int DEFAULT NULL,
  `Cost` int DEFAULT NULL,
  `Attack` double DEFAULT NULL,
  `Defence` double DEFAULT NULL,
  `CarryCap` int DEFAULT NULL,
  `Consumption` int DEFAULT NULL,
  `Speed` double DEFAULT NULL,
  `SpecialSkills` int DEFAULT NULL,
  `HP` int DEFAULT NULL,
  `TroopTypeId` int DEFAULT NULL,
  PRIMARY KEY (`TroopLevelId`)
);

LOCK TABLES `trooplevel` WRITE;
INSERT INTO `trooplevel` VALUES (1,1,77,8.4,5.3,30,1,1,0,1,1),
(2,2,154,8.8,5.5,35,1,1.2,0,2,1),
(3,3,231,9.2,5.8,40,1,1.4,0,3,1),
(4,4,308,9.6,6,45,2,1.6,0,4,1),
(5,5,385,10,6.3,50,2,1.8,0,5,1),
(6,1,77,5.3,8.4,30,1,0.8,0,1,2),
(7,2,154,5.5,8.8,35,1,1,0,2,2),
(8,3,231,5.8,9.2,40,1,1.2,0,3,2),
(9,4,308,6,9.6,45,2,1.4,0,4,2),
(10,5,385,6.3,10,50,2,1.6,0,5,2),
(11,1,121,15.8,10.5,0,2,1.6,0,3,3),
(12,2,242,16.5,11,0,2,1.8,0,4,3),
(13,3,363,17.3,11.5,0,2,2,0,5,3),
(14,4,484,18,12,0,3,2.2,0,6,3),
(15,5,605,18.8,12.5,0,3,2.4,0,7,3),
(16,1,121,4.2,3.2,0,2,2.5,0,3,4),
(17,2,242,4.4,3.3,0,2,2.7,0,4,4),
(18,3,363,4.6,3.4,0,2,2.9,0,5,4),
(19,4,484,4.8,3.6,0,3,3.1,0,6,4),
(20,5,605,5,3.8,0,3,3.3,0,7,4),
(21,1,1800,0,0,0,5,0.5,25,1,5);
UNLOCK TABLES;

