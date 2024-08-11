using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Core.Common.Code
{
    public class Code<T>
        where T : Enum
    {
        private static ReadOnlySpan<char> allowedChar =>
            "abcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        public string Value { get; init; }
        public T Type { get; init; }
        public DateTime? ValidTo { get; init; }
        public bool Used { get; set; } = false;

        public Code(T type)
        {
            Type = type;
            Value = RandomNumberGenerator.GetString(allowedChar, 8);
        }

        public Code(T type, DateTime? validTo)
        {
            Type = type;
            Value = RandomNumberGenerator.GetString(allowedChar, 8);
            ValidTo = validTo;
        }

        /// <param name="type"></param>
        /// <param name="duration">Code duration in seconds</param>
        public Code(T type, uint duration)
        {
            Type = type;
            Value = RandomNumberGenerator.GetString(allowedChar, 8);
            ValidTo = DateTime.Now.AddSeconds(duration);
        }

        public bool Check(string value, T type)
        {
            if (value != Value || !Type.Equals(type))
            {
                return false;
            }

            if (ValidTo != null && ValidTo < DateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}
