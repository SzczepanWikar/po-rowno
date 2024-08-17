using System.Net;

namespace Core.Common.Exceptions
{
    public class ForbiddenException : HttpException
    {
        public ForbiddenException()
            : base(HttpStatusCode.Forbidden) { }

        public ForbiddenException(string message)
            : base(HttpStatusCode.Forbidden, message) { }
    }
}
