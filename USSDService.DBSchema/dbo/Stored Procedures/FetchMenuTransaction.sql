
CREATE PROCEDURE [dbo].[FetchMenuTransaction]
	(
		 @Id Bigint 
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
    Where [Id] = @Id

End