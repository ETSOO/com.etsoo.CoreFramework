﻿using RazorEngineCore;
using System.Collections.Concurrent;

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
        /// <returns>Result</returns>
        public static async Task<string> RenderAsync<M>(string templateFile, M model)
        {
            var template = await File.ReadAllTextAsync(templateFile);
            return await RenderAsync(templateFile, template, model);
        }

        /// <summary>
        /// Render Razor template to string
        /// 将Razor模板渲染为字符串
        /// </summary>
        /// <param name="key">Identifier key</param>
        /// <param name="template">Template content</param>
        /// <param name="model">Data model</param>
        /// <returns>Result</returns>
        public static async Task<string> RenderAsync<M>(string key, string template, M model)
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
                });

                templateCache.TryAdd(key, compiledTemplate);
            }

            // Run and return the result
            return await compiledTemplate.RunAsync(model);
        }
    }
}
