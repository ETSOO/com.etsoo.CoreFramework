using Microsoft.Extensions.Logging;

namespace com.etsoo.DI
{
    /// <summary>
    /// Fire and forget service interface
    /// 异步环境的非等待执行服务接口
    /// </summary>
    public interface IFireAndForgetService
    {
        /// <summary>
        /// Fire an action
        /// 触发动作
        /// </summary>
        /// <param name="bullet">Action</param>
        void FireAsync(Func<ILogger, Task> bullet);

        /// <summary>
        /// Fire an action
        /// 触发动作
        /// </summary>
        /// <typeparam name="T1">Dependency 1</typeparam>
        /// <param name="bullet">Action</param>
        void FireAsync<T1>(Func<T1, ILogger, Task> bullet) where T1 : notnull;

        /// <summary>
        /// Fire an action
        /// 触发动作
        /// </summary>
        /// <typeparam name="T1">Dependency 1</typeparam>
        /// <typeparam name="T2">Dependency 2</typeparam>
        /// <param name="bullet">Action</param>
        void FireAsync<T1, T2>(Func<T1, T2, ILogger, Task> bullet)
            where T1 : notnull
            where T2 : notnull;

        /// <summary>
        /// Fire an action
        /// 触发动作
        /// </summary>
        /// <typeparam name="T1">Dependency 1</typeparam>
        /// <typeparam name="T2">Dependency 2</typeparam>
        /// <typeparam name="T3">Dependency 3</typeparam>
        /// <param name="bullet">Action</param>
        void FireAsync<T1, T2, T3>(Func<T1, T2, T3, ILogger, Task> bullet)
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull;
    }
}
