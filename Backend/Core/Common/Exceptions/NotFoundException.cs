using System.Net;

namespace Core.Common.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException()
            : base(HttpStatusCode.NotFound) { }

        public NotFoundException(string message)
            : base(HttpStatusCode.NotFound, message) { }
    }
}
