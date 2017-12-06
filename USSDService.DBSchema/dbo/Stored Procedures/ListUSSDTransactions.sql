CREATE PROCEDURE [dbo].[ListUSSDTransactions]
		 @Number Bigint =null
		,@MobileNumber  varchar(20) = null
		,@SessionGuid varchar(50) = null 
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

	Where 
			(@Number is null or [N].[Number] = @Number)
		AND (@MobileNumber is null or [T].[MobileNumber] = @MobileNumber)
		AND (@SessionGuid is null or [T].[SessionGuid] = @SessionGuid)

	Order by [T].[Id]
END
