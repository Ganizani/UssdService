CREATE TABLE [dbo].[USSDCampaigns] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [Name]                  VARCHAR (250)   NOT NULL,
    [Description]           NVARCHAR (MAX)  NULL,
    [ProcessorType]         NVARCHAR (800)  NULL,
    [CampaignStartDate]     DATETIME        NULL,
    [CampaignEndDate]       DATETIME        NULL,
    [CreatedDate]           DATETIME        NOT NULL,
    [CreatedByUserId]       INT             NOT NULL,
    [USSDNumberId]          INT             NULL,
    [BackButton]            VARCHAR (50)    NULL,
    [CommunicationTypeId]   INT             NULL,
    [WaitForHTTPResponse]   BIT             DEFAULT ((0)) NOT NULL,
    [IsBillable]            BIT             DEFAULT ((0)) NOT NULL,
    [BillAmount]            NUMERIC (18, 2) NULL,
    [SendHTTPResponseBySMS] BIT             DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Campaigns_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_USSDCampaigns_CommunicationTypeId] FOREIGN KEY ([CommunicationTypeId]) REFERENCES [dbo].[CommunicationTypes] ([Id]),
    CONSTRAINT [FK_USSDCampaigns_USSDNumberId] FOREIGN KEY ([USSDNumberId]) REFERENCES [dbo].[UssdNumbers] ([Id])
);

