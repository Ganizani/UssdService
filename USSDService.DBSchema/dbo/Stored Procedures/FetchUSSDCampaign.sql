CREATE PROCEDURE [dbo].[FetchUSSDCampaign]
	@Id int = 0
	
AS
Begin

	SELECT top 1 
	 C.[Id]                   
    ,C.[Name]                 
    ,C.[Description]          
    ,C.[ProcessorType]        
    ,C.[CampaignStartDate]    
    ,C.[CampaignEndDate]      
    ,C.[CreatedDate]          
    ,C.[CreatedByUserId]      
    ,C.[USSDNumberId]         
    ,C.[BackButton]           
    ,C.[CommunicationTypeId]  
    ,C.[WaitForHTTPResponse]  
    ,C.[IsBillable]           
    ,C.[BillAmount]           
    ,C.[SendHTTPResponseBySMS]
	,N.Number

 FROM [dbo].[USSDCampaigns] C
   inner join [dbo].[UssdNumbers] N On C.USSDNumberId = N.Id
 where 
	C.[Id] = @Id

 End