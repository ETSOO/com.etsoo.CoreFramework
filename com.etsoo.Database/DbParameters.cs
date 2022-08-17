using Dapper;
using System.Data;

namespace com.etsoo.Database
{
    /// <summary>
    /// Dapper database parameter, removed SqlMapper.ReplaceLiterals and Output
    /// Dapper 数据库参数
    /// https://github.com/DapperLib/Dapper/blob/main/Dapper/DynamicParameters.cs
    /// </summary>
    public class DbParameters : IDbParameters
    {
        private sealed class ParamInfo
        {
            public string Name { get; init; } = default!;
            public object? Value { get; set; }
            public ParameterDirection ParameterDirection { get; set; }
            public DbType? DbType { get; set; }
            public int? Size { get; set; }
            public IDbDataParameter? AttachedParam { get; set; }
            internal bool CameFromTemplate { get; set; }
            public byte? Precision { get; set; }
            public byte? Scale { get; set; }
        }

        private static readonly Dictionary<SqlMapper.Identity, Action<IDbCommand, object>> paramReaderCache = new();
        private readonly Dictionary<string, ParamInfo> parameters = new();
        private List<object>? templates;

        object? SqlMapper.IParameterLookup.this[string name] =>
            parameters.TryGetValue(name, out var param) ? param.Value : null;

        /// <summary>
        /// All the names of the param in the bag, use Get to yank them out
        /// </summary>
        public IEnumerable<string> ParameterNames => parameters.Select(p => p.Key);

        /// <summary>
        /// If true, the command-text is inspected and only values that are clearly used are included on the connection
        /// </summary>
        public bool RemoveUnused { get; set; } = true;

        /// <summary>
        /// Construct a dynamic parameter bag
        /// </summary>
        public DbParameters()
        {
        }

        /// <summary>
        /// Construct a dynamic parameter bag
        /// Performance lost because of reflection needs to be considered
        /// </summary>
        /// <param name="template">Can be an anonymous type or a DynamicParameters bag</param>
        public DbParameters(object template)
        {
            AddDynamicParams(template);
        }

        /// <summary>
        /// Append a whole object full of params to the dynamic
        /// EG: AddDynamicParams(new {A = 1, B = 2}) // will add property A and B to the dynamic
        /// </summary>
        /// <param name="param"></param>
        public void AddDynamicParams(object param)
        {
            var obj = param;
            if (obj != null)
            {
                if (obj is DbParameters subDynamic)
                {
                    if (subDynamic.parameters != null)
                    {
                        foreach (var kvp in subDynamic.parameters)
                        {
                            parameters.Add(kvp.Key, kvp.Value);
                        }
                    }

                    if (subDynamic.templates != null)
                    {
                        templates ??= new List<object>();
                        foreach (var t in subDynamic.templates)
                        {
                            templates.Add(t);
                        }
                    }
                }
                else
                {
                    if (obj is IEnumerable<KeyValuePair<string, object>> dictionary)
                    {
                        foreach (var kvp in dictionary)
                        {
                            Add(kvp.Key, kvp.Value, null, null, null);
                        }
                    }
                    else
                    {
                        templates ??= new List<object>();
                        templates.Add(obj);
                    }
                }
            }
        }

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
        public void Add(string name, object? value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null)
        {
            parameters[Clean(name)] = new ParamInfo
            {
                Name = name,
                Value = value,
                ParameterDirection = direction ?? ParameterDirection.Input,
                DbType = dbType,
                Size = size,
                Precision = precision,
                Scale = scale
            };
        }

        private static string Clean(string name)
        {
            if (!string.IsNullOrEmpty(name) && name[0] is '@' or ':' or '?')
            {
                return name[1..];
            }
            return name;
        }

        void SqlMapper.IDynamicParameters.AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            AddParameters(command, identity);
        }

        /// <summary>
        /// Clear all null value parameters
        /// </summary>
        public void ClearNulls()
        {
            foreach (var key in parameters.Keys)
            {
                var p = parameters[key];
                if (p.Value == null && p.ParameterDirection == ParameterDirection.Input)
                {
                    // Remove null value item but keep DbNull.Value item
                    parameters.Remove(key);
                }
            }
        }

        /// <summary>
        /// Remove parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <returns>Result</returns>
        public bool Remove(string name)
        {
            return parameters.Remove(Clean(name));
        }

        /// <summary>
        /// Get the value of a parameter
        /// </summary>
        /// <typeparam name="T">Generic return value type</typeparam>
        /// <param name="name">Parameter name</param>
        /// <returns>The value, note DBNull.Value is not returned, instead the value is returned as null</returns>
        public T? Get<T>(string name) where T : IConvertible
        {
            var paramInfo = parameters[Clean(name)];
            var attachedParam = paramInfo.AttachedParam;
            var val = attachedParam == null ? paramInfo.Value : attachedParam.Value;
            if (val == DBNull.Value || val == null)
            {
                if (default(T) != null)
                {
                    throw new ApplicationException("Attempting to cast a DBNull to a non nullable type!");
                }
                return default;
            }
            return (T)val;
        }

        /// <summary>
        /// Add all the parameters needed to the command just before it executes
        /// </summary>
        /// <param name="command">The raw command prior to execution</param>
        /// <param name="identity">Information about the query</param>
        protected void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            if (templates != null)
            {
                foreach (var template in templates)
                {
                    var newIdent = identity.ForDynamicParameters(template.GetType());
                    lock (paramReaderCache)
                    {
                        if (!paramReaderCache.TryGetValue(newIdent, out var appender))
                        {
                            appender = SqlMapper.CreateParamInfoGenerator(newIdent, true, RemoveUnused);
                            paramReaderCache[newIdent] = appender;
                        }
                        appender(command, template);
                    }
                }

                // The parameters were added to the command, but not the
                // DynamicParameters until now.
                foreach (IDbDataParameter param in command.Parameters)
                {
                    // If someone makes a DynamicParameters with a template,
                    // then explicitly adds a parameter of a matching name,
                    // it will already exist in 'parameters'.
                    if (!parameters.ContainsKey(param.ParameterName))
                    {
                        parameters.Add(param.ParameterName, new ParamInfo
                        {
                            AttachedParam = param,
                            CameFromTemplate = true,
                            DbType = param.DbType,
                            Name = param.ParameterName,
                            ParameterDirection = param.Direction,
                            Size = param.Size,
                            Value = param.Value
                        });
                    }
                }
            }

            foreach (var param in parameters.Values)
            {
                if (param.CameFromTemplate) continue;

                var dbType = param.DbType;
                var val = param.Value;
                var name = Clean(param.Name);

                if (val is SqlMapper.ICustomQueryParameter cp)
                {
                    cp.AddParameter(command, name);
                }
                else if (val is IDbDataParameter dp)
                {
                    // Add database specific parameter
                    command.Parameters.Add(dp);
                }
                else
                {
                    var add = !command.Parameters.Contains(name);
                    IDbDataParameter p;
                    if (add)
                    {
                        p = command.CreateParameter();
                        p.ParameterName = name;
                    }
                    else
                    {
                        p = (IDbDataParameter)command.Parameters[name];
                    }

                    p.Direction = param.ParameterDirection;

                    if (val != null)
                    {
                        Type type;
                        if (val is Enum eval)
                        {
                            type = Enum.GetUnderlyingType(eval.GetType());
                            val = Convert.ChangeType(eval, type);
                        }
                        else
                        {
                            type = val.GetType();
                        }

                        dbType ??= DatabaseUtils.TypeToDbType(type);
                        p.Value = val;
                    }
                    else
                    {
                        p.Value = DBNull.Value;
                    }

                    if (dbType.HasValue) p.DbType = dbType.Value;

                    if (param.Size != null) p.Size = param.Size.Value;
                    else if (val is string sval && sval.Length <= DbString.DefaultLength) p.Size = DbString.DefaultLength;

                    if (param.Precision != null) p.Precision = param.Precision.Value;
                    if (param.Scale != null) p.Scale = param.Scale.Value;

                    if (add)
                    {
                        command.Parameters.Add(p);
                    }
                    param.AttachedParam = p;
                }
            }
        }
    }
}
