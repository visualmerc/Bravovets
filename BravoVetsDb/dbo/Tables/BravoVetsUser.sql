CREATE TABLE [dbo].[BravoVetsUser] (
    [BravoVetsUserId] INT              IDENTITY (1, 1) NOT NULL,
    [VeterinarianId]  INT              NOT NULL,
    [MerckId]         INT NOT NULL,
    [Email]           NVARCHAR (255)   NULL,
    [FirstName]       NVARCHAR (100)   NOT NULL,
    [Lastname]        NVARCHAR (100)   NOT NULL,
    [BravoVetsCountryId]      INT              NOT NULL,
	CultureName	   NVARCHAR(6)	  NOT NULL,
    [RepContactId]    INT              NULL,
    [EmailOptIn]      BIT              CONSTRAINT [DF_BravoVetsUser_EmailOptIn] DEFAULT ((0)) NOT NULL,
    [AcceptedTandC]   BIT              CONSTRAINT [DF_BravoVetsUser_AcceptedTandC] DEFAULT ((0)) NOT NULL,
    [BravoVetsStatusId]        INT              CONSTRAINT [DF_BravoVetsUser_StatusId] DEFAULT ((1)) NOT NULL,
    [CreateDateUtc]      DATETIME         NOT NULL,
    [ModifiedDateUtc]    DATETIME         NOT NULL,
    [Deleted]         BIT              CONSTRAINT [DF_BravoVetsUser_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_BravoVetsUser] PRIMARY KEY CLUSTERED ([BravoVetsUserId] ASC),
    CONSTRAINT [FK_BravoVetsUser_Veterinarian] FOREIGN KEY ([VeterinarianId]) REFERENCES [dbo].[Veterinarian] ([VeterinarianId]), 
    CONSTRAINT [FK_BravoVetsUser_BravoVetsCountry] FOREIGN KEY (BravoVetsCountryId) REFERENCES dbo.BravoVetsCountry(BravoVetsCountryId), 
    CONSTRAINT [FK_BravoVetsUser_Status] FOREIGN KEY (BravoVetsStatusId) REFERENCES dbo.BravoVetsStatus(BravoVetsStatusId)
);


GO

CREATE UNIQUE INDEX [IX_BravoVetsUser_MerckId] ON [dbo].[BravoVetsUser] ([MerckId])
