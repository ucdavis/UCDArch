// using System;
// using System.ServiceModel;
// using Microsoft.AspNetCore.Mvc.Filters;
// using UCDArch.Web.Services;

// namespace UCDArch.Web.Attributes
// {
//     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
//     public class ServiceMessageAttribute : ActionFilterAttribute
//     {
//         private readonly string _appName;

//         public ServiceMessageAttribute(string appName)
//         {
//             _appName = appName;
//         }

//         private string _cacheKey;

//         public string CacheKey
//         {
//             get { return _cacheKey ?? "ServiceMessages"; }
//             set { _cacheKey = value; }
//         }

//         /// <summary>
//         /// Set the cache absolute cache expiration in seconds.  Default is one day from now.
//         /// </summary>
//         public int CacheExpirationInSeconds { get; set; }

//         private TimeSpan CacheExpiration
//         {
//             get
//             {
//                 return CacheExpirationInSeconds == default(int)
//                            ? TimeSpan.FromDays(1)
//                            : TimeSpan.FromSeconds(CacheExpirationInSeconds);
//             }
//         }

//         /// <summary>
//         /// Full qualified Url to the message service.  Either MessageServiceAppSettingsKey or MessageServiceAppSettingsKey must be set.
//         /// </summary>
//         public string MessageServiceUrl { get; set; }

//         /// <summary>
//         /// AppSettings key which holds the servicel Url. Either MessageServiceAppSettingsKey or MessageServiceAppSettingsKey must be set.
//         /// </summary>
//         public string MessageServiceAppSettingsKey { get; set; }

//         /// <summary>
//         /// If provided, ViewData["ViewDataKey"] will contain a null-safe array of services messages.
//         /// </summary>
//         public string ViewDataKey { get; set; }

//         private string ServiceUrl
//         {
//             get
//             {
//                 if (MessageServiceUrl == null && MessageServiceAppSettingsKey == null)
//                     throw new InvalidOperationException(
//                         "Either MessageServiceAppSettingsKey or MessageServiceUrl must be set.");

//                 if (MessageServiceUrl != null && MessageServiceAppSettingsKey != null)
//                     throw new InvalidOperationException(
//                         "Either MessageServiceAppSettingsKey or MessageServiceUrl must be set, but not both.");

//                 return MessageServiceUrl ?? configuration.GetValue<string>(MessageServiceAppSettingsKey);
//             }
//         }

//         public override void OnActionExecuting(ActionExecutingContext filterContext)
//         {
//             var cache = HttpRuntime.Cache;

//             if (!string.IsNullOrWhiteSpace(ViewDataKey))
//             {
//                 filterContext.Controller.ViewData[ViewDataKey] = cache[CacheKey] ?? new ServiceMessage[0];
//             }

//             if (cache[CacheKey] != null) return;

//             if (string.IsNullOrWhiteSpace(ServiceUrl))
//                 throw new InvalidOperationException(
//                     "Service Url is not valid:  Please set either MessageServiceAppSettingsKey or MessageServiceUrl");

//             cache[CacheKey] = new ServiceMessage[0];

//             var binding = new BasicHttpBinding();

//             if (ServiceUrl.StartsWith("https://")) binding.Security.Mode = BasicHttpSecurityMode.Transport;

//             var client = new MessageServiceClient(binding, new EndpointAddress(ServiceUrl));

//             client.BeginGetMessages(_appName, OnMessagesRecieved, client);
//         }

//         private void OnMessagesRecieved(IAsyncResult ar)
//         {
//             var client = (MessageServiceClient)ar.AsyncState;

//             try
//             {
//                 // Get the results.
//                 var messages = client.EndGetMessages(ar);

//                 // Insert into the cache
//                 HttpRuntime.Cache.Insert(CacheKey, messages, null, DateTime.Now.Add(CacheExpiration), Cache.NoSlidingExpiration);
//             }
//             finally
//             {
//                 //Close the client
//                 client.Close();
//             }
//         }
//     }
// }