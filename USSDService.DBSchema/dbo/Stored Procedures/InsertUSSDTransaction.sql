CREATE PROCEDURE [dbo].[InsertUSSDTransaction]
    @Id                    BIGINT          output
    ,@USSDCampaignId        Int			  
    ,@Message		        NVARCHAR (159)
    ,@MobileNumber          NVARCHAR (20)  
    ,@CreatedDate           DATETIME       
    ,@CreatedByUserId       INT           
    ,@USSDNumberId          INT           
    ,@SessionGuid           UNIQUEIDENTIFIER
    ,@MobileNetworkId	    INT             
AS
Begin
	Insert into  [dbo].[USSDTransactions]
	(
		[USSDCampaignId]
		,[Message]
		,[MobileNumber]
		,[CreatedDate]
		,[CreatedByUserId]
		,[USSDNumberId]
		,[SessionGuid]
		,[MobileNetworkId]
	)
	Values
	(
		 @USSDCampaignId 
		,@Message		 
		,@MobileNumber   
		,@CreatedDate    
		,@CreatedByUserId
		,@USSDNumberId   
		,@SessionGuid    
		,@MobileNetworkId

	)

	Set @Id = SCOPE_IDENTITY()
End