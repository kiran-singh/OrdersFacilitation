using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace OrdersApiÍ
{
    public class CustomException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public IDictionary<string, ICollection<string>> ModelErrors { get; set; } = new Dictionary<string, ICollection<string>>();

        public string Result => ModelErrors.Any()
            ? JsonConvert.SerializeObject(ModelErrors)
            : JsonConvert.SerializeObject(new {error = Message});

        public CustomException(HttpStatusCode statusCode, string message = null) : base(message)
        {
            StatusCode = statusCode;
        }

        public CustomException(HttpStatusCode statusCode, IDictionary<string, ICollection<string>> modelErrors)
        {
            ModelErrors = modelErrors;
            StatusCode = statusCode;
        }
    }
}