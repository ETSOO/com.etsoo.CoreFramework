using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace com.etsoo.DI
{
    /// <summary>
    /// Fire and forget service
    /// 异步环境的非等待执行服务
    /// </summary>
    public class FireAndForgetService
    {
        private readonly ILogger<FireAndForgetService> logger;
        private readonly IServiceScopeFactory scopeFactory;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="scopeFactory">Scope factory</param>
        public FireAndForgetService(
            ILogger<FireAndForgetService> logger,
            IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Fire an action
        /// 触发动作
        /// </summary>
        /// <typeparam name="T1">Dependency 1</typeparam>
        /// <param name="bullet">Action</param>
        public void FireAsync<T1>(Func<T1, ILogger, Task> bullet) where T1 : notnull
        {
            Task.Run(async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                try
                {
                    var d1 = scope.ServiceProvider.GetRequiredService<T1>();
                    await bullet(d1, logger);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Fire and Forget service crashed!");
                }
            });
        }

        /// <summary>
        /// Fire an action
        /// 触发动作
        /// </summary>
        /// <typeparam name="T1">Dependency 1</typeparam>
        /// <typeparam name="T2">Dependency 2</typeparam>
        /// <param name="bullet">Action</param>
        public void FireAsync<T1, T2>(Func<T1, T2, ILogger, Task> bullet)
            where T1 : notnull
            where T2 : notnull
        {
            Task.Run(async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                try
                {
                    var d1 = scope.ServiceProvider.GetRequiredService<T1>();
                    var d2 = scope.ServiceProvider.GetRequiredService<T2>();
                    await bullet(d1, d2, logger);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Fire and Forget service crashed!");
                }
            });
        }

        /// <summary>
        /// Fire an action
        /// 触发动作
        /// </summary>
        /// <typeparam name="T1">Dependency 1</typeparam>
        /// <typeparam name="T2">Dependency 2</typeparam>
        /// <typeparam name="T3">Dependency 3</typeparam>
        /// <param name="bullet">Action</param>
        public void FireAsync<T1, T2, T3>(Func<T1, T2, T3, ILogger, Task> bullet)
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull
        {
            Task.Run(async () =>
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                try
                {
                    var d1 = scope.ServiceProvider.GetRequiredService<T1>();
                    var d2 = scope.ServiceProvider.GetRequiredService<T2>();
                    var d3 = scope.ServiceProvider.GetRequiredService<T3>();
                    await bullet(d1, d2, d3, logger);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Fire and Forget service crashed!");
                }
            });
        }
    }
}