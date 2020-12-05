using com.etsoo.CoreFramework.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.etsoo.CoreFramework.Repositories
{
    /// <summary>
    /// Base repository
    /// 基础仓库
    /// </summary>
    /// <typeparam name="T">Id field type</typeparam>
    public class RepositoryBase<T> where T : struct, IComparable
    {
        /// <summary>
        /// Application
        /// 程序对象
        /// </summary>
        protected ICoreApplication App { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="app">Application</param>
        public RepositoryBase(ICoreApplication app) => (App) = (app);
    }
}
