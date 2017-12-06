CREATE TABLE [dbo].[USSDMenus] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [UssdCampaignId] INT           NOT NULL,
    [Name]           NVARCHAR (188) NOT NULL,
    [ParentMenuId]   INT           DEFAULT ((0)) NOT NULL,
    [ReturnMenuId]   INT           DEFAULT ((0)) NOT NULL,
    [ReturnValue]    NVARCHAR (50) NULL,
    [DisplayData]    NVARCHAR (188) NULL,
    [ShowBack]       BIT           DEFAULT ((0)) NULL,
    [OrderNumber]    INT           DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Menus_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

