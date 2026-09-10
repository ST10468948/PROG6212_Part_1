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
    FinishTime TIME(0) NOT NULL, -- Storing race duration as hh:mm:ss
    Position INT NOT NULL CHECK (Position > 0),
    TotalFinishers INT NOT NULL CHECK (TotalFinishers > 0),
    RecordedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_Position_TotalFinishers CHECK (Position <= TotalFinishers)
);
GO

