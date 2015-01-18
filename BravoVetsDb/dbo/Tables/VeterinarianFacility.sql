CREATE TABLE [dbo].[VeterinarianFacility] (
    [VeterinarianFacilityId] INT            IDENTITY (1, 1) NOT NULL,
    [VeterinarianId]         INT            NOT NULL,
    [FacilityName]			 NVARCHAR (256) NOT NULL,
    [StreetAddress1]         NVARCHAR (256) NULL,
    [StreetAddress2]         NVARCHAR (256) NULL,
    [City]                   NVARCHAR (100) NULL,
	StateProvince			 NVARCHAR (24) NULL,
    [Country]                NVARCHAR (100) NULL,
    [PostalCode]             NVARCHAR (24)  NULL,
    [PrimaryPhoneNumber]            NVARCHAR (24)  NULL,
    [SecondaryPhoneNumber]            NVARCHAR (24)  NULL,
    [EmailAddress]           NVARCHAR (128) NULL,
    FaxNumber           NVARCHAR (24) NULL,
    [CreateDateUtc]             DATETIME       NOT NULL,
    [ModifiedDateUtc]           DATETIME       NOT NULL,
    [Deleted]                BIT            NOT NULL DEFAULT 0,
	IsEditable				 BIT			NOT NULL DEFAULT 0,
    CONSTRAINT [PK_VeterinarianFacility] PRIMARY KEY CLUSTERED ([VeterinarianFacilityId] ASC),
    CONSTRAINT [FK_VeterinarianFacility_Veterinarian] FOREIGN KEY ([VeterinarianId]) REFERENCES [dbo].[Veterinarian] ([VeterinarianId])
);

