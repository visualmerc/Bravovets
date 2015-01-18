CREATE TABLE [dbo].[Contact] (
    [ContactId]      INT            IDENTITY (1, 1) NOT NULL,
    [VeterinarianId] INT            NOT NULL,
    [FirstName]      NVARCHAR (100) NOT NULL,
    [Lastname]       NVARCHAR (100) NOT NULL,
    [Email]          NVARCHAR (255) NULL,
    [CreateDateUtc]     DATETIME       NOT NULL,
    [ModifiedDateUtc]   DATETIME       NOT NULL,
    [Deleted]        BIT            CONSTRAINT [DF_Contact_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([ContactId] ASC),
    CONSTRAINT [FK_Contact_Veterinarian] FOREIGN KEY ([VeterinarianId]) REFERENCES [dbo].[Veterinarian] ([VeterinarianId])
);

