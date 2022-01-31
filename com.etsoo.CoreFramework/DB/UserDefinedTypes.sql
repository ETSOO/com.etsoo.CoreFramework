-- et_guid_items --
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='et_guid_items')
	DROP TYPE [et_guid_items]

GO

CREATE TYPE [dbo].[et_guid_items] AS TABLE (
    [Id]   UNIQUEIDENTIFIER NOT NULL,
    [Item] VARCHAR (128)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));

GO

-- et_int_smallint_items --
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='et_int_smallint_items')
	DROP TYPE [et_int_smallint_items]

GO

CREATE TYPE [dbo].[et_int_smallint_items] AS TABLE(
	[Key] INT NOT NULL,
	[Item] SMALLINT NOT NULL,
	PRIMARY KEY CLUSTERED ([Key] ASC)
)

GO

-- et_int_ids --
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='et_int_ids')
	DROP TYPE [et_int_ids]

GO

CREATE TYPE [dbo].[et_int_ids] AS TABLE (
    [Id] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));

GO

-- et_bigint_ids --
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='et_bigint_ids')
	DROP TYPE [et_bigint_ids]

GO

CREATE TYPE [dbo].[et_bigint_ids] AS TABLE (
    [Id] BIGINT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));

GO

-- et_nvarchar_ids --
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='et_nvarchar_ids')
	DROP TYPE [et_nvarchar_ids]

GO

CREATE TYPE [dbo].[et_nvarchar_ids] AS TABLE (
    [Id] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));

GO

-- et_uniqueidentifier_ids --
IF EXISTS (SELECT * FROM sys.types WHERE is_table_type = 1 AND name ='et_uniqueidentifier_ids')
	DROP TYPE [et_uniqueidentifier_ids]

GO

CREATE TYPE [dbo].[et_uniqueidentifier_ids] AS TABLE (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC));

GO