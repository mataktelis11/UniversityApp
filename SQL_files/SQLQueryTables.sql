-- *Tables file*
-- Create a new Database and execute this file to create the Tables.
-- After that, run 'data.sql' to add demo data to the tables

drop table if exists dbo.Course_has_students;
drop table if exists dbo.Courses;
drop table if exists dbo.Secretaries;
drop table if exists dbo.Professors;
drop table if exists dbo.Students;
drop table if exists dbo.Users;



CREATE TABLE dbo.Users  
(  
	userid int PRIMARY KEY,
	Username varchar(50) NOT NULL UNIQUE,
	Password varchar(70) NOT NULL,
	Role varchar(20) NOT NULL
);


CREATE TABLE dbo.Students
(
	studentId int PRIMARY KEY,
	Registration_number int NOT NULL UNIQUE,
	Name varchar(45) NOT NULL,
	Surname varchar(45) NOT NULL,
	Department varchar(45) NOT NULL,
	userid int UNIQUE,
	CONSTRAINT FK_Students_Users FOREIGN KEY (userid) REFERENCES Users(userid),
);


CREATE TABLE dbo.Secretaries
(
	SecretaryId int PRIMARY KEY,
	Name varchar(45) NOT NULL,
	Surname varchar(45) NOT NULL,
	Department varchar(45) NOT NULL,
	userid int UNIQUE,
	CONSTRAINT FK_Secretariess_Users FOREIGN KEY (userid) REFERENCES Users(userid),
);


CREATE TABLE dbo.Professors
(
	ProfessorId int PRIMARY KEY,
	AFM int NOT NULL UNIQUE,
	Name varchar(45) NOT NULL,
	Surname varchar(45) NOT NULL,
	Department varchar(45) NOT NULL,
	userid int UNIQUE,
	CONSTRAINT FK_Professors_Users FOREIGN KEY (userid) REFERENCES Users(userid),
);


CREATE TABLE dbo.Courses
(
	CourseId int PRIMARY KEY,
	Title varchar(60) NOT NULL,
	Semester int NOT NULL,
	ProfessorId int,
	CONSTRAINT FK_Courses_Professors FOREIGN KEY (ProfessorId) REFERENCES Professors(ProfessorId),
);


CREATE TABLE dbo.Course_has_students
(
	GradeId int PRIMARY KEY,
	CourseId int,
	StudentId int,
	Grade int,
	CONSTRAINT Course_has_students_Uq UNIQUE (CourseId, StudentId),
	CONSTRAINT FK_Courses_Grade FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
	CONSTRAINT FK_Students_Grade FOREIGN KEY (StudentId) REFERENCES Students(StudentId)
);

