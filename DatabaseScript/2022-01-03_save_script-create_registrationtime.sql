USE dumdum;

CREATE TABLE registrationtime (
  LastChangeId int NOT NULL AUTO_INCREMENT,
  RegistrationTime bigint,
  PlayerId int NOT NULL,
  PRIMARY KEY (LastChangeId)
  );
