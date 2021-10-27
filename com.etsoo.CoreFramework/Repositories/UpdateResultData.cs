namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Update action result data
    /// 更新操作结果数据
    /// </summary>
    public record UpdateResultData<T> where T : struct
    {
        /// <summary>
        /// ChangedFields data invalid
        /// </summary>
        public const string ChangedFields = "ChangedFields";

        /// <summary>
        /// UpdatableFields data invalid
        /// </summary>
        public const string UpdatableFields = "UpdatableFields";

        /// <summary>
        /// Conditions data invalid
        /// </summary>
        public const string Conditions = "Conditions";

        /// <summary>
        /// UpdateFields data invalid
        /// </summary>
        public const string UpdateFields = "UpdateFields";

        /// <summary>
        /// Updated id
        /// 更新的编号
        /// </summary>
        public T Id { get; init; }

        /// <summary>
        /// Rows affected
        /// 影响的行数
        /// </summary>
        public int RowsAffected { get; init; }
    }
}
