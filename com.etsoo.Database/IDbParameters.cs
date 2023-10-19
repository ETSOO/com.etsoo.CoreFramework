using Dapper;
using System.Data;

namespace com.etsoo.Database
{
    /// <summary>
    /// Dapper database parameters
    /// Dapper 数据库参数
    /// https://github.com/DapperLib/Dapper/DynamicParameters
    /// </summary>
    public interface IDbParameters : SqlMapper.IDynamicParameters, SqlMapper.IParameterLookup
    {
        /// <summary>
        /// If true, the command-text is inspected and only values that are clearly used are included on the connection
        /// </summary>
        bool RemoveUnused { get; set; }

        /// <summary>
        /// All the names of the param in the bag, use Get to yank them out
        /// </summary>
        IEnumerable<string> ParameterNames { get; }

        /// <summary>
        /// Add a parameter to this dynamic parameter list.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="dbType">The type of the parameter.</param>
        /// <param name="direction">The in or out direction of the parameter.</param>
        /// <param name="size">The size of the parameter.</param>
        /// <param name="precision">The precision of the parameter.</param>
        /// <param name="scale">The scale of the parameter.</param>
        void Add(string name, object? value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null);

        /// <summary>
        /// Clear all null value parameters
        /// </summary>
        void ClearNulls();

        /// <summary>
        /// Get the value of a parameter
        /// </summary>
        /// <typeparam name="T">Generic return value type</typeparam>
        /// <param name="name">Parameter name</param>
        /// <returns>The value, note DBNull.Value is not returned, instead the value is returned as null</returns>
        T? Get<T>(string name) where T : IConvertible;

        /// <summary>
        /// Remove parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Result</returns>
        bool Remove(string name);
    }
}