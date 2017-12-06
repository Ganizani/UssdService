
CREATE PROCEDURE [dbo].[ListMenuTransactions]
	(
		  @FilterUSSDMenuId BigInt = null
         ,@FilterUSSDCampaignId int = null
         ,@FilterUSSDTransactionId BigInt =  null
       
	)
AS
Begin
	Select top 1
		 Id
		,USSDMenuId
		,USSDCampaignId
		,USSDTransactionId
		,MobileNetworkID
		,MobileNumber
		,EnteredSelectionValue
		,MenuSection
		,CreateDate
	From [dbo].[MenuTransactions]
    Where 
			(@FilterUSSDCampaignId  is null or [USSDCampaignId] = @FilterUSSDCampaignId)
		AND	(@FilterUSSDMenuId is null or [USSDMenuId] = @FilterUSSDMenuId)
		AND (@FilterUSSDTransactionId is null or [USSDTransactionId] =@FilterUSSDTransactionId)

  order by [Id]
End