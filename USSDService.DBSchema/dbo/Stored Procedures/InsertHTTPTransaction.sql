CREATE PROCEDURE [dbo].[InsertHTTPTransaction]
	 @Id					bigint output
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
	Insert into [dbo].[HTTPTransactions]
	 (
		 [USSDCampaignId]		
		,[USSDTransactionId]	
		,[MenuId]				
		,[MobileNumber]			
		,[ResolveURL]			
		,[ClientReference]		
		,[ClientMessage]		
		,[ClientWebResponse]	
		,[CreatedDate]			
		,[CreatedByUserId]		
		,[Errors]
	
	 )
	 Values
	 (
		 @USSDCampaignId	
		,@USSDTransactionId	
		,@MenuId			
		,@MobileNumber		
		,@ResolveURL		
		,@ClientReference	
		,@ClientMessage		
		,@ClientWebResponse	
		,@CreatedDate		
		,@CreatedByUserId		
		,@Errors
	)
	 Set  @Id = SCOPE_IDENTITY() 
End
