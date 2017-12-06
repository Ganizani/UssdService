
CREATE PROCEDURE [dbo].[InsertMenuTransaction]
	(
		 @Id Bigint output
		,@USSDMenuId			int
		,@USSDCampaignId		int
		,@USSDTransactionId		bigint 
		,@MobileNetworkID		int 
		,@MobileNumber			varchar(50)
		,@EnteredSelectionValue varchar(159)
		,@MenuSection			int
		,@CreateDate			Datetime
	)
AS
Begin
	Insert into  [dbo].[MenuTransactions]
	(
		 USSDMenuId
		,USSDCampaignId
		,USSDTransactionId
		,MobileNetworkID
		,MobileNumber
		,EnteredSelectionValue
		,MenuSection
		,CreateDate
	)
	Values
	(
		@USSDMenuId			
		,@USSDCampaignId		
		,@USSDTransactionId		
		,@MobileNetworkID		
		,@MobileNumber			
		,@EnteredSelectionValue 
		,@MenuSection			
		,@CreateDate			

	)

	Set @Id = SCOPE_IDENTITY()
End