CREATE TABLE [dbo].[MenuOptions] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Command]      NVARCHAR (250) NULL,
    [DisplayOrder] INT            NULL,
    [Display]      BIT            NULL,
    [Callback]     NVARCHAR (100) NULL,
    [Text]         NVARCHAR (255) NULL,
    CONSTRAINT [PK_MenuOptions_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

