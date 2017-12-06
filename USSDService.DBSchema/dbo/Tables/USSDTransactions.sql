
CREATE TABLE [dbo].[USSDTransactions] (
    [Id]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    [USSDCampaignId]        Int				NOT NULL,
    [Message]		        NVARCHAR (159)  Not NULL,
    [MobileNumber]          NVARCHAR (20)   Not NULL,
    [CreatedDate]           DATETIME        NOT NULL,
    [CreatedByUserId]       INT             NOT NULL,
    [USSDNumberId]          INT             Not NULL,
    [SessionGuid]           UNIQUEIDENTIFIER    Not NULL,
    [MobileNetworkId]	    INT             NULL,
    CONSTRAINT [PK_USSDTransactions_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_USSDTransactions_USSDNumberId] FOREIGN KEY ([USSDNumberId]) REFERENCES [dbo].[USSDNumbers] ([Id]),
    CONSTRAINT [FK_USSDTransactions_MobileNetworkId] FOREIGN KEY ([MobileNetworkId]) REFERENCES [dbo].[MobileNetworks] ([Id]),
	CONSTRAINT [FK_USSDTransactions_USSDCampaignId] FOREIGN KEY ([USSDCampaignId]) REFERENCES [dbo].[USSDCampaigns] ([Id])
);
