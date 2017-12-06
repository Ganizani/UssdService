CREATE PROCEDURE [dbo].[FetchHTTPTransaction]
		 @Id int
		
AS

BEGIN

	Select top 1
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
	inner join [dbo].[USSDMenus] M on T.MenuId = M.Id	

	Where 
			[T].[Id] = @Id

END
