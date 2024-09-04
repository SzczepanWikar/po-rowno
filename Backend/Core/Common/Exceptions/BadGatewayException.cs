using System.Net;

namespace Core.Common.Exceptions
{
    public sealed class BadGatewayException : HttpException
    {
        public BadGatewayException()
            : base(HttpStatusCode.BadGateway) { }

        public BadGatewayException(string message)
            : base(HttpStatusCode.BadGateway, message) { }
    }
}
