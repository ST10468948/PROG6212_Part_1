CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO


CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE
);
GO


CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    
    RoleID INT NOT NULL FOREIGN KEY REFERENCES Roles(RoleID),
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    ProfilePicUrl VARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO


CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    EventName VARCHAR(150) NOT NULL,
    Description VARCHAR(MAX) NOT NULL,
    EventDate DATETIME NOT NULL,
    Location VARCHAR(200) NOT NULL,
    DistanceKm DECIMAL(6,2) NOT NULL CHECK (DistanceKm > 0),
    EventType VARCHAR(50) NOT NULL CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    BannerUrl VARCHAR(500) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO


CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL FOREIGN KEY REFERENCES Events(EventID) ON DELETE CASCADE,
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NULL
);
GO

CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    EventID INT NOT NULL FOREIGN KEY REFERENCES Events(EventID),
    CategoryID INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryID),
    EnrolmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(30) NOT NULL DEFAULT 'Confirmed' CHECK (Status IN ('Confirmed', 'Pending', 'Cancelled')),
    CONSTRAINT UQ_Participant_Event UNIQUE (ParticipantID, EventID)
);
GO


CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Enrolments(EnrolmentID) ON DELETE CASCADE,
    ParticipantID INT NOT NULL FOREIGN KEY REFERENCES Users(UserID),
    FinishTime TIME(0) NOT NULL,
    Position INT NOT NULL CHECK (Position > 0),
    TotalFinishers INT NOT NULL CHECK (TotalFinishers > 0),
    RecordedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_Position_TotalFinishers CHECK (Position <= TotalFinishers)
);
GO

INSERT INTO Roles (RoleName) 
VALUES ('Organiser'), ('Participant');


INSERT INTO Users (RoleID, FullName, Email, PasswordHash, ProfilePicUrl) 
VALUES
(1, 'Seidzo Enge', 'sipho.events@raceday.co.za', 'dummy_hash_1', 'https://images.raceday.local/profiles/seidzo.jpg'),
(1, 'Thendo Mbo', 'ansie.m@capeathletics.org.za', 'dummy_hash_2', 'https://images.raceday.local/profiles/thando.jpg'),
(2, 'Khanyi Molele', 'thabo.molefe@gmail.com', 'dummy_hash_3', 'https://images.raceday.local/profiles/khanyi.jpg'),
(2, 'Liz Muller', 'liezl.vanzyl@outlook.com', 'dummy_hash_4', 'https://images.raceday.local/profiles/liz.jpg');


INSERT INTO Events (OrganiserID, EventName, Description, EventDate, Location, DistanceKm, EventType, BannerUrl) 
VALUES
(1, 'Soweto 10k Community Challenge', 'Scenic township road run highlighting historical landmarks in Soweto.', '2026-10-15 06:30:00', 'Soweto, Johannesburg', 10.00, 'Run', 'https://images.raceday.local/banners/soweto10k.jpg'),
(1, 'Cape Peninsula Cycle Classic', 'Coastal road cycling challenge around the iconic False Bay scenic routes.', '2026-11-20 06:00:00', 'Simon''s Town, Cape Town', 42.50, 'Cycle', 'https://images.raceday.local/banners/peninsula_cycle.jpg'),
(2, 'Durban Promenade Sunrise Walk', 'Family-friendly morning fitness walk along the Durban beachfront promenade.', '2026-12-05 07:00:00', 'North Beach, Durban', 5.00, 'Walk', 'https://images.raceday.local/banners/durban_walk.jpg');


INSERT INTO Categories (EventID, CategoryName, Description) 
VALUES
(1, 'Open Senior (10km)', 'Ages 20 to 39 competitive division'),
(1, 'Masters (10km)', 'Ages 40+ competitive division'),
(2, 'Elite Road (42.5km)', 'Licensed competitive road cycling'),
(2, 'Fun Ride (42.5km)', 'Recreational non-seeded category'),
(3, 'General Public (5km)', 'All ages welcome fitness walk');


INSERT INTO Enrolments (ParticipantID, EventID, CategoryID, Status) 
VALUES
(3, 1, 1, 'Confirmed'),
(4, 1, 1, 'Confirmed'),
(3, 2, 3, 'Confirmed'),
(4, 3, 5, 'Confirmed');


INSERT INTO Results (EnrolmentID, ParticipantID, FinishTime, Position, TotalFinishers) 
VALUES
(1, 3, '00:41:18', 14, 210),
(2, 4, '00:48:05', 47, 210);

SELECT 
    e.EventName, 
    u.FullName AS Participant, 
    c.CategoryName, 
    en.Status,
    r.FinishTime, 
    r.Position,
    r.TotalFinishers
FROM Enrolments en
INNER JOIN Events e ON en.EventID = e.EventID
INNER JOIN Users u ON en.ParticipantID = u.UserID
INNER JOIN Categories c ON en.CategoryID = c.CategoryID
LEFT JOIN Results r ON en.EnrolmentID = r.EnrolmentID;
GO
