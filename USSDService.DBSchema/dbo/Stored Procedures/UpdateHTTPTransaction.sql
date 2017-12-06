CREATE PROCEDURE [dbo].[UpdateHTTPTransaction]
	 @Id					bigint
	,@USSDCampaignId		int
	,@USSDTransactionId		bigint
	,@MenuId				int
	,@MobileNumber			varchar(20)
	,@ResolveURL			varchar(200)
	,@ClientReference		varchar(50)
	,@ClientMessage			varchar(159)
	,@ClientWebResponse		varchar(800)
	,@Errors				nvarchar(max)
	,@CreatedDate			Datetime
	,@CreatedByUserId		int

AS

Begin
	Update [dbo].[HTTPTransactions]
	 set
		 [USSDCampaignId]		= @USSDCampaignId	
		,[USSDTransactionId]	= @USSDTransactionId	
		,[MenuId]				= @MenuId			
		,[MobileNumber]			= @MobileNumber		
		,[ResolveURL]			= @ResolveURL		
		,[ClientReference]		= @ClientReference	
		,[ClientMessage]		= @ClientMessage		
		,[ClientWebResponse]	= @ClientWebResponse	
		,[CreatedDate]			= @CreatedDate		
		,[CreatedByUserId]		= @CreatedByUserId	
		,[Errors]				= @Errors
	Where [Id] =  @Id 
End
