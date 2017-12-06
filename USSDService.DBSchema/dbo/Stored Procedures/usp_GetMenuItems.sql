
Create Procedure [dbo].[usp_GetMenuItems]
	(
		 @USSDCampaignId int
		,@USSDNumber varchar(20) = null	
	)
AS

SELECT TOP 1000 
	   [M].[Id] [MenuId]
      ,[M].[UssdCampaignId]
      ,[M].[Name] 
      ,[M].[ParentMenuId]
      ,[M].[ReturnMenuId]
      ,[M].[ReturnValue]
      ,[M].[DisplayData]
      ,[M].[ShowBack]
      ,[M].[OrderNumber]
	  ,[C].[Name]   as  [CampaignName]          
	  ,[C].[Description]        
	  ,[C].[ProcessorType]      
	  ,[C].[CampaignStartDate]  
	  ,[C].[CampaignEndDate]    
	  ,[C].[CreatedDate]        
	  ,[C].[CreatedByUserId]    
	  ,[C].[USSDNumberId]       
	  ,[C].[BackButton]         
	  ,[C].[CommunicationTypeId]
	  ,[C].[WaitForHTTPResponse]
	  ,[C].[IsBillable]         
	  ,[C].[BillAmount]
	  ,[C].[SendHTTPResponseBySMS]          
	  ,[CT].CommunicationText
	  
  FROM [dbo].[USSDMenus] M
    inner join [dbo].[USSDCampaigns]  C on M.UssdCampaignId = C.Id
	inner join [dbo].[CommunicationTypes] CT on C.CommunicationTypeId = CT.Id 
	inner join [dbo].[UssdNumbers] N on C.USSDNumberId = N.Id
	Where [M].[UssdCampaignId] = @USSDCampaignId
			And (@USSDNumber is null or [N].[Number] = @USSDNumber)
