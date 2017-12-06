
CREATE PROCEDURE [dbo].[UpdateUSSDTransaction]
	(
	 @Id                    BIGINT     
    ,@USSDCampaignId        Int			  
    ,@Message		        NVARCHAR (159)
    ,@MobileNumber          NVARCHAR (20)  
    ,@CreatedDate           DATETIME       
    ,@CreatedByUserId       INT           
    ,@USSDNumberId          INT           
    ,@SessionGuid           UNIQUEIDENTIFIER
    ,@MobileNetworkId	    INT 
	)            
AS
Begin
	Update [dbo].[USSDTransactions]
	 set
		 [USSDCampaignId]	= @USSDCampaignId 
		,[Message]			= @Message		 
		,[MobileNumber]		= @MobileNumber   
		,[CreatedDate]		= @CreatedDate    
		,[CreatedByUserId]	= @CreatedByUserId
		,[USSDNumberId]		= @USSDNumberId   
		,[SessionGuid]		= @SessionGuid    
		,[MobileNetworkId]	= @MobileNetworkId
	Where [Id] =  @Id 
End
