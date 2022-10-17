using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace SmartMeterServer.Attributes
{
    public class SmartMeterExceptionHandlingFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IModelBinderFactory _modelBinderFactory;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IStringLocalizer _errorLocalizer;

        public SmartMeterExceptionHandlingFilterAttribute(
            IModelBinderFactory factory,
            IModelMetadataProvider modelMetadataProvider,
            IActionContextAccessor actionContextAccessor,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _modelBinderFactory = factory;
            _modelMetadataProvider = modelMetadataProvider;
            _actionContextAccessor = actionContextAccessor;
            _errorLocalizer = stringLocalizerFactory.Create(
                nameof(Resources.Errors),
                Assembly.GetExecutingAssembly().GetName().Name!
            );
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is Abstract.Exceptions.SmartMeterException sme)
            {
                if (!context.ActionDescriptor.Parameters.All(p => p is ControllerParameterDescriptor))
                {
                    return;
                }

                var controllerParameters = context.ActionDescriptor.Parameters
                    .Select(p => (ControllerParameterDescriptor)p);

                var formParameters = controllerParameters
                    .Where(p => p.ParameterInfo.GetCustomAttribute<FromFormAutoErrorAttribute>() != null);

                if (formParameters.Count() != 1)
                {
                    return;
                }

                var modelDescriptor = formParameters.Single();
                Type modelType = modelDescriptor.ParameterType;
                FromFormAutoErrorAttribute attribute = modelDescriptor.ParameterInfo.GetCustomAttribute<FromFormAutoErrorAttribute>()!;

                string viewName = attribute.ViewName ?? context.RouteData.Values["action"].ToString() ?? string.Empty;

                var binder = _modelBinderFactory.CreateBinder(
                    new ModelBinderFactoryContext 
                    {
                        BindingInfo = modelDescriptor.BindingInfo,
                        Metadata = _modelMetadataProvider.GetMetadataForType(modelType),
                    });

                var valueProviderFactory = new FormValueProviderFactory();
                var valueprovidercontext = new ValueProviderFactoryContext(_actionContextAccessor.ActionContext!);
                valueProviderFactory.CreateValueProviderAsync(valueprovidercontext).Wait();

                var mbContext = new DefaultModelBindingContext()
                {
                    ModelState = context.ModelState,
                    ModelMetadata = _modelMetadataProvider.GetMetadataForType(modelType),
                    ActionContext = _actionContextAccessor.ActionContext,
                    ValueProvider = new CompositeValueProvider(valueprovidercontext.ValueProviders),
                };

                try
                {
                    binder.BindModelAsync(mbContext).Wait();
                }
                catch (Exception)
                {
                    return;
                }

                if (mbContext.Result.Model is Models.ErrorViewModel evm)
                {
                    evm.ErrorTitle = _errorLocalizer[sme.ErrorKey];
                    evm.ErrorDetail = _errorLocalizer[sme.MessageKey];

                    var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState);
                    viewData = new ViewDataDictionary<Models.ErrorViewModel>(viewData, mbContext.Model);

                    context.Result = new ViewResult()
                    {
                        ViewData = viewData,
                        ViewName = viewName,
                    };
                }                
            }
        }
    }
}
