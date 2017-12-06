
CREATE PROCEDURE [dbo].[UpdateMenuTransaction]
	(
		 @Id Bigint 
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
	Update  [dbo].[MenuTransactions]
		Set
		 USSDMenuId				= @USSDMenuId			
		,USSDCampaignId			= @USSDCampaignId		
		,USSDTransactionId		= @USSDTransactionId		
		,MobileNetworkID		= @MobileNetworkID		
		,MobileNumber			= @MobileNumber			
		,EnteredSelectionValue	= @EnteredSelectionValue 
		,MenuSection			= @MenuSection			
		,CreateDate				= @CreateDate			
	
	Where [Id] =  @Id
End