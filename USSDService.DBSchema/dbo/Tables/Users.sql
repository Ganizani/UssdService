CREATE TABLE [dbo].[Users] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [UserName]      VARCHAR (50)   NOT NULL,
    [FirstName]     NVARCHAR (100) NULL,
    [LastName]      NVARCHAR (100) NULL,
    [Password]      NVARCHAR (100) NOT NULL,
    [EmailAddress]  NVARCHAR (100) NULL,
    [LastLoginDate] DATETIME       NULL,
    [LoginAttempt]  INT            NULL,
    CONSTRAINT [PK_Users_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);

