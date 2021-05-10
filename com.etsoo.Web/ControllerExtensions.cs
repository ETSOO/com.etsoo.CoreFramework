using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;

namespace com.etsoo.Web
{
    /// <summary>
    /// Controller extensions
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Render Razor view as string
        /// https://stackoverflow.com/questions/40912375/return-view-as-string-in-net-core
        /// 将Razor视图渲染为字符串
        /// </summary>
        /// <typeparam name="TModel">Data model generic type</typeparam>
        /// <param name="controller">API controller</param>
        /// <param name="viewName">View name</param>
        /// <param name="model">Data model</param>
        /// <param name="isMainPage">Is main page</param>
        /// <returns>Result</returns>
        public static async Task<string> RenderViewAsync<TModel>(this ControllerBase controller, string? viewName, TModel model, bool isMainPage = true)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                // Action name as view name
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            using var writer = new StringWriter();

            var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            if (viewEngine == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, isMainPage);

            if (viewResult.Success == false)
            {
                throw new ArgumentNullException(viewName);
            }

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                viewData,
                null,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
        }
    }
}
