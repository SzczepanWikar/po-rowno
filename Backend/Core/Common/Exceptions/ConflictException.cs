using System.Net;

namespace Core.Common.Exceptions
{
    public class ConflictException : HttpException
    {
        public ConflictException()
            : base(HttpStatusCode.Conflict) { }

        public ConflictException(string message)
            : base(HttpStatusCode.Conflict, message) { }
    }
}
