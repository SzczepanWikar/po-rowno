using System.Net;

namespace Core.Common.Exceptions
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; init; }

        public HttpException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
