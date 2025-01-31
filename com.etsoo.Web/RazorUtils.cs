using com.etsoo.Utils.Serialization;
using RazorEngineCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace com.etsoo.Web
{
    /// <summary>
    /// Razor utils
    /// https://github.com/adoconnection/RazorEngineCore
    /// Razor工具类
    /// </summary>
    public static class RazorUtils
    {
        private static readonly ConcurrentDictionary<string, IRazorEngineCompiledTemplate> templateCache = new();

        /// <summary>
        /// Render Razor template to string
        /// 将Razor模板渲染为字符串
        /// </summary>
        /// <param name="templateFile">Template file</param>
        /// <param name="model">Data model</param>
        /// <param name="types">Assembly types</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<string> RenderAsync<M>(string templateFile, M model, IEnumerable<Type>? types = null, CancellationToken cancellationToken = default) where M : class
        {
            var template = await File.ReadAllTextAsync(templateFile, cancellationToken);
            return await RenderAsync(templateFile, template, model, types, cancellationToken);
        }

        /// <summary>
        /// Render Razor template to string
        /// 将Razor模板渲染为字符串
        /// </summary>
        /// <param name="key">Identifier key</param>
        /// <param name="template">Template content</param>
        /// <param name="model">Data model</param>
        /// <param name="types">Assembly types</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<string> RenderAsync<M>(string key, string template, M model, IEnumerable<Type>? types = null, CancellationToken cancellationToken = default) where M : class
        {
            // Get or add the compiled version
            if (!templateCache.TryGetValue(key, out var compiledTemplate))
            {
                // Engine
                var razorEngine = new RazorEngine();

                // Compile
                compiledTemplate = await razorEngine.CompileAsync(template, builder =>
                {
                    builder.AddAssemblyReference(typeof(M));

                    if (types != null)
                    {
                        foreach (var assembly in types)
                        {
                            builder.AddAssemblyReference(assembly);
                        }
                    }
                }, cancellationToken);

                templateCache.TryAdd(key, compiledTemplate);
            }

            // Run and return the result
            return await compiledTemplate.RunAsync(model);
        }

        /// <summary>
        /// Render Razor template to string
        /// 将Razor模板渲染为字符串
        /// </summary>
        /// <param name="key">Identifier key</param>
        /// <param name="template">Template content</param>
        /// <param name="jsonData">JSON data</param>
        /// <param name="types">Assembly types</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static Task<string> RenderAsync(string key, string template, string jsonData, IEnumerable<Type>? types = null, CancellationToken cancellationToken = default)
        {
            var doc = JsonDocument.Parse(jsonData);
            var dic = doc.RootElement.ToDictionary();
            return RenderAsync(key, template, dic, types, cancellationToken);
        }
    }
}
