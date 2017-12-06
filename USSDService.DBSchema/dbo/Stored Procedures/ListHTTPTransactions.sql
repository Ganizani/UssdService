CREATE PROCEDURE [dbo].[ListHTTPTransactions]
		 @MobileNumber  varchar(20) = null
		,@ClientReference varchar(50) = null 
AS

BEGIN

	Select 
	 [T].Id
	,[T].USSDCampaignId
	,[T].USSDTransactionId
	,[T].MenuId
	,[T].MobileNumber
	,[T].ResolveURL
	,[T].ClientReference
	,[T].ClientMessage
	,[T].ClientWebResponse
	,[T].CreatedDate
	,[T].CreatedByUserId
	,[C].[Description] [CampaignName]
	,[M].Name [MenuName]

	
	From [dbo].[HTTPTransactions] T
	inner join [dbo].[USSDCampaigns] C on T.USSDCampaignId = C.Id	
	inner join [dbo].[USSDTransactions] UT on T.USSDTransactionId = UT.Id
	inner join [dbo].[Menus] M on T.MenuId = M.Id	

	Where 

			(@MobileNumber is null or [T].[MobileNumber] = @MobileNumber)
		AND (@ClientReference is null or [T].[ClientReference] = @ClientReference)

	Order by [T].[Id]
END
