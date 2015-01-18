CREATE TABLE [dbo].[QueueContentDeliveryLog]
(
	[QueueContentDeliveryLogId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	DeliverySessionId UNIQUEIDENTIFIER NOT NULL,
	DeliverySessionStartTimeUtc DATETIME NOT NULL,
	[QueueContentId] INT NOT NULL,
	WasDelivered BIT NOT NULL DEFAULT 0,
	DeliverySessionTimeUtc DATETIME NULL,
	PublishError NVARCHAR(512) NULL
)
