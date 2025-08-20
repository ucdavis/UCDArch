using System;
using Microsoft.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.Mvc;

namespace UCDArch.Web.ActionResults
{
    /// <summary>
    /// An ActionResult to return JSON from ASP.NET MVC to the browser using Json.NET.
    /// Taken from http://james.newtonking.com/archive/2008/10/16/asp-net-mvc-and-json-net.aspx
    /// </summary>
    public class JsonNetResult : ActionResult
    {
        public JsonNetResult() : this(null, null, null, JsonDateConversionStrategy.Default) { }
        public JsonNetResult(Object data) : this(data, null, null, JsonDateConversionStrategy.Default) { }
        public JsonNetResult(Object data, JsonDateConversionStrategy dateConversionStrategy) : this(data, null, null, dateConversionStrategy) { }
        public JsonNetResult(Object data, String contentType) : this(data, contentType, null, JsonDateConversionStrategy.Default) { }
        public JsonNetResult(Object data, String contentType, Encoding encoding, JsonDateConversionStrategy dateConversionStrategy)
        {
            SerializerSettings = new JsonSerializerSettings();

            switch (dateConversionStrategy)
            {
                case JsonDateConversionStrategy.Default:
                    break;
                case JsonDateConversionStrategy.JavaScript:
                    SerializerSettings.Converters.Add(new JavaScriptDateTimeConverter());
                    break;
                case JsonDateConversionStrategy.Iso:
                    SerializerSettings.Converters.Add(new IsoDateTimeConverter());
                    break;
                case JsonDateConversionStrategy.Microsoft:
                    SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dateConversionStrategy");
            }

            Data = data;
            ContentType = contentType;
            ContentEncoding = encoding;
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            var mediaType = new MediaTypeHeaderValue(!string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json");

            mediaType.Encoding = ContentEncoding ?? Encoding.UTF8;
            response.ContentType = mediaType.ToString();

            if (Data != null)
            {
                using (var streamWriter = new StreamWriter(response.BodyWriter.AsStream()))
                using (var writer = new JsonTextWriter(streamWriter) { Formatting = Formatting })
                {
                    JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                    serializer.Serialize(writer, Data);
                    writer.Flush();
                }
            }
        }

        public string JsonResultString
        {
            get
            {
                if (Data == null)
                {
                    return "[Data was Null]";
                }

                var stringWriter = new StringWriter();
                var writer = new JsonTextWriter(stringWriter) { Formatting = Formatting };
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);
                writer.Flush();

                return stringWriter.GetStringBuilder().ToString();
            }
        }

        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }
        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }
    }
}