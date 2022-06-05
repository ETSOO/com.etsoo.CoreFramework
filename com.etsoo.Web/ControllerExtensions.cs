using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string? viewName, TModel model, bool isMainPage = true)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                // Action name as view name
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            await using var writer = new StringWriter();

            if (controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) is not ICompositeViewEngine viewEngine)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, isMainPage);

            if (viewResult.Success == false)
            {
                throw new ArgumentNullException(viewName);
            }

            var viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return writer.GetStringBuilder().ToString();
        }
    }
}
