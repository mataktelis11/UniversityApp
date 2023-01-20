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


---- adding 'Users'
--insert into dbo.Users (userid, Username, Password, Role) values(121, 'telis', '123', 'Students');
--insert into dbo.Users (userid, Username, Password, Role) values(453, 'lakis', '123', 'Students');
--insert into dbo.Users (userid, Username, Password, Role) values(698, 'bazel', '123', 'Students');
--insert into dbo.Users (userid, Username, Password, Role) values(677, 'alepis', '123', 'Professors');
--insert into dbo.Users (userid, Username, Password, Role) values(899 ,'manos', '123', 'Professors');
--insert into dbo.Users (userid, Username, Password, Role) values(999, 'jennie', '123', 'Secretaries');

------ adding 'Students'
--insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (1,12, 'Aristotelis', 'Matakias', 'IT', 121);
--insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (2,13, 'Lakis', 'Mamalakis', 'IT', 453);
--insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (3,14, 'Vasilis', 'Gjata', 'IT', 698);

---- adding 'Professors'
--insert into dbo.Professors (ProfessorId, AFM, Name, Surname, Department, userid) values (1, 100, 'Timmy', 'Alepis', 'IT', 677);
--insert into dbo.Professors (ProfessorId, AFM, Name, Surname, Department, userid) values (2, 200, 'Chris', 'Manousopoulos', 'IT', 899);

---- adding 'Secretaries'
--insert into dbo.Secretaries(SecretaryId, Name, Surname, Department, userid) values (1, 'Jennie', 'Sklaveniti', 'IT', 999);

-- adding 'Courses'

--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (1, 'Calculus 1', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (2, 'Introduction to Programming', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (3, 'Introduction to Computer Science', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (4, 'Logic Design of Digital Systems', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (5, 'Mathematics fro Computer Science', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (6, 'Internet Techologies', 7, 1);

--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (7, 'Object-oriented programming', 2, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (8, 'Calculus 2', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (9, 'Computer Architecture', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (10, 'Discrete Mathematics', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (11, 'Data Structures', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (12, 'Applied Algebra', 7, 1);

--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (13, 'Object-oriented application development', 2, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (14, 'Compilers', 3, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (15, 'Operating Systems', 2, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (16, 'Mathematical programming', 2, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (17, 'Statistics and Probability', 2, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (18, 'Applications of Graph Theory', 2, 2);

--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (19, 'Algorithms', 4, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (20, 'Databases', 4, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (21, 'Computer Networks', 4, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (22, 'Applied Combinatorics', 4, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (23, 'Information and Coding Theory', 4, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (24, 'Principles and Applications of Signals and Systems', 4, 2);

--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (25, 'Pattern Recognition', 6, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (26, 'Human-Computer Interaction', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (27, 'Information Systems', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (28, 'Database Management Systems', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (29, 'Programming in Logic', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (30, 'Game theory and applications', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (31, 'Learning Management Software', 5, 2);

--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (32, 'Artificial Intelligence and Expert Systems', 6, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (33, 'Software Engineering', 6, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (34, 'Data Analytics', 6, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (35, 'Bioinformatics', 6, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (36, 'Multimedia Systems', 6, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (37, 'Parallel Computing', 6, 2);

--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (38, 'Image Analysis', 7, 1);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (39, 'Introduction to Infromation Retrieval', 4, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (40, 'Virtual Reality', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (41, 'Computational Geometry', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (42, 'Computer Game Development Technologies', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (43, 'Geographical Information Systems', 5, 2);
--insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (44, 'Knowlegde Management', 5, 2);



---- adding 'Course_has_students'
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (1, 77, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (2, 67, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (3, 50, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (4, 50, 2, 10);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (5, 77, 3,1);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (6, 50, 3, 6);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (7, 69, 3, 10);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (8, 70, 3, 9);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (9, 71, 3, 10);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (10, 72, 3, 10);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (11, 73, 3, 10);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId) values (12, 74, 3);


-- testing

--select * from dbo.Users;

--select * from dbo.Students;
--select * from Courses;
--select * from Course_has_students;
