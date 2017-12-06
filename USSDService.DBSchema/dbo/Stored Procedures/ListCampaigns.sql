CREATE PROCEDURE [dbo].[ListCampaigns]
	@FilterName  varchar(100) = null,
	@FilterDescription varchar(200) =  null, 
	@FilterUSSDNumber varchar(20)= null,
	@FilterUSSDNumberId int = null
AS
	SELECT 
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
 where (CampaignStartDate is not null and (CampaignEndDate is null or CampaignStartDate > GETDATE()))
		AND (@FilterName is null Or [name] = @FilterName)
		AND (@FilterDescription is null or [Description] like '%' +@FilterDescription + '%')		
		AND (@FilterUSSDNumber is null Or [Number] = @FilterUSSDNumber)
		AND (@FilterUSSDNumberId is null Or [USSDNumberId] = @FilterUSSDNumberId)
		
