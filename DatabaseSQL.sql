-- Select * from * --
select * from Users;
select * from Campus;
select * from Schools;
select * from Teachers;
select * from Lecturers;
select * from Students;
select * from StudentSchools;
select * from Gradings;
select * from Planning;
select * from Execution;
select * from Overall;
select * from Commentary;

-- Drop Table * --
drop table Users;
drop table Campus;
drop table Schools;
drop table Teachers;
drop table Lecturers;
drop table Students;
drop table StudentSchools;
drop table Gradings;
drop table Planning;
drop table Execution;
drop table Overall;
drop table Commentary;

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

insert into Users
values ('admin@studentteacher.co.za', 'etair0mMHrbojg90EdvO0g==', 'Admin', 'Admin');

-- VC Campus Table Creation
create table dbo.Campus (
    Code varchar(8) not null,
    Province varchar(255) not null,
    City varchar(255) not null,
    Name varchar(255) not null,

    PRIMARY KEY (Code)
)
-- insert into Campus 
-- values ('PORT1234', 'Eatsern Cape', 'Port Elizabeth', 'VCPE');

-- School Table Creation
create table Schools (
    Code varchar(8) not null,
    Name varchar(255) not null,
    Campus varchar(8) not null,
    Quintile varchar(2) not null,

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
    Qualification varchar(255),
    YearOfStudy  int not null,
    Campus varchar(8) not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (Campus) REFERENCES Campus(Code)
)

-- StudentSchools Table Creation --
create table StudentSchools (
    id int IDENTITY(1, 1) not null,
    Student VARCHAR(10) not null,
    School varchar(8) not null,
    PlacementYear int not null,

    PRIMARY KEY (id),
    FOREIGN KEY(Student) REFERENCES dbo.Students(Number),
    FOREIGN KEY(School) REFERENCES dbo.Schools(Code)
)

--Grading Table Creation--
create table dbo.Gradings (
    Number int IDENTITY(1, 1) not null,
    Student VARCHAR(10) not null,
    Teacher varchar(6) not null,
    YearOfStudy int not null,
    Date date not null,
    Grade int not null,
    Topic varchar(255) not null,
    Subject varchar(255) not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (Teacher) REFERENCES Teachers(Number),
    FOREIGN KEY (Student) REFERENCES Students(Number)
)

--Section A: Planning--
create table Planning (
    Number int IDENTITY(1, 1) not null,
    --CRITERIA START--
    SectionAtoD int not null,
    SectionE int not null,
    --CRITERIA END--
    GradingNumber int not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (GradingNumber) REFERENCES Gradings(Number)
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
    GradingNumber int not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (GradingNumber) REFERENCES Gradings(Number)
)

--Section C: Overall--
create table Overall (
    Number int IDENTITY(1, 1) not null,
    --CRITERIA START--
    Presence int not null,
    Environment int not null,
    --CRITERIA END--
    GradingNumber int not null,

    PRIMARY KEY (Number),
    FOREIGN KEY (GradingNumber) REFERENCES Gradings(Number)
)

--Commentary Table Creation--
create table Commentary (
    id int IDENTITY(1, 1) not null,
    Criteria varchar(255) not null, --SectionAtoD/SectionE/Intro/Teaching/Closure/Assessment/Presence/Environment--
    GradingNumber int not null,
    Comment varchar(max) not null,

    PRIMARY KEY (id),
    FOREIGN KEY (GradingNumber) REFERENCES Gradings(Number)
)

-- Currently Unsused Table Ides --
--Module Table Creation--

-- create table dbo.Modules (
--     Number varchar(8) not null,
--     Name VARCHAR(Max) not null,

--     PRIMARY KEY (Number)
-- )

-- StudentModules Table Creation

-- create table dbo.StudentModules (
--     id int IDENTITY(1, 1) not null,
--     Student VARCHAR(10) not null,
--     Module varchar(8) not null,

--     PRIMARY KEY (id),
--     FOREIGN KEY (Student) REFERENCES dbo.Students(Number),
--     FOREIGN KEY (Module) REFERENCES dbo.Modules(Number)
-- )

-- LecturerModules Table Creation

-- create table dbo.LecturerModules (
--     id int IDENTITY(1, 1) not null,
--     Lecturer varchar(6) not null,
--     Module varchar(8) not null,

--     PRIMARY KEY (id),
--     FOREIGN KEY (Lecturer) REFERENCES dbo.Lecturers(Number),
--     FOREIGN KEY (Module) REFERENCES dbo.Modules(Number)
-- )