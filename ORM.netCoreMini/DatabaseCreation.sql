CREATE DATABASE OrmTest
GO
USE OrmTest
GO
CREATE TABLE Projects
(
    Id INT IDENTITY PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    RemainingTime INT NOT NULL
)

CREATE TABLE Departments
(
    Id INT IDENTITY PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
)

CREATE TABLE Employees
(
    Id INT IDENTITY PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    IsEmployed BIT NOT NULL,
    DepartmentId INT 
    CONSTRAINT FK_Working_At_Department FOREIGN KEY
    REFERENCES Departments(Id)
)


CREATE TABLE EmployeesWorkingOnProjects
(
    ProjectId INT NOT NULL
    CONSTRAINT FK_Employees_Projects 
    REFERENCES Project(Id),
    EmployeeId INT NOT NULL
    CONSTRAINT FK_Employees_Employee
    REFERENCES Employees(Id),
    CONSTRAINT PK_Projects_Emploeey
    PRIMARY KEY (ProjectId, EmployeeId)
)

GO
INSERT INTO OrmTest.dbo.Departments(Name) VALUES('Research');
INSERT INTO OrmTest.dbo.Departments(Name) VALUES('Development');
INSERT INTO OrmTest.dbo.Employees (FirstName, LastName, IsEmployed, DepartmentId) VALUES
('Peter', 'Pan', 1, 1),
('John', 'Snow', 0, 1),
('George', 'Newton', 1, 1),
('Maria', 'Wild', 1, 1);

INSERT INTO OrmTest.dbo.Projects (Name)
VALUES ("First Project"), ('Second Project');

INSERT INTO OrmTest.dbo.EmplyeesProjects (ProjectId, EmployeeId) VALUES
(1, 1),
(1, 3),
(2, 1),
(2, 2);
