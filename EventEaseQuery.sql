-- DATABASE CREATION
USE master
IF EXISTS (SELECT * FROM sys.databases WHERE name='EventEaseDB')
DROP DATABASE EventEaseDB
CREATE DATABASE EventEaseDB

USE EventEaseDB;

-- Table Creation

CREATE TABLE Venue(
venueID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
venueName VARCHAR(250) NOT NULL,
location VARCHAR(250) NOT NULL,
capacity VARCHAR(250) NOT NULL,
imageurl VARCHAR(250) NOT NULL
);



CREATE TABLE Events(
eventsID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
eventName VARCHAR(250) NOT NULL,
eventDate VARCHAR(250) NOT NULL,
description VARCHAR(500) NOT NULL,
venueID INT NOT NULL,
);



ALTER TABLE Events
ADD CONSTRAINT FK_venueID FOREIGN KEY (venueID) REFERENCES Venue(venueID);


CREATE TABLE Bookings
(
bookingsid INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
eventsid INT,
venueid INT, 
bookingDate VARCHAR(250)
);

ALTER TABLE Bookings
ADD CONSTRAINT FK_venueID_Bookings FOREIGN KEY (venueID) REFERENCES Venue(venueID);

ALTER TABLE Bookings
ADD CONSTRAINT FK_eventsID FOREIGN KEY (eventsID) REFERENCES Events(eventsID);

ALTER TABLE Bookings
ALTER COLUMN bookingDate Date;

Alter Table Events
ALTER COLUMN eventDate Date;

-- Creation of EventType

Create table EventsType
(
eventsTypeID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
eventsTypeName varchar(250) NOT NULL
);


Alter Table Events     -- adding new column for events type foreign key
Add eventsTypeID int NOT NULL;

Alter table Events  -- making eventsTypeID a foreaign key of Events
add constraint FK_eventsTypeID foreign key (eventsTypeID) references EventsType(eventsTypeID);


Alter Table Bookings     -- adding new column for bookings type foreign key
Add eventsTypeID INT ;

ALTER TABLE Bookings
   ADD CONSTRAINT eventsTypeID 
   FOREIGN KEY (eventsTypeID)
   REFERENCES EventsType(eventsTypeID);


insert into EventsType  -- adding types of events
values 
('Sports'),
('Corporate'),
('Festival'), 
('Seminar'),
('Birthday'), 
('Workshop') ;

select * from EventsType; -- confirmation of eventstypes being there

