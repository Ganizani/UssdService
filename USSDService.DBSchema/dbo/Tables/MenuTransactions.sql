
CREATE TABLE [dbo].[MenuTransactions]
(
	  [Id] BIGINT NOT NULL IDENTITY(1,1)
	 ,[USSDMenuId] Int Not Null
	 ,[USSDCampaignId] Int Not Null
	 ,[USSDTransactionId] BigInt Not Null
	 ,[MobileNetworkID] int Not null
	 ,[MobileNumber] varchar (20)
	 ,[EnteredSelectionValue] varchar(159)
	 ,[CreateDate] Datetime
	 ,[MenuSection] int
	 ,Constraint [PK_MenuTransactions_Id] Primary Key([Id])
	 ,Constraint [FK_MenuTransactions_USSDMenuId] Foreign Key ([USSDMenuId]) References [dbo].[USSDMenus] ([Id])
	 ,Constraint [FK_MenuTransactions_USSDCampaignId] Foreign Key ([USSDCampaignId]) References [dbo].[USSDCampaigns] ([Id])
	 ,Constraint [FK_MenuTransactions_USSDTransactionId] Foreign Key ([USSDTransactionId]) References [dbo].[USSDTransactions] ([Id])
	 ,Constraint [FK_MenuTransactions_MobileNetworkID] Foreign Key ([MobileNetworkID]) References [dbo].[MobileNetworks] ([Id])
	  
	 )