
-- please run Part1 first!
-- adding 'Courses' and 'Course_has_students'


insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (1, 'Calculus 1', 1, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (2, 'Introduction to Programming', 1, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (3, 'Introduction to Computer Science', 1, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (4, 'Logic Design of Digital Systems', 1, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (5, 'Mathematics fro Computer Science', 1, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (6, 'Internet Techologies', 1, 1);

insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (7, 'Object-oriented programming', 2, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (8, 'Calculus 2', 2, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (9, 'Computer Architecture', 2, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (10, 'Discrete Mathematics', 2, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (11, 'Data Structures', 2, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (12, 'Applied Algebra', 2, 1);

insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (13, 'Object-oriented application development', 3, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (14, 'Compilers', 3, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (15, 'Operating Systems', 3, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (16, 'Mathematical programming', 3, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (17, 'Statistics and Probability', 3, 2);
insert into dbo.Courses (CourseId, Title, Semester) values (18, 'Applications of Graph Theory', 3);

insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (19, 'Algorithms', 4, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (20, 'Databases', 4, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (21, 'Computer Networks', 4, 2);
insert into dbo.Courses (CourseId, Title, Semester) values (22, 'Applied Combinatorics', 4);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (23, 'Information and Coding Theory', 4, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (24, 'Principles and Applications of Signals and Systems', 4, 2);

insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (25, 'Pattern Recognition', 5, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (26, 'Human-Computer Interaction', 5, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (27, 'Information Systems', 5, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (28, 'Database Management Systems', 5, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (29, 'Programming in Logic', 5, 2);
insert into dbo.Courses (CourseId, Title, Semester) values (30, 'Game theory and applications', 5);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (31, 'Learning Management Software', 5, 2);

insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (32, 'Artificial Intelligence and Expert Systems', 6, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (33, 'Software Engineering', 6, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (34, 'Data Analytics', 6, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (35, 'Bioinformatics', 6, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (36, 'Multimedia Systems', 6, 2);
insert into dbo.Courses (CourseId, Title, Semester) values (37, 'Parallel Computing', 6);

insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (38, 'Image Analysis', 7, 1);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (39, 'Introduction to Infromation Retrieval', 7, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (40, 'Virtual Reality', 7, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (41, 'Computational Geometry', 7, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (42, 'Computer Game Development Technologies', 7, 2);
insert into dbo.Courses (CourseId, Title, Semester, ProfessorId) values (43, 'Geographical Information Systems', 7, 2);
insert into dbo.Courses (CourseId, Title, Semester) values (44, 'Knowlegde Management', 7);

--insert into dbo.Students (studentId, Registration_number, Name, Surname, Department, userid) values (1,'19001','Jared', 'Ibbeson', 'IT', '31');

--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (1, 1, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (2, 2, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (3, 3, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (4, 4, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (5, 5, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (6, 6, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (7, 7, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (8, 8, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (9, 9, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (10, 10, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (11, 11, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (12, 12, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (13, 13, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (14, 14, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (15, 15, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (16, 16, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (17, 17, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (18, 18, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (19, 19, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (20, 20, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (21, 21, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (22, 22, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (23, 23, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (24, 24, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (25, 25, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (26, 26, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (27, 27, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (28, 28, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (29, 29, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (30, 30, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (31, 31, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (32, 32, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (33, 33, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (34, 35, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (36, 36, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (37, 37, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (38, 38, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (39, 39, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (40, 40, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (41, 41, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (42, 42, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (43, 43, 1, 5);
--insert into dbo.Course_has_students (GradeId, CourseId, StudentId, Grade) values (44, 44, 1, 5);