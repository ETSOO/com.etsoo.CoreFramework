IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[ef_get_first_day]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[ef_get_first_day]

GO

-- =============================================
-- Create date: 2022/02/12
-- Description:	Get first day of month or year
-- =============================================
CREATE FUNCTION [dbo].[ef_get_first_day]
(
	-- Add the parameters for the function here
	@Date datetime,
	@Interval varchar(20)
)
RETURNS datetime
AS
BEGIN
	IF @Interval = 'y' OR @Interval = 'year'
		RETURN DATEADD(YEAR, DATEDIFF(YEAR, 0, @Date), 0)

	RETURN DATEADD(DAY, 1, EOMONTH(@Date, -1))
END

GO

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[ef_get_update_json_sql]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[ef_get_update_json_sql]

GO

-- =============================================
-- Create date: 2022/01/31
-- Description:	Get update fields json SQL
-- =============================================
CREATE FUNCTION [dbo].[ef_get_update_json_sql]
(
	-- Add the parameters for the function here
	@Fields AS dbo.et_nvarchar_ids READONLY,
	@TableName nvarchar(max),
	@Conditions nvarchar(max) = 'Id = @Id'
)
RETURNS nvarchar(max)
AS
BEGIN
	RETURN 'SELECT @JsonData = (SELECT ' + (SELECT STRING_AGG(QUOTENAME(Id), ', ') FROM @Fields) + ' FROM ' + @TableName + ' WHERE ' + @Conditions + ' FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)'
END

GO

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[ef_get_update_sql]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[ef_get_update_sql]

GO

-- =============================================
-- Create date: 2022/01/31
-- Description:	Get update fields SQL
-- =============================================
CREATE FUNCTION [dbo].[ef_get_update_sql]
(
	-- Add the parameters for the function here
	@Fields AS dbo.e2t_nvarchar_ids READONLY,
	@TableName nvarchar(max),
	@Conditions nvarchar(max) = 'Id = @Id'
)
RETURNS nvarchar(max)
AS
BEGIN
	DECLARE @Sql nvarchar(max) = 'SELECT ' + (SELECT STRING_AGG(QUOTENAME(Id), ', ') FROM @Fields) + ' FROM ' + @TableName + ' WHERE ' + @Conditions
	RETURN @Sql
END

GO

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[ef_hide_data]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[ef_hide_data]

GO

-- =============================================
-- Create date: 2021/10/25
-- Description:	Hide part of data
-- =============================================
CREATE FUNCTION [dbo].[ef_hide_data]
(
	-- Add the parameters for the function here
	@Data nvarchar(128)
)
RETURNS nvarchar(128)
AS
BEGIN
	IF @Data IS NULL
		RETURN @Data

	DECLARE @Len int = LEN(@Data)

	IF @Len < 4
		RETURN LEFT(@Data, 1) + '***'

	IF @Len < 6
		RETURN LEFT(@Data, 2) + '***'

	IF @Len < 8
		RETURN LEFT(@Data, 2) + '***' + RIGHT(@Data, 2)

	IF @Len < 12
		RETURN LEFT(@Data, 3) + '***' + RIGHT(@Data, 3)

	RETURN LEFT(@Data, 4) + '***' + RIGHT(@Data, 4)
END

GO

IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[ef_hide_email]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[ef_hide_email]

GO

-- =============================================
-- Create date: 2021/10/25
-- Description:	Hide part of email
-- =============================================
CREATE FUNCTION [dbo].[ef_hide_email]
(
	-- Add the parameters for the function here
	@Email varchar(128)
)
RETURNS varchar(128)
AS
BEGIN
	IF @Email IS NULL
		RETURN @Email

	DECLARE @Index int = CHARINDEX('@', @Email)
	IF @Index = 0
		RETURN dbo.ef_hide_data(@Email)

	RETURN dbo.ef_hide_data(LEFT(@Email, @Index - 1)) + RIGHT(@Email, LEN(@Email) - @Index + 1)
END