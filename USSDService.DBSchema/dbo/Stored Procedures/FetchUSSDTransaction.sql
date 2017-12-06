CREATE PROCEDURE [dbo].[FetchUSSDTransaction]
	@Id Bigint 
AS

BEGIN

	Select 
	 [T].[Id]
	,[T].[USSDCampaignId]	
	,[T].[Message]			
	,[T].[MobileNumber]		
	,[T].[CreatedDate]		
	,[T].[CreatedByUserId]	
	,[T].[USSDNumberId]		
	,[T].[SessionGuid]		
	,[T].[MobileNetworkId]
	,[C].[Description] [CampaignName]
	,[N].[Number]
	,[MN].MobileNetworkName [MobileNetwork]

	
	From [dbo].[USSDTransactions] T
	inner join [dbo].[USSDCampaigns] C on T.USSDCampaignId = C.Id	
	inner join [dbo].[UssdNumbers] N on T.USSDNumberId = N.Id
	inner join [dbo].[MobileNetworks] MN on T.MobileNetworkId = MN.Id	

	Where [T].[Id] = @Id
END
