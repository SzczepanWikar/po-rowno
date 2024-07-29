using System.Net;

namespace Core.Common.Exceptions
{
    public class BadRequestException : HttpException
    {
        public BadRequestException()
            : base(HttpStatusCode.BadRequest) { }

        public BadRequestException(string message)
            : base(HttpStatusCode.BadRequest, message) { }
    }
}
