CREATE TABLE [dbo].[VeterinarianFacilityChannel]
(
	[VeterinarianFacilityChannelId] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY,
	[VeterinarianFacilityId] INT NOT NULL,
	ChannelTypeId INT NOT NULL,
	ChannelValue NVARCHAR (24)  NOT NULL,
    [CreateDateUtc]       DATETIME       NOT NULL,
    [ModifiedDateUtc]     DATETIME       NOT NULL,
    [Deleted]          BIT            NOT NULL, 
    CONSTRAINT [FK_VeterinarianFacilityChannel_ChannelType] FOREIGN KEY (ChannelTypeId) REFERENCES dbo.ChannelType (ChannelTypeId), 
    CONSTRAINT [FK_VeterinarianFacilityChannel_ToTable] FOREIGN KEY (VeterinarianFacilityId) REFERENCES dbo.VeterinarianFacility (VeterinarianFacilityId)

)
