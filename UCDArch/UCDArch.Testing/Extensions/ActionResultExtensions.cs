using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;

namespace UCDArch.Testing.Extensions
{
    public static class ActionResultExtensions
    {
        public static T AssertResultIs<T>(this ActionResult result) where T : ActionResult
        {
            var converted = result as T;

            if (converted == null)
            {
                throw new AssertFailedException(string.Format("Expected result {0}. Recieved result {1}.", typeof(T).Name, result.GetType().Name));
            }

            return converted;
        }

        public static ViewResult AssertViewRendered(this ActionResult result)
        {
            return result.AssertResultIs<ViewResult>();
        }

        public static PartialViewResult AssertPartialViewRendered(this ActionResult result)
        {
            return result.AssertResultIs<PartialViewResult>();
        }

        public static RedirectToActionResult AssertActionRedirect(this ActionResult result)
        {
            return result.AssertResultIs<RedirectToActionResult>();
        }

        public static TViewData WithViewData<TViewData>(this PartialViewResult actionResult)
        {
            return AssertViewDataModelType<TViewData>(actionResult);
        }

        public static TViewData WithViewData<TViewData>(this ViewResult actionResult)
        {
            return AssertViewDataModelType<TViewData>(actionResult);
        }

        private static TViewData AssertViewDataModelType<TViewData>(ActionResult actionResult)
        {
            var actualViewData = actionResult switch
            {
                (ViewResult viewResult) => viewResult.ViewData.Model,
                (PartialViewResult partialViewResult) => partialViewResult.ViewData.Model,
                _ => throw new ArgumentException("ActionResult is not a ViewResult or PartialViewResult")
            };
            var expectedType = typeof(TViewData);

            if (actualViewData == null && expectedType.IsValueType)
            {
                throw new AssertFailedException(string.Format("Expected view data of type '{0}', actual was NULL",
                                                                       expectedType.Name));
            }

            if (actualViewData == null)
            {
                return (TViewData)actualViewData;
            }

            if (!typeof(TViewData).IsAssignableFrom(actualViewData.GetType()))
            {
                throw new AssertFailedException(string.Format("Expected view data of type '{0}', actual was '{1}'",
                                                                       typeof(TViewData).Name, actualViewData.GetType().Name));
            }

            return (TViewData)actualViewData;
        }

        public static RedirectToRouteResult ToAction(this RedirectToRouteResult result, string action)
		{
			return result.WithParameter("action", action);

		}

		public static RedirectToRouteResult ToAction<TController>(this RedirectToRouteResult result, Expression<Action<TController>> action)
         where TController : ControllerBase
		{
			var methodCall = (MethodCallExpression)action.Body;
			string actionName = methodCall.Method.Name;

			const string ControllerSuffix = "Controller";
			var controllerTypeName = typeof(TController).Name;
			if (controllerTypeName.EndsWith(ControllerSuffix, StringComparison.OrdinalIgnoreCase))
			{
				controllerTypeName = controllerTypeName.Substring(0, controllerTypeName.Length - ControllerSuffix.Length);
			}
			return result.ToController(controllerTypeName).ToAction(actionName);
		}

		public static RedirectToRouteResult WithParameter(this RedirectToRouteResult result, string paramName, object value)
		{
			if(!result.RouteValues.ContainsKey(paramName))
			{
				throw new AssertFailedException(string.Format("Could not find a parameter named '{0}' in the result's Values collection.", paramName));
			}

			var paramValue = result.RouteValues[paramName];

			if(!paramValue.Equals(value))
			{
				throw new AssertFailedException(string.Format("When looking for a parameter named '{0}', expected '{1}' but was '{2}'.", paramName, value, paramValue));
			}

			return result;
		}

        public static RedirectToRouteResult ToController(this RedirectToRouteResult result, string controller)
		{
			return result.WithParameter("controller", controller);
		}
    }
}