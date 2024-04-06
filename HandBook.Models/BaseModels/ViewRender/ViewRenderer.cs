using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace HandBook.Models.BaseModels.ViewRender
{
    public class ViewRenderer
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViewRenderer(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, string[] additionalViewLocations = null, HttpContext httpContext = null)
        {
            try
            {
                var actionContext = GetActionContext(httpContext);
                var view = FindView(actionContext, viewName, additionalViewLocations);

                using (var output = new StringWriter())
                {
                    var viewContext = new ViewContext(
                        actionContext,
                        view,
                        new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                        {
                            Model = model
                        },
                        new TempDataDictionary(_httpContextAccessor.HttpContext, _tempDataProvider),
                        output,
                        new HtmlHelperOptions()
                    );

                    await view.RenderAsync(viewContext);

                    return output.ToString();
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
          
        }

        private ActionContext GetActionContext(HttpContext httpContext)
        {
            httpContext ??= _httpContextAccessor.HttpContext;
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return actionContext;
        }

        private IView FindView(ActionContext actionContext, string viewName, string[] additionalViewLocations = null)
        {
            var viewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);
            if (!viewResult.Success && additionalViewLocations != null)
            {
                foreach (var location in additionalViewLocations)
                {
                    viewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: location + "/" + viewName, isMainPage: false);
                    if (viewResult.Success)
                    {
                        return viewResult.View;
                    }
                }
            }

            if (viewResult.Success)
            {
                return viewResult.View;
            }

            var searchedLocations = viewResult.SearchedLocations;
            var errorMessage = string.Join(Environment.NewLine, searchedLocations);
            throw new InvalidOperationException($"The view '{viewName}' could not be found. The following locations were searched:{Environment.NewLine}{errorMessage}");
        }
    }

}
