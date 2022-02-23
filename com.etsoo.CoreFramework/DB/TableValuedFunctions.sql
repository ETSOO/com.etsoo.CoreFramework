IF EXISTS (SELECT *
           FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[ef_get_range_months]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[ef_get_range_months]

GO

-- =============================================
-- Create date: 2022/02/12
-- Description:	Get range months
-- =============================================
CREATE FUNCTION [dbo].[ef_get_range_months]
(
	-- Add the parameters for the function here
	@Start datetime,
	@Count tinyint
)
RETURNS TABLE
AS
RETURN 
(
	-- Temporary table recursive query
    WITH dt(StartDate, EndDate, RowIndex) 
    AS
    (
		SELECT @Start, DATEADD(MONTH, 1, @Start), 0
			UNION ALL
		SELECT DATEADD(MONTH, 1, dt.StartDate), DATEADD(MONTH, 1, dt.EndDate), dt.RowIndex + 1 FROM dt WHERE dt.RowIndex < @Count - 1
    )
    
    -- Return
    SELECT * FROM dt
)