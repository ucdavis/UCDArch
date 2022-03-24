// using System;
// using System.Reflection;
// using Microsoft.AspNetCore.Mvc.Filters;

// namespace UCDArch.Web.Attributes
// {
//     /// <summary>
//     /// Sets the ViewData["Version"] to a version number corresponding to the last build date.
//     /// Format: {MajorVersion}.{year}.{month}.{day}
//     /// </summary>
//     [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
//     public class VersionAttribute : ActionFilterAttribute
//     {
//         public int MajorVersion { get; set; }
//         public string VersionKey { get; set; }

//         public VersionAttribute()
//         {
//             MajorVersion = 1;
//             VersionKey = "Version";
//         }

//         /// <summary>
//         /// Grabs the date time stamp and places the version in Cache if it does not exist
//         /// and places the version in ViewData
//         /// </summary>
//         /// <param name="filterContext"></param>
//         private void LoadAssemblyVersion(ActionExecutingContext filterContext)
//         {
//             var version = filterContext.HttpContext.Cache[VersionKey] as string;

//             if (string.IsNullOrEmpty(version))
//             {

//                 var assembly = Assembly.GetExecutingAssembly();

//                 var buildDate = RetrieveLinkerTimestamp(assembly.Location);

//                 version = string.Format("{0}.{1}.{2}.{3}", MajorVersion, buildDate.Year, buildDate.Month,
//                                             buildDate.Day);

//                 //Insert version into the cache until tomorrow (Today + 1 day)
//                 filterContext.HttpContext.Cache.Insert(VersionKey, version, null, DateTime.Today.AddDays(1), Cache.NoSlidingExpiration);
//             }

//             filterContext.Controller.ViewData[VersionKey] = version;
//         }

//         /// <summary>
//         /// Grabs the build linker time stamp
//         /// </summary>
//         /// <param name="filePath"></param>
//         /// <returns></returns>
//         /// <remarks>
//         /// http://stackoverflow.com/questions/2050396/getting-the-date-of-a-net-assembly
//         /// and
//         /// http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html
//         /// </remarks>
//         private static DateTime RetrieveLinkerTimestamp(string filePath)
//         {
//             const int peHeaderOffset = 60;
//             const int linkerTimestampOffset = 8;
//             var b = new byte[2048];
//             System.IO.FileStream s = null;
//             try
//             {
//                 s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
//                 s.Read(b, 0, 2048);
//             }
//             finally
//             {
//                 if (s != null)
//                     s.Close();
//             }
//             var dt = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(BitConverter.ToInt32(b, BitConverter.ToInt32(b, peHeaderOffset) + linkerTimestampOffset));
//             return dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
//         }

//         public override void OnActionExecuting(ActionExecutingContext filterContext)
//         {
//             LoadAssemblyVersion(filterContext);

//             base.OnActionExecuting(filterContext);
//         }
//     }
// }
