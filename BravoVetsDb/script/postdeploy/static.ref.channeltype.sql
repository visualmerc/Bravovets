
SET IDENTITY_INSERT [dbo].[ChannelType] ON 

GO
INSERT [dbo].[ChannelType] ([ChannelTypeId], [ChannelTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES (1, N'BusinessPhone', CAST(0x0000A2D40102EAFB AS DateTime), CAST(0x0000A2D40102EAFB AS DateTime), 0)
GO
INSERT [dbo].[ChannelType] ([ChannelTypeId], [ChannelTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES (2, N'CellPhone', CAST(0x0000A2D40102EAFC AS DateTime), CAST(0x0000A2D40102EAFC AS DateTime), 0)
GO
INSERT [dbo].[ChannelType] ([ChannelTypeId], [ChannelTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES (3, N'Fax', CAST(0x0000A2D40102EB02 AS DateTime), CAST(0x0000A2D40102EB02 AS DateTime), 0)
GO
INSERT [dbo].[ChannelType] ([ChannelTypeId], [ChannelTypeName], [CreateDateUtc], [ModifiedDateUtc], [Deleted]) VALUES (4, N'Email', CAST(0x0000A2D40102EB03 AS DateTime), CAST(0x0000A2D40102EB03 AS DateTime), 0)
GO
SET IDENTITY_INSERT [dbo].[ChannelType] OFF
GO
