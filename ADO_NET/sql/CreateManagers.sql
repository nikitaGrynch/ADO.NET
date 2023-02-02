CREATE TABLE Managers (
	Id			CHAR(36) NOT NULL PRIMARY KEY,
	Surname		VARCHAR(50) NOT NULL,
	Name		VARCHAR(50) NOT NULL,
	Secname		VARCHAR(50) NOT NULL,
	Id_main_dep CHAR(36) NOT NULL ,
	Id_sec_dep	CHAR(36) ,
	Id_chief	CHAR(36),

	FOREIGN KEY( Id_main_dep ) REFERENCES Departments( Id ),
	FOREIGN KEY( Id_sec_dep ) REFERENCES Departments( Id )
)  ENGINE = INNODB DEFAULT CHARSET = UTF8 ;