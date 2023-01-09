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


-- adding 'Users'
insert into dbo.Users (userid, Username, Password, Role) values(121, 'telis', '123', 'student');
insert into dbo.Users (userid, Username, Password, Role) values(453, 'lakis', '123', 'student');
insert into dbo.Users (userid, Username, Password, Role) values(677, 'alepis', '123', 'prof');
insert into dbo.Users (userid, Username, Password, Role) values(899 ,'manos', '123', 'prof');
insert into dbo.Users (userid, Username, Password, Role) values(999, 'jennie', '123', 'sec');

---- adding 'Students'
insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (1,12, 'Aristotelis', 'Matakias', 'IT', 121);
insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (2,13, 'Lakis', 'Mamalakis', 'IT', 453);

-- adding 'Professors'
insert into dbo.Professors (ProfessorId, AFM, Name, Surname, Department, userid) values (1, 100, 'Timmy', 'Alepis', 'IT', 677);
insert into dbo.Professors (ProfessorId, AFM, Name, Surname, Department, userid) values (2, 200, 'Chris', 'Manousopoulos', 'IT', 899);

-- adding 'Secretaries'
insert into dbo.Secretaries(SecretaryId, Name, Surname, Department, userid) values (1, 'Jennie', 'Sklaveniti', 'IT', 999);

-- adding 'Courses'
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (77, 'Image Prosessing', 3, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (50, 'Pattern Recognition', 6, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (67, 'Introduction to IR', 4, 2);

-- adding 'Course_has_students'
insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (1, 77, 1, 5);
insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (2, 67, 1, 5);
insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (3, 50, 1, 5);
insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (4, 50, 2, 10);


-- testing

select * from dbo.Users;

select * from dbo.Students;

