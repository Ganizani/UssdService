CREATE TABLE [dbo].[CommunicationTypes] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [CommunicationText] NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

