-- drop TABLE dbo.Execution;
-- drop TABLE dbo.Grading;
-- drop TABLE dbo.Lecturer;
-- drop TABLE dbo.Module;
-- drop TABLE dbo.Overall;
-- drop TABLE dbo.Planning;
-- drop TABLE dbo.Student;
-- drop TABLE dbo.Teacher;
-- drop TABLE dbo.[User];

select * from dbo.Users
select * from dbo.Teachers
select * from dbo.Schools
select * from dbo.Lecturers
select * from dbo.StudentModules

--User Table Creation--
create table dbo.Users (
    Email varchar(255) not NULL,
    Password varchar(max) not null,
    -- VC Admin, VC Lecturer, School, School Teacher
    Type varchar(max) not null,
    -- VC Admin, VC Lecturer, School, School Teacher, Superviser
    Role varchar(255) not null,

    PRIMARY KEY (Email)
)

-- VC Campus Table Creation
create table dbo.Campus (
    Code varchar(8) not null,
    Province varchar(255) not null,
    City varchar(255) not null,

    PRIMARY KEY (Code)
)

drop TABLE dbo.Schools
-- School Table Creation
create table Schools (
    Code varchar(8) not null,
    Name varchar(255) not null,
    Province varchar(255) not null,
    City varchar(255) not null,
    Campus varchar(8) not null,

    PRIMARY KEY (Code),
    FOREIGN KEY (Campus) REFERENCES dbo.Campus(Code)
)

--Teacher Table Creation--
create table dbo.Teachers (
    Number varchar(6) not null,
    FirstName varchar(max) not null,
    LastName varchar(max) not null,
    School varchar(8) not null,
    Email varchar (255) not null,

    PRIMARY KEY (Number),
    FOREIGN KEY(Email) REFERENCES dbo.Users(Email),
    FOREIGN KEY(School) REFERENCES dbo.Schools(Code)
)

--Lecturer Table Creation--
create table dbo.Lecturers (
    Number varchar(6) not null,
    FirstName varchar(MAX) not null,
    LastName VARCHAR(MAX) not null,
    Email varchar (255) not null,
    Campus varchar(8) not null,

    PRIMARY KEY (Number),
    FOREIGN KEY(Email) REFERENCES dbo.Users(Email),
    FOREIGN KEY(Campus) REFERENCES dbo.Campus(Code)
)

--Student Creation Table--
create table dbo.Students (
    Number VARCHAR(10) not null,
    FirstName varchar(max) not null,
    LastName VARCHAR(max) not null,

    PRIMARY KEY (Number)
)

--Module Table Creation--
create table dbo.Modules (
    Number varchar(8) not null,
    Name VARCHAR(Max) not null,

    PRIMARY KEY (Number)
)

-- StudentModules Table Creation
create table dbo.StudentModules (
    id int IDENTITY(1, 1) not null,
    Student VARCHAR(10) not null,
    Module varchar(8) not null,

    PRIMARY KEY (id),
    FOREIGN KEY (Student) REFERENCES dbo.Students(Number),
    FOREIGN KEY (Module) REFERENCES dbo.Modules(Number)
)

-- LecturerModules Table Creation
create table dbo.LecturerModules (
    id int IDENTITY(1, 1) not null,
    Lecturer varchar(6) not null,
    Module varchar(8) not null,

    PRIMARY KEY (id),
    FOREIGN KEY (Lecturer) REFERENCES dbo.Lecturers(Number),
    FOREIGN KEY (Module) REFERENCES dbo.Modules(Number)
)

-- StudentSchools Table Creation
create table dbo.StudentSchools (
    id int IDENTITY(1, 1) not null,
    Student VARCHAR(10) not null,
    School varchar(8) not null,
)

--Grading Table Creation--
create table dbo.Gradings (
    Number int IDENTITY(1, 1) not null,
    StudentNumber VARCHAR(10) not null,
    TeacherNumber varchar(6) not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (TeacherNumber) REFERENCES Teacher(Number),
    FOREIGN KEY (StudentNumber) REFERENCES Student(Number)
)

--Section A: Planning--
create table Planning (
    Number int IDENTITY(1, 1) not null,
    --CRITERIA START--
    SectionAtoD int not null,
    SectionE int not null,
    --CRITERIA END--
    Commentary varchar(max) not null,
    GradingNumber int not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (GradingNumber) REFERENCES Grading(Number)
)

--Section B: Execution--
create table Execution (
    Number int IDENTITY(1, 1) not null,
    --CRITERIA START--
    Intro int not null,
    Teaching int not null,
    Closure int not null,
    Assessment int not null,
    --CRITERIA END--
    Commentary varchar(max) not null,
    GradingNumber int not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (GradingNumber) REFERENCES Grading(Number)
)

--Section C: Overall--
create table Overall (
    Number int IDENTITY(1, 1) not null,
    --CRITERIA START--
    Presence int not null,
    Environment int not null,
    --CRITERIA END--
    Commentary varchar(max) not null,
    GradingNumber int not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (GradingNumber) REFERENCES Grading(Number)
)