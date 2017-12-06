CREATE TABLE [dbo].[HTTPTransactions]
(
	[Id]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    [USSDCampaignId]        Int				NOT NULL,
    [USSDTransactionId]     BigInt			NOT NULL,
    [MenuId]		        int			    Not NULL,
    [MobileNumber]          NVARCHAR (20)   Not NULL,
    [ResolveURL]            NVARCHAR (200)  NULL,
	[ClientReference]       NVARCHAR (50)  NULL,
	[ClientMessage]         NVARCHAR (159)  NULL,
	[ClientWebResponse]     NVARCHAR (800)  NULL,
	[Errors]				NVARCHAR(Max) null,
	[CreatedDate]           DATETIME        NOT NULL,
    [CreatedByUserId]       INT             NOT NULL,
    CONSTRAINT [PK_HTTPTransactions_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HTTPTransactions_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[USSDMenus] ([Id]),
    CONSTRAINT [FK_HTTPTransactions_USSDTransactionId] FOREIGN KEY ([USSDTransactionId]) REFERENCES [dbo].[USSDTransactions] ([Id]),
	CONSTRAINT [FK_HTTPTransactions_USSDCampaignId] FOREIGN KEY ([USSDCampaignId]) REFERENCES [dbo].[USSDCampaigns] ([Id])
);

