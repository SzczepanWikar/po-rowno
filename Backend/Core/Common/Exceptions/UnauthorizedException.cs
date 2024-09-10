using System.Net;

namespace Core.Common.Exceptions
{
    public sealed class UnauthorizedException : HttpException
    {
        public UnauthorizedException()
            : base(HttpStatusCode.Unauthorized) { }

        public UnauthorizedException(string message)
            : base(HttpStatusCode.Unauthorized, message) { }
    }
}
