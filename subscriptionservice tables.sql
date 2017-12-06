use SubscriptionService
Go

create table [dbo].[PaymentFrequencies]
(
	 [Id] tinyint not null identity(1,1) primary key
	,[PaymentFrequencyName]  varchar(100) not null
	
)
go
insert into PaymentFrequencies([PaymentFrequencyName])
			values  ('Daily'),
					('Weekly'),
					('Monthly')
Go
create table [dbo].[PaymentMethods]
(
	 [Id] tinyint not null identity(1,1) primary key
	,[PaymentMethodName]  varchar(100) not null
	
)
go
insert into PaymentMethods([PaymentMethodName])
			values  ('EFT'),
					('Cash'),
					('Credit Card'),
					('Mobile Money'),
					('Direct Deposit')
Go

create table [dbo].[StatusTypes]
(
	 [Id] tinyint not null identity(1,1) primary key
	,[StatusTypeName]  varchar(50) not null
	
)
go
insert into [dbo].[StatusTypes]([StatusTypeName])
			values  ('Subscription'),
					('Subscription Service'),
					('Billing')
					
Go

create table [dbo].[Statuses]
(
	 [Id] int not null identity(1,1) primary key
	,[StatusName]  varchar(50) not null
	,[StatusTypeId] tinyint not null
)
go
alter table [dbo].[Statuses] add constraint [FK_Statuses_StatusTypeId]  Foreign Key([StatusTypeId])
references [dbo].[StatusTypes] ([Id])
go

insert into [dbo].[Statuses]([StatusName],[StatusTypeId])
			values  ('Active' ,1),
					('InActive',1),
					('Suspended', 1),
					('Pending', 1),
					('Cancelled', 1),
					('Active' ,2),
					('InActive',2),
					('Suspended',2)
					
Go
create table [dbo].[SubscriptionServices]
(
	 [Id] int not null identity(1,1) primary key
	,[Description] varchar(max) not null
	,[DisplayName] varchar(50) not null
	,[Keyword] varchar(20) not null
	,[UssdNumber] varchar(20) null
	,[ShortNumber] varchar(20) null
	,[PaymentFrequencyId] tinyint not null
	,[StatusId] int not null
	,[PaymentMethodId] tinyint not null
	,[UnsubKeyword] varchar(20) null
	,[UnsubSMSNumber] varchar(20) not null
	,[PRSRate] money null
	,[InitialBillingAmount] numeric(18,2) default(0)
	,[BillingAmount] numeric(18,2) not null
	 

)
Go
alter table [dbo].[SubscriptionServices] add constraint [FK_SubscriptionServices_StatusId]  Foreign Key([StatusId])
references [dbo].[Statuses] ([Id])
go
alter table [dbo].[SubscriptionServices] add constraint [FK_SubscriptionServices_PaymentFrequencyId]  Foreign Key([PaymentFrequencyId])
references [dbo].[PaymentFrequencies] ([Id])
go
alter table [dbo].[SubscriptionServices] add constraint [FK_SubscriptionServices_PaymentMethodId]  Foreign Key([PaymentMethodId])
references [dbo].[PaymentMethods] ([Id])
go

create table [dbo].[Subscriptions]
(
	 [Id] bigint not null identity(1,1) primary key
	,[MobileNumber]  varchar(250) not null
	,[StatusId] int not null
	,[SubscriptionServiceId] int
	,[CreatedDate] datetime not null
	,[FirstBillingDate] datetime null
	,[NextBillingDate] datetime null
	,[ReferenceNumber] varchar(50) null
	,[InitialBillingAmount] numeric(18,2) default(0)
	,[BillingAmount] numeric(18,2) not null
	


)
Go
alter table [dbo].[Subscriptions] add constraint [FK_Subscriptions_StatusId]  Foreign Key([StatusId])
references [dbo].[Statuses] ([Id])
go
alter table [dbo].[Subscriptions] add constraint [FK_Subscriptions_SubscriptionServiceId]  Foreign Key([SubscriptionServiceId])
references [dbo].[SubscriptionServices] ([Id])
go
alter table [dbo].[Subscriptions]add constraint [CK_Subscriptions_BillingAmount] Check ([BillingAmount] > 0)
Go