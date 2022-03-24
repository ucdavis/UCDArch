using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UCDArch.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class BypassAntiForgeryTokenAttribute : ActionFilterAttribute { }
}